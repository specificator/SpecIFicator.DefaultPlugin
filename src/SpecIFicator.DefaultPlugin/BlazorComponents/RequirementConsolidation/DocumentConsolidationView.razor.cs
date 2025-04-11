using Microsoft.AspNetCore.Components;
using MDD4All.SpecIF.ViewModels;
using System.ComponentModel;
using SpecIFicator.DefaultPlugin.ViewModels;

namespace SpecIFicator.DefaultPlugin.BlazorComponents.RequirementConsolidation
{
    public partial class DocumentConsolidationView
    {
        [CascadingParameter]
        public HierarchyViewModel DataContext { get; set; }

        [Parameter]
        public ConsolidationViewModel ConsolidationViewModel { get; set; }

        protected override void OnInitialized()
        {
            DataContext.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedNode")
            {
                InvokeAsync(() => StateHasChanged());
            }
        }

        protected void OnLoadingFinished()
        {
            InvokeAsync(() => StateHasChanged());
        }

    }
}
