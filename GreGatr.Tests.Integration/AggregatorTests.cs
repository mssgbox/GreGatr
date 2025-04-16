using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using GreGatr.Domain.Services;
using System.Xml;

namespace GreGatr.Tests.Integration
{
    internal class AggregatorTests
    {
        [Test]
        public async Task TestAggregateMoviesAsync()
        {
            var aggregator = new Aggregator();

            await aggregator.AggregateMoviesAsync();

            // Validate that the output file was written
            Assert.That(File.Exists(Settings.OutputFileName), Is.True);

            // Optionally, validate the content of the output file (here we assume a specific structure or summary)
            SyndicationFeed outputFeed;
            using (var reader = XmlReader.Create(Settings.OutputFileName))
            {
                outputFeed = SyndicationFeed.Load(reader);
            }
            var firstScore = outputFeed.Items.FirstOrDefault()?.ElementExtensions.First();
            Assert.That(firstScore?.OuterName, Is.EqualTo(Settings.ResultNameRT));
        }
    }
}
