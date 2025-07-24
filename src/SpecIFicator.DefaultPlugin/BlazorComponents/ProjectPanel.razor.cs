using MDD4All.SpecIF.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpecIFicator.DefaultPlugin.BlazorComponents
{
    public partial class ProjectPanel
    {
        [Inject]
        private IStringLocalizer<ProjectPanel> L { get; set; }

        [Parameter]
        public EventCallback<string> ProjectTitleChanged { get; set; }

        [Parameter]
        public EventCallback<string> ProjectDescriptionChanged { get; set; }

        private string _projectTitle { get; set; } = string.Empty;
        private string _projectDescription { get; set; } = string.Empty;
        public bool TitleIsInvalid = true;

        [Parameter]
        public string ProjectTitle
        {
            get => _projectTitle;
            set
            {
                if (_projectTitle != value)
                {
                    _projectTitle = value;
                    if (value == null || value.Trim().Length == 0 || !System.Text.RegularExpressions.Regex.IsMatch(ProjectTitle, @"^[a-zA-Z\s]+$"))
                    {
                        TitleIsInvalid = true;
                    }
                    else
                    {
                        TitleIsInvalid = false;
                    }
                    ProjectTitleChanged.InvokeAsync(value);
                }
            }

        }

        [Parameter]
        public string ProjectDescription
        {
            get => _projectDescription;
            set
            {
                if (_projectDescription != value)
                {
                    _projectDescription = value;
                    ProjectDescriptionChanged.InvokeAsync(value);
                }
            }
        }
    }
}