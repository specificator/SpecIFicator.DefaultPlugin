using MDD4All.SpecIF.DataProvider.Contracts;
using Microsoft.AspNetCore.Components;
using SpecIFicator.Framework.Contracts;
using MDD4All.SpecIF.DataProvider.File;
using MDD4All.Configuration.Contracts;
using SpecIFicator.DefaultPlugin.Configurations;
using MDD4All.Configuration;
using Microsoft.Extensions.Localization;
using MDD4All.SpecIF.ViewModels;
using MDD4All.SpecIF.DataModels;

namespace SpecIFicator.DefaultPlugin.DataConnectors
{
    public partial class SpecIfMultiFileConnector : ISpecIfDataConnector
    {
        public static string Title = "Files";

        [Inject]
        private IStringLocalizer<SpecIfMultiFileConnector> L { get; set; }

        [Parameter]
        public string MetadataPath { get; set; } = "";

        [Parameter]
        public string DataPath { get; set; } = "";

        public string ErrorMessage { get; set; } = "";

        private IConfigurationReaderWriter<MultiFileConnectorConfiguration> configurationReaderWriter;

        private MultiFileConnectorConfiguration configuration;

        protected override void OnInitialized()
        {
            configurationReaderWriter = new FileConfigurationReaderWriter<MultiFileConnectorConfiguration>("SpecIFicator/plugins");

            configuration = configurationReaderWriter.GetConfiguration();

            if(configuration == null)
            {
                configuration = new MultiFileConnectorConfiguration();
            }

            MetadataPath = configuration.MetadataPath;
            DataPath = configuration.DataPath;

        }

        [CascadingParameter]
        private DataConnectorViewModel DataContext { get; set; }

        private void HandleConnectClick()
        {
            ErrorMessage = "";

            bool isWrongPath = false;

            string errorMessageID = ValidatePath(DataPath);
            if (!string.IsNullOrEmpty(errorMessageID))
            {
                ErrorMessage = L["Label.DataPath"] + " - " + L[errorMessageID];
                
                isWrongPath = true;
            }
            else
            {
                errorMessageID = ValidatePath(MetadataPath);
                if (!string.IsNullOrEmpty(errorMessageID))
                {

                    ErrorMessage = L["Label.MetadataPath"] + " - " + L[errorMessageID];
                    isWrongPath = true;
                }
            }

            if (!isWrongPath)
            {
                if (configuration != null && configurationReaderWriter != null)
                {
                    configuration.MetadataPath = MetadataPath;
                    configuration.DataPath = DataPath;
                    configurationReaderWriter.StoreConfiguration(configuration);
                }

                ISpecIfMetadataReader metadataReader = new SpecIfFileMetadataReader<ExtendedSpecIF>(MetadataPath);
                ISpecIfMetadataWriter metadataWriter = new SpecIfFileMetadataWriter(MetadataPath);
                ISpecIfDataReader dataReader = new SpecIfFileDataReader<ExtendedSpecIF>(DataPath);
                ISpecIfDataWriter dataWriter = new SpecIfFileDataWriter<ExtendedSpecIF>(DataPath, metadataReader, dataReader);

                DataContext.SpecIfDataProviderFactory.MetadataReader = metadataReader;
                DataContext.SpecIfDataProviderFactory.MetadataWriter = metadataWriter;
                DataContext.SpecIfDataProviderFactory.DataReader = dataReader;
                DataContext.SpecIfDataProviderFactory.DataWriter = dataWriter;

                DataContext.ConnectCommand.Execute(null);
            }
        }

        private string ValidatePath(string path)
        {
            string result = "";

            try
            {
                if(!Directory.Exists(path))
                {
                    throw new DirectoryNotFoundException();
                }
            }
            catch
            {
                result = "Error.Path";
            }

            return result;
        }
    }
}