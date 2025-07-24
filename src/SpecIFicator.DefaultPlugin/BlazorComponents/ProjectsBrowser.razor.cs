using MDD4All.SpecIF.DataFactory;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using MDD4All.SpecIF.ViewModels;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MDD4All.SpecIF.DataModels.Manipulation;
using Microsoft.Extensions.Localization;
using System.Diagnostics;

namespace SpecIFicator.DefaultPlugin.BlazorComponents
{
    public partial class ProjectsBrowser
    {
        private ProjectsViewModel _projectsViewModel;
        [Inject]
        private IStringLocalizer<ProjectsBrowser> L { get; set; }
        [Inject]
        private ISpecIfDataProviderFactory DataProviderFactory { get; set; }
        private ISpecIfMetadataReader MetadataReader { get; set; }
        private ISpecIfDataReader DataReader { get; set; }
        private ISpecIfDataWriter DataWriter { get; set; }
        private bool ShowNewProjectDialog { get; set; } = false;
        private bool IsInEditMode { get; set; } = false;
        private string _selectedProjectID = string.Empty;
        private string currentTitle = string.Empty;
        private string currentDescription = string.Empty;
        protected override void OnInitialized()
        {
            MetadataReader = DataProviderFactory.MetadataReader;
            DataReader = DataProviderFactory.DataReader;
            DataWriter = DataProviderFactory.DataWriter;
            _projectsViewModel = new ProjectsViewModel(MetadataReader, DataWriter, DataReader);
        }

        private void OnEditButtonClicked(ProjectViewModel viewModelToEdit)
        {
            _selectedProjectID = viewModelToEdit.ProjectID;
            ShowNewProjectDialog = true;
            IsInEditMode = true;
            currentTitle = viewModelToEdit.ProjectTitle;
            currentDescription = viewModelToEdit.ProjectDescription;
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
                    List<string> parameterList = new List<string> { currentTitle, currentDescription, _selectedProjectID };
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
                    List<string> parameterList = new List<string> { currentTitle, currentDescription };
                    _projectsViewModel.AddNewProjectCommand.Execute(parameterList);
                }
                ShowNewProjectDialog = false;
            }
            currentTitle = string.Empty;
            currentDescription = string.Empty;
        }
    }
}
