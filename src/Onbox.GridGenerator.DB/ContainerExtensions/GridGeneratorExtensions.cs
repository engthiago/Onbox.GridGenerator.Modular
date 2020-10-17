using Onbox.Abstractions.V8;
using Onbox.GridGenerator.Abstractions;
using Onbox.GridGenerator.Models;
using Onbox.GridGenerator.Services;

namespace Onbox.GridGenerator.ContainerExtensions
{
    public static class GridGeneratorExtensions
    {
        public static IContainer AddGridGenerator(this IContainer container)
        {
            var gridSettings = new GridSettings
            {
                DefaultLength = 4000,
                MinimumLength = 500,
                DefaultOffset = 2000,
                MinimumOffset = 500
            };
            container.AddSingleton(gridSettings);

            container.AddTransient<IGridInfoFactory, GridInfoFactory>();
            container.AddTransient<IGridInfoService, GridInfoService>();
            container.AddTransient<IRevitGridService, RevitGridService>();
            container.AddTransient<IGridCollector, GridCollector>();

            return container;
        }
    }
}
