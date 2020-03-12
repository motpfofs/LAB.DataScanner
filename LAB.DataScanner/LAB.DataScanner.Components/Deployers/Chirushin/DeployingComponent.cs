namespace LAB.DataScanner.Components.Deployers.Chirushin
{
    public class DeployingComponent
    {
        public int componentId;
        public int typeId;
        public string instanceName;
        public string configJson;
        public string typeName;
        public string appTypeVersion;
        public string packagePath;

        public string senderExchange;
        public string[] senderRoutingKeys;
        public string receiverQueue;
    }
}
