using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Base;
using MDD4All.SpecIF.DataProvider.Contracts;
using MDD4All.SpecIF.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SpecIFicator.DefaultPlugin.BlazorComponents.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpecIFicator.DefaultPlugin.ViewModels
{
    public class ConsolidationViewModel : ViewModelBase
    {

        public List<DesktopObjectViewModel> DesktopObjectViewModels { get; set; } = new List<DesktopObjectViewModel>();


        public ElementReference ConsolidationDocumentReference { get; set; }



        private ISpecIfDataProviderFactory _dataProviderFactory;

        private bool _showOpenLeftDocumentDialog;

        private bool _showOpenRightDocumentDialog;

        public bool ShowOpenLeftHierarchyDialog
        {
            get
            {
                return _showOpenLeftDocumentDialog;
            }

            set
            {
                _showOpenLeftDocumentDialog = value;
                RaisePropertyChanged();
            }
        }

        
        public bool ShowOpenRightHierarchyDialog
        {
            get
            {
                return _showOpenRightDocumentDialog;
            }

            set
            {
                _showOpenRightDocumentDialog = value;
                RaisePropertyChanged();
            }
        }

        public ConsolidationViewModel(ISpecIfDataProviderFactory specIfDataProviderFactory)
        {
            _dataProviderFactory = specIfDataProviderFactory;
            InitializeCommands();


        }

        

        private void InitializeCommands()
        {
            OpenLeftDocumentCommand = new RelayCommand(ExecuteOpenLeftDocumentCommand);
            OnOpenLeftDocumentDialogClose = new RelayCommand(ExecuteOpenLeftDocumentDialogClose);
            OpenRightDocumentCommand = new RelayCommand(ExecuteOpenRightDocumentCommand);
            OnOpenRightDocoumentDialogClose = new RelayCommand(ExecuteOpenRightDocumentDialogClose);
            
        }

        public ICommand OpenLeftDocumentCommand { get; set; }
        public ICommand OnOpenLeftDocumentDialogClose { get; set; }

        public ICommand OpenRightDocumentCommand { get; set; }

        public ICommand OnOpenRightDocoumentDialogClose { get; set; }

        public HierarchyViewModel HierarchyViewModelLeft { get; set; }

        public HierarchyViewModel HierachyViewModelRight { get; set; }

        private void ExecuteOpenLeftDocumentCommand()
        {
            ShowOpenLeftHierarchyDialog = true;

        }
        public string OpenNodeID { get; set; } = "";



        private void ExecuteOpenLeftDocumentDialogClose()
        {
            Key key = new Key();
            key.ID = OpenNodeID;

            HierarchyViewModelLeft = new HierarchyViewModel(_dataProviderFactory, key);

        }

        private void ExecuteOpenRightDocumentCommand()
        {

            ShowOpenRightHierarchyDialog = true;

        }

        private void ExecuteOpenRightDocumentDialogClose()
        {
            Key key = new Key();
            key.ID = OpenNodeID;

            HierachyViewModelRight = new HierarchyViewModel(_dataProviderFactory, key);
        }


        public void RefreshView()
        {
            RaisePropertyChanged();
        }
       





    }
}
