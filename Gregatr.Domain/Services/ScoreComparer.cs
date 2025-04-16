using System;
using System.ServiceModel.Syndication;
using System.Xml.Linq;

namespace GreGatr.Domain.Services
{
    public class ScoreComparer : IComparer<SyndicationItem>
    {
        public int Compare(SyndicationItem? itemOne, SyndicationItem? itemTwo)
        {
            //if (x == null || y == null) return 0; 
            //for now we want to throw an exception, if somehow item(s) are null
            int higherScoreOne = GetHigherScore(itemOne!);
            int higherScoreTwo = GetHigherScore(itemTwo!);

            //explicit results instead of return higherScoreOne.CompareTo(higherScoreTwo); 
            //default sort order with ICompare.Compare (and CompareTo in comment above) is acsending (smaller value on top) i.e: if (higherScoreOne < higherScoreTwo) return -1;
            //for descending order flip the logic:
            if (higherScoreOne > higherScoreTwo)
                return -1;
            else if (higherScoreOne == higherScoreTwo)
                return 0;
            else 
                return 1; //(higherScoreOne < higherScoreTwo)
        }   

        private static int GetHigherScore(SyndicationItem item)
        {
            int higherScore = 0;
            foreach (var scoreExtension in item.ElementExtensions)
            {
                string scoreStr =((XElement)(XElement.ReadFrom(scoreExtension.GetReader()))).Value;
                int.TryParse(scoreStr, out int score);

                if (score > higherScore) higherScore = score;
            }
            return higherScore;
        }
    }
}

