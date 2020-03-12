namespace LAB.DataScanner.Components.Engines
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using LAB.DataScanner.Components.MessageBroker.RabbitMqBuilder;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    public class UrlsGeneratorEngine : IUrlsGeneratorEngine
    {
        private readonly IRmqPublisher rmqPublisher;
        private readonly IConfigurationSection configurationSection;

        private string templateUrl;
        private string[] sequences;
        private string exchangeName;
        private string[] routingKeys;

        public UrlsGeneratorEngine(IRmqPublisher rmqPublisher,
            IConfigurationSection configurationSection)
        {
            this.rmqPublisher = rmqPublisher;
            this.configurationSection = configurationSection;
        }

        public void Start()
        {
            ReadAllSettings();

            var listOfRanges = this.BuildRangeOptions(sequences);
            var rangeOptions = this.BuildCrossList(listOfRanges)
                .Where(w => w.Count() == sequences.Length)
                .ToList();

            var urlsList = this.BuildUrlsList(templateUrl, rangeOptions);

            if (!string.IsNullOrWhiteSpace(exchangeName))
            {
                this.Publish(urlsList, exchangeName, routingKeys);
            }
            else
            {
                throw new Exception("Missing binding exchange info in configuration");
            }
        }

        private void Publish(IEnumerable<string> urlsList, string exchangeName, string[] routingKeys)
        {
            foreach (var url in urlsList)
            {
                foreach (var rk in routingKeys)
                {
                    this.rmqPublisher.Publish(Encoding.UTF8.GetBytes(url), exchangeName, rk);
                }
            }
        }

        private List<int[]> BuildRangeOptions(string[] sequences)
        {
            var listOfRanges = new List<int[]>();
            for (int i = 0; i < sequences.Length; i++)
            {
                var sequenceExpression = sequences[i].Split("..");
                var lowValue = int.Parse(sequenceExpression[0]);
                var highValue = int.Parse(sequenceExpression[1]);

                listOfRanges.Add(Enumerable.Range(lowValue, (highValue - lowValue) + 1).ToArray());
            }

            return listOfRanges;
        }

        private IEnumerable<string> BuildUrlsList(string templateUrl, List<IEnumerable<int>> rangeOptions)
        {
            var retVal = new List<string>();

            foreach (var rangeOption in rangeOptions)
            {
                var url = templateUrl;

                for (int i = 0; i < rangeOption.Count(); i++)
                {
                    var fragmentToReplace = "{" + i + "}";

                    url = url.Replace(fragmentToReplace, rangeOption.ElementAt(i).ToString());
                }

                retVal.Add(url);
            }

            return retVal;
        }

        private IEnumerable<IEnumerable<T>> BuildCrossList<T>(IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> accumulator = new[] { Enumerable.Empty<T>() };
            var result = new List<IEnumerable<T>>();
            var firstSequence = true;
            foreach (var sequence in sequences)
            {
                var local = new List<IEnumerable<T>>();
                foreach (var accseq in accumulator)
                {
                    if (!firstSequence)
                        result.Add(accseq);

                    foreach (var item in sequence)
                        local.Add(accseq.Concat(new[] { item }));
                }
                firstSequence = false;
                accumulator = local;
            }

            return result.Concat(accumulator);
        }

        private void ReadAllSettings()
        {
            var applicationSection = configurationSection.GetSection("Application");
            var bindingSection = configurationSection.GetSection("Binding");

            this.templateUrl = applicationSection.GetValue<string>("UrlTemplate");
            this.sequences = JsonConvert.DeserializeObject<string[]>(applicationSection.GetValue<string>("Sequences"));
            
            this.exchangeName = bindingSection.GetValue<string>("SenderExchange");
            this.routingKeys = JsonConvert.DeserializeObject<string[]>(bindingSection.GetValue<string>("SenderRoutingKeys"));
        }
    }
}
