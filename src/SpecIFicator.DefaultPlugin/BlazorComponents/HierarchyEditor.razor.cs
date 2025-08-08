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
        private IStringLocalizer<HierarchyEditor> L { get; set; }

        [Inject]
        private ISpecIfDataProviderFactory DataProviderFactory { get; set; }

       
        private ISpecIfMetadataReader MetadataReader { get; set; }

        
        private ISpecIfDataReader DataReader { get; set; }

        
        private ISpecIfDataWriter DataWriter { get; set; }

        // the hierarchy key
        [CascadingParameter]
        public string DataContext { get; set; }

        public DefaultPluginHierarchyViewModel HierarchyViewModel { get; set; }

        private NodeViewModel SelectedNode { get; set; }

        private Type _toolbarType;

        private Type _treeType;

        private Type _hierarchyViewType;


        protected override void OnInitialized()
        {
            MetadataReader = DataProviderFactory.MetadataReader;
            DataReader = DataProviderFactory.DataReader;
            DataWriter = DataProviderFactory.DataWriter;

            if (DataContext != null)
            {
                Key key = new Key();
                key.InitailizeFromKeyString(DataContext);

                HierarchyViewModel = new DefaultPluginHierarchyViewModel(DataProviderFactory, key);

                HierarchyViewModel.PropertyChanged += OnStateChanged;

                _toolbarType = DynamicConfigurationManager.GetComponentType("MainToolBar",
                                                                            GetType().FullName,
                                                                            HierarchyViewModel
                                                                            .RootNode.RootResourceClassKey);

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
                StateHasChanged();
            }
        }
    }
}
