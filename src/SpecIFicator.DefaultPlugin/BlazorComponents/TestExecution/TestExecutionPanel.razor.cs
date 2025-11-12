using Microsoft.AspNetCore.Components;
using MDD4All.SpecIF.DataModels.Manipulation;
using Microsoft.Extensions.Localization;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.ViewModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using SpecIFicator.Framework.Configuration;

namespace SpecIFicator.DefaultPlugin.BlazorComponents.TestExecution
{
    public partial class TestExecutionPanel
    {
        [Inject]
        private IStringLocalizer<HierarchyEditor> L { get; set; } = null!;

        [Inject]
        private ISpecIfDataProviderFactory DataProviderFactory { get; set; } = null!;


        private ISpecIfMetadataReader MetadataReader { get; set; } = null!;


        private ISpecIfDataReader DataReader { get; set; } = null!;


        private ISpecIfDataWriter DataWriter { get; set; } = null!;

        // the hierarchy key
        [CascadingParameter]
        public string? DataContext { get; set; }

        public HierarchyViewModel HierarchyViewModel { get; set; } = null!;

        private NodeViewModel? SelectedNode { get; set; }

        private Type? _treeType = null;

        private Type? _hierarchyViewType = null;


        protected override void OnInitialized()
        {
            MetadataReader = DataProviderFactory.MetadataReader;
            DataReader = DataProviderFactory.DataReader;
            DataWriter = DataProviderFactory.DataWriter;

            if (DataContext != null)
            {
                string hierarchyKeyString = "";
                string projectID = "PRJ-DEFAULT";
                if (DataContext != null)
                {
                    if (DataContext.Contains("/"))
                    {
                        string[] tokens = DataContext.Split("/");
                        hierarchyKeyString = tokens[0];
                        projectID = tokens[1];
                    }
                    else
                    {
                        hierarchyKeyString = DataContext;
                    }
                    Key key = new Key();
                    key.InitailizeFromKeyString(hierarchyKeyString);

                    HierarchyViewModel = new HierarchyViewModel(DataProviderFactory, key, projectID);

                    HierarchyViewModel.PropertyChanged += OnStateChanged;

                    InitializeTypes();
                }

            }
        }

        private void InitializeTypes()
        {
            if (HierarchyViewModel.RootNode.RootResourceClassKey != null)
            {

                _treeType = DynamicConfigurationManager.GetComponentType("TestExecutionTree",
                                                                         GetType().FullName!,
                                                                         HierarchyViewModel
                                                                            .RootNode.RootResourceClassKey);

                _hierarchyViewType = DynamicConfigurationManager.GetComponentType("TestExecutionView",
                                                                                  GetType().FullName!,
                                                                                  HierarchyViewModel.RootNode.RootResourceClassKey);

            }
        }

        private void OnStateChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "StateChanged")
            {
                if(_treeType == null && HierarchyViewModel.RootNode.ReferencedResourceInitialized)
                {
                    InitializeTypes();
                }
                InvokeAsync(() =>
                {
                    StateHasChanged();
                });
                
            }
        }
    }
}