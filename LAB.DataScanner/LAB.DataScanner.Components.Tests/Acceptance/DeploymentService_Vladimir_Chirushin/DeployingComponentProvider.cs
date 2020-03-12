namespace LAB.DataScanner.Components.Tests.Acceptance.DeploymentService_Vladimir_Chirushin
{
    using System.Collections.Generic;
    using System.Linq;
    using LAB.DataScanner.Components.Deployers.Chirushin;
    using Newtonsoft.Json;

    public class DeployingComponentProvider : IComponentDetailsReader
    {
        private List<DeployingComponent> deployingComponent = new List<DeployingComponent>();

        public DeployingComponent GetComponentDetails(int serviceId)
        {
            return deployingComponent
                        .Where(x => x.componentId == serviceId)
                        .FirstOrDefault();
        }

        public void RegisterApp(int componentId, string applicationName, string appVersion, Dictionary<string, string> config)
        {
            deployingComponent.Add(
                new DeployingComponent()
                {
                    componentId = componentId,
                    typeId = componentId,
                    instanceName = applicationName,
                    configJson = JsonConvert.SerializeObject(config),
                    typeName = applicationName,
                    appTypeVersion = appVersion,
                    packagePath = "C:\\temp\\" + appVersion + "\\LAB.DataScanner." + applicationName.Remove(applicationName.Length - 4) + ".zip",
                    senderExchange = config["Binding_SenderExchange"],
                    senderRoutingKeys = config["Binding_SenderRoutingKeys"].Split(','),
                    receiverQueue = config["Binding_ReceiverQueue"]
                });
        }

        public DeployingComponent GetSimpleImageExtractor()
        {
            return new DeployingComponent()
            {
                componentId = 3,
                typeId = 3,
                instanceName = "SimpleImageParserType",
                configJson =
                @"{
""Binding_SenderExchange"":""TestSenderExchange"",
""Binding_SenderRoutingKeys"":""TestSenderRoutingKeys1 TestSenderRoutingKeys2 TestSenderRoutingKeys3"",
""Application_TargetUrls"":""https://habr.com/ru/ https://www.spbstu.ru/"",
}",
                typeName = "SimpleImageParserType",
                appTypeVersion = "20191224.10",
                packagePath = "C:\\temp\\20191224.10\\LAB.DataScanner.SimpleImageParser.zip",

                senderExchange = "SimpleImageParserType",
                senderRoutingKeys = new string[2]{
                    "SimpleImageParserType_CsvToJsonParser",
                    "SimpleImageParserType_SimpleImageParser"
                },
                receiverQueue = "SimpleHttpGetScraperType"
            };
        }
    }
}