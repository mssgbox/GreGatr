using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;
using GreGatr.Domain.Services;
namespace GreGatr.Tests.Unit
{
    [TestFixture]
    public class AggregatorTests
    {
        private const string ValidConfigFilePath = "validConfig.json";
        private const string InvalidConfigFilePath = "invalidConfig.json";
        private const string FeedFilePath = "testFeed.xml";
        private const string OutputFilePath = "outputFile.xml";

        [SetUp]
        public void SetUp()
        {
            // Create a sample valid config file (this would normally be read from disk)
            if (!File.Exists(ValidConfigFilePath))
            {
                var validConfigJson = "[{\"ResultName\": \"TestResult\", \"ImageSrc\": \"image.png\", \"ContentURI\": \"http://test.com\"}]";
                File.WriteAllText(ValidConfigFilePath, validConfigJson);
            }

            // Ensure any necessary test feed file exists
            if (!File.Exists(FeedFilePath))
            {
                var testFeedXml = "<rss version=\"2.0\"><channel><title>Test Feed</title><item><title>Test Item</title><description>Test Description</description></item></channel></rss>";
                File.WriteAllText(FeedFilePath, testFeedXml);
            }
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up after tests
            if (File.Exists(ValidConfigFilePath)) File.Delete(ValidConfigFilePath);
            if (File.Exists(FeedFilePath)) File.Delete(FeedFilePath);
            if (File.Exists(OutputFilePath)) File.Delete(OutputFilePath);
        }

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

        [Test]
        public void GenerateAndWriteFeed_ShouldWriteFeedToFile()
        {
            var aggregator = new Aggregator();
            aggregator.WriteFeed(new SyndicationFeed());

            // Assert the feed file has been created
            Assert.That(File.Exists(Settings.OutputFileName), Is.True);
        }

    }
}
