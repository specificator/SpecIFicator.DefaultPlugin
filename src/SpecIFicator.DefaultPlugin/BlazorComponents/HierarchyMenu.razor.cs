using MDD4All.SpecIF.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace SpecIFicator.DefaultPlugin.BlazorComponents
{
    public partial class HierarchyMenu
    {
        [Inject]
        private IStringLocalizer<HierarchyMenu> L { get; set; }

        [Parameter]
        public string HierarchyKeyString { get; set; }

        [Parameter]
        public ProjectViewModel ProjectViewModel { get; set; }

        [Parameter]
        public EventCallback<bool> ShowChanged { get; set; }

        private bool show = false;

        private async Task OutFocus()
        {
            await Task.Delay(200);
            this.show = false;
        }
        private void OnDeleteHierarchyButtonClicked()
        {
            ProjectViewModel.DeleteHierarchyCommand.Execute(HierarchyKeyString);
            ShowChanged.InvokeAsync(false);
        }
    }
}