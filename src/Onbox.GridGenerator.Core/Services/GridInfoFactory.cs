using Onbox.GridGenerator.Abstractions;
using Onbox.GridGenerator.Models;

namespace Onbox.GridGenerator.Services
{
    public class GridInfoFactory : IGridInfoFactory
    {
        private readonly GridSettings gridSettings;

        public GridInfoFactory(GridSettings gridSettings)
        {
            this.gridSettings = gridSettings;
        }

        public GridInfo CreateVertical(string name, double previousOffset)
        {
            var gridInfo = this.Create(name, this.gridSettings.DefaultOffset, previousOffset, this.gridSettings.DefaultLength);
            gridInfo.IsVertical = true;
            return gridInfo;
        }

        public GridInfo CreateVertical(string name, double offset, double previousOffset, double length)
        {
            var gridInfo = this.Create(name, offset, previousOffset, length);
            gridInfo.IsVertical = true;
            return gridInfo;
        }

        public GridInfo CreateHorizontal(string name, double previousOffset)
        {
            var gridInfo = this.Create(name, this.gridSettings.DefaultOffset, previousOffset, this.gridSettings.DefaultLength);
            gridInfo.IsVertical = false;
            return gridInfo;
        }

        public GridInfo CreateHorizontal(string name, double offset, double previousOffset, double length)
        {
            var gridInfo = this.Create(name, offset, previousOffset, length);
            gridInfo.IsVertical = false;
            return gridInfo;
        }


        private GridInfo Create(string name, double offset, double previousOffset, double length)
        {
            var totalOffset = previousOffset + offset;
            var gridInfo = new GridInfo
            {
                Name = name,
                Offset = offset,
                TotalOffset = totalOffset,
                Length = length
            };

            return gridInfo;
        }
    }
}
