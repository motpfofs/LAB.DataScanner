namespace LAB.DataScanner.Components.Persisters.SimpleDataPersister.Model
{
    using System.Collections.Generic;

    public class DataSchema
    {
        public string TableName { get; set; }

        public Dictionary<string, string> Header { get; set; }
    }
}
