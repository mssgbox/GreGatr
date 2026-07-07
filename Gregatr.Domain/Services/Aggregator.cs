using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Text.Json;
using System.Xml;
using Gregatr.Domain.Services;//TODO: why necessary here?
using GreGatr.Domain.Entities;
public static class Settings
{
    //TODO: Settings provider, config file
    //private const string DefaultFeedUri = "http://www.netflix.com/NewWatchInstantlyRSS";

    //public static string FilesDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName, "files");
    public static readonly string baseDir = AppDomain.CurrentDomain.BaseDirectory;
    public static readonly string envDir = Environment.CurrentDirectory;
    public static readonly string outputDir = envDir + "/wwwroot/";
    public static readonly string ReferencesConfigFileName = "ReferencesConfig.json";
    public static readonly string SourceFeedFileName = "InputMovieList.xml";
    public static readonly string OutputFileName = "outputFeedFile.xml";

    public static readonly string DaNamespace = "http://pisanina.blogspot.com";
    public static readonly string Prefix = "da";
    public static readonly string XMLNamespace = "http://www.w3.org/2000/xmlns/";
    public static readonly string ResultNameRT = "RTScore";

}

namespace GreGatr.Domain.Services
{

    public class Aggregator
    {
        public async System.Threading.Tasks.Task AggregateMovies2Async()
        {
            //Setup source: address, path, etc
            var movieList = Fetcher.GetContentAsync("https://hdrezka.tv/animation/best");

            //var outputFeed = new AggregatedFeed(source);
            ////TODO: references are an ISource?
            //var references = SetupReferences();
            //await outputFeed.AddReferencesAsync(references);
            ////System.Diagnostics.Debug.WriteLine(sw.Elapsed.ToString());
            //outputFeed.Sort();
            //WriteFeed(outputFeed);

        }
        public async System.Threading.Tasks.Task AggregateMoviesAsync()
        {
            var references = SetupReferences();
            if (references != null)
            {
                //Uri sourceFeedUri = new Uri(DefaultFeedUri);
                var movieList = Fetcher.GetSourceFeed(System.IO.Path.Combine(Settings.baseDir,Settings.SourceFeedFileName));
                var outputFeed = new AggregatedFeed(movieList);
                //AddReferences has some async work (fetch two references per item asynchronously)
                //async has to be "bubbled up" all the way to parent caller
                //Stopwatch sw = Stopwatch.StartNew();
                await outputFeed.AddReferencesAsync(references);
                //System.Diagnostics.Debug.WriteLine(sw.Elapsed.ToString());
                outputFeed.Sort();
                WriteFeed(outputFeed);
            }
        }
        // Set up references dynamically from the configuration file
        public List<Reference>? SetupReferences()
        {
            // Read the configuration file content
            string json = System.IO.File.ReadAllText(System.IO.Path.Combine(Settings.baseDir, Settings.ReferencesConfigFileName));
            // Deserialize the JSON content into a ReferenceConfig List
            var references = JsonSerializer.Deserialize<List<Reference>>(json);
            return references;
        }
        public void WriteFeed(SyndicationFeed outputFeed)
        {
            // Clear the current response
            //HttpContext.Current.Response.Clear();
            // Using block for automatic resource management
            //TODO: Consider multiple users generating the same file

            using (var feedWriter = XmlWriter.Create(System.IO.Path.Combine(Settings.outputDir, Settings.OutputFileName))) //(HttpContext.Current.Response.OutputStream))
            {
                outputFeed.SaveAsRss20(feedWriter);
            }
        }





    }

}
