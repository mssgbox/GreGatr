using HtmlAgilityPack;
using System;
using System.Text.Json;
namespace Gregatr.Domain.Services
{
    internal class Parser
    {
        private HtmlDocument? _content;
#if DEBUG
        //TODO: AK - multiple processes / threads
        public static int _countScoreRT;//, _countNoScoreRT, _countNotFoundRT, _countNotFoundMeta, _countNoScoreMeta, _countScoreMeta;
#endif
        internal object ParseContent(string relevantHTML, string searchPath)
        {
            //System.Console.WriteLine($"[LOG]: {searchPath} finished");
            //System.Console.Write($"[==start]: {relevantHTML} ==end");
            
            string result = "Not Found";
            _content = new HtmlDocument();
            _content.LoadHtml(relevantHTML);

            HtmlNode node = _content.DocumentNode.SelectSingleNode(searchPath);
            
            if (node != null)
            {
                var score = String.Empty;
                score = node.InnerText;
                score = score.Replace("%", ""); // RT specific processing
                if (score != String.Empty)
                {
                    result = score;
                    _countScoreRT++;
                }
                
            }
            return result;
        }
internal object ParseJSONContent(string relevantHTML, string jsonPath)
        {
            //System.Console.WriteLine($"[LOG]: {searchPath} finished");
            //System.Console.Write($"[==start]: {relevantHTML} ==end");
            
            string result = "Not Found";
            _content = new HtmlDocument();
            _content.LoadHtml(relevantHTML);


        // This single XPath works universally on Metacritic, Rotten Tomatoes, and IMDb
        HtmlNode node = _content.DocumentNode.SelectSingleNode(jsonPath);
            
            if (node != null)
            {
                using (JsonDocument jsonDoc = JsonDocument.Parse(node.InnerText))
                {
                    JsonElement root = jsonDoc.RootElement;

                    //TODO Handle arrays (some sites embed multiple schemas in a single tag block)
                    if (root.ValueKind == JsonValueKind.Object)
                    {
                        if (TryExtractScore(root, out string score)) result = score;
                    }
                }
            }
            return result;
        }

        private static bool TryExtractScore(JsonElement element, out string score)
    {
        score = String.Empty;
        if (element.TryGetProperty("aggregateRating", out JsonElement aggregateRating) &&
            aggregateRating.TryGetProperty("ratingValue", out JsonElement ratingValue))
        {
            // Metacritic stores numbers as integers/doubles directly; RT stores them as strings
            score = ratingValue.ToString();
            return true;
        }
        return false;
    }
//        private string FindResult()
//        {
//            //NOTE: Result class when more functionality
//            string result = string.Empty; //default value
//            if (_content == null)
//            {
//#if DEBUG
//                if (ResultName == Settings.ResultNameRT)
//                { _countNotFoundRT++; }
//                else { _countNotFoundMeta++; }
//#endif
//                result = "Not Found";
//                // If there is no document, there is no point
//                return result;
//            }

//            HtmlNode node = _content.DocumentNode.SelectSingleNode(SearchPath);
//            if (node is null)
//            {
//                //page found, but no node
//#if DEBUG
//                if (ResultName == Settings.ResultNameRT)
//                { _countNoScoreRT++; }
//                else { _countNoScoreMeta++; }
//#endif
//                result = "No Node";
//                return result;
//            }
//            else
//            {
//                result = node.InnerText;
//                result = result.Replace("%", ""); //NOTE: RT specific

//#if DEBUG
//                if (result == string.Empty || result == "tbd" || result.Contains("No Review")) //TODO: AK -can be 'No Reviews Yet...'?
//                {
//                    if (ResultName == Settings.ResultNameRT)
//                    { _countNoScoreRT++; }
//                    else { _countNoScoreMeta++; }
//                }
//                else
//                {
//                    if (ResultName == Settings.ResultNameRT)
//                    { _countScoreRT++; }
//                    else { _countScoreMeta++; }
//                }
//#endif
//            }
//            return result;
//        }
    }
}
