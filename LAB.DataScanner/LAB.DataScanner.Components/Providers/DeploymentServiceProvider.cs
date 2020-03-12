namespace LAB.DataScanner.Components.Providers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using LAB.DataScanner.Components.Deployers;

    public class DeploymentServiceProvider : IDeploymentServiceProvider
    {
        private readonly IEnumerable<IDeploymentService> deploymentServices;

        private CancellationToken cancelationToken = new CancellationToken();

        public DeploymentServiceProvider(IEnumerable<IDeploymentService> deploymentServices)
        {
            this.deploymentServices = deploymentServices;
        }

        public Task Start(string name)
        {
            if (deploymentServices != null)
            {
                var strategy = deploymentServices.FirstOrDefault(s => s.CanExecute(name));
                if (strategy != null)
                {
                    strategy.Start(cancelationToken);
                }
            }

            return Task.CompletedTask;
        }

        public Task Stop(string name)
        {
            if (deploymentServices != null)
            {
                var strategy = deploymentServices.FirstOrDefault(s => s.CanExecute(name));
                if (strategy != null)
                {
                    strategy.Stop();
                }
            }
            return Task.CompletedTask;
        }
    }
}