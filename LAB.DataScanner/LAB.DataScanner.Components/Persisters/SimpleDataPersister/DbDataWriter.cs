namespace LAB.DataScanner.Components.Persisters.SimpleDataPersister
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using LAB.DataScanner.Components.Persisters.SimpleDataPersister.Model;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    public class DbDataWriter : IDbDataWriter
    {
        public const string ConnectionSectionName = "ConnectionString";
        public const string DataSchemaConfigurationName = "DataSchemaBase64";
        private const string ApplicationConfigurationSectionName = "Application";

        private readonly IConfigurationSection configurationSection;
        private DataSchema dataSchema;
        private string connectionString;

        public DbDataWriter(IConfigurationSection configurationSection)
        {
            this.configurationSection = configurationSection;
        }

        public void SaveMessagesToDb(string[] jsonStrings)
        {
            ReadAllSettings();

            var optionsBuilder = new DbContextOptionsBuilder<PersisterContext>();
            var options = optionsBuilder
                .UseSqlServer(connectionString)
                .Options;

            using (var context = new PersisterContext(dataSchema, options))
            {
                foreach (var jsonString in jsonStrings)
                {
                    var entityType = context.Model.GetEntityTypes().First().ClrType;
                    var entityTypeInstance = Activator.CreateInstance(entityType);

                    var entityInChangeTracker = context.Add(entityTypeInstance);

                    var dataToWrite = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);

                    foreach (var key in dataToWrite.Keys)
                    {
                        entityInChangeTracker.Property(key).CurrentValue = dataToWrite[key];
                    }
                }

                var tableCreatingScript = context.Database.GenerateCreateScript().Replace("\r\nGO\r\n", "");
                var sqlCommandString = $"IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{dataSchema.TableName}]') AND type in (N'U')) BEGIN {tableCreatingScript} END";

                context.Database.ExecuteSqlCommand(sqlCommandString);
                context.SaveChanges();
            }
        }

        private void ReadAllSettings()
        {

            var appconfiguration = configurationSection
                .GetSection(ApplicationConfigurationSectionName);

            connectionString = appconfiguration
                .GetValue<string>(ConnectionSectionName);

            var dataSchemaBase64Encoded = appconfiguration
                .GetValue<string>(DataSchemaConfigurationName);

            byte[] dataSchemaJsonAsByteArray = Convert.FromBase64String(dataSchemaBase64Encoded);
            var dataSchemaJsonString = Encoding.UTF8.GetString(dataSchemaJsonAsByteArray);

            dataSchema = JsonConvert.DeserializeObject<DataSchema>(dataSchemaJsonString);
        }
    }
}
