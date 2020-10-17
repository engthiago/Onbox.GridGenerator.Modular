using Onbox.Abstractions.V8;
using Onbox.GridGenerator.Abstractions;
using Onbox.GridGenerator.Services;

namespace Onbox.GridGenerator.ContainerExtensions
{
    public static class UnitConverterExtensions
    {
        public static IContainer AddUnitConverters(this IContainer container)
        {
            container.AddSingleton<ILengthConverter, RevitLengthConverter>();

            return container;
        }
    }
}
