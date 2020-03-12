namespace LAB.DataScanner.ComponentConfigurator
{
    using System.Net.Http;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;
    using LAB.DataScanner.ComponentConfigurator.Services;
    using LAB.DataScanner.Components.Providers;
    using LAB.DataScanner.Components.Deployers;
    using LAB.DataScanner.Components.Deployers.Chirushin;
    using LAB.DataScanner.Components.Loggers;
    using LAB.DataScanner.Components.MessageBroker.RabbitMq;
    using LAB.DataScanner.Components.MessageBroker.RabbitMqBuilder;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<IISServerOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });

            services.AddControllers();

            services.AddHostedService<DeploymentHostedService>();

            var deploymentServiceConfigSection = this.Configuration.GetSection("DeploymentService");
            var rmqPublisher = new RmqPublisherBuilder()
                .UsingDefaultConnectionSetting()
                .Build();

            var rmqConsumer = new RmqConsumerBuilder()
                .UsingDefaultConnectionSetting()
                .Build();

            services.AddSingleton<IRmqConsumer>(rmqConsumer);
            services.AddSingleton<IRmqPublisher>(rmqPublisher);
            services.AddScoped<IRmqBindingService, RmqBindingService>();
            services.AddScoped<IRmqManagementService, RmqManagementService>();
            services.AddSingleton<IDeploymentServiceProvider, DeploymentServiceProvider>();
            services.AddSingleton<IConfigurationSection>(deploymentServiceConfigSection);
            
            //dependency for DeploymentServiceChirushin
            services.AddSingleton<IDeploymentService, DeploymentServiceChirushin>();
            services.AddSingleton<IDeploymentManagerWrapper, DeploymentManagerWrapper>();
            services.AddSingleton<IComponentDetailsReader, ComponentDetailsReader>();

            services.AddSingleton<IInfoLogger, Logger>();
            
            services.AddScoped(typeof(HttpClient));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Component Configurator", Description = "Component Configurator" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Test Api v0.1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}