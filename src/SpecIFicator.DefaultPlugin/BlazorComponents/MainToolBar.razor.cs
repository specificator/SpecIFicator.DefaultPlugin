using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MDD4All.SpecIF.ViewModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using SpecIFicator.DefaultPlugin.ViewModels;

namespace SpecIFicator.DefaultPlugin.BlazorComponents
{
    public partial class MainToolBar
    {
        [Inject]
        private IStringLocalizer<MainToolBar> L { get; set; }

        [Inject]
        private ISpecIfDataProviderFactory DataProviderFactory { get; set; }

        [CascadingParameter]
        public DefaultPluginHierarchyViewModel DataContext { get; set; }

        private string _projectID = "PRJ-DEFAULT";

        protected override void OnInitialized()
        {
            DataContext.PropertyChanged += OnPropertyChanged;
            _projectID = DataContext.DataReader.GetProjectIDFromNodeID(DataContext.RootNode.NodeID).ToString();
        }

        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            StateHasChanged();
        }

        private void OnEditResourceClicked()
        {
            DataContext.StartEditResourceCommand.Execute(HierarchyViewModel.EDIT_EXISTING);
            StateHasChanged();
        }

        private void OnNewChildResourceClicked()
        {
            DataContext.StartEditResourceCommand.Execute(HierarchyViewModel.NEW_CHILD);
            StateHasChanged();
        }

        private void OnNewResourceBelowClicked()
        {
            DataContext.StartEditResourceCommand.Execute(HierarchyViewModel.NEW_BELOW);
            StateHasChanged();
        }

        private void OnNewResourceAboveClicked()
        {
            DataContext.StartEditResourceCommand.Execute(HierarchyViewModel.NEW_ABOVE);
            StateHasChanged();
        }

        private void OnDeleteResourceClicked()
        {
            DataContext.StartDeleteResourceCommand.Execute(null);
            StateHasChanged();
        }

        private void OnAddStatementClicked()
        {
            DataContext.StartAddStatementCommand.Execute(null);
            StateHasChanged();
        }

        private void OnNewCommentClicked()
        {
            DataContext.ShowCommentsCommand.Execute(null);
            StateHasChanged();
        }

        private async Task OnAddStatementDialogClose(bool accepted)
        {
            if (accepted)
            {
                DataContext.ConfirmAddStatementCommand.Execute(null);
            }
            else
            {
                DataContext.CancelAddStatementCommand.Execute(null);
            }
            StateHasChanged();
        }

        

        private async Task OnDeleteDialogClose(bool accepted)
        {
            if (accepted)
            {
                DataContext.DeleteResourceCommand.Execute(null);
            }
            else
            {
                DataContext.CancelDeleteResourceCommand.Execute(null);
            }
            StateHasChanged();
        }

        private async Task OnCommentDialogClose(bool accepted)
        {
            DataContext.ShowComments = false;
            StateHasChanged();
        }
    }
}