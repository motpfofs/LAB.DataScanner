namespace LAB.DataScanner.SimpleTextParser
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;
    using LAB.DataScanner.Components.Engines;
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

    public class SimpleTextParserManagedService : StatelessService
    {
        public SimpleTextParserManagedService(StatelessServiceContext context)
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

                var loggerService = new LoggerService(Log.Logger, applicationName);

                loggerService.RunWithExceptionLogging(() =>
                {
                    var rmqPublisher = new RmqPublisherBuilder()
                    .UsingConfigConnectionSettings(configuration.GetSection("Config:RmqConnectionSettings"))
                    .Build();

                    var rmqConsumer = new RmqConsumerBuilder()
                        .UsingConfigConnectionSettings(configuration.GetSection("Config:RmqConnectionSettings"))
                        .Build();

                    var serviceProvider = new ServiceCollection()
                        .AddSingleton<ILoggerServiceSerilog>(loggerService)
                        .AddSingleton<IConfigurationSection>(configurationSection)
                        .AddSingleton<IRmqConsumer>(rmqConsumer)
                        .AddSingleton<IRmqPublisher>(rmqPublisher)
                        .AddSingleton<ITextExtractorEngine, TextExtractorEngine>()
                        .BuildServiceProvider();

                    var engine = serviceProvider.GetService<ITextExtractorEngine>();
                    engine.Start();

                    loggerService.Information("Service started");
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
