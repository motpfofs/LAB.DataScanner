namespace LAB.DataScanner.Components.MessageBroker.RabbitMqBuilder
{
    using Microsoft.Extensions.Configuration;
    using RabbitMQ.Client;

    public abstract class RmqBuilder<T> where T : class
    {
        protected string userName = "guest";
        protected string password = "guest";
        protected string hostName = "localhost";
        protected int port = 5672;
        protected string virtualHost = "/";

        protected IConnectionFactory connectionFactory;
        protected IConnection connection;
        protected IModel amqpChannel;
        public RmqBuilder<T> UsingDefaultConnectionSetting()
        {
            return this;
        }

        public RmqBuilder<T> UsingConfigConnectionSettings(IConfigurationSection configurationSection)
        {
            this.userName = configurationSection.GetValue<string>("UserName");
            this.password = configurationSection.GetValue<string>("Password");
            this.hostName = configurationSection.GetValue<string>("HostName");
            this.port = configurationSection.GetValue<int>("Port");
            this.virtualHost = configurationSection.GetValue<string>("VirtualHost");
            return this;
        }

        public RmqBuilder<T> UsingCustomHost(string hostName)
        {
            this.hostName = hostName;
            return this;
        }

        public RmqBuilder<T> UsingCustomCredentials(string userName, string userPassword)
        {
            this.userName = userName;
            this.password = userPassword;
            return this;
        }

        public abstract T Build();
    }
}
