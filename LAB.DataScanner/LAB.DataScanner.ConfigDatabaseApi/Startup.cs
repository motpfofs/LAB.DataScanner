namespace LAB.DataScanner.ConfigDatabaseApi
{
    using System.Linq;
    using LAB.DataScanner.ConfigDatabaseApi.Models;
    using Microsoft.AspNet.OData.Builder;
    using Microsoft.AspNet.OData.Extensions;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OData.Edm;
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOData();
            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2); 
            
            services.AddDbContext<ILABDataScannerConfigDatabaseContext, LABDataScannerConfigDatabaseContext>(
                 options => options.UseSqlServer(Configuration.GetConnectionString("DataScannerConfigDatabase")));

            services.AddCors(options =>
            {
                options.AddPolicy("AllowUIOrigin", builder => builder.WithOrigins("http://localhost:4200"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors("AllowUIOrigin");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc(routebuilder =>
            {
                routebuilder.EnableDependencyInjection();
                routebuilder.Expand().Select().Count().Filter().OrderBy().MaxTop(10);
                routebuilder.MapODataServiceRoute("ODataRoute", "odata", GetEdmModel());
            });
        }

        private static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            {
                var et = builder.EntitySet<ApplicationType>("ApplicationTypes").EntityType;
                et.HasKey(p => p.TypeId);
                et.HasMany(c => c.ApplicationInstance);
            }

            {
                var et = builder.EntitySet<ApplicationInstance>("ApplicationInstances").EntityType;
                et.HasKey(p => p.InstanceId);
            }

            {
                var et = builder.EntitySet<Binding>("Bindings").EntityType;
                et.HasKey(p => new { p.PublisherInstanceId, p.ConsumerInstanceId });
                et.HasRequired(c => c.ConsumerInstance);
                et.HasRequired(c => c.PublisherInstance);
            }

            return builder.GetEdmModel();
        }
    }
}
