using Gregatr.Domain.Services;
//using System.Runtime.CompilerServices;

//[assembly: InternalsVisibleTo("GreGatr.Tests")]
namespace GreGatr.Domain.Entities
{
    public class ManipulationRule
    {
        public enum Type
        {
            Replace,
            ReplaceAndApply,
            Prepend,
            ToLower
        }
        public Type RuleType { get; }
        public string? Find { get; }
        public string? ReplaceWith { get; }
        public string? Prefix { get; }
        public ManipulationRule? NestedRule { get; }


        // Constructor for ManipulationRule
        public ManipulationRule(Type ruleType, string? prefix = null, string? find = null, string? replaceWith = null, ManipulationRule? nestedRule = null)
        {
            RuleType = ruleType;
            Prefix = prefix;
            Find = find;
            ReplaceWith = replaceWith;
            NestedRule = nestedRule;
        }

        public void Apply(ref string itemIdentifier)
        {
            ApplyControlled(ref itemIdentifier, 0);
        }
        private void ApplyControlled(ref string itemIdentifier, int depth)
        {
            //nested rules recursion depth control, recompile if limit changes (unlikely)
            depth++;
            if (depth > 10) return;


            switch (RuleType)
            {
                case Type.Replace:
                    if (Find is not null) itemIdentifier = itemIdentifier.Replace(Find, ReplaceWith);
                    break;
                case Type.ReplaceAndApply:

                    string temp = string.Empty;
                    if (Find is not null && ReplaceWith is not null)
                        temp = itemIdentifier.Replace(Find, ReplaceWith);
                    //if itemIdentifier has a substring match to replace, then proceed with additional processing
                    if (!(itemIdentifier.Equals(temp)))
                    {
                        itemIdentifier = temp;
                        if (NestedRule is not null) NestedRule.ApplyControlled(ref itemIdentifier, depth);
                    }
                    break;
                case Type.Prepend:
                    itemIdentifier = Prefix + itemIdentifier;
                    break;
                case Type.ToLower:
                    itemIdentifier = itemIdentifier.ToLower();
                    break;
            }
        }
    }
    public class Result
    {
        public string Name;

        public string Content;
    }
    public class Reference
    {



        /// <summary>
        /// //no need for explicit backing field, if auto property can be set only privatley, (no need for => expression either)
        /// </summary>
        public Uri? ContentURI { get; private set; }
        public required string BaseUrl { get; set; }
        public required string ImageSrc { get; set; }
        /// <summary>
        /// The path to the searched result within this reference
        public required string SearchPath { get; set; }
        public required string ResultName { get; set; }

        public Result? Result { get; }
        public List<ManipulationRule>? ItemMatchRules { get; set; }


        
        internal async Task<object> GetResultAsync(string itemIdentifier)
        {
            /// <summary>
            /// The list of rules that the reference will use to manipulate the supplied identifier.
            /// This is done to "translate" the identifier in order to provide matching content.
            /// </summary>
            ///private List<ManipulationRule> _itemMatchRules;
            if (ItemMatchRules != null)
            {
                foreach (var rule in ItemMatchRules)
                {
                    rule.Apply(ref itemIdentifier);
                }
            }
            //TODO: IoC
            var content = await Fetcher.GetContentAsync(BaseUrl+itemIdentifier);
            var parser = new Parser(); 
            var result = parser.ParseContent(content, SearchPath);
            System.Diagnostics.Debug.WriteLine($"{BaseUrl} finished");

            return result;
        }
    }
}


