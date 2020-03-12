namespace LAB.DataScanner.Components.Extractors.TextExtractor
{
    public interface IExtractionMethods
    {
        string Between(string startWord, string stopWord, string parsingData);
        string Contains(string targetWord, string parsingData);
    }
}
