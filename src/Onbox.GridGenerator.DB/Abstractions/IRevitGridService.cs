using Autodesk.Revit.DB;
using Onbox.GridGenerator.Models;
using System.Collections.Generic;

namespace Onbox.GridGenerator.Abstractions
{
    public interface IRevitGridService
    {
        ICollection<ElementId> DeleteGrids(Document doc);
        List<Grid> CreateRevitGrids(Document doc, List<GridInfo> gridInfoList);
        List<GridInfo> ConvertToGridInfoList(List<Grid> grids, bool areVertical);
    }
}