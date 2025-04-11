using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.ViewModels;
using SpecIFicator.DefaultPlugin.ViewModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using SpecIFicator.Framework.Services;
using SpecIFicator.Framework.Services.DataModels;

namespace SpecIFicator.DefaultPlugin.BlazorComponents.RequirementConsolidation
{
    public partial class DocumentConsolidationItemContainer
    {
        [Inject]
        private ISpecIfDataProviderFactory DataProviderFactory { get; set; }

        [Inject]
        public BrowserDimensionService BrowserDimensionService { get; set; }

        [Parameter]
        public NodeViewModel DataContext { get; set; }

        private HierarchyViewModel ParentViewModel { get; set; }

        [Parameter]
        public ConsolidationViewModel ConsolidationViewModel { get; set; }

        public bool IsInList { get; set; } = false;

        [Parameter]
        public EventCallback OnLoadingFinished { get; set; }

        ElementReference ElementReference { get; set; }

        private DesktopObjectViewModel ObjectUnderTouch { get; set; }

        private void OnSelectResource(NodeViewModel node)
        {
            node.Tree.SelectedNode = node;
        }

        private string ResourceSelectedStyle = "selected";

        private string ResourceUnselectedStyle = "unselected";

        protected override void OnInitialized()
        {
            DataContext.PropertyChanged += DataContextPropertyChanged;

            if (DataContext.Tree != null && DataContext.Tree is HierarchyViewModel)
            {
                ParentViewModel = (HierarchyViewModel)DataContext.Tree;
            }
        }

        private void OnCommentButtonClick()
        {
            if (ParentViewModel is DefaultPluginHierarchyViewModel)
            {
                DefaultPluginHierarchyViewModel viewModel = (DefaultPluginHierarchyViewModel)ParentViewModel;

                viewModel.ShowCommentsCommand.Execute(null);
            }
        }

        private void DataContextPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsLoading")
            {
                if (DataContext.IsLoading == false)
                {
                    InvokeAsync(() =>
                    {
                        OnLoadingFinished.InvokeAsync();
                    });
                }
            }
        }

        bool _isDragFromLeft = false;

        public async void OnDragStart(DragEventArgs args)
        {
            BrowserDimension browserDimension = await BrowserDimensionService.GetBrowserDimension();
            if (args.PageX < 250 )
            {
                DesktopObjectViewModel desktopObjectViewModel = new DesktopObjectViewModel();


                desktopObjectViewModel.OffsetX = (int)args.PageX;
                desktopObjectViewModel.OffsetY = (int)args.PageY;

                _isDragFromLeft = true;
            }
        }

        public async void OnDragEnd(DragEventArgs args)
        {
            BrowserDimension browserDimension = await BrowserDimensionService.GetBrowserDimension();


            if (args.PageX > 250 && args.PageX < browserDimension.Width - 250 && _isDragFromLeft)
            {

                DesktopObjectViewModel desktopObjectViewModel = new DesktopObjectViewModel();

                desktopObjectViewModel.OffsetX = (int)args.PageX;
                desktopObjectViewModel.OffsetY = (int)args.PageY;


                Node newNode = new Node();
                newNode.ResourceReference = new Key(DataContext.ReferencedResource.Key.ID, DataContext.ReferencedResource.Key.Revision);

                desktopObjectViewModel.NodeViewModel = new NodeViewModel(DataProviderFactory.MetadataReader,
                                                                         DataProviderFactory.DataReader,
                                                                         DataProviderFactory.DataWriter,
                                                                         null,
                                                                         newNode);

                desktopObjectViewModel.Parent = ConsolidationViewModel;

                ConsolidationViewModel.DesktopObjectViewModels.Add(desktopObjectViewModel);


                ConsolidationViewModel.RefreshView();

                ;

                _isDragFromLeft = false;
            }
        }

        public void OnTouchStart(TouchEventArgs t)
        {
            if (t.TargetTouches.Length > 0)
            {
                TouchPoint StartTouchPoint = t.TargetTouches[0];
                if (StartTouchPoint.PageX < 250)
                {


                    ObjectUnderTouch = new DesktopObjectViewModel();


                    ObjectUnderTouch.OffsetX = (int)StartTouchPoint.PageX;
                    ObjectUnderTouch.OffsetY = (int)StartTouchPoint.PageY;

                    IsInList = false;
                }
                else
                {
                    IsInList = true;
                }
            }

        }

        public async void OnTouchMove(TouchEventArgs t)
        {
            
            if (t.TargetTouches.Length > 0)
            {
                BrowserDimension browserDimension = await BrowserDimensionService.GetBrowserDimension();

                TouchPoint MoveTouchPoint = t.TargetTouches[0];

                if (ObjectUnderTouch != null)
                {
                    ObjectUnderTouch.OffsetX = (int)t.TargetTouches[0].PageX;
                    ObjectUnderTouch.OffsetY = (int)t.TargetTouches[0].PageY;
                }

                if (MoveTouchPoint.PageX > 250 && MoveTouchPoint.PageX < browserDimension.Width - 250)
                {

                    if (IsInList == false)
                    {
                        Node newNode = new Node();
                        newNode.ResourceReference = new Key(DataContext.ReferencedResource.Key.ID, DataContext.ReferencedResource.Key.Revision);

                        ObjectUnderTouch.NodeViewModel = new NodeViewModel(DataProviderFactory.MetadataReader,
                                                                                 DataProviderFactory.DataReader,
                                                                                 DataProviderFactory.DataWriter,
                                                                                 null,
                                                                                 newNode);

                        ObjectUnderTouch.Parent = ConsolidationViewModel;

                        ConsolidationViewModel.DesktopObjectViewModels.Add(ObjectUnderTouch);

                        ConsolidationViewModel.RefreshView();


                        StateHasChanged();

                        IsInList = true;
                    }
                }
            }
        }
    }
}

