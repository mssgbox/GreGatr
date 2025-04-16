using System.ServiceModel.Syndication;
using System.Xml.Linq;
using GreGatr.Domain.Entities;
using NUnit.Framework.Interfaces;

namespace GreGatr.Tests.Unit
{
    [TestFixture]
    public class MovieFeedTests
    {
        // Helper method to create a test SyndicationFeed
        private static SyndicationFeed CreateTestSourceFeed()
        {
            var feed = new SyndicationFeed("Test Feed", "Description", new Uri("http://example.com"));

            var item1 = new SyndicationItem("Movie A", "Description of Movie A", new Uri("http://example.com/movieA"));
            var item2 = new SyndicationItem("Movie B", "Description of Movie B", new Uri("http://example.com/movieB"));
            feed.Items = new List<SyndicationItem> { item1, item2 };
            return feed;
        }
        // Helper method to create SyndicationItem for testing
        private static SyndicationItem CreateSyndicationItem(string name, string description, Uri link, string score1, string score2)
        {
            var item = new SyndicationItem(name, description, link);
            item.ElementExtensions.Add(new SyndicationElementExtension(XElement.Parse($"<score>{score1}</score>")));
            item.ElementExtensions.Add(new SyndicationElementExtension(XElement.Parse($"<score>{score2}</score>")));
            return item;
        }
        private static AggregatedFeed CreateTestMovieFeed()
        {
            // Arrange
            var sourceFeed = CreateTestSourceFeed();

            var movieFeed = new AggregatedFeed(sourceFeed);

            AddScores(movieFeed.Items.ElementAt(0), "55", "65");
            AddScores(movieFeed.Items.ElementAt(1), "55", "65");

            return movieFeed;
        }
        private static void AddScores(SyndicationItem item, string score1, string score2)
        {
            item.ElementExtensions.Add(new SyndicationElementExtension("RTScore", Settings.DaNamespace, score1));
            item.ElementExtensions.Add(new SyndicationElementExtension("MetaScore", Settings.DaNamespace, score2));
        }

        [Test]
        public void ModifySourceFeedDirectly()
        {
            // Arrange
            var sourceFeed = CreateTestSourceFeed();


            // Act
            var movieFeed = new AggregatedFeed(sourceFeed);

            // Assert
            Assert.That(movieFeed.Items.Count(), Is.EqualTo(sourceFeed.Items.Count()), "The number of items should match the source feed.");
            Assert.That(movieFeed.Items, Is.Not.Null, "Items should not be null.");

            // Verify that the namespace was added correctly
            var expectedNamespace = Settings.DaNamespace;
            var hasNamespace = movieFeed.AttributeExtensions.Any(ext => ext.Value == expectedNamespace);
            Assert.That(hasNamespace, Is.True, "Custom namespace should be added to AttributeExtensions.");
        }

        [Test]
        public void Constructor_InitializesMovieFeedCorrectly()
        {
            // Arrange
            var sourceFeed = CreateTestSourceFeed();

            // Act
            var movieFeed = new AggregatedFeed(sourceFeed);

            // Assert
            Assert.That(movieFeed.Items.Count(), Is.EqualTo(sourceFeed.Items.Count()), "The number of items should match the source feed.");
            Assert.That(movieFeed.Items, Is.Not.Null, "Items should not be null.");

            // Verify that the namespace was added correctly
            var expectedNamespace = Settings.DaNamespace;
            var hasNamespace = movieFeed.AttributeExtensions.Any(ext => ext.Value == expectedNamespace);
            Assert.That(hasNamespace, Is.True, "Custom namespace should be added to AttributeExtensions.");
        }

        [Test]
        public void Sort_SortsFeedItemsCorrectly()
        {
            // Arrange
            var movieFeed = CreateTestMovieFeed();

            // Add additional mock items to test sorting
            var itemC = CreateSyndicationItem("Movie C", "Description of Movie C", new Uri("http://example.com/movieC"), "invalidScore", "9");
            var itemD = CreateSyndicationItem("Movie D", "Description of Movie D", new Uri("http://example.com/movieD"), "8", "9");
            movieFeed.Items = new List<SyndicationItem> { itemD, itemC, movieFeed.Items.First(), movieFeed.Items.Last() };

            // Act
            movieFeed.Sort();

            // Assert
            var sortedItems = movieFeed.Items.ToList();
            Assert.That(sortedItems[0].Title.Text == "Movie A");
            Assert.That(sortedItems[1].Title.Text == "Movie B");
            Assert.That(sortedItems[2].Title.Text == "Movie D");
            //Movie C was added after D, should stay after D, because higher valid scores for C and D are equal
            Assert.That(sortedItems[3].Title.Text == "Movie C");

        }

    }
}
