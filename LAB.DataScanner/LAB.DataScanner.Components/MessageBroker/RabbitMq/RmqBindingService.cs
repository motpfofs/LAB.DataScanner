namespace LAB.DataScanner.Components.MessageBroker.RabbitMq
{
    using System;
    using System.Threading.Tasks;
    using LAB.DataScanner.Components.Models;

    public class RmqBindingService : IRmqBindingService
    {
        private const string SuccessMessage = "Configuration is completed";
        private const string EmptyExchangeErrorMessage = "Exchange name cannot be empty";
        private const string EmptyQueueErrorMessage = "Queue name cannot be empty";

        private IRmqManagementService RmqManagementService { get; }

        public RmqBindingService(IRmqManagementService RmqManagementService)
        {
            this.RmqManagementService = RmqManagementService;
        }      
         
        public string Build(BindingConfig data)
        {
            try
            {
                this.Configure(data).GetAwaiter().GetResult();
            }
            catch (ArgumentException ex)
            {
                return ex.Message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return SuccessMessage;
        }

        private async Task Configure(BindingConfig exchangeData)
        {
            ValidateBindingData(exchangeData);

            this.RmqManagementService.Auth();
            await this.RmqManagementService.CreateDirectExchange(exchangeData.ComponentName);
            foreach (var queue in exchangeData.Bindings)
            {
                await this.RmqManagementService.CreateQueue(queue.ComponentName);
                await this.RmqManagementService.BindQueueToExchange(exchangeData.ComponentName, queue);
            }
        }

        private void ValidateBindingData(BindingConfig exchangeData)
        {
            if (string.IsNullOrWhiteSpace(exchangeData.ComponentName))
            {
                throw new ArgumentException(EmptyExchangeErrorMessage);
            }

            foreach (var queue in exchangeData.Bindings)
            {
                if (string.IsNullOrWhiteSpace(queue.ComponentName))
                {
                    throw new ArgumentException(EmptyQueueErrorMessage);
                }
            }
        }
    }   
}