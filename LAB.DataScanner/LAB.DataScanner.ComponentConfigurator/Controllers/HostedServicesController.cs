namespace LAB.DataScanner.ComponentConfigurator.Controllers
{
    using System.Threading.Tasks;
    using LAB.DataScanner.Components.Providers;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/hosted-services/[action]")]
    [ApiController]
    public class HostedServicesController : ControllerBase
    {
        private readonly IDeploymentServiceProvider deploymentServiceProvider;
        public HostedServicesController(
            IDeploymentServiceProvider deploymentServiceProvider)
        {
            this.deploymentServiceProvider = deploymentServiceProvider;
        }

        [HttpPost]
        public async Task<ActionResult> Start(string targetDeploymentService)
        {
            await this.deploymentServiceProvider.Start(targetDeploymentService);
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> Stop(string targetDeploymentService)
        {
            await this.deploymentServiceProvider.Stop(targetDeploymentService);
            return Ok();
        }
    }
}
