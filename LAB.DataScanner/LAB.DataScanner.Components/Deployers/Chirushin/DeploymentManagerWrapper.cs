namespace LAB.DataScanner.Components.Deployers.Chirushin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Client;
    using Microsoft.ServiceFabric.Common;
    using Newtonsoft.Json;

    public class DeploymentManagerWrapper : IDeploymentManagerWrapper
    {
        private const string DefaultVersion = "1.0.0";

        private IServiceFabricClient serviceFabricClient;

        public void InitializeSfClientConnection(string sfClusterUri)
        {
            serviceFabricClient = new ServiceFabricClientBuilder()
                   .UseEndpoints(new Uri(sfClusterUri))
                   .BuildAsync().Result;
        }

        public async Task<IEnumerable<ApplicationTypeInfo>> GetApllicationsAsync()
        {
            var applicationTypeInfo = await serviceFabricClient.ApplicationTypes.GetApplicationTypeInfoListAsync();
            return applicationTypeInfo.Data;
        }

        public bool IsApplicationExist(string applicationName)
        {
            return (GetApllicationsAsync().Result
                .Where(x => x.Name == applicationName)
                .FirstOrDefault() != null);
        }

        public async Task<IEnumerable<ApplicationInfo>> GetServicesAsync()
        {
            var serviceInstancesInfo = await serviceFabricClient.Applications.GetApplicationInfoListAsync();
            return serviceInstancesInfo.Data;
        }

        public bool IsServiceExist(string serviceName)
        {
            var serviceNameUri = "fabric:/" + serviceName;
            return (GetServicesAsync().Result
                .Where(x => x.Name == serviceNameUri)
                .FirstOrDefault() != null);
        }

        public async void RemoveServiceInstanceAsync(string applicationId)
        {
            await serviceFabricClient
                .Applications
                .DeleteApplicationAsync(applicationId);
        }

        public async void RemoveAppTypeAsync(
            string applicationTypeName,
            string applicationVersion)
        {
            var unprovisionApplicationTypeDescriptionInfo =
                new UnprovisionApplicationTypeDescriptionInfo(applicationVersion);

            await serviceFabricClient
                .ApplicationTypes
                .UnprovisionApplicationTypeAsync(
                    applicationTypeName,
                    unprovisionApplicationTypeDescriptionInfo);
        }

        public async void CreateApplicationAsync(DeployingComponent deployingComponent)
        {
            var appParams = new Dictionary<string, string>();
            appParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(deployingComponent.configJson);

            string routingKeysAsString = null;
            foreach (var rk in deployingComponent.senderRoutingKeys)
            {
                routingKeysAsString = routingKeysAsString + ',' + rk;
            }

            appParams.Add("Config:Bindnig:SenderRoutingKeys", routingKeysAsString);
            appParams.Add("Config:Bindnig:SenderExchange", deployingComponent.senderExchange);
            appParams.Add("Config:Bindnig:ReceiverQueue", deployingComponent.receiverQueue);

            var appDesc = new ApplicationDescription(new ApplicationName("fabric:/" + deployingComponent.instanceName),
                deployingComponent.typeName,
                DefaultVersion,
                appParams);

            await serviceFabricClient
                .Applications
                .CreateApplicationAsync(appDesc);
        }

        public void CreateApplicationTypeAsync(string applicationName)
        {
            var provisionApplicationTypeDescriptionBase =
                new ProvisionApplicationTypeDescription(applicationName);

            serviceFabricClient
                .ApplicationTypes
                .ProvisionApplicationTypeAsync(provisionApplicationTypeDescriptionBase).Wait();
        }

        public async Task<ApplicationInfo> GetApplicationInfoAsync(string applicationId)
        {
            return await serviceFabricClient
                .Applications
                .GetApplicationInfoAsync(applicationId);
        }

        public async void RemovePackageImageAsync(string contentPath)
        {
            await serviceFabricClient
                .ImageStore
                .DeleteImageStoreContentAsync(contentPath);
        }

        public void StartServiceFromPackage(
            string packagePath,
            DeployingComponent deployingComponent)
        {
            serviceFabricClient
                .ImageStore
                .UploadApplicationPackageAsync(packagePath)
                .Wait();

            this.CreateApplicationTypeAsync(deployingComponent.typeName);
            this.CreateApplicationAsync(deployingComponent);
        }
    }
}
