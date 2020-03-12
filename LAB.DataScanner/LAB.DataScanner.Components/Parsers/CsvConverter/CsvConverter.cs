namespace LAB.DataScanner.Components.Parsers.CsvConverter
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class CsvConverter : ICsvConverter
    {
        private const char ValueSeparator = ',';

        private char[] newLineSeparator = new[] { '\r', '\n' };

        public string ConvertToJson(string fileName, string csvString, int[] columns, int rows)
        {
            var csvLines = csvString.Split(newLineSeparator, StringSplitOptions.RemoveEmptyEntries);

            var parsedCsv = new List<Dictionary<string, string>>();
            Dictionary<string, string> elementAsDict;

            string[] csvHeader = csvLines[0].Split(ValueSeparator);
            string[] parsingElement;
            int maxRows = csvLines.Length < rows ? csvLines.Length : rows;
            for (int i = 1; i < maxRows; i++)
            {
                parsingElement = csvLines[i].Split(ValueSeparator);

                elementAsDict = new Dictionary<string, string>();
                foreach (int column in columns)
                {
                    if (column <= parsingElement.Length)
                    {
                        elementAsDict.Add(csvHeader[column - 1], parsingElement[column - 1]);
                    }
                }

                parsedCsv.Add(elementAsDict);
            }

            var dataToJson = new Dictionary<string, List<Dictionary<string, string>>>();
            dataToJson.Add(fileName, parsedCsv);

            var result = JsonConvert.SerializeObject(dataToJson);

            return result;
        }
    }
}