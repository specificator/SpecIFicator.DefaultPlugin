using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using SpecIFicator.Framework.Configuration;
using SpecIFicator.Framework.Configuration.DataModels;

namespace SpecIFicator.DefaultPlugin.BlazorComponents.MetadataEditor
{
    public partial class MetadataNavigation
    {
        [Inject]
        public IStringLocalizerFactory LocalizerFactory { get; set; }

        [Inject]
        private IStringLocalizer<MetadataEditorPage> L { get; set; }

        [Parameter]
        public Type SelectedComponentType { get; set; }

        [Parameter]
        public EventCallback<Type> SelectedComponentTypeChanged { get; set; }

        private List<ComponentDefinition> ComponentDefinitions { get; set; } = new List<ComponentDefinition>();

        private bool collapseNavMenu = true;
        private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;
        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        private async Task UpdateSelectedTypeAsync(Type type)
        {
            await SelectedComponentTypeChanged.InvokeAsync(type);
        }

        protected override void OnInitialized()
        {
            DynamicComponentConfiguration configuration = DynamicConfigurationManager.GetDynamicComponentConfiguration(GetType().FullName);

            if (configuration != null)
            {
                foreach (ComponentDefinition? component in configuration.Components)
                {
                    if (component != null)
                    {
                        ComponentDefinitions.Add(component);
                    }
                }
            }
        }
    }
}