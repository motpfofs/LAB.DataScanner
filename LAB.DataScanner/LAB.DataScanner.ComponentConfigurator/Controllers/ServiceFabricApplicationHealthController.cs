namespace LAB.DataScanner.ComponentConfigurator.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.ServiceFabric.Client;

    [Route("api/InstanceHealthCheck")]
    [ApiController]
    public class ServiceFabricApplicationHealthController : ControllerBase
    {
        private const string sfClusterUri = "http://localhost:19080";

        private readonly IServiceFabricClient serviceFabricClient;

        public ServiceFabricApplicationHealthController()
        {
            this.serviceFabricClient = new ServiceFabricClientBuilder()
                   .UseEndpoints(new Uri(sfClusterUri))
                   .BuildAsync().Result;
        }

        [HttpGet]
        public ActionResult Get(string instanceName)
        {
            var test = this.serviceFabricClient.Applications.GetApplicationHealthAsync(instanceName);
            return Ok(new
            {
                test.Id,
                test.IsCompletedSuccessfully
            });
        }
    }
}
