using Onbox.Abstractions.V8;
using Onbox.Di.V8;
using Onbox.GridGenerator.ContainerExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Onbox.GridGenerator.DA
{
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
