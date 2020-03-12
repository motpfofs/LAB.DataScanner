namespace LAB.DataScanner.Components.Extractors.ImageExtractor
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IImageLinksExtractor
    {
        Task<List<string>> Extract(string urlToParse);
    }
}
