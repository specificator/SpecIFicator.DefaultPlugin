using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.ViewModels;
using SpecIFicator.DefaultPlugin.ViewModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using MDD4All.SpecIF.DataModels.Helpers;
using MDD4All.SpecIF.DataModels.Manipulation;
using SpecIFicator.Framework.Services;
using SpecIFicator.Framework.Services.DataModels;

namespace SpecIFicator.DefaultPlugin.BlazorComponents.RequirementConsolidation
{
    public partial class DesktopObjectView
    {
        [Inject]
        public ISpecIfDataProviderFactory DataProviderFactory { get; set; }

        [Inject]
        public BrowserDimensionService BrowserDimensionService { get; set; }


        [Parameter]
        public DesktopObjectViewModel DataContext { get; set; } = new DesktopObjectViewModel();


        private int DimensionX { get; set; }

        public async void OnDragStart(DragEventArgs args)
        {
            BrowserDimension browserDimension = await BrowserDimensionService.GetBrowserDimension();


            if (args.PageX > 250 && args.PageX < browserDimension.Width - 250)
            {
                DataContext.OffsetX = (int)args.PageX;
                DataContext.OffsetY = (int)args.PageY;
            }
        }

        public async void OnDragEnd(DragEventArgs args)
        {
            BrowserDimension browserDimension = await BrowserDimensionService.GetBrowserDimension();


            DimensionX = browserDimension.Width;
            if (args.PageX > 250 && args.PageX < browserDimension.Width - 250)
            {
                DataContext.OffsetX = (int)args.PageX;
                DataContext.OffsetY = (int)args.PageY;
            }


            if (args.PageX > browserDimension.Width - 250)
            {
                // add to right document
                ConsolidationViewModel consolidationViewModel = DataContext.Parent;

                if(consolidationViewModel.HierachyViewModelRight != null)
                {

                    NodeViewModel anchor = consolidationViewModel.HierachyViewModelRight.LinearResourceList.Last();

                    Node newNode = new Node()
                    {
                        ID = SpecIfGuidGenerator.CreateNewSpecIfGUID(),
                        Revision = SpecIfGuidGenerator.CreateNewRevsionGUID(),
                        ChangedAt = DateTime.Now,
                        IsHierarchyRoot = false,

                    };

                    Key resourceKey = new Key(DataContext.NodeViewModel.ReferencedResource.Key.ID, 
                                              DataContext.NodeViewModel.ReferencedResource.Key.Revision);

                    newNode.ResourceReference = resourceKey;

                    NodeViewModel nodeViewModel = new NodeViewModel(DataProviderFactory.MetadataReader,
                                                                    DataProviderFactory.DataReader,
                                                                    DataProviderFactory.DataWriter,
                                                                    consolidationViewModel.HierachyViewModelRight.TreeRootNodes[0].Tree,
                                                                    newNode);


                    if(anchor.Parent == null)
                    {
                        anchor.HierarchyNode.AddChildNode(newNode);
                        anchor.Children.Add(nodeViewModel);

                        nodeViewModel.Parent = anchor;

                        DataProviderFactory.DataWriter.AddNodeAsFirstChild(anchor.NodeID, newNode);

                    }
                    else
                    {
                        NodeViewModel parentNodeViewModel = anchor.Parent as NodeViewModel;

                        if(parentNodeViewModel != null)
                        {
                            

                            parentNodeViewModel.HierarchyNode.Nodes.Insert(anchor.Index + 1, newNode);

                            parentNodeViewModel.Children.Insert(anchor.Index + 1, nodeViewModel);

                            nodeViewModel.Parent = parentNodeViewModel;

                            DataProviderFactory.DataWriter.AddNodeAsPredecessor(anchor.NodeID, newNode);
                        }

                        
                    }

                    

                    // remeove from desktop
                    DataContext.Parent.DesktopObjectViewModels.Remove(DataContext);

                }



                


            } // if

            DataContext.Parent.RefreshView();

        }

        private async void OnTouchStart(TouchEventArgs t)
        {
            if (t.TargetTouches.Length > 0)
            {
                TouchPoint moveTouchPoint = t.TargetTouches[0];

                BrowserDimension browserDimension = await BrowserDimensionService.GetBrowserDimension();

                if (moveTouchPoint.PageX > 250 && moveTouchPoint.PageX < browserDimension.Width - 250)
                {

                    TouchPoint StartTouchPoint = t.TargetTouches[0];
                    DataContext.OffsetX = (int)StartTouchPoint.PageX;
                    DataContext.OffsetY = (int)StartTouchPoint.PageY;

                    _isElementAdded = false;
                }
            }

        }

        private bool _isElementAdded = true;

        public async void OnTouchMove(TouchEventArgs t)
        {
            if (t.TargetTouches.Length > 0)
            {
                
                TouchPoint MoveTouchPoint = t.TargetTouches[0];
                BrowserDimension browserDimension = await BrowserDimensionService.GetBrowserDimension();


                DimensionX = browserDimension.Width;

                // operate on desktop
                if (MoveTouchPoint.PageX > 250 && MoveTouchPoint.PageX < browserDimension.Width - 250)
                {
                    if (DataContext != null)
                    {
                        DataContext.OffsetX = (int)MoveTouchPoint.PageX;
                        DataContext.OffsetY = (int)MoveTouchPoint.PageY;
                    }

                }
                else if(MoveTouchPoint.PageX > browserDimension.Width - 250) // drag to the right
                { 
                    if(!_isElementAdded)
                    { 
                        // add to right document
                        ConsolidationViewModel consolidationViewModel = DataContext.Parent;

                        if (consolidationViewModel.HierachyViewModelRight != null)
                        {

                            NodeViewModel anchor = consolidationViewModel.HierachyViewModelRight.LinearResourceList.Last();

                            Node newNode = new Node()
                            {
                                ID = SpecIfGuidGenerator.CreateNewSpecIfGUID(),
                                Revision = SpecIfGuidGenerator.CreateNewRevsionGUID(),
                                ChangedAt = DateTime.Now,
                                IsHierarchyRoot = false,

                            };

                            Key resourceKey = new Key(DataContext.NodeViewModel.ReferencedResource.Key.ID,
                                                      DataContext.NodeViewModel.ReferencedResource.Key.Revision);

                            newNode.ResourceReference = resourceKey;

                            NodeViewModel nodeViewModel = new NodeViewModel(DataProviderFactory.MetadataReader,
                                                                            DataProviderFactory.DataReader,
                                                                            DataProviderFactory.DataWriter,
                                                                            consolidationViewModel.HierachyViewModelRight.TreeRootNodes[0].Tree,
                                                                            newNode);


                            if (anchor.Parent == null)
                            {
                                anchor.HierarchyNode.AddChildNode(newNode);
                                anchor.Children.Add(nodeViewModel);

                                nodeViewModel.Parent = anchor;

                                DataProviderFactory.DataWriter.AddNodeAsFirstChild(anchor.NodeID, newNode);

                            }
                            else
                            {
                                NodeViewModel parentNodeViewModel = anchor.Parent as NodeViewModel;

                                if (parentNodeViewModel != null)
                                {


                                    parentNodeViewModel.HierarchyNode.Nodes.Insert(anchor.Index + 1, newNode);

                                    parentNodeViewModel.Children.Insert(anchor.Index + 1, nodeViewModel);

                                    nodeViewModel.Parent = parentNodeViewModel;

                                    DataProviderFactory.DataWriter.AddNodeAsPredecessor(anchor.NodeID, newNode);
                                }


                            }



                            // remeove from desktop
                            DataContext.Parent.DesktopObjectViewModels.Remove(DataContext);

                            _isElementAdded = true;
                        }
                    }
                    DataContext.Parent.RefreshView();
                }
            }
        }
    }
}