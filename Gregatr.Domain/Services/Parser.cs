using HtmlAgilityPack;

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
