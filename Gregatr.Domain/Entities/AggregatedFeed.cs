using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.ServiceModel.Syndication;
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
            //TODO: check for performance problems from copying huge feeds under load

            var tagPrefix = new XmlQualifiedName(Settings.Prefix, Settings.XMLNamespace);
            base.AttributeExtensions.Add(tagPrefix, Settings.DaNamespace);

            this.Items = sourceFeed.Items;

        }
        public void Sort()
        {
            //TODO: use LINQ instead of IComparer?
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
                //add the info synchronously: add await at this point or WhenAll()
             
                List<Task<object>> tasks = new List<Task<object>>();
                foreach (var reference in references)
                {
                    //try to add references asynchronously
                    var task = reference.GetResultAsync(feedItem.Title.Text);//AddReferenceInfoAsync(reference, feedItem);
                    tasks.Add(task);
                }
                var results = await Task.WhenAll(tasks);
                
                //TODO: smells hacky; also consider AggregatedFeedItem class
                for(int i = 0; i<references.Count(); i++)
                {
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
            if (reference.ContentURI != null)
            {
                summary = summary.Insert(0, $"{reference.ImageSrc}<a href='{reference.ContentURI.AbsoluteUri}'>{result}</a><br />");
            }
            else
            {
                summary = summary.Insert(0, $"{reference.ImageSrc}{result}<br />");
            }
            feedItem.Summary = new TextSyndicationContent(summary);
        }
    }
}
