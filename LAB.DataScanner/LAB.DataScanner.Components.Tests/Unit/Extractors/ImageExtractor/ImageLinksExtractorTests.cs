namespace LAB.DataScanner.Components.Tests.Unit.Extractors.ImageExtractor
{
    using System.Collections.Generic;
    using LAB.DataScanner.Components.DataRetriever;
    using LAB.DataScanner.Components.Extractors.ImageExtractor;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    [TestClass]
    public class ImageLinksExtractorTests
    {
        [TestMethod]
        public void ShouldCallDataRetrieverForCorrectLink()
        {
            // Arrange
            var dataRetrieverMock = Substitute.For<IDataRetriever>();
            var sut = new ImageLinksExtractor(dataRetrieverMock);

            string testLinkToPage = "TargetPage";

            // Act
            var result = sut.Extract(testLinkToPage);

            // Assert
            dataRetrieverMock
                .Received(1)
                .RetrieveStringAsync(Arg.Is(testLinkToPage));
        }

        [TestMethod]
        public void ShouldReturnLinksListWithAllImageLinks()
        {
            // Arrange
            var dataRetrieverMock = Substitute.For<IDataRetriever>();
            var sut = new ImageLinksExtractor(dataRetrieverMock);

            string testLinkToPage = "http://ya.ru";
            List<string> expectedLinks = new List<string>();
            expectedLinks.Add("http://ya.ru/images/testImage1.png");
            expectedLinks.Add("http://ya.ru/images/testImage2.png");
            expectedLinks.Add("http://ya.ru/images/testImage3.png");

            string testPageContent = @"<!DOCTYPE html>
<html>

   <head>
      <title>Using Image in Webpage</title>
   </head>
	
   <body>
      <p>Simple Image Insert</p>
      <img src = ""./images/testImage1.png"" alt = ""Test Image 1"" />
      <img src = ""./images/testImage2.png"" alt = ""Test Image 2"" />
      <img src = ""./images/testImage3.png"" alt = ""Test Image 3"" />
      </body>
   
</html> ";
            dataRetrieverMock
                .RetrieveStringAsync(Arg.Is(testLinkToPage))
                .Returns(testPageContent);

            // Act
            var result = sut.Extract(testLinkToPage).Result;

            // Assert
            CollectionAssert.AreEqual(result, expectedLinks);
        }
    }
}
