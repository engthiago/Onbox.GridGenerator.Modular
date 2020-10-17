using Autodesk.DesignScript.Runtime;
using Onbox.Abstractions.V8;
using Onbox.Di.V8;
using Onbox.GridGenerator.ContainerExtensions;


namespace Onbox.GridGenerator.Dynamo
{
    [IsVisibleInDynamoLibrary(false)]
    public class ContainerLifeCycle
    {
        public static IContainer Create()
        {
            var container = new Container();

            container.AddUnitConverters();
            container.AddGridGenerator();

            return container;
        }
    }
}
