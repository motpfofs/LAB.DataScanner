namespace LAB.DataScanner.Components.Extractors.TextExtractor
{
    using System.Text;

    public class ExtractionMethods : IExtractionMethods
    {
        private char[] sentenceSplitter = new char[] { '.', '!', '?' };

        public string Between(string startWord, string stopWord, string parsingData)
        {
            int startPosition, endPosition;
            if (parsingData.Contains(startWord) && parsingData.Contains(stopWord))
            {
                startPosition = parsingData.IndexOf(startWord, 0) + startWord.Length;
                endPosition = parsingData.IndexOf(stopWord, startPosition);
                return parsingData.Substring(startPosition, endPosition - startPosition);
            }
            else
            {
                return string.Empty;
            }
        }

        public string Contains(string targetWord, string parsingData)
        {
            var result = new StringBuilder();
            var sentences = parsingData.Split(sentenceSplitter);
            foreach (var sentence in sentences)
            {
                if (sentence.Contains(targetWord))
                {
                    result.Append(sentence);
                }
            }

            return result.ToString().Trim();
        }
    }
}
