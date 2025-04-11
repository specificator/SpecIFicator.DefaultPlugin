using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using MDD4All.SpecIF.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
using SpecIFicator.DefaultPlugin.ViewModels;

namespace SpecIFicator.DefaultPlugin.BlazorComponents.RequirementConsolidation
{
    public partial class ConsolidationView
    {
        [Inject]
        private IStringLocalizer<ConsolidationView> L{ get; set; }

        [Inject]
        public ISpecIfDataProviderFactory DataProviderFactory { get; set; }

        public ConsolidationViewModel DataContext { get; set; }

        protected override void OnInitialized()
        {
            DataContext = new ConsolidationViewModel(DataProviderFactory);
            DataContext.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            StateHasChanged();
        }
     
        private async Task OnOpenLeftDocumentDialogClose(bool accepted)
        {
            if (accepted)
            {
                // bei OK
                DataContext.OnOpenLeftDocumentDialogClose.Execute(this);
            }
            else 
            {
                // bei Abbruch
            }

            DataContext.ShowOpenLeftHierarchyDialog = false;
            StateHasChanged();


        }

        private async Task OnOpenRightDocumentDialogClose(bool accepted)
        {
             if (accepted)
             {
                // bei OK
                DataContext.OnOpenRightDocoumentDialogClose.Execute(this);
             }
             else
             {

             }

            DataContext.ShowOpenRightHierarchyDialog = false;
            StateHasChanged();
        }
    }   
}


