using Onbox.Abstractions.V8;
using Onbox.Revit.V8.Commands;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Onbox.GridGenerator.UIApp.Revit.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class HelloCommand : RevitAppCommand<App>
    {
        public override Result Execute(IContainerResolver container, ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Asks the container for a new instance a message service
            var messageService = container.Resolve<IMessageService>();
            messageService.Show("Hello Onbox Framework!");

            return Result.Succeeded;
        }
    }
}