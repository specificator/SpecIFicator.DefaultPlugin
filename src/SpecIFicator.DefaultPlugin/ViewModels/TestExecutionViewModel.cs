using GalaSoft.MvvmLight.Command;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataModels.Manipulation;
using MDD4All.SpecIF.ViewModels;
using SpecIFicator.DefaultPlugin.ViewModelExtensions;
using System.Windows.Input;

namespace SpecIFicator.DefaultPlugin.ViewModels
{
    public class TestExecutionViewModel
    {
        public TestExecutionViewModel(HierarchyViewModel hierarchyViewModel)

        {
            HierarchyViewModel = hierarchyViewModel;
            InitializeCommands();

        }
        private void InitializeCommands()
        {
            GoToPreviousCommand = new RelayCommand(ExecuteGoToPrevious);
            GoToNextCommand = new RelayCommand(ExecuteGoToNext);
            SaveTestResultCommand = new RelayCommand(ExecuteSaveTestResult, CanSave);
        }

        public HierarchyViewModel HierarchyViewModel { get; set; }

        public ResourceViewModel SelectedResource
        {
            get
            {
                ResourceViewModel result = null;
                if (HierarchyViewModel.SelectedNode != null)
                {
                    NodeViewModel? selectedNode = HierarchyViewModel.SelectedNode as NodeViewModel;

                    result = selectedNode.ReferencedResource;
                }

                return result;
            }
        }

        public NodeViewModel SelectedNode
        {
            get
            {
                NodeViewModel? result = null;

                result = HierarchyViewModel.SelectedNode as NodeViewModel;

                //NodeViewModel? result = HierarchyViewModel.SelectedNode as NodeViewModel; 

                return result;
            }
        }

        public Dictionary<string, string> VerdictValues
        {
            get
            {
                Dictionary<string, string> result = new Dictionary<string, string>();

                result.Add("V-Verdict-0", "None");
                result.Add("V-Verdict-1", "Pass");
                result.Add("V-Verdict-2", "Inconclusive");
                result.Add("V-Verdict-3", "Fail");
                result.Add("V-Verdict-4", "Error");

                return result;
            }
        }

        private Dictionary<Key, NodeViewModel> ChangedNodes = new Dictionary<Key, NodeViewModel>();

        public string Verdict
        {
            get
            {
                string result = "";
                if (SelectedResource != null)
                {
                    if (SelectedResource.ResourceClassID == "RC-TestStep" || SelectedResource.ResourceClassID == "RC-TestCase")
                    {
                        result = SelectedResource.Resource.GetPropertyValue("U2TP:Verdict", SelectedResource.MetadataReader);//aktuelle verdict 
                    }
                }
                return result;
            }

            set
            {
                if (SelectedResource != null)
                {
                    List<PropertyViewModel> properties = SelectedResource.Properties;

                    foreach (PropertyViewModel property in properties)
                    {
                        if (property.PropertyClass.ID == "PC-TestVerdict")
                        {
                            if (!ChangedNodes.ContainsKey(SelectedNode.HierarchyKey))
                            {
                                ChangedNodes.Add(SelectedNode.HierarchyKey, SelectedNode);
                            }

                            property.SetSingleEnumerationValue(value);
                            HierarchyViewModel.StateChanged = true;
                            break;
                        }
                    }

                    if (SelectedNode.Parent != null)
                    {
                        NodeViewModel testCaseNode = SelectedNode.Parent as NodeViewModel;

                        SetParentNodeVerdict(testCaseNode);

                        if (SelectedNode.Parent.Parent != null)
                        {
                            NodeViewModel testSuiteNode = SelectedNode.Parent.Parent as NodeViewModel;

                            SetParentNodeVerdict(testSuiteNode);
                        }
                    }
                }
            }
        }

        private void SetParentNodeVerdict(NodeViewModel parentNode)
        {
            string resultingVerdict = "V-Verdict-0";

            foreach (NodeViewModel child in parentNode.Children)
            {
                string childVerdict = child.ReferencedResource.Resource.GetPropertyValue("U2TP:Verdict", SelectedResource.MetadataReader);

                if (childVerdict != null)
                {
                    if (child.ReferencedResource.ResourceClassID == "RC-TestStep" || child.ReferencedResource.ResourceClassID == "RC-TestCase")
                    {
                        if (childVerdict == "V-Verdict-0" && resultingVerdict == "V-Verdict-0")  // None: nicht getestet
                        {
                            resultingVerdict = "V-Verdict-0";
                        }
                        else if (childVerdict == "V-Verdict-1" && (resultingVerdict == "V-Verdict-1" || resultingVerdict == "V-Verdict-0")) //Pass
                        {
                            resultingVerdict = "V-Verdict-1";
                        }
                        else if (childVerdict == "V-Verdict-2" && (resultingVerdict == "V-Verdict-2" || resultingVerdict == "V-Verdict-1" || resultingVerdict == "V-Verdict-0")) //Inconclusive
                        {
                            resultingVerdict = "V-Verdict-2";
                        }
                        else if (childVerdict == "V-Verdict-3" && (resultingVerdict == "V-Verdict-3" || resultingVerdict == "V-Verdict-1" || resultingVerdict == "V-Verdict-2" || resultingVerdict == "V-Verdict-0")) //Fail
                        {
                            resultingVerdict = "V-Verdict-3";
                        }
                        else if (childVerdict == "V-Verdict-4" && (resultingVerdict == "V-Verdict-4" || resultingVerdict == "V-Verdict-1" || resultingVerdict == "V-Verdict-0" || resultingVerdict == "V-Verdict-2" || resultingVerdict == "V-Verdict-3")) //Error
                        {
                            resultingVerdict = "V-Verdict-4";
                        }
                    }
                }
            }

            List<PropertyViewModel> properties = parentNode.ReferencedResource.Properties;

            foreach (PropertyViewModel property in properties)
            {
                if (property.PropertyClass.ID == "PC-TestVerdict")
                {
                    if (!ChangedNodes.ContainsKey(parentNode.HierarchyKey))
                    {
                        ChangedNodes.Add(parentNode.HierarchyKey, parentNode);
                    }

                    property.SetSingleEnumerationValue(resultingVerdict);
                    HierarchyViewModel.StateChanged = true;
                    break;
                }
            }

        }

        public string ReasonMessage
        {
            get
            {
                string result = "";

                if (SelectedResource != null)
                {
                    if (SelectedResource.ResourceClassID == "RC-TestStep")
                    {
                        result = SelectedResource.Resource.GetPropertyValue("U2TP:ReasonMessage", SelectedResource.MetadataReader);
                    }
                }
                return result;
            }

            set
            {

                if (SelectedResource != null)
                {
                    if (SelectedResource.ResourceClassID == "RC-TestStep")
                    {
                        SelectedResource.Resource.SetPropertyValue("U2TP:ReasonMessage", value, SelectedResource.MetadataReader);

                        if (!ChangedNodes.ContainsKey(SelectedNode.HierarchyKey))
                        {
                            ChangedNodes.Add(SelectedNode.HierarchyKey, SelectedNode);
                        }
                    }

                }
            }
        }
        
        public string ReasonMessageFormat
        {
            get
            {
                string result = "plain";

                if(SelectedResource != null && SelectedResource.ResourceClassID == "RC-TestStep")
                {
                    foreach(PropertyClass propertyClass in SelectedResource.PropertyClasses)
                    {
                        if(propertyClass.Title == "U2TP:ReasonMessage" && !string.IsNullOrEmpty(propertyClass.Format))
                        {
                            result = propertyClass.Format;
                            break;
                        }
                    }
                }

                return result;
            }
        }

        public string TestCaseTitle
        {
            get
            {
                string result = "";

                NodeViewModel? selectedNode = HierarchyViewModel.SelectedNode as NodeViewModel;

                if (selectedNode != null)
                {
                    result = selectedNode.TestCaseTitle(HierarchyViewModel.PrimaryLanguage);
                }

                return result;
            }
        }

        public string TestObject
        {
            get
            {
                string result = "";

                if (SelectedResource != null && SelectedResource.Properties != null)
                {
                    result = SelectedResource.Resource.GetPropertyValue("ISTQB:TestObject", SelectedResource.MetadataReader);
                }

                return result;
            }

        }

        public ICommand GoToNextCommand { get; private set; }

        public ICommand GoToPreviousCommand { get; private set; }

        public ICommand SaveTestResultCommand { get; private set; }

        #region COMMAND_IMPLEMENTATIONS
        private void ExecuteGoToPrevious()
        {
            if (SelectedResource.ResourceClassID == "RC-TestStep" || SelectedResource.ResourceClassID == "RC-TestCase")
            {
                NodeViewModel? currentNodeViewModel = SelectedNode as NodeViewModel;

                NodeViewModel? previousTestStep = null;

                NodeViewModel? parent = currentNodeViewModel.Parent as NodeViewModel;

                foreach (NodeViewModel? child in parent.Children)
                {
                    if (child == currentNodeViewModel)
                    {
                        break;
                    }
                    else
                    {
                        previousTestStep = child;
                    }
                }
                if (previousTestStep != null)
                {
                    HierarchyViewModel.SelectedNode = previousTestStep;
                }
                else
                {
                    NodeViewModel? parentTestCase = currentNodeViewModel.Parent as NodeViewModel;
                    HierarchyViewModel.SelectedNode = parentTestCase;
                }
            }
        }
        private void ExecuteGoToNext()
        {
            if (SelectedResource.ResourceClassID == "RC-TestStep")
            {
                NodeViewModel? currentNodeViewModel = SelectedNode as NodeViewModel;
                NodeViewModel? parent = currentNodeViewModel.Parent as NodeViewModel;
                NodeViewModel? nextTestStep = null;

                bool foundCurrentTestStep = false;

                foreach (NodeViewModel child in parent.Children)
                {
                    if (foundCurrentTestStep && child.ReferencedResource.ResourceClassID == "RC-TestStep")
                    {
                        nextTestStep = child;
                        break;
                    }
                    if (child == currentNodeViewModel)
                    {
                        foundCurrentTestStep = true;
                    }
                }
                // next teststep found
                if (nextTestStep != null)
                {
                    HierarchyViewModel.SelectedNode = nextTestStep;
                }
                else
                {
                    NodeViewModel? grandparend = parent.Parent as NodeViewModel;
                    if (grandparend != null)
                    {
                        NodeViewModel? nextTestCase = null;

                        bool currentTestCaseFound = false;


                        foreach (NodeViewModel child in grandparend.Children)
                        {
                            if (child.ReferencedResource != null)
                            {

                                if (currentTestCaseFound && child.ReferencedResource.ResourceClassID == "RC-TestCase")
                                {
                                    nextTestCase = child;
                                    break;
                                }

                                if (child == parent)
                                {
                                    currentTestCaseFound = true;
                                }
                            }
                        }

                        if (nextTestCase != null)
                        {
                            HierarchyViewModel.SelectedNode = nextTestCase;
                        }
                    }
                }
            }
            else if (SelectedResource.ResourceClassID == "RC-TestCase" || SelectedResource.ResourceClassID == "RC-TestSuite")
            {
                NodeViewModel? currentNodeViewModel = SelectedNode as NodeViewModel;

                if (currentNodeViewModel.Children != null && currentNodeViewModel.Children.Count > 0)
                {
                    NodeViewModel? child = currentNodeViewModel.Children[0] as NodeViewModel;
                    HierarchyViewModel.SelectedNode = child;
                }
            }
        }

        private void ExecuteSaveTestResult()
        {
            foreach(KeyValuePair<Key, NodeViewModel> keyValuePair in ChangedNodes)
            {
                NodeViewModel changedNode = keyValuePair.Value;

                Resource changedResource = changedNode.ReferencedResource.Resource;

                Resource newRevisionResource = changedResource.CreateNewRevisionForEdit(HierarchyViewModel.MetadataReader);

                HierarchyViewModel.DataWriter.AddResource(newRevisionResource, HierarchyViewModel.ProjectID);

                ResourceViewModel changedResourceViewModel = new ResourceViewModel(HierarchyViewModel.MetadataReader,
                                                              HierarchyViewModel.DataReader,
                                                              HierarchyViewModel.DataWriter,
                                                              newRevisionResource);

                changedNode.ReferencedResource = changedResourceViewModel;

                changedNode.HierarchyNode.ResourceReference.Revision = changedResourceViewModel.Resource.Revision;

                HierarchyViewModel.DataWriter.UpdateHierarchy(changedNode.HierarchyNode);
            }

            ChangedNodes.Clear();

        }

        private bool CanSave()
        {
            return ChangedNodes.Count > 0;
        }

        #endregion

    }
}