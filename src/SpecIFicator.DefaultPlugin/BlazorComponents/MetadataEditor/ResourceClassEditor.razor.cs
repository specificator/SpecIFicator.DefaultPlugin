using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataModels.Manipulation;
using MDD4All.SpecIF.ViewModels.Metadata;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace SpecIFicator.DefaultPlugin.BlazorComponents.MetadataEditor
{
    public partial class ResourceClassEditor
    {
        [Inject]
        private IStringLocalizer<MetadataEditorPage> L { get; set; }

        [Parameter]
        public ResourceClassesViewModel DataContext { get; set; }

        protected override void OnInitialized()
        {
            if (DataContext.PropertyClasses != null && DataContext.PropertyClasses.Any())
            {
                PropertyClassViewModel firstElement = DataContext.PropertyClasses[0];
                Key key = new Key(firstElement.PropertyClass.ID, firstElement.PropertyClass.Revision);

                DataContext.ResourceClassUnderEdit.SelectedPropertyClassToAdd = key;
            }
        }

        private async Task OnPropertySelectionChanged(ChangeEventArgs args)
        {
            string selection = args.Value.ToString();
            if (!string.IsNullOrEmpty(selection))
            {
                DataContext.ResourceClassUnderEdit.SelectedPropertyClassToAdd = new Key();
                DataContext.ResourceClassUnderEdit.SelectedPropertyClassToAdd.InitailizeFromKeyString(selection);


            }
        }
    }
}