using MDD4All.SpecIF.DataFactory;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using MDD4All.SpecIF.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using MDD4All.SpecIF.DataModels.Manipulation;


namespace SpecIFicator.DefaultPlugin.BlazorComponents
{
    public partial class ProjectViewer
    {
        [CascadingParameter]
        public string ProjectID { get; set; }

        [Inject]
        private IStringLocalizer<ProjectViewer> L { get; set; }

        [Inject]
        private ISpecIfDataProviderFactory DataProviderFactory { get; set; }

        public ProjectViewModel DataContext { get; set; }
        private ISpecIfMetadataReader MetadataReader { get; set; }
        private ISpecIfDataReader DataReader { get; set; }
        private ISpecIfDataWriter DataWriter { get; set; }
        private Key SelectedResourceClassKey { get; set; }
        private bool ShowNewHierarchyDialog { get; set; }
        private ResourceViewModel NewHierarchyViewModel { get; set; }

        protected override void OnInitialized()
        {
            MetadataReader = DataProviderFactory.MetadataReader;
            DataReader = DataProviderFactory.DataReader;
            DataWriter = DataProviderFactory.DataWriter;

            if (AvailableResourceClasses != null && AvailableResourceClasses.Any())
            {
                ResourceClass firstResourceClass = AvailableResourceClasses[0];

                SelectedResourceClassKey = new Key(firstResourceClass.ID, firstResourceClass.Revision);
            }
            if (ProjectID != null)
            {
                foreach (ProjectDescriptor descriptor in DataReader.GetProjectDescriptions())
                {
                    if (descriptor.ID == ProjectID)
                    {
                        DataContext = new ProjectViewModel(descriptor, MetadataReader, DataWriter, DataReader);
                    }
                }

            }
        }

        private List<ResourceClass> AvailableResourceClasses
        {
            get
            {
                List<ResourceClass> result = new List<ResourceClass>();

                result = MetadataReader.GetAllResourceClasses();

                return result;
            }
        }
        private void OnNewHierarchyButtonClicked()
        {
            Resource hierarchyResource = SpecIfDataFactory.CreateResource(SelectedResourceClassKey, MetadataReader);

            NewHierarchyViewModel = new ResourceViewModel(MetadataReader, DataReader, DataWriter, hierarchyResource);
            NewHierarchyViewModel.IsInEditMode = true;

            ShowNewHierarchyDialog = true;
            StateHasChanged();
        }


        private void OnNewHierarchyDialogClose(bool accepted)
        {
            if (accepted)
            {
                DataContext.CreateNewHierarchyCommand.Execute(NewHierarchyViewModel.Resource);
                this.OnInitialized();
            }
            ShowNewHierarchyDialog = false;
            StateHasChanged();
        }

        private void IsLoadingChanged(bool value)
        {
            if (value == false)
            {
                InvokeAsync(() => StateHasChanged());
            }
        }
    }
}
