using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using DesignAutomationFramework;
using Newtonsoft.Json;
using Onbox.GridGenerator.Abstractions;
using Onbox.GridGenerator.Models;
using System;
using System.IO;

namespace Onbox.GridGenerator.DA
{
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    public class App : IExternalDBApplication
    {
        public ExternalDBApplicationResult OnStartup(ControlledApplication application)
        {
            DesignAutomationBridge.DesignAutomationReadyEvent += this.OnApplicationReady;
            return ExternalDBApplicationResult.Succeeded;
        }

        private void OnApplicationReady(object sender, DesignAutomationReadyEventArgs e)
        {
            try
            {
                var appData = e.DesignAutomationData;
                var doc = appData.RevitApp.NewProjectDocument(UnitSystem.Metric) ?? throw new Exception("Could not create project");
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

                    doc.SaveAs("result.rvt");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("---ERROR---");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public ExternalDBApplicationResult OnShutdown(ControlledApplication application)
        {
            return ExternalDBApplicationResult.Succeeded;
        }

    }
}
