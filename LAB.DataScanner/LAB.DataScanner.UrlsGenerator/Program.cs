namespace LAB.DataScanner.UrlsGenerator
{
    using Microsoft.ServiceFabric.Services.Runtime;
    using System.Threading;

    internal static class Program
    {
        public static void Main(string[] args)
        {
            // The ServiceManifest.XML file defines one or more service type names.
            // Registering a service maps a service type name to a .NET type.
            // When Service Fabric creates an instance of this service type,
            // an instance of the class is created in this host process.

            ServiceRuntime.RegisterServiceAsync("UrlsGeneratorManagedServiceType",
                context => new UrlsGeneratorManagedService(context)).GetAwaiter().GetResult();

            // Prevents this host process from terminating so services keep running.
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
