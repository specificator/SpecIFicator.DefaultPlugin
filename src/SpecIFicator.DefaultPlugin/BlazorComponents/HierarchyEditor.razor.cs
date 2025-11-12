using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using MDD4All.SpecIF.ViewModels;
using Microsoft.AspNetCore.Components;
using MDD4All.SpecIF.DataModels.Manipulation;
using SpecIFicator.Framework.Configuration;
using Microsoft.Extensions.Localization;
using SpecIFicator.DefaultPlugin.ViewModels;

namespace SpecIFicator.DefaultPlugin.BlazorComponents
{
    public partial class HierarchyEditor
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
        public string DataContext { get; set; } = null!;

        public DefaultPluginHierarchyViewModel? HierarchyViewModel { get; set; } = null;

        private NodeViewModel? SelectedNode { get; set; }

        private Type? _toolbarType = null;

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

                HierarchyViewModel = new DefaultPluginHierarchyViewModel(DataProviderFactory, key, projectID);

                HierarchyViewModel.PropertyChanged += OnStateChanged;


                InitializeTypes();

            }
        }

        private void InitializeTypes()
        {
            if (_toolbarType == null && HierarchyViewModel?.RootNode.RootResourceClassKey != null)
            {
                _toolbarType = DynamicConfigurationManager.GetComponentType("MainToolBar",
                                                                        GetType().FullName,
                                                                        HierarchyViewModel.RootNode.RootResourceClassKey);

                _treeType = DynamicConfigurationManager.GetComponentType("Tree",
                                                                            GetType().FullName,
                                                                            HierarchyViewModel
                                                                            .RootNode.RootResourceClassKey);

                _hierarchyViewType = DynamicConfigurationManager.GetComponentType("HierarchyView",
                                                                                    GetType().FullName,
                                                                                    HierarchyViewModel.RootNode.RootResourceClassKey);
            }
        }

        private void OnStateChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "StateChanged")
            {
                InitializeTypes();

                InvokeAsync(() =>
                {
                    StateHasChanged();
                }
                );

            }
        }
    }
}
