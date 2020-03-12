﻿namespace LAB.DataScanner.SimpleTextParser
{
    using System;
    using System.Threading;
    using Microsoft.ServiceFabric.Services.Runtime;

    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.
                ServiceRuntime.RegisterServiceAsync("SimpleTextParserManagedServiceType",
                    context => new SimpleTextParserManagedService(context)).GetAwaiter().GetResult();

                // Prevents this host process from terminating so services keep running.
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
