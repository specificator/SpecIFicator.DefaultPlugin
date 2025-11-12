using MDD4All.SpecIF.DataProvider.Contracts;
using MDD4All.SpecIF.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace SpecIFicator.DefaultPlugin.BlazorComponents
{
    public partial class ProjectsBrowser
    {
        private ProjectsViewModel _projectsViewModel = null!;

        [Inject]
        private IStringLocalizer<ProjectsBrowser> L { get; set; } = null!;

        [Inject]
        private ISpecIfDataProviderFactory DataProviderFactory { get; set; } = null!;

        private ISpecIfMetadataReader MetadataReader { get; set; } = null!;

        private ISpecIfDataReader DataReader { get; set; } = null!;

        private ISpecIfDataWriter DataWriter { get; set; } = null!;
        
        private bool ShowNewProjectDialog { get; set; } = false;
        
        private bool IsInEditMode { get; set; } = false;
        
        private string _selectedProjectID = string.Empty;
        
        private string _currentTitle = string.Empty;
        
        private string _currentDescription = string.Empty;
        
        protected override void OnInitialized()
        {
            MetadataReader = DataProviderFactory.MetadataReader;
            DataReader = DataProviderFactory.DataReader;
            DataWriter = DataProviderFactory.DataWriter;
            _projectsViewModel = new ProjectsViewModel(MetadataReader, DataProviderFactory.MetadataWriter, DataWriter, DataReader);
        }

        private void OnEditButtonClicked(ProjectViewModel viewModelToEdit)
        {
            _selectedProjectID = viewModelToEdit.ProjectID;
            _currentTitle = viewModelToEdit.ProjectTitle;
            _currentDescription = viewModelToEdit.ProjectDescription;

            IsInEditMode = true;
            ShowNewProjectDialog = true;
        }

        private void OnNewProjectButtonClicked()
        {
            ShowNewProjectDialog = true;
        }

        private void OnNewProjectDialogClose(bool accepted)
        {
            if (IsInEditMode)
            {
                if (accepted)
                {
                    List<string> parameterList = new List<string> { _currentTitle, _currentDescription, _selectedProjectID };
                    _projectsViewModel.EditProjectCommand.Execute(parameterList);
                    _selectedProjectID = string.Empty;
                }
                ShowNewProjectDialog = false;
                IsInEditMode = false;
                StateHasChanged();
            }
            else
            {
                if (accepted)
                {
                    List<string> parameterList = new List<string> { _currentTitle, _currentDescription };
                    _projectsViewModel.AddNewProjectCommand.Execute(parameterList);
                }
                ShowNewProjectDialog = false;
            }
            _currentTitle = string.Empty;
            _currentDescription = string.Empty;
        }
    }
}
