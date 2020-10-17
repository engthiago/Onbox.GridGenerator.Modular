using Autodesk.DesignScript.Runtime;
using Revit.Elements;

namespace Onbox.GridGenerator.Dynamo.Services
{
    [IsVisibleInDynamoLibrary(false)]
    public class DynamoElementService
    {
        public DynamoElementService()
        {
        }

        public Element Wrap(Autodesk.Revit.DB.Element element, bool isRevitOwned)
        {
            return element.ToDSType(isRevitOwned);
        }
    }
}
