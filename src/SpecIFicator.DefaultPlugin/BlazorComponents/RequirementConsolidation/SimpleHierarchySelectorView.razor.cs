using Microsoft.AspNetCore.Components;
using MDD4All.SpecIF.ViewModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using SpecIFicator.DefaultPlugin.ViewModels;

namespace SpecIFicator.DefaultPlugin.BlazorComponents.RequirementConsolidation
{
    public partial class SimpleHierarchySelectorView
    {
        private ProjectsViewModel _projectsViewModel;

        [Inject]
        private ISpecIfDataProviderFactory DataProviderFactory { get; set; }

        private ISpecIfMetadataReader MetadataReader { get; set; }

        private ISpecIfDataReader DataReader { get; set; }

        private ISpecIfDataWriter DataWriter { get; set; }

        [Parameter]
        public ConsolidationViewModel DataContext { get; set; }

        protected override void OnInitialized()
        {
            MetadataReader = DataProviderFactory.MetadataReader;
            DataReader = DataProviderFactory.DataReader;
            DataWriter = DataProviderFactory.DataWriter;

            _projectsViewModel = new ProjectsViewModel(MetadataReader, DataWriter, DataReader);
        }

        private void OnRadioButtonClicked(string nodeID)
        {
            DataContext.OpenNodeID = nodeID;
        }
    }
}