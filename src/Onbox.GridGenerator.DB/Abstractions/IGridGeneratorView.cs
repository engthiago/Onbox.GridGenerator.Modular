using Onbox.GridGenerator.Models;
using Onbox.Mvc.Revit.Abstractions.V8;
using Onbox.Mvc.Abstractions.V8;
using System.Collections.Generic;

namespace Onbox.GridGenerator.Abstractions
{
    /// <summary>
    /// A contract a view designed to have Revit as parent window
    /// </summary>
    public interface IGridGeneratorView : IRevitMvcViewBase, IMvcViewModal
    {
        List<GridInfo> GetHorizontalGrids();
        void SetHorizontalGrids(List<GridInfo> gridInfoList);

        List<GridInfo> GetVerticalGrids();
        void SetVerticalGrids(List<GridInfo> gridInfoList);
    }
}