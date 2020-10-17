using Onbox.GridGenerator.Models;
using System.Collections.Generic;

namespace Onbox.GridGenerator.Abstractions
{
    public interface IGridInfoService
    {
        double GetGridTotalOffset(GridInfo grid, List<GridInfo> grids);
        double GetGridLength(List<GridInfo> perpendicularGrids);
        List<GridInfo> AddNewGrid(List<GridInfo> grids, bool isVertical);
        List<GridInfo> RemoveGrid(List<GridInfo> grids, GridInfo gridToRemove, bool isVertical);
        List<GridInfo> UpdateGrids(List<GridInfo> parallelGrids, List<GridInfo> perpendicularGrids, bool isVertical);

    }
}