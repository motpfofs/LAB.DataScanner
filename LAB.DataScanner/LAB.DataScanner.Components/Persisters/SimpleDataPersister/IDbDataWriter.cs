namespace LAB.DataScanner.Components.Persisters.SimpleDataPersister
{
    public interface IDbDataWriter
    {
        void SaveMessagesToDb(string[] jsonStrings);
    }
}
