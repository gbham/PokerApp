
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerApp
{   
    public static class Utils
    {
        //Not sure if this should be in Utils
        internal static void Shuffle<T>(this List<T> list)
        {
            var rng = new Random();
            var n = list.Count;

            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        internal static string GetUserInputtedCommand(string input)
        {
            var action = "";

            for (var i = 0; i < input.Length; i++)
            {
                if (Char.IsLetter(input[i]))
                {
                    action += input[i];
                }
            }

            return action.ToLower();
        }

        internal static int GetUserInputtedValue(string input)
        {
            var value = "";

            for (var i = 0; i < input.Length; i++)
            {
                if (Char.IsDigit(input[i]))
                {
                    value = input.Substring(i, (input.Length - i));
                    break;
                }
            }

            if (string.IsNullOrWhiteSpace(value)) { return 0; }

            return Convert.ToInt32(value);
        }

        internal static void DetermineBestHandFromBoardWith2ThreeOfAKinds(Player player, string matchedCharOne, string matchedCharTwo)
        {
            player.HighestThreeOfAKindValue = Utils.GetHighestCardFromTwoChoices(matchedCharOne, matchedCharTwo);

            //if i have found 2 instances of three of a kind, then whatever is not the highest, will be the value for the pair in the "full house"
            if (player.HighestThreeOfAKindValue.Equals(Deck.ReplaceFaceCardWithInts(matchedCharOne)))
            {
                player.HighestPair = Deck.ReplaceFaceCardWithInts(matchedCharTwo);
            }
            else
            {
                player.HighestPair = Deck.ReplaceFaceCardWithInts(matchedCharOne);
            }
        }

        internal static void DetermineBestHandFromBoardWith2Pairs(Player player, string matchedCharOne, string matchedCharTwo)
        {
            player.HighestPair = Utils.GetHighestCardFromTwoChoices(matchedCharOne, matchedCharTwo);

            //if i have found 2 instances of pair, then whatever is not the highest, will be the value for the 2nd highest pair
            if (player.HighestPair.Equals(Deck.ReplaceFaceCardWithInts(matchedCharOne)))
            {
                player.SecondHighestPair = Deck.ReplaceFaceCardWithInts(matchedCharTwo);
            }
            else
            {
                player.SecondHighestPair = Deck.ReplaceFaceCardWithInts(matchedCharOne);
            }
        }

        internal static void DetermineBestHandFromBoardWith3Pairs(Player player, string matchedCharOne, string matchedCharTwo, string matchedCharThree)
        {
            var highestChars = Utils.DetermineHighestCardsFromThreeChoices(matchedCharOne, matchedCharTwo, matchedCharThree);

            player.HighestPair = highestChars[0];
            player.SecondHighestPair = highestChars[1];
        }

        //Not sure what I should do with the functions below, "Player" was gettnig a bit full and undecided on the best place to store them
        internal static int GetHighestCardFromTwoChoices(string matchedCharOne, string matchedCharTwo)
        {
            matchedCharOne = matchedCharOne.Replace("A", "14").Replace("J", "11").Replace("Q", "12").Replace("K", "13");
            matchedCharTwo = matchedCharTwo.Replace("A", "14").Replace("J", "11").Replace("Q", "12").Replace("K", "13");

            var matchedChars = new List<int>() { Convert.ToInt32(matchedCharOne), Convert.ToInt32(matchedCharTwo) };
            return matchedChars.Max();
        }

        internal static List<int> DetermineHighestCardsFromThreeChoices(string matchedCharOne, string matchedCharTwo, string matchedCharThree)
        {
            matchedCharOne = matchedCharOne.Replace("A", "14").Replace("J", "11").Replace("Q", "12").Replace("K", "13");
            matchedCharTwo = matchedCharTwo.Replace("A", "14").Replace("J", "11").Replace("Q", "12").Replace("K", "13");
            matchedCharThree = matchedCharThree.Replace("A", "14").Replace("J", "11").Replace("Q", "12").Replace("K", "13");

            var matchedChars = new List<int>() { Convert.ToInt32(matchedCharOne), Convert.ToInt32(matchedCharTwo), Convert.ToInt32(matchedCharThree) };
            var highestChars = new List<int>();

            highestChars.Add(matchedChars.Max());
            matchedChars.Remove(highestChars[0]);

            highestChars.Add(matchedChars.Max());
            matchedChars.Remove(highestChars[1]);

            highestChars.Add(matchedChars.Max());

            return highestChars;
        }

        internal static List<int> GetBestKickersFrom(List<string> list)
        {
            list = Deck.RemoveSuits(list);
            var bestKickersList = Deck.ReplaceFaceCardsWithInts(list);

            bestKickersList.Sort();
            bestKickersList.Reverse();

            return bestKickersList;
        }

        internal static int GetBestKickerFrom(List<string> list)
        {
            list = Deck.RemoveSuits(list);
            var bestKickerList = Deck.ReplaceFaceCardsWithInts(list);

            bestKickerList.Sort();
            bestKickerList.Reverse();

            var bestKicker = bestKickerList[0];

            return bestKicker;
        }

        //"10" is the only card that has 2 digits at this stage in the code (face cards are still J,Q,K etc)
        internal static string HandleAnyInstancesOf10CardFound(string matchedChar)
        {
            //"10" is the only card that has 2 digits at this stage in the code (face cards are still J,Q etc)
            //this means if I matched a 1 in my reg ex, then that 1 must belong to a 10.
            if (matchedChar == "1") { matchedChar = "10"; }

            return matchedChar;
        }
    }
}
