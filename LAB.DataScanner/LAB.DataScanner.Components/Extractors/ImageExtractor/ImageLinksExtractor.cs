namespace LAB.DataScanner.Components.Extractors.ImageExtractor
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HtmlAgilityPack;
    using LAB.DataScanner.Components.DataRetriever;

    public class ImageLinksExtractor : IImageLinksExtractor
    {
        private readonly IDataRetriever dataRetriever;

        public ImageLinksExtractor(IDataRetriever dataRetriever)
        {
            this.dataRetriever = dataRetriever;
        }

        public async Task<List<string>> Extract(string urlToParse)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(await dataRetriever.RetrieveStringAsync(urlToParse));

            List<string> imageLinks = new List<string>();

            var allImageNodes = doc.DocumentNode.SelectNodes("//img/@src");

            foreach (HtmlNode node in allImageNodes)
            {
                var rawImageLink = node.Attributes["src"];

                var imageLink = new Uri(rawImageLink.Value, UriKind.RelativeOrAbsolute);
                if (!imageLink.IsAbsoluteUri)
                {
                    imageLink = new Uri(new Uri(urlToParse), imageLink);
                }

                imageLinks.Add(imageLink.ToString());
            }

            return imageLinks;
        }
    }
}
