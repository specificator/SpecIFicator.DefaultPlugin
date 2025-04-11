using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using SpecIFicator.DefaultPlugin.ViewModels;

namespace SpecIFicator.DefaultPlugin.BlazorComponents
{
    public partial class ResourceEditor
    {
        [CascadingParameter]
        public DefaultPluginHierarchyViewModel DataContext { get; set; }

        [Inject]
        private IStringLocalizer<MainToolBar> L { get; set; }

        private async Task OnEditDialogClose(bool accepted)
        {
            if (accepted)
            {
                DataContext.ConfirmEditResourceCommand.Execute(null);
            }
            else
            {
                DataContext.CancelEditResourceCommand.Execute(null);
            }
            StateHasChanged();
        }
    }
}