using Microsoft.AspNetCore.Components;
using MDD4All.SpecIF.ViewModels;
using MDD4All.UI.BlazorComponents.Services;

namespace SpecIFicator.DefaultPlugin.BlazorComponents
{
    public partial class ResourcePanel
    {
        [Inject]
        public ClipboardDataProvider ClipboardDataProvider { get; set; }
        
        [Parameter]
        public HierarchyViewModel HierarchyViewModel { get; set; }

        [Parameter]
        public ResourceViewModel ResourceViewModel 
        {
            get; set;
        }

        [Parameter]
        public bool IsMultilinguismEnabled { get; set; } = false;

        [Parameter]
        public string PrimaryLanguage { get; set; } = "en";

        [Parameter]
        public string SecondaryLanguage { get; set; } = "de";

        private string CopyButtonIconClass { get; set; } = "bi bi-clipboard-fill";

        protected override void OnInitialized()
        {
            
        }

        private async void CopyKeyToClipboardAsync()
        {
            string copyIcon = CopyButtonIconClass;
            CopyButtonIconClass = "bi bi-check";
            await ClipboardDataProvider.WriteTextAsync(ResourceViewModel.Key.ToString());
            await Task.Delay(TimeSpan.FromSeconds(2));
            CopyButtonIconClass = copyIcon;
            StateHasChanged();
        }
    }
}