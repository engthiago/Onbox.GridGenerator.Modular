using Autodesk.Revit.DB;
using Onbox.GridGenerator.Abstractions;
using System.Collections.Generic;
using System.Linq;

namespace Onbox.GridGenerator.Services
{
    public class GridCollector : IGridCollector
    {
        public GridCollector()
        {
        }

        public ICollection<ElementId> CollectAllGrids(Document doc)
        {
            var gridIdList = new FilteredElementCollector(doc)
                             .OfCategory(BuiltInCategory.OST_Grids)
                             .WhereElementIsNotElementType()
                             .ToElementIds();

            return gridIdList;
        }

        public List<Grid> CollectHorizontalGrids(Document doc)
        {
            // Get existing horizontal grids
            var horizontalGrids = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Grids)
                .WhereElementIsNotElementType()
                .Cast<Grid>()
                .Where(g => g.IsCurved == false)
                .Where(g =>
                {
                    var line = g.Curve as Line;
                    if (line.Direction.IsAlmostEqualTo(XYZ.BasisX))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                })
                .OrderBy(g => (g.Curve as Line).Origin.Y)
                .ToList();

            return horizontalGrids;

        }

        public List<Grid> CollectVerticalGrids(Document doc)
        {
            // Get existing vertical grids
            var verticalGrids = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Grids)
                .WhereElementIsNotElementType()
                .Cast<Grid>()
                .Where(g => g.IsCurved == false)
                .Where(g =>
                {
                    var line = g.Curve as Line;
                    if (line.Direction.IsAlmostEqualTo(XYZ.BasisY))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                })
                .OrderBy(g => (g.Curve as Line).Origin.X)
                .ToList();

            return verticalGrids;
        }
    }
}
