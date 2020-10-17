using Onbox.GridGenerator.Abstractions;
using Onbox.GridGenerator.Models;
using Onbox.Mvc.Revit.V8;
using Onbox.Revit.Abstractions.V8;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Onbox.GridGenerator.Views
{
    /// <summary>
    /// A view designed to have Revit as parent window
    /// </summary>
    public partial class GridGeneratorView : RevitMvcViewBase, IGridGeneratorView
    {
        private readonly IGridInfoService gridService;

        public ObservableCollection<GridInfo> VerticalGrids { get; set; }
        public ObservableCollection<GridInfo> HorizontalGrids { get; set; }

        public GridGeneratorView(
            IRevitAppData appData,
            IGridInfoService gridService
            ) : base(appData)
        {
            InitializeComponent();
            this.gridService = gridService;

            // Initialize grid lists
            VerticalGrids = new ObservableCollection<GridInfo>();
            HorizontalGrids = new ObservableCollection<GridInfo>();
        }

        public List<GridInfo> GetHorizontalGrids()
        {
            return this.HorizontalGrids.ToList();
        }

        public void SetHorizontalGrids(List<GridInfo> horizontalGrids)
        {
            this.HorizontalGrids = new ObservableCollection<GridInfo>(horizontalGrids);
        }

        public List<GridInfo> GetVerticalGrids()
        {
            return this.VerticalGrids.ToList();
        }

        public void SetVerticalGrids(List<GridInfo> verticalGrids)
        {
            this.VerticalGrids = new ObservableCollection<GridInfo>(verticalGrids);
        }

        public void OnAddHorizontalGrid(object sender, RoutedEventArgs e)
        {
            var grids = this.gridService.AddNewGrid(this.HorizontalGrids.ToList(), false);
            var updatedHorizontalGrids = this.gridService.UpdateGrids(grids, this.VerticalGrids.ToList(), false);
            var updatedVerticalGrids = this.gridService.UpdateGrids(this.VerticalGrids.ToList(), grids, true);

            this.SetVerticalGrids(updatedVerticalGrids);
            this.SetHorizontalGrids(updatedHorizontalGrids);
        }

        public void OnAddVerticalGrid(object sender, RoutedEventArgs e)
        {
            var grids = this.gridService.AddNewGrid(this.VerticalGrids.ToList(), true);
            var updatedHorizontalGrids = this.gridService.UpdateGrids(this.HorizontalGrids.ToList(), grids, false);
            var updatedVerticalGrids = this.gridService.UpdateGrids(grids, this.HorizontalGrids.ToList(), true);

            this.SetVerticalGrids(updatedVerticalGrids);
            this.SetHorizontalGrids(updatedHorizontalGrids);
        }

        public void OnRemoveHorizontalGrid(object sender, RoutedEventArgs e)
        {
            if (GetGridInfoDataContext(sender) is GridInfo grid)
            {
                var grids = this.gridService.RemoveGrid(this.HorizontalGrids.ToList(), grid, false);
                var updatedHorizontalGrids = this.gridService.UpdateGrids(grids, this.VerticalGrids.ToList(), false);
                var updatedVerticalGrids = this.gridService.UpdateGrids(this.VerticalGrids.ToList(), grids, true);

                this.SetVerticalGrids(updatedVerticalGrids);
                this.SetHorizontalGrids(updatedHorizontalGrids);
            }

        }

        public void OnRemoveVerticalGrid(object sender, RoutedEventArgs e)
        {
            if (GetGridInfoDataContext(sender) is GridInfo grid)
            {
                var grids = this.gridService.RemoveGrid(this.VerticalGrids.ToList(), grid, true);
                var updatedHorizontalGrids = this.gridService.UpdateGrids(this.HorizontalGrids.ToList(), grids, false);
                var updatedVerticalGrids = this.gridService.UpdateGrids(grids, this.HorizontalGrids.ToList(), true);

                this.SetVerticalGrids(updatedVerticalGrids);
                this.SetHorizontalGrids(updatedHorizontalGrids);
            }
        }

        private GridInfo GetGridInfoDataContext(object sender)
        {
            if (sender is FrameworkElement element)
            {
                if (element.DataContext is GridInfo gridInfo)
                {
                    return gridInfo;
                }
            }

            return null;
        }

        private void OnCreateGrids(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
