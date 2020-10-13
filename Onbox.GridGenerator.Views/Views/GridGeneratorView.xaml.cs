using Onbox.Mvc.Abstractions.V8;
using Onbox.Mvc.Revit.Abstractions.V8;
using Onbox.Mvc.Revit.V8;
using Onbox.Revit.Abstractions.V8;
using System;
using System.Windows;

namespace Onbox.GridGenerator.Views
{
    /// <summary>
    /// A contract a view designed to have Revit as parent window
    /// </summary>
    public interface IGridGeneratorView : IRevitMvcViewBase, IMvcViewModal
    {
    }

    /// <summary>
    /// A view designed to have Revit as parent window
    /// </summary>
    public partial class GridGeneratorView : RevitMvcViewBase, IGridGeneratorView
    {
        public GridGeneratorView(IRevitAppData appData) : base(appData)
        {
            InitializeComponent();
        }
    }
}