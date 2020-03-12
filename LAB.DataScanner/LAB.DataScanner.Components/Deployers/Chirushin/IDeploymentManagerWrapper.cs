namespace LAB.DataScanner.Components.Deployers.Chirushin
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Common;

    public interface IDeploymentManagerWrapper
    {
        void InitializeSfClientConnection(string sfClusterUri);
        Task<IEnumerable<ApplicationTypeInfo>> GetApllicationsAsync();
        bool IsApplicationExist(string applicationName);
        Task<IEnumerable<ApplicationInfo>> GetServicesAsync();
        bool IsServiceExist(string serviceName);
        void RemoveServiceInstanceAsync(string applicationId);
        void RemoveAppTypeAsync(
            string applicationTypeName,
            string applicationVersion);
        void CreateApplicationAsync(DeployingComponent deployingComponent);
        void CreateApplicationTypeAsync(string applicationName);
        Task<ApplicationInfo> GetApplicationInfoAsync(string applicationId);
        void RemovePackageImageAsync(string contentPath);
        void StartServiceFromPackage(
            string packagePath,
            DeployingComponent deployingComponent);
    }
}
