using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerApp
{   
    static class Deck
    {
        public static List<string> LiveDeck;

        private static List<string> SortedDeck = new List<string> { "AD", "2D", "3D", "4D", "5D", "6D", "7D", "8D", "9D", "10D", "JD", "QD", "KD",
                                                                    "AH", "2H", "3H", "4H", "5H", "6H", "7H", "8H", "9H", "10H", "JH", "QH", "KH",
                                                                    "AS", "2S", "3S", "4S", "5S", "6S", "7S", "8S", "9S", "10S", "JS", "QS", "KS",
                                                                    "AC", "2C", "3C", "4C", "5C", "6C", "7C", "8C", "9C", "10C", "JC", "QC", "KC"};
                
        
        public static List<string> PokerHandsList = new List<string> { "High Card", "One Pair", "Two Pair", "Three of a Kind", "Straight", "Flush",
                                                                       "Full House", "Four of a Kind", "Straight Flush", "Royal Flush"};

        public enum EnumPokerHands
        {
            HighCard,
            OnePair,
            TwoPair,
            ThreeOfAKind,
            Straight,
            Flush,
            FullHouse,
            FourOfAKind,
            StraightFlush,
            RoyalFlush
        }     

        internal static void  ShuffleDeck() 
        {
            LiveDeck = SortedDeck.ToList();                   
            LiveDeck.Shuffle();           
        }

        //Replacing all suits and face cards so calculations can be done against ints
        internal static List<int> ReplaceFaceCardsWithInts(List<string> fullBoardList)
        {
            //think I can also do something like below using Select().ToList() for replace, instead of looping through each position
            for (var i = 0; i < fullBoardList.Count; i++)
            {
                fullBoardList[i] = fullBoardList[i].Replace("A", "14").Replace("J", "11").Replace("Q", "12").Replace("K", "13");
            }
            
            var onlyNumbersList = fullBoardList.Select(int.Parse).ToList();

            return onlyNumbersList;
        }

        internal static int ReplaceFaceCardWithInts(string card)
        {
            card = card.Replace("A", "14").Replace("J", "11").Replace("Q", "12").Replace("K", "13");

            return Convert.ToInt32(card);
        }

        internal static List<string> RemoveSuits(List<string> fullBoardList)
        {
            for (var i = 0; i < fullBoardList.Count; i++)
            {
                fullBoardList[i] = fullBoardList[i].Replace("H", "").Replace("D", "").Replace("S", "").Replace("C", "");
            }

            return fullBoardList;
        }
    }
}
