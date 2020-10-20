using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using DesignAutomationFramework;
using Newtonsoft.Json;
using Onbox.GridGenerator.Abstractions;
using Onbox.GridGenerator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Onbox.GridGenerator.DA
{
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    public class GridGeneratorApp : IExternalDBApplication
    {
        public ExternalDBApplicationResult OnShutdown(ControlledApplication application)
        {
            DesignAutomationBridge.DesignAutomationReadyEvent += this.OnApplicationReady;
            return ExternalDBApplicationResult.Succeeded;
        }

        private void OnApplicationReady(object sender, DesignAutomationReadyEventArgs e)
        {
            var appData = e.DesignAutomationData;
            var doc = appData.RevitApp.NewProjectDocument(UnitSystem.Metric);
            e.Succeeded = true;

            // Create container and resolve necessary classes
            using (var container = ContainerLifeCycle.Create())
            {
                var revitGridService = container.Resolve<IRevitGridService>();

                // Get gridInfo list
                var json = File.ReadAllText("gridCollection.json");
                var gridCollection = JsonConvert.DeserializeObject<GridCollection>(json);

                using (var t = new Transaction(doc, "Create grids"))
                {
                    t.Start();

                    revitGridService.CreateRevitGrids(doc, gridCollection.GridInfoList);

                    t.Commit();
                }
            }
        }

        public ExternalDBApplicationResult OnStartup(ControlledApplication application)
        {
            return ExternalDBApplicationResult.Succeeded;
        }
    }
}
