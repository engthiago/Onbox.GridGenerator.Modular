using Onbox.Abstractions.V8;
using Onbox.Mvc.Revit.V8;
using Onbox.Mvc.V8.Messaging;

namespace Onbox.GridGenerator.Views
{
    public class MvcContainerPipeline : IContainerPipeline
    {
        public IContainer Pipe(IContainer container)
        {
            // Adds Support for Revit Mvc
            container.AddRevitMvc();

            return container;
        }
    }

}