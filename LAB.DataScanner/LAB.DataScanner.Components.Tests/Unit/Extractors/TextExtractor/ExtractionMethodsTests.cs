namespace LAB.DataScanner.Components.Tests.Unit.Extractors.TextExtractor
{
    using LAB.DataScanner.Components.Extractors.TextExtractor;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ExtractionMethodsTests
    {
        [TestMethod]
        public void ShouldReturnTextBetweenTwoTargetWords()
        {
            // Arrange
            string testStartWord = "brown";
            string testEndWord = "lazy";
            string testParsingData = "Quick brown fox jump over the lazy dog.";
            string expectedResult = " fox jump over the ";
            var sut = new ExtractionMethods();

            // Act
            var result = sut.Between(testStartWord, testEndWord, testParsingData);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }
        
        [TestMethod]
        public void ShouldReturnSentenceThatContainsTargetWord()
        {
            // Arrange
            string targetWord = "sentence";
            string testParsingData = "Target word is not here. New sentence? It can be anywhere! Is it here? Create new sentence. Searching for target word.";
            string expectedResult = "New sentence Create new sentence";
            var sut = new ExtractionMethods();

            // Act
            var result = sut.Contains(targetWord, testParsingData);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
