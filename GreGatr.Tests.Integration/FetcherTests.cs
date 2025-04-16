using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;
using System.ServiceModel.Syndication;
using System;
using Gregatr.Domain.Services;
//using Moq;

namespace Gregatr.Tests.Integration
    {
        [TestFixture] // Test class containing multiple tests
        public class FetcherTests
        {
            private string _testUrl;

            [SetUp] // Setup method to initialize any common data or state for each test
            public void SetUp()
            {
                _testUrl = "https://hdrezka.tv/animation/best"; // Replace with an actual URL for integration tests
            }

            // Test for successful content fetching
            [Test] // Marks the method as a test
            public async Task GetContentAsync_ShouldReturnContent_WhenRequestIsSuccessful()
            {
                // Arrange
                var expectedContentSnippet = "hdrezka";  // Replace with a part of the content you expect from the URL

                // Act
                var result = await Fetcher.GetContentAsync(_testUrl);

                // Assert
                Assert.That(result, Is.Not.Null.And.Contains(expectedContentSnippet), "Content should contain expected text.");
            }
        // Test for successful content fetching
        [Test] // Marks the method as a test
        public async Task GetContentPlayWriteAsync_ShouldReturnContent_WhenRequestIsSuccessful()
        {
            // Arrange
            var expectedContentSnippet = "hdrezka";  // Replace with a part of the content you expect from the URL

            // Act
            var result = await Fetcher.GetContentPlaywriteAsync(_testUrl);

            // Assert
            Assert.That(result, Is.Not.Null.And.Contains(expectedContentSnippet), "Content should contain expected text.");
        }
        // Test for handling a 404 Not Found response
        [Test]
            public async Task GetContentAsync_ShouldReturnEmpty_WhenUrlNotFound()
            {
                // Arrange
                var invalidUrl = "https://nonexistent-url.com";  // A URL that does not exist

                // Act
                var result = await Fetcher.GetContentAsync(invalidUrl);

                // Assert
                Assert.That(result, Is.Empty, "Content should be empty for a 404 response.");
            }


            // Test the SyndicationFeed loading from a real feed URL
            [Test]
            public void GetSourceFeed_ShouldReturnValidFeed_FromRealUri()
            {
                // Arrange
                var feedUri = new Uri("https://rss.nytimes.com/services/xml/rss/nyt/HomePage.xml"); // A real RSS feed

                // Act
                var feed = Fetcher.GetSourceFeed(feedUri);

                // Assert
                Assert.That(feed, Is.Not.Null, "Feed should not be null.");
                Assert.That(feed.Items, Is.Not.Empty, "Feed should contain items.");
            }

            [TearDown] // Cleanup after each test (if needed)
            public void TearDown()
            {
                // Clean up after each test if needed (e.g., closing resources).
            }
        }
    }
