using Autodesk.Revit.DB;
using Onbox.GridGenerator.Abstractions;
using Onbox.GridGenerator.Models;
using System.Collections.Generic;

namespace Onbox.GridGenerator.Services
{
    public class RevitGridService : IRevitGridService
    {
        private readonly IGridInfoFactory gridInfoFactory;
        private readonly IGridCollector gridCollector;
        private readonly ILengthConverter lengthConverter;

        public RevitGridService(
            IGridInfoFactory gridInfoFactory,
            IGridCollector gridCollector,
            ILengthConverter lengthConverter
            )
        {
            this.gridInfoFactory = gridInfoFactory;
            this.gridCollector = gridCollector;
            this.lengthConverter = lengthConverter;
        }

        public ICollection<ElementId> DeleteGrids(Document doc)
        {
            var grids = this.gridCollector.CollectAllGrids(doc);

            return doc.Delete(grids);
        }

        public List<Grid> CreateRevitGrids(Document doc, List<GridInfo> gridInfoList)
        {
            var grids = new List<Grid>();

            foreach (var gridInfo in gridInfoList)
            {
                var offset = this.lengthConverter.ConvertMillimetersToInternal(gridInfo.TotalOffset);
                var length = this.lengthConverter.ConvertMillimetersToInternal(gridInfo.Length);

                if (doc.Application.ShortCurveTolerance > length)
                {
                    throw new System.ArgumentException($"Length of the grid '{gridInfo.Name}' is smaller than minimum size allowed.");
                }

                XYZ start;
                XYZ end;
                if (gridInfo.IsVertical)
                {
                    start = XYZ.BasisX.Multiply(offset);
                    end = start.Add(XYZ.BasisY.Multiply(length));
                }
                else
                {
                    start = XYZ.BasisY.Multiply(offset);
                    end = start.Add(XYZ.BasisX.Multiply(length));
                }

                var line = Line.CreateBound(start, end);
                var grid = Grid.Create(doc, line);
                grid.Name = gridInfo.Name;
                grids.Add(grid);
            }

            return grids;
        }

        private Line GetGridLine(Grid grid)
        {
            if (grid == null)
            {
                throw new System.ArgumentNullException("Grid can not be null.");
            }

            var line = grid.Curve as Line;
            // That means that this grid was not drawn as a line
            if (line == null)
            {
                throw new System.InvalidOperationException("Grids drawn as curves, arcs or other non linear geometry are not supported.");
            }

            return line;
        }


        public double GetGridXOffset(Grid grid)
        {
            var line = this.GetGridLine(grid);
            return line.Origin.X;
        }

        public double GetGridYOffset(Grid grid)
        {
            var line = this.GetGridLine(grid);
            return line.Origin.Y;
        }

        public List<GridInfo> ConvertToGridInfoList(List<Grid> grids, bool areVertical)
        {
            // Convert grids to GridInfo
            var gridInfoList = new List<GridInfo>();

            var currentOffset = 0.0;
            if (areVertical)
            {
                foreach (var grid in grids)
                {
                    var gridInfo = this.ConvertToVerticalGridInfo(grid, currentOffset);
                    gridInfoList.Add(gridInfo);
                    currentOffset += gridInfo.Offset;
                }
            }
            else
            {
                foreach (var grid in grids)
                {
                    var gridInfo = this.ConvertToHorizontalGridInfo(grid, currentOffset);
                    gridInfoList.Add(gridInfo);
                    currentOffset += gridInfo.Offset;
                }
            }

            return gridInfoList;
        }

        private GridInfo ConvertToHorizontalGridInfo(Grid grid, double currentOffset)
        {
            // Length
            var lengthInFeet = this.GetGridLine(grid).Length;
            var lengthInMm = this.lengthConverter.ConvertInternalToMillimeters(lengthInFeet);

            // Offset
            var offsetInFeet = this.GetGridYOffset(grid);
            var offsetInMm = this.lengthConverter.ConvertInternalToMillimeters(offsetInFeet);

            offsetInMm -= currentOffset;

            var gridInfo = this.gridInfoFactory.CreateHorizontal(grid.Name, offsetInMm, currentOffset, lengthInMm);
            return gridInfo;
        }

        private GridInfo ConvertToVerticalGridInfo(Grid grid, double currentOffset)
        {
            // Length
            var lengthInFeet = this.GetGridLine(grid).Length;
            var lengthInMm = this.lengthConverter.ConvertInternalToMillimeters(lengthInFeet);

            // Offset
            var offsetInFeet = this.GetGridXOffset(grid);
            var offsetInMm = this.lengthConverter.ConvertInternalToMillimeters(offsetInFeet);

            offsetInMm -= currentOffset;

            var gridInfo = this.gridInfoFactory.CreateVertical(grid.Name, offsetInMm, currentOffset, lengthInMm);
            return gridInfo;
        }
    }
}
