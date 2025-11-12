using GalaSoft.MvvmLight;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataModels.Manipulation;
using MDD4All.SpecIF.DataProvider.Contracts;
using MDD4All.SpecIF.ViewModels.Cache;
using MDD4All.UI.DataModels.Tree;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using MDD4All.SpecIF.DataFactory;
using MDD4All.SpecIF.DataModels.Helpers;
using MDD4All.SpecIF.ViewModels;

namespace SpecIFicator.DefaultPlugin.ViewModels
{
    public class TestSuiteSelectionTreeViewModel : ViewModelBase, ITree
    {

        public TestSuiteSelectionTreeViewModel(ISpecIfDataProviderFactory specIfDataProviderFactory, 
                                               Key key,
                                               string projectID)
        {
            _specIfDataProviderFactory = specIfDataProviderFactory;
            _metadataReader = _specIfDataProviderFactory.MetadataReader;
            _specIfDataWriter = _specIfDataProviderFactory.DataWriter;
            _specIfDataReader = _specIfDataProviderFactory.DataReader;

            InitializeCommands();

            Node hierarchyRoot = _specIfDataReader.GetHierarchyByKey(key, projectID);
            RootNode = InitializeTestSuiteSelectionTree(hierarchyRoot);
            _treeRootNodes = new ObservableCollection<ITreeNode>();
            _treeRootNodes.Add(RootNode);
            SelectedNode = RootNode;
            ProjectID = projectID;


            Key testSuiteKey = new Key("RC-TestSuite", "1.1");

            Resource testSuiteResource = SpecIfDataFactory.CreateResource(testSuiteKey, _metadataReader);

            TestSuiteUnderEdit = new ResourceViewModel(_metadataReader, _specIfDataReader, _specIfDataWriter, testSuiteResource);

            TestSuiteUnderEdit.IsInEditMode = true;

            ShowNewTestSuite = true;
        }

        public bool ShowNewTestSuite { get; set; } = true;
        
        private void InitializeCommands()
        {
            EditNewTestSuiteCommand = new RelayCommand(ExecuteEditNewTestSuite);
            CreateTestSuiteCommand = new RelayCommand(ExecuteCreateTestsuite);
        }

        public Key TestSuiteHierarchyKey { get; set; }

        public string ProjectID { get; set; }

        public ResourceViewModel TestSuiteUnderEdit { get; set; }

        private void ExecuteEditNewTestSuite()
        {
            
        }

        private void ExecuteCreateTestsuite()
        {
            // hier musst du programmieren

            if (SelectedNode != null)
            {
                Node testSuiteNode = new Node();

                testSuiteNode.Nodes = new List<Node>();

                Resource testSuiteResource = TestSuiteUnderEdit.Resource;

                testSuiteNode.ResourceReference = new Key(testSuiteResource.ID, testSuiteResource.Revision);

                testSuiteResource.SetPropertyValue(
                                                    new Key
                                                    {
                                                        ID = "PC-TestVerdict",
                                                        Revision = "1.1"
                                                    },
                                                    new Value
                                                    {
                                                        StringValue = "V-Verdict-0"
                                                    }
                                                   );

                _specIfDataWriter.AddResource(testSuiteResource, ProjectID);

                // Knoten & Resource speichern

                foreach (TestResourceNodeViewModel checkedNode in CheckedNodes)
                {
                    // 1. Neuen Knoten anlegen
                    //Node testCaseNode = new Node();

                    // 2. Methode CopyTestCase() aufrufen

                    if (checkedNode.ReferencedResource.ResourceClassID == "RC-TestCase")
                    {
                         CopyTestCase(checkedNode.Node, testSuiteNode);
                    }
                    // 3. Resultat-Knoten in Baum hängen
                    //testSuiteNode.Nodes.Add(testCaseNode);
                }
                // test suite als neue Hierarchy speichern
                _specIfDataWriter.AddHierarchy(testSuiteNode, ProjectID);

                TestSuiteHierarchyKey = new Key(testSuiteNode.ID, testSuiteNode.Revision);
            }
        }

        public void CopyTestCase(Node node, Node currentTarget)
        {

            // 2. Resource für Quellkonten (node) holen 
            Resource sourceResource = _specIfDataReader.GetResourceByKey(node.ResourceReference);

            // 3. Resource kopieren in eine neue Resource
            Resource resourceCopy = sourceResource.CreateNewRevisionForEdit(_metadataReader);
            resourceCopy.ID = SpecIfGuidGenerator.CreateNewSpecIfGUID();

            // set all test results to "not tested"
            resourceCopy.SetPropertyValue(new Key
                                                {
                                                    ID = "PC-TestVerdict",
                                                    Revision = "1.1"
                                                },
                                                new Value
                                                {
                                                    StringValue = "V-Verdict-0"
                                                }
                                           );

            // Neuen Target Knoten erzeugen
            Node newTargetNode = new Node();

            // 4. Kopierte Resource als Referenz in den neuen Knoten hängen
            newTargetNode.ResourceReference = new Key(resourceCopy.ID, resourceCopy.Revision);
            // neuen Knoten an den currentTarget Knoten hängen
            currentTarget.Nodes.Add(newTargetNode);

            // 5. Kopierte Resource speichern. 
            _specIfDataWriter.AddResource(resourceCopy, ProjectID);

            // 6. Rekursive Aufruf für die Kindknoten

            foreach (Node childNode in node.Nodes)
            {
                CopyTestCase(childNode, newTargetNode);
            }
        }

        private void SaveNewHierarchy(Node parent)
        {
            for (int index = 0; index < parent.Nodes.Count; index++)
            {
                Node childNode = parent.Nodes[index];
                if (index == 0)
                {
                    _specIfDataWriter.AddNodeAsFirstChild(parent.ID, childNode, ProjectID);
                }
                else
                {
                    Node predecessorNode = parent.Nodes[index - 1];

                    _specIfDataWriter.AddNodeAsPredecessor(predecessorNode.ID, childNode, ProjectID); //Recursiveanker ?
                }
                SaveNewHierarchy(childNode); // Rekursive Methode
            }
        }

        public TestResourceNodeViewModel InitializeTestSuiteSelectionTree(Node hierarchyRootNode)
        {
            TestResourceNodeViewModel result = null;//zuerst wert ist null bei dieser Variable.

            // hier den Baum aufbauen!!

            Node testModel = new Node();

            FindTestModel(hierarchyRootNode, ref testModel);

            TestResourceNodeViewModel testModelNode = new TestResourceNodeViewModel(_metadataReader,
                                                                                    _specIfDataReader,
                                                                                    _specIfDataWriter,
                                                                                    this,
                                                                                    testModel);
            testModelNode.IsExpanded = true;
            List<Node> testCases = new List<Node>();
            FindAllTestCases(hierarchyRootNode, ref testCases);

            foreach (Node testCase in testCases)
            {
                TestResourceNodeViewModel testResourceNodeViewModel = new TestResourceNodeViewModel(_metadataReader,
                                                                                                     _specIfDataReader,
                                                                                                     _specIfDataWriter,
                                                                                                     this,
                                                                                                     testCase);

                testResourceNodeViewModel.Parent = testModelNode;
                testModelNode.Children.Add(testResourceNodeViewModel);
            }
            result = testModelNode;
            return result;//node1 wird zurückgegeben.
        }

        public void FindTestModel(Node node, ref Node result)
        {
            ResourceViewModel resourceViewModel = CachedViewModelFactory.GetResourceViewModel(node.ResourceReference,
                                                                                              _metadataReader,
                                                                                              _specIfDataReader,
                                                                                              _specIfDataWriter);
            //finde nur TestModel
            if (resourceViewModel.ResourceClassID == "RC-TestModel")
            {
                result = node;
            }
            foreach (Node childNode in node.Nodes)
            {
                FindTestModel(childNode, ref result);
            }
        }
        //methode schreiben , um alle testCases in der hirarchy zu finden.
        public void FindAllTestCases(Node node, ref List<Node> result)
        {
            ResourceViewModel resourceViewModel = CachedViewModelFactory.GetResourceViewModel(node.ResourceReference,
                                                                                              _metadataReader,
                                                                                              _specIfDataReader,
                                                                                              _specIfDataWriter);

            //finde nur TestCases
            if (resourceViewModel.ResourceClassID == "RC-TestCase")
            {
                result.Add(node);
            }
            foreach (Node childNode in node.Nodes)
            {
                FindAllTestCases(childNode, ref result);
            }
        }
        public List<TestResourceNodeViewModel> CheckedNodes
        {
            get
            {
                List<TestResourceNodeViewModel> result = new List<TestResourceNodeViewModel>();
                TestResourceNodeViewModel testResourceRootNodeViewModel = _treeRootNodes[0] as TestResourceNodeViewModel;
                FindAllCheckedNodes(testResourceRootNodeViewModel, ref result);
                return result;
            }
        }
        
        public void FindAllCheckedNodes(TestResourceNodeViewModel child, ref List<TestResourceNodeViewModel> result)
        {
            if (child.IsChecked)
            {
                result.Add(child);
            }

            foreach (TestResourceNodeViewModel childNode in child.Children)
            {
                FindAllCheckedNodes(childNode, ref result);
            }

            
        }
        public void CheckAllNodes(TestResourceNodeViewModel testResourceNodeViewModel)
        {
            testResourceNodeViewModel.IsChecked = true;

            foreach (TestResourceNodeViewModel child in testResourceNodeViewModel.Children)
            {
                CheckAllNodes(testResourceNodeViewModel);
            }
        }
        private ISpecIfDataProviderFactory _specIfDataProviderFactory;
        
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
        public TestResourceNodeViewModel RootNode { get; set; }

        private ObservableCollection<ITreeNode> _treeRootNodes = new ObservableCollection<ITreeNode>();

        public ObservableCollection<ITreeNode> TreeRootNodes
        {
            get
            {
                return _treeRootNodes;
            }
        }
        
        private ITreeNode _selectedNode;

        public ITreeNode SelectedNode
        {
            get
            {
                return _selectedNode;
            }

            set
            {
                _selectedNode = value;
                RaisePropertyChanged("SelectedNode");
            }
        }
        
        public ICommand EditNewTestSuiteCommand { get; set; }
        
        public ICommand CreateTestSuiteCommand { get; set; }

    }
}
