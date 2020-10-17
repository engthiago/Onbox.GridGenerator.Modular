using Onbox.Abstractions.V8;
using Onbox.Core.V8;
using Onbox.Revit.V8;
using Onbox.Revit.V8.Applications;
using Onbox.Revit.V8.UI;
using Autodesk.Revit.UI;
using Onbox.GridGenerator.UIApp.Revit.Commands.Availability;
using Onbox.GridGenerator.ContainerExtensions;
using Onbox.GridGenerator.Views;
using Onbox.GridGenerator.UIApp.Revit.Commands;

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
            var addinPanelManager = ribbonManager.CreatePanel("Grid Generator Modular");
            addinPanelManager.AddPushButton<GridGeneratorCommand, AvailableOnProject>($"Grid{br}Generator", "onbox_logo");
        }

        public override Result OnStartup(IContainer container, UIControlledApplication application)
        {
            // Here you can add all necessary dependencies to the container
            container.AddOnboxCore();

            container.AddUnitConverters();
            container.AddGridGenerator();

            container.Pipe<MvcContainerPipeline>();

            return Result.Succeeded;
        }

        public override Result OnShutdown(IContainerResolver container, UIControlledApplication application)
        {
            // No Need to cleanup the Container, the framework will do it for you
            return Result.Succeeded;
        }
    }

}