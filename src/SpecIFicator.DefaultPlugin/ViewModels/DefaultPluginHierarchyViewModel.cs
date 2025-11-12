using GalaSoft.MvvmLight.Command;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using MDD4All.SpecIF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpecIFicator.DefaultPlugin.ViewModels
{
    public class DefaultPluginHierarchyViewModel : HierarchyViewModel
    {
        public DefaultPluginHierarchyViewModel(ISpecIfDataProviderFactory specIfDataProviderFactory, Key key, string projectID) : 
                                                        base(specIfDataProviderFactory, key, projectID)
        {
            InitalizeCommands();
        }

        private void InitalizeCommands()
        {
            ShowCommentsCommand = new RelayCommand(ExecuteNewComment);
        }

        public bool ShowComments { get; set; }

        public ICommand ShowCommentsCommand { get; set; }

        private void ExecuteNewComment()
        {
            ShowComments = true;
        }


    }
}
