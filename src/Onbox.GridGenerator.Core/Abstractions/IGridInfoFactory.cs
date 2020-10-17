using Onbox.GridGenerator.Models;

namespace Onbox.GridGenerator.Abstractions
{
    public interface IGridInfoFactory
    {
        GridInfo CreateHorizontal(string name, double previousOffset);
        GridInfo CreateHorizontal(string name, double offset, double previousOffset, double length);
        GridInfo CreateVertical(string name, double previousOffset);
        GridInfo CreateVertical(string name, double offset, double previousOffset, double length);
    }
}