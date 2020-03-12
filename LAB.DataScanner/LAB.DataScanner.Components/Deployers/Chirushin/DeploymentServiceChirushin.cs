namespace LAB.DataScanner.Components.Deployers.Chirushin
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using LAB.DataScanner.Components.MessageBroker.RabbitMqBuilder;
    using LAB.DataScanner.Components.Models;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using RabbitMQ.Client.Events;

    public class DeploymentServiceChirushin : IDeploymentService
    {
        private const string sfClusterUriConfigurationName = "sfClusterUri";
        private const string sfTempPackagesLocation = "C:\\temp\\PackagePreparePlace_VC\\";
        private const string ServiceName = "DeploymentServiceChirushin";

        public readonly IRmqConsumer rmqConsumer;
        public readonly IDeploymentManagerWrapper serviceFabricClientHandler;
        public readonly IComponentDetailsReader componentDetailsReader;
        public readonly IConfigurationSection configurationSection;

        private string sfClusterUri;

        public DeploymentServiceChirushin(
            IRmqConsumer rmqConsumer,
            IDeploymentManagerWrapper serviceFabricClientHandler,
            IComponentDetailsReader componentDetailsReader,
            IConfigurationSection configurationSection)
        {
            this.rmqConsumer = rmqConsumer;
            this.serviceFabricClientHandler = serviceFabricClientHandler;
            this.componentDetailsReader = componentDetailsReader;
            this.configurationSection = configurationSection;
        }

        public bool CanExecute(string name)
        {
            return (ServiceName == name);
        }

        public void Start(CancellationToken stoppingToken)
        {
            var applicationConfiguration = configurationSection.GetSection("Application");
            sfClusterUri = applicationConfiguration.GetValue<string>(sfClusterUriConfigurationName);
            this.rmqConsumer.StartListening(this.OnReceive);
        }

        private void OnReceive(object sender, BasicDeliverEventArgs e)
        {
            var messageAsByteArray = e.Body;

            var queueDeployMessage = JsonConvert
                .DeserializeObject<QueueDeployMessageModel>(
                    Encoding.UTF8.GetString(messageAsByteArray));

            var deployingComponent = componentDetailsReader.GetComponentDetails(queueDeployMessage.ComponentId);

            serviceFabricClientHandler.InitializeSfClientConnection(sfClusterUri);

            bool isTypePresentInServiceFabric = 
                serviceFabricClientHandler
                    .IsApplicationExist(deployingComponent.typeName);

            bool isServiceInstancePresentInServiceFabric = 
                serviceFabricClientHandler
                .IsServiceExist(deployingComponent.instanceName);


            if (queueDeployMessage.DeploymentType == "Deploy")
            {
                RunDeploymentProcess(
                    deployingComponent,
                    isServiceInstancePresentInServiceFabric,
                    isTypePresentInServiceFabric);
            }

            if (queueDeployMessage.DeploymentType == "Remove")
            {
                RunRemovingProcess(
                    deployingComponent,
                    isServiceInstancePresentInServiceFabric,
                    isTypePresentInServiceFabric);
            }
            rmqConsumer.Ack(new BasicDeliverEventArgs() { DeliveryTag = e.DeliveryTag });
        }

        public void Stop()
        {
            this.rmqConsumer.StopListening();
        }

        private void RunDeploymentProcess(  
            DeployingComponent deployingComponent,
            bool isServiceInstancePresentInServiceFabric,
            bool isTypePresentInServiceFabric)
        {
            if (isServiceInstancePresentInServiceFabric == false)
            {
                if (isTypePresentInServiceFabric == false)
                {
                    var preparedPackagePath = PrepareApplicationPackage(deployingComponent);
                    serviceFabricClientHandler
                        .StartServiceFromPackage(
                            preparedPackagePath,
                            deployingComponent);
                    return;
                }

                serviceFabricClientHandler
                    .CreateApplicationAsync(deployingComponent);
            }
            else
            {
                throw new Exception("Instance already exist in ServiceFabric.");
            }
        }
        private void RunRemovingProcess(
            DeployingComponent deployingComponent,
            bool isServiceInstancePresentInServiceFabric,
            bool isTypePresentInServiceFabric)
        {

            if (isTypePresentInServiceFabric == true)
            {
                if (isServiceInstancePresentInServiceFabric == true)
                {
                    serviceFabricClientHandler
                        .RemoveServiceInstanceAsync(deployingComponent.instanceName);
                }

                serviceFabricClientHandler
                    .RemoveAppTypeAsync(
                        deployingComponent.typeName,
                        deployingComponent.appTypeVersion);
            }
            else
            {
                throw new Exception("ApplicationType doesn't exist in Service fabric!");
            }
        }

        private string PrepareApplicationPackage(DeployingComponent deployingComponent)
        {
            var zipPackagePath = deployingComponent.packagePath;
            var zipPackage = new FileInfo(zipPackagePath);
            if (!zipPackage.Exists)
            {
                throw new ArgumentOutOfRangeException($"File {zipPackagePath} does not exist");
            }

            var packageLocation = Path.Combine(sfTempPackagesLocation, $"{deployingComponent.typeName}");
            Directory.CreateDirectory(packageLocation);

            ZipFile.ExtractToDirectory(zipPackagePath, packageLocation, true);

            var servicePackageFolderPath = Path.Combine(packageLocation, "ServicePackage");
            var codeFolderPath = Path.Combine(servicePackageFolderPath, "Code");
            var configFolderPath = Path.Combine(servicePackageFolderPath, "Config");
            var runtimeFolderPath = Path.Combine(codeFolderPath, "runtimes");

            Directory.CreateDirectory(servicePackageFolderPath);
            Directory.CreateDirectory(codeFolderPath);
            Directory.CreateDirectory(configFolderPath);

            var files = Directory.EnumerateFiles(packageLocation, "*.dll")
                .Union(Directory.EnumerateFiles(packageLocation, "*.exe"))
                .Union(Directory.EnumerateFiles(packageLocation, "*.pdb"))
                .Union(Directory.EnumerateFiles(packageLocation, "*.json"));

            foreach (var file in files)
            {
                File.Move(file, Path.Combine(codeFolderPath, Path.GetFileName(file)));
            }

            Directory.Move(Path.Combine(packageLocation, "runtimes"), runtimeFolderPath);

            var manifestLocation = Path.Combine(packageLocation, "ServiceFabricManifests");

            File.Move(Path.Combine(manifestLocation, "ApplicationManifest.xml"), Path.Combine(packageLocation, "ApplicationManifest.xml"));
            File.Move(Path.Combine(manifestLocation, "ServiceManifest.xml"), Path.Combine(servicePackageFolderPath, "ServiceManifest.xml"));
            File.Move(Path.Combine(manifestLocation, "Settings.xml"), Path.Combine(configFolderPath, "Settings.xml"));

            Directory.Delete(manifestLocation, true);
            return packageLocation;
        }
    }
}