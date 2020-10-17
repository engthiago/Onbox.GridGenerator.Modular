using Onbox.Abstractions.V8;
using Onbox.Revit.V8.Commands;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Onbox.GridGenerator.Abstractions;
using Onbox.GridGenerator.Models;
using System.Collections.Generic;

namespace Onbox.GridGenerator.UIApp.Revit.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class GridGeneratorCommand : RevitAppCommand<App>
    {
        public override Result Execute(IContainerResolver container, ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;

            // Get current grids
            var gridCollector = container.Resolve<IGridCollector>();
            var horizontalGrids = gridCollector.CollectHorizontalGrids(doc);
            var verticalGrids = gridCollector.CollectVerticalGrids(doc);

            // Convert Grids to GridInfo
            var revitGridService = container.Resolve<IRevitGridService>();
            var horizontalGridInfoList = revitGridService.ConvertToGridInfoList(horizontalGrids, false);
            var verticalGridInfoList = revitGridService.ConvertToGridInfoList(verticalGrids, true);

            // Instantiate view and set grid info lists
            var gridGeneratorView = container.Resolve<IGridGeneratorView>();
            gridGeneratorView.SetHorizontalGrids(horizontalGridInfoList);
            gridGeneratorView.SetVerticalGrids(verticalGridInfoList);

            var dialogResult = gridGeneratorView.ShowDialog();
            if (dialogResult == true)
            {
                var grids = new List<GridInfo>();
                grids.AddRange(gridGeneratorView.GetHorizontalGrids());
                grids.AddRange(gridGeneratorView.GetVerticalGrids());

                using (var t = new Transaction(doc, "Create grids"))
                {
                    t.Start();

                    // Clear Existing grids and create the new ones
                    revitGridService.DeleteGrids(doc);
                    revitGridService.CreateRevitGrids(doc, grids);

                    t.Commit();
                }
            }

            return Result.Succeeded;
        }
    }
}
