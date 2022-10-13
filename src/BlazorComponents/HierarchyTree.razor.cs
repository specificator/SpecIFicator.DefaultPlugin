using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.ViewModels;
using SpecIFicator.Framework.CascadingValues;

namespace SpecIFicator.DefaultPlugin.BlazorComponents
{
    public partial class HierarchyTree
    {
        [CascadingParameter]
        public HierarchyEditorContext DataContext { get; set; }


        public HierarchyViewModel HierarchyViewModel { get; set; }

        private HierarchyViewModel SelectedNode { get; set; }

        protected override void OnInitialized()
        {
            HierarchyViewModel = DataContext.HierarchyViewModel;

            _selectedNode = HierarchyViewModel.SelectedNode;
        }
    }
}