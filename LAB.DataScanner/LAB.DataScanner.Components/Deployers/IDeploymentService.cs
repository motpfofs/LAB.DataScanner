namespace LAB.DataScanner.Components.Deployers
{
    using System.Threading;

    public interface IDeploymentService
    {
        public bool CanExecute(string name);

        void Start(CancellationToken stoppingToken);

        void Stop();
    }
}