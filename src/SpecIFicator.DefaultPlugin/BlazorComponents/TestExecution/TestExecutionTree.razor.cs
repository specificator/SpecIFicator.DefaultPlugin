using Microsoft.AspNetCore.Components;
using MDD4All.SpecIF.ViewModels;
using MDD4All.UI.DataModels.Tree;

namespace SpecIFicator.DefaultPlugin.BlazorComponents.TestExecution
{
    public partial class TestExecutionTree
    {
        [CascadingParameter]
        public HierarchyViewModel DataContext { get; set; }

        protected override void OnInitialized()
        {
            DataContext.PropertyChanged += OnPropertyChanged;
        }

        void OnSelectionChanged(ITreeNode node)
        {
            DataContext.SelectedNode = node as NodeViewModel;
        }

        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs arguments)
        {
            StateHasChanged();
        }
    }
}