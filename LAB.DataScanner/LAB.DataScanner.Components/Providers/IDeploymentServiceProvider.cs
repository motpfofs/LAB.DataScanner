namespace LAB.DataScanner.Components.Providers
{
    using System.Threading.Tasks;

    public interface IDeploymentServiceProvider
    {
        Task Start(string name);
        Task Stop(string name);
    }
}