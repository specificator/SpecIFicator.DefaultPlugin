using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using MDD4All.SpecIF.ViewModels;
using MDD4All.SpecIF.ViewModels.Cache;
using MDD4All.UI.DataModels.Tree;
using System.Collections.ObjectModel;

namespace SpecIFicator.DefaultPlugin.ViewModels
{
    public class TestResourceNodeViewModel : ITreeNode
    {

        public TestResourceNodeViewModel(ISpecIfMetadataReader metadataReader,
                                         ISpecIfDataReader dataReader,
                                         ISpecIfDataWriter dataWriter,
                                         ITree tree,
                                         Node node)
        {
            _metadataReader = metadataReader;
            _specIfDataReader = dataReader;
            _specIfDataWriter = dataWriter;
            Tree = tree;
            Node = node;

            InitializeReferencedResource(node.ResourceReference);
        }

        private void InitializeReferencedResource(Key key)
        {
            Task.Run(() => InitializeReferencedResourceAsync(key));
        }


        private async void InitializeReferencedResourceAsync(Key key)
        {
            await Task.Run(() =>
            {
                ResourceViewModel resourceViewModel = CachedViewModelFactory.GetResourceViewModel(key,
                                                                             MetadataReader,
                                                                             DataReader,
                                                                             DataWriter);

                //Task.Delay(1500).Wait();

                ReferencedResource = resourceViewModel;

                //ReferencedResourceInitialized = true;
                //RaisePropertyChanged("IsLoading");
                TreeStateChanged?.Invoke(this, EventArgs.Empty);
            });

        }

        public string Title
        {
            get
            {
                return ReferencedResource.Title;
            }
        }

        private ISpecIfMetadataReader _metadataReader;

        public ISpecIfMetadataReader MetadataReader
        {
            get { return _metadataReader; }

        }

        private ISpecIfDataReader _specIfDataReader;

        public ISpecIfDataReader DataReader
        {
            get
            {
                return _specIfDataReader;
            }
        }

        private ISpecIfDataWriter _specIfDataWriter;


        public ISpecIfDataWriter DataWriter
        {
            get
            {
                return _specIfDataWriter;
            }
        }

        public Node Node { get; set; }

        public ResourceViewModel ReferencedResource { get; set; }

        public ITree Tree { get; }

        public ObservableCollection<ITreeNode> Children { get; } = new ObservableCollection<ITreeNode>();

        public ITreeNode Parent { get; set; }

        public int Index
        {
            get
            {
                int result = -1;
                if (Parent != null)
                {
                    for (int i = 0; i < Parent.Children.Count; i++)
                    {//linke seite wird geschrieben, rechte seite wird gelesen.

                        ITreeNode node = Parent.Children[i];
                        if (node == this)
                        {
                            result = i;
                            break;
                        }
                    }
                }
                else
                {
                    result = 0;
                }

                return result;
            }
        }

        public bool HasChildNodes
        {
            get
            {
                bool result = false;

                if (Children.Count > 0)
                {
                    result = true;
                }
                return result;
            }
        }

        public bool IsExpanded { get; set; }

        public bool IsSelected
        {
            get
            {
                bool result = false;
                if (Tree.SelectedNode == this)
                {
                    result = true;
                }
                return result;
            }
            set
            {
            }
        }
        public bool IsLoading { get; set; }

        public bool IsDisabled { get; set; }

        public event EventHandler TreeStateChanged;

        private bool _isChecked = false;
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
            }
        }

        public string DragDropOperationInformation => throw new NotImplementedException();

        public void CheckCurrentNodeAndAllChildNodes(TestResourceNodeViewModel node, bool isChecked)
        {
            node.IsChecked = isChecked;
            foreach (TestResourceNodeViewModel childNode in node.Children)
            {
                CheckCurrentNodeAndAllChildNodes(childNode, isChecked);
            }
        }

        private TestResourceNodeViewModel GetTestResourceNodeViewModel(int index)
        {
            return (TestResourceNodeViewModel)Children[index];
        }
    }
}
