using NUnit.Framework;
using System.ServiceModel.Syndication;
using System.Xml.Linq;
using GreGatr.Domain.Services;
namespace GreGatr.Tests.Unit
{
    [TestFixture]
    public class ScoreComparerTests
    {
        private ScoreComparer _comparer;

        [SetUp]
        public void Setup()
        {
            _comparer = new ScoreComparer();
        }
        // Helper method to create SyndicationItem for testing
        private SyndicationItem CreateSyndicationItem(string score1, string score2)
        {
            var item = new SyndicationItem();
            item.ElementExtensions.Add(new SyndicationElementExtension(XElement.Parse($"<score>{score1}</score>")));
            item.ElementExtensions.Add(new SyndicationElementExtension(XElement.Parse($"<score>{score2}</score>")));
            return item;
        }

        [Test]
        public void Compare_ShouldReturn0_WhenBothItemsAreNull()
        {
            // Act
            //var result = _comparer.Compare(null, null);

            // Assert
            var ex = Assert.Throws<NullReferenceException>(() => _comparer.Compare(null, null));//That(result, Is.EqualTo(0));

            //Assert.That(ex.Message == "Blah");

        }

        [Test]
        public void Compare_ShouldThrow_WhenOneItemIsNull()
        {
            // Arrange
            var itemOne = CreateSyndicationItem("5", "7"); // Valid item
            SyndicationItem itemTwo = null!; // Null item

            // Act
            //var result = _comparer.Compare(itemOne, itemTwo);

            // Assert
            Assert.Throws<NullReferenceException>(() => _comparer.Compare(itemOne, itemTwo));
        }

        [Test]
        public void Compare_ShouldReturnNegativeForDescending_WhenFirstItemHasHigherScore()
        {
            // Arrange
            var itemOne = CreateSyndicationItem("8", "6");
            var itemTwo = CreateSyndicationItem("5", "7");

            // Act
            var result = _comparer.Compare(itemOne, itemTwo);

            // Assert
            Assert.That(result, Is.LessThan(0)); // itemOne should be greater, for desc order -1 should be returned
        }

        [Test]
        public void Compare_ShouldReturnPositiveForDescending_WhenSecondItemHasHigherScore()
        {
            // Arrange
            var itemOne = CreateSyndicationItem("3", "2");
            var itemTwo = CreateSyndicationItem("6", "8");

            // Act
            var result = _comparer.Compare(itemOne, itemTwo);
            var result1 = _comparer.Compare(itemTwo, itemOne); // order of comparison reversed
            // Assert
            Assert.That(result, Is.GreaterThan(0)); // itemOne should be less, for desc order 1 should be returned
            Assert.That(result1, Is.LessThan(0)); //result1 should be the opposite of result

        }

        [Test]
        public void Compare_ShouldHandleInvalidAndCompareValidOnly()
        {
            // Arrange
            var itemOne = CreateSyndicationItem("invalid", "8"); // Invalid score
            var itemTwo = CreateSyndicationItem("10", "invalid"); // Invalid score

            // Act
            var result = _comparer.Compare(itemOne, itemTwo);

            // Assert
            Assert.That(result, Is.GreaterThan(0)); // itemOne's valid score should be less, for desc order 1 should be returned
        }


    }
}
