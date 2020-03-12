namespace LAB.DataScanner.ComponentConfigurator.Services
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using LAB.DataScanner.Components.Deployers;

    public class DeploymentHostedService : IHostedService
    {
        public IDeploymentService DeploymentService { get; }

        public DeploymentHostedService(IDeploymentService deploymentService)
        {
            this.DeploymentService = deploymentService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() => { this.DeploymentService.Start(cancellationToken); });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() => { this.DeploymentService.Stop(); });
        }
    }
}