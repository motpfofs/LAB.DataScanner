namespace LAB.DataScanner.Components.Tests.Acceptance.DeploymentService_Vladimir_Chirushin
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using BoDi;
    using LAB.DataScanner.Components.Deployers.Chirushin;
    using LAB.DataScanner.Components.MessageBroker.RabbitMqBuilder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.ServiceFabric.Client;
    using Newtonsoft.Json;
    using TechTalk.SpecFlow;
    using SFClient = Microsoft.ServiceFabric.Client;

    [Binding]
    public class DeploymentServiceSteps
    {
        private readonly IObjectContainer objectContainer;
        public DeploymentServiceSteps(IObjectContainer objectContainer)
        {
            this.objectContainer = objectContainer;
        }

        [Given(@"I have new deployment message into this (.*) exchange with this data")]
        public void IHaveNewDeploymentMessageIntoThisDeploymentExchangeWithThisData(string exchangeName, Table table)
        {
            var rmqPublisher = new RmqPublisherBuilder()
                .UsingExchange(exchangeName)
                .UsingDefaultConnectionSetting()
                .Build();

            var messages = table.Rows.Select(s => new
            {
                ComponentId = s["ComponentId"],
                DeploymentType = s["DeploymentType"]
            });

            foreach (var msg in messages)
            {
                var messageAsByteArray = Encoding.UTF8.GetBytes(
                    JsonConvert.SerializeObject(msg));
                rmqPublisher.Publish(messageAsByteArray);
            }
        }

        [Given(@"I have empty database for application configuration")]
        public void GivenIHaveEmptyDatabaseForApplicationConfiguration()
        {
            objectContainer.RegisterInstanceAs<DeployingComponentProvider>(new DeployingComponentProvider());
        }

        [Given(@"I have in config database application instance '(.*)' with this id '(.*)' with build version '(.*)' with these properties")]
        public void GivenIHaveInConfigDatabaseApplicationInstanceWithThisIdWithBuildVersionWithTheseProperties(string applicationName, int appId, string appVersion, Table table)
        {
            var appSettings = table.Rows.ToDictionary(r => r["Key"], r => r["Value"]);

            var deployingComponentProvider = objectContainer.Resolve<DeployingComponentProvider>();
            deployingComponentProvider.RegisterApp(appId, applicationName, appVersion, appSettings);
        }

        [Given(@"I have DeploymentService with this parameters")]
        public void GivenIHaveDeploymentServiceWithThisParameters(Table table)
        {
            var clientsettings = table.Rows.ToDictionary(r => r["Key"], r => r["Value"]);

            var configurationRoot = new ConfigurationBuilder()
                .AddInMemoryCollection(clientsettings)
                .Build();

            var configurationSection = configurationRoot.GetSection("Config");

            var rmqConsumer = new RmqConsumerBuilder()
                .UsingQueue(
                    configurationSection
                        .GetSection("Binding")
                        .GetValue<string>("ReceiverQueue"))
                .UsingDefaultConnectionSetting()
                .Build();

            var deploymentManagerHandler = new DeploymentManagerWrapper();
            var componentDetailsReader = objectContainer.Resolve<DeployingComponentProvider>();

            CancellationToken cancellationToken = new CancellationToken();

            var deploymentService = new DeploymentServiceChirushin(
                rmqConsumer,
                deploymentManagerHandler,
                componentDetailsReader,
                configurationSection);
            deploymentService.Start(cancellationToken);
        }

        [Given(@"I have no any applications in local service fabric")]
        public void GivenIHaveNoAnyApplicationsInLocalServiceFabric()
        {
            var t = Task.Run(async () =>
            {
                var sfClusterUri = "http://localhost:19080";

                var sfClient = await new SFClient.ServiceFabricClientBuilder()
                           .UseEndpoints(new Uri(sfClusterUri))
                           .BuildAsync();

                var applicationList = await sfClient.Applications.GetApplicationInfoListAsync();

                foreach (var application in applicationList.Data)
                {
                    var applicationId = application.Id;
                    await sfClient.Applications.DeleteApplicationAsync(applicationId);
                }
            });

            Task.WaitAll(t);
        }

        [Given(@"I have no any application types in local service fabric")]
        public void GivenIHaveNoAnyApplicationTypesInLocalServiceFabric()
        {
            var t = Task.Run(async () =>
            {
                var sfClusterUri = "http://localhost:19080";

                var sfClient = await new SFClient.ServiceFabricClientBuilder()
                           .UseEndpoints(new Uri(sfClusterUri))
                           .BuildAsync();

                var applicationTypes = await sfClient.ApplicationTypes.GetApplicationTypeInfoListAsync();

                foreach (var applicationType in applicationTypes.Data)
                {
                    var applicationTypeName = applicationType.Name;
                    var applicationTypeVersion = applicationType.Version;
                    await sfClient.ApplicationTypes.UnprovisionApplicationTypeAsync(applicationTypeName,
                        new Microsoft.ServiceFabric.Common.UnprovisionApplicationTypeDescriptionInfo(applicationTypeVersion));
                }

            });

            Task.WaitAll(t);
        }

        [Given(@"I have empty folder for sfpkgs here '(.*)'")]
        public void GivenIHaveEmptyFolderForSfpkgsHere(string sfTempPackagesLocation)
        {
            if (Directory.Exists(sfTempPackagesLocation))
            {
                Directory.Delete(sfTempPackagesLocation, true);
            }

            Directory.CreateDirectory(sfTempPackagesLocation);
        }

        [Then(@"I check if there is instance with this name '(.*)' in service fabric")]
        public void ThenICheckIfThereIsInstanceWithThisNameInServiceFabric(string serviceName)
        {
            var t = Task.Run(async () =>
            {
                var sfClusterUri = "http://localhost:19080";

                var sfClient = await new ServiceFabricClientBuilder()
                           .UseEndpoints(new Uri(sfClusterUri))
                           .BuildAsync();

                var applicationList = await sfClient.Applications.GetApplicationInfoListAsync();
                if (applicationList.Data
                    .Where(x => x.Name == serviceName)
                    .FirstOrDefault() == null)
                {
                    throw new Exception($"There is no service with name {sfClusterUri}");
                }
            });
            Task.WaitAll(t);
        }

        [Then(@"I wait for (.*) seconds to check data")]
        public void ThenIWaitForSecondsToCheckData(int delayAmount)
        {
            Thread.Sleep(TimeSpan.FromSeconds(delayAmount));
        }
    }
}