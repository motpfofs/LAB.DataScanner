namespace LAB.DataScanner.Components.Tests.Unit.Parsers.CsvConvertor
{
    using LAB.DataScanner.Components.Parsers.CsvConverter;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CsvConverterTests
    {
        [TestMethod]
        public void ShouldParseCsvToJson()
        {
            // Arrange
            string testFileName = "TestFileName";
            string testData =
@"#,Name,City
1,Gauguin,Paris
2,Cezanne,Aix-en-Provence
3,Edgar,Paris
4.Claude,Paris 
5,Edouard,Paris
6,Renoir,Limotges";
            string expectedResult = @"{""TestFileName"":[{""#"":""1"",""Name"":""Gauguin"",""City"":""Paris""},{""#"":""2"",""Name"":""Cezanne"",""City"":""Aix-en-Provence""},{""#"":""3"",""Name"":""Edgar"",""City"":""Paris""},{""#"":""4.Claude"",""Name"":""Paris ""},{""#"":""5"",""Name"":""Edouard"",""City"":""Paris""},{""#"":""6"",""Name"":""Renoir"",""City"":""Limotges""}]}";

            var sut = new CsvConverter();

            // Act
            var result = sut.ConvertToJson(testFileName, testData, new int[] { 1, 2, 3, 4, 5, 6 }, 705);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
