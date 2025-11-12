using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MDD4All.SpecIF.DataFactory;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataModels.Manipulation;
using MDD4All.SpecIF.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SpecIFicator.DefaultPlugin.ViewModels
{
    public class CommentsViewModel : ViewModelBase
    {

        private HierarchyViewModel _hierarchyViewModel;

        public CommentsViewModel(HierarchyViewModel context) 
        { 
            _hierarchyViewModel = context;
            
            InitializeCommands();
            InitailizeComments();
        }

        private void InitializeCommands()
        {
            NewCommentCommand = new RelayCommand(ExecuteNewComment);
            SaveCommentCommand = new RelayCommand(ExecuteSaveComment);
            CancelEditCommentCommand = new RelayCommand(ExecuteCancelEditComment);
        }

        private void InitailizeComments()
        {
            ResourceViewModel resourceViewModel = ((NodeViewModel)_hierarchyViewModel.SelectedNode).ReferencedResource;

            if (resourceViewModel != null)
            {
                foreach (StatementViewModel incomingStatement in resourceViewModel.IncomingStatements)
                {
                    if (incomingStatement.StatementClassKey.ID == "SC-refersTo")
                    {
                        ResourceViewModel commentElement = incomingStatement.SubjectResource;
                        if (commentElement.Resource.GetTypeName(_hierarchyViewModel.MetadataReader) == "SpecIF:Comment")
                        {
                            _comments.Add(commentElement);
                        }
                    }
                }

                _comments.Sort((x, y) => y.Resource.ChangedAt.CompareTo(x.Resource.ChangedAt));
            }

            RaisePropertyChanged();
        }

        private List<ResourceViewModel> _comments = new List<ResourceViewModel>();

        public List<ResourceViewModel> Comments 
        {
            get
            {
                return _comments;
            }
        }

        private NodeViewModel ReferedNode { get; set; }

        private ResourceViewModel CommentUnderEdit { get; set; } = null;


        public string TitleUnderEdit
        {
            get
            {
                string result = "";

                if (CommentUnderEdit != null)
                {
                    PropertyViewModel propertyViewModel = CommentUnderEdit.Properties.Find(prop => prop.PropertyClass.Title == "dcterms:title");
                    if(propertyViewModel != null)
                    {
                        result = propertyViewModel.PrimaryLanguageStringValue;
                    }
                }

                return result;
            }

            set
            {
                if (CommentUnderEdit != null)
                {
                    PropertyViewModel propertyViewModel = CommentUnderEdit.Properties.Find(prop => prop.PropertyClass.Title == "dcterms:title");
                    if (propertyViewModel != null)
                    {
                        propertyViewModel.PrimaryLanguageStringValue = value;
                    }
                }
            }
        }

        public string DescriptionUnderEdit
        {
            get
            {
                string result = "";

                if (CommentUnderEdit != null)
                {
                    PropertyViewModel propertyViewModel = CommentUnderEdit.Properties.Find(prop => prop.PropertyClass.Title == "dcterms:description");
                    if (propertyViewModel != null)
                    {
                        result = propertyViewModel.PrimaryLanguageStringValue;
                    }
                }

                return result;
            }

            set
            {
                if (CommentUnderEdit != null)
                {
                    PropertyViewModel propertyViewModel = CommentUnderEdit.Properties.Find(prop => prop.PropertyClass.Title == "dcterms:description");
                    if (propertyViewModel != null)
                    {
                        propertyViewModel.PrimaryLanguageStringValue = value;
                    }
                }
            }
        }

        public bool ShowCommentEditor
        {
            get
            {
                return CommentUnderEdit != null;
            }
        }

        public ICommand NewCommentCommand { get; private set; }

        public ICommand SaveCommentCommand { get; private set; }

        public ICommand CancelEditCommentCommand { get; private set; }

        private void ExecuteNewComment()
        {
            ReferedNode = (NodeViewModel)_hierarchyViewModel.SelectedNode;

            Resource newComment = SpecIfDataFactory.CreateResource(new Key("RC-Comment", "1.1"), _hierarchyViewModel.MetadataReader);
            
            CommentUnderEdit = new ResourceViewModel(_hierarchyViewModel.MetadataReader,
                                                     _hierarchyViewModel.DataReader,
                                                     _hierarchyViewModel.DataWriter, 
                                                     newComment);
        }

        private void ExecuteSaveComment()
        {
            _hierarchyViewModel.DataWriter.AddResource(CommentUnderEdit.Resource, _hierarchyViewModel.ProjectID);

            Statement statement = SpecIfDataFactory.CreateStatement(new Key("SC-refersTo", "1.1"),
                                                                        CommentUnderEdit.Key,
                                                                        ReferedNode.ReferencedResource.Key,
                                                                        _hierarchyViewModel.MetadataReader);

            Task.Run(() =>
            {
                _hierarchyViewModel.DataWriter.AddStatement(statement, _hierarchyViewModel.ProjectID);

                ReferedNode.ReferencedResource.ReinitializeStatementsAsync().Wait();
                ReferedNode = null;
                _comments.Insert(0, CommentUnderEdit);
                CommentUnderEdit = null;
                RaisePropertyChanged();
            });
                        
            
        }

        private void ExecuteCancelEditComment()
        {
            ReferedNode = null;
            CommentUnderEdit = null;
            RaisePropertyChanged();
        }

    }
}
