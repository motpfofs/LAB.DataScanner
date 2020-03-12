namespace LAB.DataScanner.CsvToJsonParser
{
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;
    using LAB.DataScanner.Components.Engines;
    using LAB.DataScanner.Components.MessageBroker.RabbitMqBuilder;
    using LAB.DataScanner.Components.Parsers.CsvConverter;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.ServiceFabric.AspNetCore.Configuration;
    using Microsoft.ServiceFabric.Services.Runtime;

    public class CsvToJsonParserManagedService : StatelessService
    {
        public CsvToJsonParserManagedService(StatelessServiceContext context)
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

                var appConfiguration = configuration.GetSection("Config");

                var rmqPublisher = new RmqPublisherBuilder()
                    .UsingConfigConnectionSettings(configuration.GetSection("Config:RmqConnectionSettings"))
                    .Build();

                var rmqConsumer = new RmqConsumerBuilder()
                    .UsingConfigConnectionSettings(configuration.GetSection("Config:RmqConnectionSettings"))
                    .Build();

                var serviceProvider = new ServiceCollection()
                    .AddSingleton<IConfigurationSection>(appConfiguration)
                    .AddSingleton<IRmqConsumer>(rmqConsumer)
                    .AddSingleton<IRmqPublisher>(rmqPublisher)
                    .AddSingleton<ICsvConverter, CsvConverter>()
                    .AddSingleton<ICsvToJsonConverterEngine, CsvToJsonConverterEngine>()
                    .BuildServiceProvider();

                var engine = serviceProvider.GetService<ICsvToJsonConverterEngine>();
                engine.Start();
            });
        }
    }
}
