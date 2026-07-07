using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace GreGatr.Domain.Entities
{
    /// <summary>
    /// This custom SyndicationFeed adds custom attributes to its items (movie scores) based on supplied References
    /// has a custom Sort behavior
    /// </summary>
    public class AggregatedFeed : SyndicationFeed
    {
        public AggregatedFeed()
        {

            var tagPrefix = new XmlQualifiedName(Settings.Prefix, Settings.XMLNamespace);
            this.AttributeExtensions.Add(tagPrefix, Settings.DaNamespace);
        }
        public AggregatedFeed(SyndicationFeed sourceFeed)
        {
            //Copy items from source
            //NOTE: this.Items = sourceFeed.Items is just assigning the IEnumerable<> reference, not actually copying.

            var tagPrefix = new XmlQualifiedName(Settings.Prefix, Settings.XMLNamespace);
            base.AttributeExtensions.Add(tagPrefix, Settings.DaNamespace);

            this.Items = sourceFeed.Items;

        }
        public void Sort()
        {
            //TODO: use LINQ instead of IComparer?
/* var scores = items.ToDictionary(
    item => item,
    item => GetHigherScore(item));

items.Sort((a,b) => scores[b].CompareTo(scores[a])); */

            var itemsList = Items.ToList();
            itemsList.Sort(new Services.ScoreComparer());
            this.Items = itemsList;
        }
        internal async Task AddReferencesAsync(List<Reference> references)
        {   
            foreach (var feedItem in this.Items)
            {

                //ASYNC: 
                //foreach reference
                //get the info asynchronously: Tasks.Add without await
                //TODO: can parallelize feedItems too with SemaphorSlim to limit concurrent requests to the same Reference source
                //however need to also consider multiple users launching same enrichment at the same time
                
                //TODO: Wrap each task so failures return a fallback instead of throwing
                // var task = reference.GetResultAsync(feedItem.Title.Text)
                //     .ContinueWith(t => t.IsFaulted ? (object)"Not Found" : t.Result);
                List<Task<object>> tasks = new List<Task<object>>();
                foreach (var reference in references)
                {
                    //try to add references asynchronously
                    var task = reference.GetResultAsync(feedItem.Title.Text);//AddReferenceInfoAsync(reference, feedItem);
                    tasks.Add(task);
                }
                var results = await Task.WhenAll(tasks);
                
                //this is done synchronously because the same feed item is modified
                //TODO: consider AggregatedFeedItem class
                for(int i = 0; i<references.Count(); i++)
                {
                    //Task.WhenAll() guarantees that the result array preserves the order of the input tasks
                    AddReferenceInfo(references[i], results[i], feedItem);
                }
            }
        }
        private void AddReferenceInfo (Reference reference, Object result, SyndicationItem feedItem)
        {
            //var result = await reference.GetResultAsync(feedItem.Title.Text);
            //var result = reference.GetResult(feedItem.Title.Text);
            feedItem.ElementExtensions.Add(new SyndicationElementExtension(reference.ResultName, Settings.DaNamespace, result));
            string summary = feedItem.Summary.Text;

            string encodedResult = System.Net.WebUtility.HtmlEncode(result?.ToString() ?? string.Empty);

            if (reference.ContentURI != null)
            {
                summary = summary.Insert(0, $"{reference.ImageSrc}<a href=\"{reference.ContentURI.AbsoluteUri}\">{encodedResult}</a><br />");
            }
            else
            {
                summary = summary.Insert(0, $"{reference.ImageSrc}{encodedResult}<br />");
            }

            feedItem.Summary = new TextSyndicationContent(summary);
        }
    }
}
