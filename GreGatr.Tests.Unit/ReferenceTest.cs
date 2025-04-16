using System.Text.Json;
using GreGatr.Domain.Entities;

namespace GreGatr.Tests.Unit
{
    [TestFixture]
    public class ReferenceSetup
    {

        private const string ReferencesFilePath = "ReferencesTestConfig.json";
        [SetUp]
        public static void Setup()
        {
            var referencesConfig = new List<Reference>
            {
                SetUpReference1(),
                SetUpReference2()
            };

            // Use the default serializer to serialize the references list
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            // Serialize the references list to JSON
            var json = JsonSerializer.Serialize(referencesConfig, options);

            // Write the JSON to a file
            File.WriteAllText(ReferencesFilePath, json);
        }

        private static Reference SetUpReference1()
        {
            var reference = new Reference
            {
                BaseUrl = "http://www.rottentomatoes.com/m/",
                ImageSrc = "<img src='http://2.bp.blogspot.com/-URl-RB28yA4/TYK1UU3MfYI/AAAAAAAAANE/lji-MV2rXFs/s1600/RTFresh.jpg'/>",
                ItemMatchRules =
                [
                new (ManipulationRule.Type.Replace, null, "An ", string.Empty),
                new (ManipulationRule.Type.Replace, null, " ", "_"),
                new (ManipulationRule.Type.Replace, null, "'", string.Empty),
                new (ManipulationRule.Type.Replace, null, ":", string.Empty)
                ],
                SearchPath = "//span[@id='all-critics-meter']",
                ResultName = "RTScore"
            };
            return reference;
        }

        private static Reference SetUpReference2()
        {
            var reference = new Reference
            {
                BaseUrl = "http://www.metacritic.com/",
                ImageSrc = "<img src='http://1.bp.blogspot.com/-7XnuexOMaqk/TYK1yLzUmMI/AAAAAAAAANM/Mp7KGOphWug/s400/meta.jpg'/>",
                ItemMatchRules =
                [
                    new (ManipulationRule.Type.ReplaceAndApply, null, ": Seaso", "/seaso", new ManipulationRule(ManipulationRule.Type.Prepend, "tv/")),
                new (ManipulationRule.Type.Prepend, "movie/"),
                new (ManipulationRule.Type.Replace, null, " ", "-"),
                new (ManipulationRule.Type.Replace, null, "'", string.Empty),
                new (ManipulationRule.Type.Replace, null, ":", string.Empty),
                new (ManipulationRule.Type.ToLower)
                ],
                SearchPath = "//div[@class='score_summary metascore_summary']/div/div/a/span[@class='score_value']",
                ResultName = "MetaScore"
            };
            return reference;
        }

        [Test]
        public static void SetUpReferencesShouldProperlyDeserializeFromFile()
        {
            // Read the JSON from the file
            string json = File.ReadAllText(ReferencesFilePath);

            // Deserialize the JSON into a list of Reference objects
            var references = JsonSerializer.Deserialize<List<Reference>>(json);

            //This also makes sure the reference was set up as intended in SetUpReference2()
            Assert.That(references![1].ItemMatchRules![0].NestedRule!.Prefix!.Length, Is.GreaterThan(0));

        }
    }
}

