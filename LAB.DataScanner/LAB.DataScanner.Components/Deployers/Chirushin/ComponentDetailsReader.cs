namespace LAB.DataScanner.Components.Deployers.Chirushin
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using Microsoft.Extensions.Configuration;

    public class ComponentDetailsReader : IComponentDetailsReader
    {
        private const string dropFolderPath = "C:\\temp\\";

        public const string connectionSectionName = "ConnectionSettings";

        private const string GetNameTypeConfigQueryString =
            "SELECT ComponentName, TypeId, ConfigJson FROM components.ApplicationInstance WHERE InstanceID = ";

        private const string GetAppTypeNameQuerryString =
            "SELECT TypeName FROM meta.ApplicationType WHERE Id = ";

        private const string GetPublisherForComponentQueryString =
            "SELECT PublisherInstanceID FROM binding.Binding WHERE ConsumerInstanceID = ";

        private const string GetInstanceNameById =
            "SELECT InstanceName FROM component.ApplicationInstance WHERE InstanceID = ";

        private const string GetConsumersForComponentQueryString =
            "SELECT ConsumerInstanceID FROM binding.Binding WHERE PublisherInstanceID = ";

        public readonly IConfigurationSection configurationSection;

        public ComponentDetailsReader(IConfigurationSection configurationSection)
        {
            this.configurationSection = configurationSection;
        }

        public DeployingComponent GetComponentDetails(int serviceId)
        {
            var deployingComponent = new DeployingComponent { componentId = serviceId };

            string connString = configurationSection.GetConnectionString(connectionSectionName);

            deployingComponent = PrepareDeployingComponentWithDataFromDB(deployingComponent, connString);
            deployingComponent.senderExchange = deployingComponent.typeName.Remove(deployingComponent.typeName.Length - 4);
            deployingComponent.senderRoutingKeys = GetRoutingKeys(deployingComponent);
            deployingComponent.receiverQueue = GetSourceQueue(deployingComponent);
            deployingComponent.packagePath = dropFolderPath + deployingComponent.appTypeVersion +
                "LAB.DataScanner." + deployingComponent.typeName.Remove(deployingComponent.typeName.Length - 4) + ".zip";

            return deployingComponent;
        }

        private DeployingComponent PrepareDeployingComponentWithDataFromDB(
            DeployingComponent deployingComponent,
            string connString)
        {

            using (SqlConnection connection = new SqlConnection(connString))
            using (SqlCommand command = connection.CreateCommand())
            {
                connection.Open();

                command.CommandText = GetNameTypeConfigQueryString + deployingComponent.componentId;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        deployingComponent.instanceName = reader["ComponentName"].ToString();
                        deployingComponent.typeId = int.Parse(reader["TypeId"].ToString());
                        deployingComponent.configJson = reader["ConfigJson"].ToString();
                        deployingComponent.appTypeVersion = reader["TypeVersion"].ToString();
                    }
                }

                command.CommandText = GetAppTypeNameQuerryString + deployingComponent.typeId;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        deployingComponent.typeName = reader["TypeName"].ToString();
                    }
                }

                return deployingComponent;
            }
        }


        private string GetSourceQueue(DeployingComponent deployingComponent)
        {
            string connString = configurationSection.GetConnectionString(connectionSectionName);


            using (SqlConnection connection = new SqlConnection(connString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = GetPublisherForComponentQueryString + deployingComponent.componentId; ;

                connection.Open();

                string publisherInstanceIDAsString = null;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        publisherInstanceIDAsString = reader["PublisherInstanceID"].ToString();
                    }
                }

                command.CommandText = GetInstanceNameById + publisherInstanceIDAsString;

                string publisherName = null;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        publisherName = reader["InstanceName"].ToString();
                    }
                }

                string sourceQueue = publisherName + "_" + deployingComponent.instanceName;
                return sourceQueue;
            }
        }

        private string[] GetRoutingKeys(DeployingComponent deployingComponent)
        {
            string connString = configurationSection.GetConnectionString(connectionSectionName);

            using (SqlConnection connection = new SqlConnection(connString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = GetConsumersForComponentQueryString + deployingComponent.componentId.ToString();

                connection.Open();

                var allConsumersId = new List<int>();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        allConsumersId.Add(int.Parse(reader["ConsumerInstanceID"].ToString()));
                    }
                }

                var routingKeys = new List<string>();
                foreach (var consumerId in allConsumersId)
                {
                    command.CommandText = GetInstanceNameById + consumerId.ToString();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            routingKeys.Add(
                                deployingComponent.instanceName + "_" +
                                reader["InstanceName"].ToString());
                        }
                    }
                }

                return routingKeys.ToArray();
            }
        }
    }
}
