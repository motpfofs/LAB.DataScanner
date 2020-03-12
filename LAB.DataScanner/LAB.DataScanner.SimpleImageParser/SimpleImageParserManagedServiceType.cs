namespace LAB.DataScanner.SimpleImageParser
{
    using System;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;
    using LAB.DataScanner.Components.DataRetriever;
    using LAB.DataScanner.Components.Engines;
    using LAB.DataScanner.Components.Extractors.ImageExtractor;
    using LAB.DataScanner.Components.Loggers;
    using LAB.DataScanner.Components.MessageBroker.RabbitMqBuilder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.ServiceFabric.AspNetCore.Configuration;
    using Microsoft.ServiceFabric.Services.Runtime;
    using Serilog;
    using Serilog.Events;
    using Serilog.Formatting.Elasticsearch;
    using Serilog.Sinks.Elasticsearch;

    public class SimpleImageParserManagedServiceType : StatelessService
    {
        public SimpleImageParserManagedServiceType(StatelessServiceContext context)
          : base(context)
        {

        }

        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var builder = new ConfigurationBuilder()
                    .AddServiceFabricConfiguration();

                var configuration = builder.Build();

                var configurationSection = configuration.GetSection("Config");

                var applicationName = configurationSection.GetValue<string>("Application:Name");

                ConfigureLogging(applicationName);

                var loggerService = new LoggerServiceSerilog(Log.Logger);

                loggerService.RunWithExceptionLogging(() =>
                {
                    var rmqPublisher = new RmqPublisherBuilder()
                    .UsingConfigConnectionSettings(configuration.GetSection("Config:RmqConnectionSettings"))
                    .Build();

                    var serviceProvider = new ServiceCollection()
                        .AddSingleton<IConfigurationSection>(configurationSection)
                        .AddSingleton<IRmqPublisher>(rmqPublisher)
                        .AddScoped<IDataRetriever, HttpDataRetriever>()
                        .AddSingleton<IImageExtractorEngine, ImageExtractorEngine>()
                        .AddSingleton<IImageLinksExtractor, ImageLinksExtractor>()
                        .BuildServiceProvider();

                    var engine = serviceProvider.GetService<IImageExtractorEngine>();
                    engine.Start();

                    loggerService.Information("Service {ApplicationName} started", applicationName);
                });
            });
        }

        private static void ConfigureLogging(string applicationName)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File($"C:\\temp\\{applicationName}.log", rollingInterval: RollingInterval.Day)
                .WriteTo.Elasticsearch(
                    new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
                    {
                        MinimumLogEventLevel = LogEventLevel.Verbose,
                        BatchPostingLimit = 5,
                        IndexFormat = "datascanner",
                        InlineFields = true,
                        CustomFormatter = new ElasticsearchJsonFormatter(inlineFields: true)
                    })
                .CreateLogger();
        }
    }
}
