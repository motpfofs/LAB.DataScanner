namespace LAB.DataScanner.ComponentConfigurator.Controllers
{
    using System.Threading.Tasks;
    using LAB.DataScanner.Components.Models;
    using Microsoft.AspNetCore.Mvc;
    using LAB.DataScanner.Components.MessageBroker.RabbitMq;

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BindingsController : ControllerBase
    {
        private readonly IRmqBindingService rmqBindingService;
        public BindingsController(IRmqBindingService rmqBindingService)
        {
            this.rmqBindingService = rmqBindingService;
        }

        [HttpPost]
        public async Task<ActionResult> Build([FromBody] BindingConfig data)
        {
            return Ok(this.rmqBindingService.Build(data));           
        }
    }
}
