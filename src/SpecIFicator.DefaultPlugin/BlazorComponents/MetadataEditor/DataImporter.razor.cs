using MDD4All.FileAccess.Contracts;
using MDD4All.SpecIF.DataProvider.Contracts;
using MDD4All.SpecIF.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace SpecIFicator.DefaultPlugin.BlazorComponents.MetadataEditor
{
    public partial class DataImporter
    {
        [Inject]
        private IStringLocalizer<DataImporter> L { get; set; }

        [Inject]
        public IHttpClientFactory HttpClientFactory { get; set; }

        [Inject]
        private ISpecIfDataProviderFactory SpecIfDataProviderFactory { get; set; }

        [Inject]
        private IFileLoader FileLoader { get; set; }

        private DataImportViewModel DataContext { get; set; }

        protected override void OnInitialized()
        {
            DataContext = new DataImportViewModel(HttpClientFactory, 
                                                  SpecIfDataProviderFactory.MetadataReader,
                                                  SpecIfDataProviderFactory.MetadataWriter, 
                                                  SpecIfDataProviderFactory.DataWriter,
                                                  FileLoader);

            DataContext.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        private async Task OnFileSelectionChange(ChangeEventArgs changeEventArguments)
        {
            DataContext.MetadataFileURL = changeEventArguments.Value.ToString();   
        }

        private void OnSelectFileClicked()
        {
            // Show a modal dialog after the current event handler is completed,
            // to avoid potential reentrancy caused by running a nested message loop in the WebView2 event handler.
            // https://github.com/MicrosoftEdge/WebView2Feedback/issues/2542
            SynchronizationContext.Current?.Post((_) =>
            {
                DataContext.SelectFileCommand.Execute(null);
                StateHasChanged();
            }, null);
        }

    }
}