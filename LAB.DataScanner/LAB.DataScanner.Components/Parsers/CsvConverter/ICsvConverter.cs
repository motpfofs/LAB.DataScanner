namespace LAB.DataScanner.Components.Parsers.CsvConverter
{
    public interface ICsvConverter
    {
        string ConvertToJson(string fileName, string csvString, int[] columns, int rows);
    }
}
