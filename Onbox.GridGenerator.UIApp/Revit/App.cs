using Onbox.Abstractions.V8;
using Onbox.Core.V8;
using Onbox.Revit.V8;
using Onbox.Revit.V8.Applications;
using Onbox.Revit.V8.UI;
using Autodesk.Revit.UI;
using Onbox.GridGenerator.UIApp.Revit.Commands.Availability;
using Onbox.GridGenerator.UIApp.Revit.Commands;
using Onbox.GridGenerator.UIApp.Services;

namespace Onbox.GridGenerator.UIApp.Revit
{
    [ContainerProvider("1c2bf232-a923-4b05-8b63-999f06eba4cb")]
    public class App : RevitApp
    {
        public override void OnCreateRibbon(IRibbonManager ribbonManager)
        {
            // Here you can create Ribbon tabs, panels and buttons

            var br = ribbonManager.GetLineBreak();

            // Adds a Ribbon Panel to the Addins tab
            var addinPanelManager = ribbonManager.CreatePanel("Onbox.GridGenerator.UIApp");
            addinPanelManager.AddPushButton<HelloCommand, AvailableOnProject>($"Hello{br}Framework", "onbox_logo");

            // Adds a new Ribbon Tab with a new Panel
            var panelManager = ribbonManager.CreatePanel("Onbox.GridGenerator.UIApp", "Hello Panel");
            panelManager.AddPushButton<HelloCommand, AvailableOnProject>($"Hello{br}Framework", "onbox_logo");
        }

        public override Result OnStartup(IContainer container, UIControlledApplication application)
        {
            // Here you can add all necessary dependencies to the container
            container.AddOnboxCore();

            // Add TaskDialog Service the message service
            container.AddSingleton<IMessageService, TaskMessageService>();

            return Result.Succeeded;
        }

        public override Result OnShutdown(IContainerResolver container, UIControlledApplication application)
        {
            // No Need to cleanup the Container, the framework will do it for you
            return Result.Succeeded;
        }
    }

}