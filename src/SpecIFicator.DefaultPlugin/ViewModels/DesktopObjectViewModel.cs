using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MDD4All.SpecIF.DataFactory;
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataModels.Manipulation;
using MDD4All.SpecIF.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SpecIFicator.DefaultPlugin.BlazorComponents.RequirementConsolidation;
using System.Collections.ObjectModel;
using System.Windows.Input;



namespace SpecIFicator.DefaultPlugin.ViewModels
{

    public partial class DesktopObjectViewModel
    {
        
        public NodeViewModel NodeViewModel { get; set; } 


        public ConsolidationViewModel Parent { get; set; }




        public DesktopObjectViewModel()
        {


            InitializeCommands();

        }

        private void InitializeCommands()
        {
            OnDeleteDesktopObjectCommand = new RelayCommand(ExecuteDeleteDesktopObjectCommand);
        }

        public void ExecuteDeleteDesktopObjectCommand()
        {
           Parent.DesktopObjectViewModels.Remove(this);
           Parent.RefreshView();

        }

        public ICommand OnDeleteDesktopObjectCommand { get; set; }

        public int OffsetX { get; set; } = 0;

        public int OffsetY { get; set; } = 0;

        

        

        public string Style
        {
            get
            {
                string result = string.Empty;

                result = "position:absolute; top: " + OffsetY + "px;" + "left:" + OffsetX + "px;" + "border-color: blue;" + "border:solid;" + "px;";
                

                return result;


            }


        }

        



    }




}


	

