using Autodesk.Forge.DesignAutomation;
using Autodesk.Forge.DesignAutomation.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Onbox.GridGenerator.Models;
using Onbox.GridGenerator.Web.Models.Adsk;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DesignAutoParam = Autodesk.Forge.DesignAutomation.Model.Parameter;

namespace Onbox.GridGenerator.Web.Services
{
    public class DesignAutoService
    {
        private readonly DesignAutomationClient designAutoClient;
        private readonly string localBundlesFolder;
        private readonly string clientId;
        private readonly string forgeEnv;

        public DesignAutoService(DesignAutomationClient daClient, IWebHostEnvironment env, IConfiguration config)
        {
            this.designAutoClient = daClient;
            this.localBundlesFolder = Path.Combine(env.WebRootPath, "DesignAuto");
            this.clientId = config["Forge:ClientId"];
            this.forgeEnv = config["Forge:Environment"];
        }

        public async Task<IEnumerable<string>> GetActivitiesAsync()
        {
            var activities = await designAutoClient.GetActivitiesAsync();
            return activities.Data;
        }

        public async Task<Page<string>> GetAllBundlesAsync()
        {
            return await this.designAutoClient.GetAppBundlesAsync();
        }

        public async Task<DesignAutoBundleModel> CreateAppBundleAsync(string fileName, string engine)
        {
            string packageName = this.GetBundleName(fileName);

            string zipPath = Path.Combine(localBundlesFolder, packageName + ".zip");
            if (!File.Exists(zipPath))
            {
                throw new FileNotFoundException($"Bundle {zipPath} not found!");
            }

            var bundles = await this.GetAllBundlesAsync();

            // Check for prev versions of app bundle
            AppBundle bundleVersion;
            string qualifiedBundleId = GetQualifiedBundleId(packageName);

            if (!bundles.Data.Contains(qualifiedBundleId))
            {
                bundleVersion = await CreateNewBundleAsync(engine, packageName);
            }
            else
            {
                bundleVersion = await UpdateBundleAsync(engine, packageName);
            }

            // Upload zip file
            var uploadClient = new RestClient(bundleVersion.UploadParameters.EndpointURL);
            var req = new RestRequest(string.Empty, Method.POST);
            req.AlwaysMultipartFormData = true;
            foreach (KeyValuePair<string, string> vp in bundleVersion.UploadParameters.FormData)
            {
                req.AddParameter(vp.Key, vp.Value);
            }

            req.AddFile("file", zipPath);
            req.AddHeader("Cache-Control", "no-cache");

            await uploadClient.ExecuteTaskAsync(req);

            var appBundle = new DesignAutoBundleModel();
            appBundle.Id = qualifiedBundleId;
            appBundle.Version = (int)bundleVersion.Version;

            return appBundle;
        }

        private string GetQualifiedBundleId(string packageName)
        {
            return $"{this.clientId}.{packageName}+{this.forgeEnv}";
        }

        private async Task<AppBundle> UpdateBundleAsync(string engine, string packageName)
        {
            var bundle = new AppBundle();
            bundle.Engine = engine;
            bundle.Description = $"Grid Generator: {packageName}";

            var bundleVersion = await designAutoClient.CreateAppBundleVersionAsync(packageName, bundle);
            if (bundleVersion == null)
            {
                throw new Exception("Error trying to update existing bundle version");
            }

            var alias = new AliasPatch();
            alias.Version = (int)bundleVersion.Version;

            await this.designAutoClient.ModifyAppBundleAliasAsync(packageName, this.forgeEnv, alias);
            return bundleVersion;
        }

        private async Task<AppBundle> CreateNewBundleAsync(string engine, string packageName)
        {
            var bundle = new AppBundle();
            bundle.Package = packageName;
            bundle.Engine = engine;
            bundle.Id = packageName;
            bundle.Description = $"Grid Generator: {packageName}";

            var bundleVersion = await this.designAutoClient.CreateAppBundleAsync(bundle);
            if (bundleVersion == null)
            {
                throw new Exception("Error trying to create new first bundle version");
            }

            var alias = new Alias();
            alias.Id = this.forgeEnv;
            alias.Version = 1;
            await designAutoClient.CreateAppBundleAliasAsync(packageName, alias);
            return bundleVersion;
        }

        private string GetActivityName(string fileName)
        {
            return fileName + "Activity";
        }

        private string GetBundleName(string fileName)
        {
            return fileName + "AppBundle";
        }

        public async Task<Activity> CreateActivityAsync(string fileName, string engine)
        {
            string activityName = this.GetActivityName(fileName);
            Activity activity = this.CreateActivity(fileName, engine);

            activity = await designAutoClient.CreateActivityAsync(activity);

            var alias = new Alias();
            alias.Id = activityName + this.forgeEnv;
            alias.Version = 1;

            await designAutoClient.CreateActivityAliasAsync(activityName, alias);

            return activity;
        }

        private Activity CreateActivity(string fileName, string engine)
        {
            var bundleName = this.GetBundleName(fileName);
            var activityName = this.GetActivityName(fileName);

            var commandLine = $"$(engine.path)\\revitcoreconsole.exe /al \"$(appbundles[{bundleName}].path)\"";
            var script = string.Empty;

            var gridCollectionParam = new DesignAutoParam();
            gridCollectionParam.Description = "The input json with a collection of grids.";
            gridCollectionParam.LocalName = "gridCollection.json";
            gridCollectionParam.Verb = Verb.Get;
            gridCollectionParam.Required = true;
            gridCollectionParam.Ondemand = false;
            gridCollectionParam.Zip = true;

            var resultsParam = new DesignAutoParam();
            resultsParam.Description = "The Resulting NUnit Xml Tests.";
            resultsParam.LocalName = "results.xml";
            resultsParam.Verb = Verb.Put;
            resultsParam.Zip = false;
            resultsParam.Required = true;
            resultsParam.Ondemand = false;

            var activity = new Activity();
            activity.Id = activityName;
            activity.Appbundles = new List<string>() { $"{this.clientId}.{bundleName}+{this.forgeEnv}" };
            activity.CommandLine = new List<string>() { commandLine };
            activity.Engine = engine;
            activity.Settings = new Dictionary<string, ISetting>()
            {
                { "script", new StringSetting() { Value = script } }
            };
            activity.Parameters = new Dictionary<string, DesignAutoParam>()
            {
                { "gridCollection", gridCollectionParam },
                { "results", resultsParam }
            };
            return activity;
        }

        public async Task DeleteWorkItem(string id)
        {
            await this.designAutoClient.DeleteWorkItemAsync(id);
        }

        public async Task DeleteActivity(string unqualifiedActivityName)
        {
            await this.designAutoClient.DeleteActivityAsync(unqualifiedActivityName);
        }

        public async Task<WorkItemStatus> CreateWorkItemAsync(string activityName, GridCollection gridInput, string baseResultUrl)
        {
            XrefTreeArgument gridCollectionArg = null;
            if (gridInput != null)
            {
                gridCollectionArg = new XrefTreeArgument();
                gridCollectionArg.Url = $"data:application/json,{gridInput}";
                gridCollectionArg.Verb = Verb.Get;
            }

            var callbackResultUrl = $"{baseResultUrl}api/designauto/results";
            var resultsArg = new XrefTreeArgument();
            resultsArg.Url = callbackResultUrl;
            resultsArg.Verb = Verb.Put;

            var workItemBundle = new WorkItem();
            workItemBundle.ActivityId = $"{this.clientId}.{activityName}Activity+{activityName}Activity{this.forgeEnv}";
            workItemBundle.Arguments = new Dictionary<string, IArgument>()
            {
                { "gridCollection", gridCollectionArg },
                { "results",  resultsArg }
            };

            var status = await this.designAutoClient.CreateWorkItemAsync(workItemBundle);

            return status;
        }

        public async Task<WorkItemStatus> CheckWorkItemAsync(string id)
        {
            var status = await this.designAutoClient.GetWorkitemStatusAsync(id);

            return status;
        }

        public async Task ClearAccount()
        {
            await this.designAutoClient.DeleteForgeAppAsync("me");
        }
    }
}
