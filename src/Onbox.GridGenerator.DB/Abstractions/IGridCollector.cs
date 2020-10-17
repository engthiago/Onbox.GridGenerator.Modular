using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace Onbox.GridGenerator.Abstractions
{
    public interface IGridCollector
    {
        ICollection<ElementId> CollectAllGrids(Document doc);
        List<Grid> CollectHorizontalGrids(Document doc);
        List<Grid> CollectVerticalGrids(Document doc);
    }
}