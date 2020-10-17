using Onbox.GridGenerator.Abstractions;
using Onbox.GridGenerator.Dynamo.Services;
using Onbox.GridGenerator.Models;
using RevitServices.Persistence;
using RevitServices.Transactions;
using System;
using System.Collections.Generic;
using DynElements = Revit.Elements;

namespace Onbox.GridGenerator.Dynamo
{
    public class GridGenerator
    {
        private GridGenerator()
        {
        }

        public static GridInfo CreateGridInfo(string name, double offset, double length, bool isVertical)
        {
            using (var container = ContainerLifeCycle.Create())
            {
                var factory = container.Resolve<IGridInfoFactory>();

                if (isVertical)
                {
                    var gridInfo = factory.CreateVertical(name, offset, 0, length);
                    return gridInfo;
                }
                else
                {
                    var gridInfo = factory.CreateHorizontal(name, offset, 0, length);
                    return gridInfo;
                }
            }
        }

        public static List<DynElements.Element> GenerateRevitGrids(List<GridInfo> gridInfoList)
        {
            var doc = DocumentManager.Instance.CurrentDBDocument;
            if (doc == null)
            {
                throw new Exception("Please open a Project to continue...");
            }

            using (var container = ContainerLifeCycle.Create())
            {
                TransactionManager.Instance.EnsureInTransaction(doc);

                var revitGridService = container.Resolve<IRevitGridService>();
                revitGridService.DeleteGrids(doc);
                var revitGrids = revitGridService.CreateRevitGrids(doc, gridInfoList);

                TransactionManager.Instance.TransactionTaskDone();

                var dynamoWrapperService = container.Resolve<DynamoElementService>();
                var dynamoGrids = new List<DynElements.Element>();
                foreach (var revitGrid in revitGrids)
                {
                    dynamoGrids.Add(dynamoWrapperService.Wrap(revitGrid, false));
                }
                return dynamoGrids;
            }

        }
    }
}
