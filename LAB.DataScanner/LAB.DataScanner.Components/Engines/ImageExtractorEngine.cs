namespace LAB.DataScanner.Components.Engines
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using LAB.DataScanner.Components.DataRetriever;
    using LAB.DataScanner.Components.Extractors.ImageExtractor;
    using LAB.DataScanner.Components.MessageBroker.RabbitMqBuilder;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    public class ImageExtractorEngine : IImageExtractorEngine
    {
        private const string BindingConfigurationSectionName = "Binding";
        private const string ApplicationConfigurationSectionName = "Application";
        private const string SenderExchangeConfigurationName = "SenderExchange";
        private const string SenderRoutingKeysConfigurationName = "SenderRoutingKeys";
        private const string TargetUrlsConfigurationSection = "TargetUrls";

        private const char routingKeySplitChar = ',';

        private const char TargetUrlsSplitChar = ' ';

        private readonly IConfigurationSection configurationSection;
        private readonly IImageLinksExtractor imagesLinksExtractor;
        private readonly IRmqPublisher rmqPublisher;
        private readonly IDataRetriever dataRetriever;

        public ImageExtractorEngine(
            IImageLinksExtractor imagesLinksExtractor,
            IDataRetriever dataRetriever,
            IConfigurationSection configurationSection,
            IRmqPublisher rmqPublisher)
        {
            this.imagesLinksExtractor = imagesLinksExtractor;
            this.dataRetriever = dataRetriever;
            this.configurationSection = configurationSection;
            this.rmqPublisher = rmqPublisher;
        }

        public void Start()
        {
            var applicationConfiguration = configurationSection.GetSection(ApplicationConfigurationSectionName);
            var targetUrlsEntriesAsString = applicationConfiguration.GetValue<string>(TargetUrlsConfigurationSection);
            var targetUrlsEntries = targetUrlsEntriesAsString.Split(TargetUrlsSplitChar);

            var bindingConfiguration = configurationSection.GetSection(BindingConfigurationSectionName);

            var senderExchange = bindingConfiguration.GetValue<string>(SenderExchangeConfigurationName);
            var senderRoutingKeysAsString = bindingConfiguration.GetValue<string>(SenderRoutingKeysConfigurationName);

            var senderRoutingKeysAsArray = senderRoutingKeysAsString.Split(routingKeySplitChar);

            List<string> imagesLinks;
            byte[] imageData;
            foreach (var link in targetUrlsEntries)
            {
                imagesLinks = imagesLinksExtractor.Extract(link).Result;

                if (imagesLinks != null)
                {
                    foreach (var imageLink in imagesLinks)
                    {
                        imageData = dataRetriever.RetrieveBytesAsync(imageLink).Result;
                        string imageMessage = CreateImageMessage(imageData, imageLink);
                        byte[] messageToRabbit = Encoding.UTF8.GetBytes(imageMessage);
                        rmqPublisher.Publish(messageToRabbit, senderExchange, senderRoutingKeysAsArray);
                    }
                }
            }
        }

        private string CreateImageMessage(byte[] imageData, string imageLink)
        {
            var imageMessage = new ImageMessage();

            int pos = imageLink.LastIndexOf("/") + 1;   // cutting image name from link
            string imageName = imageLink.Substring(pos, imageLink.Length - pos);

            imageMessage.ImageAsBase64 = Convert.ToBase64String(imageData);
            imageMessage.ImageNameWithExtension = imageName;

            string output = JsonConvert.SerializeObject(imageMessage);
            return output;
        }
    }
}