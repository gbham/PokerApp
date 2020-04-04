using System;
using System.Collections.Generic;

namespace PokerApp
{   
    static class Deck
    {
        public static List<string> LiveDeck;
        private static List<string> SortedDeck = new List<string> { "AD", "2D", "3D", "4D", "5D", "6D", "7D", "8D", "9D", "10D", "JD", "QD", "KD",
                                                                    "AH", "2H", "3H", "4H", "5H", "6H", "7H", "8H", "9H", "10H", "JH", "QH", "KH",
                                                                    "AS", "2S", "3S", "4S", "5S", "6S", "7S", "8S", "9S", "10S", "JS", "QS", "KS",
                                                                    "AC", "2C", "3C", "4C", "5C", "6C", "7C", "8C", "9C", "10C", "JC", "QC", "KC"};
        
        //Not sure if these will be needed
        private static List<string> DiamondSuit = new List<string> { "AD", "2D", "3D", "4D", "5D", "6D", "7D", "8D", "9D", "10D", "JD", "QD", "KD" };

        private static List<string> HeartsSuit =  new List<string> { "AH", "2H", "3H", "4H", "5H", "6H", "7H", "8H", "9H", "10H", "JH", "QH", "KH" };

        private static List<string> SpadesSuit =  new List<string> { "AS", "2S", "3S", "4S", "5S", "6S", "7S", "8S", "9S", "10S", "JS", "QS", "KS" };

        private static List<string> ClubsSuit =   new List<string> { "AC", "2C", "3C", "4C", "5C", "6C", "7C", "8C", "9C", "10C", "JC", "QC", "KC" };

        //not sure if this is needed now
        public static List<string> PokerHands = new List<string> { "High Card", "One Pair", "Two Pair", "Three of a Kind", "Straight", "Flush", "Full House", "Four of a Kind", "Straight Flush", "Royal Flush"};

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

        //I dont need to have an "Ace" at position one cause if there is an ace on the board then that would be the starting point.
        //public enum EnumPokerCards
        //{
        //    0,
        //    1,
        //    2,
        //    3,
        //    4,
        //    5,
        //    6,
        //    7,
        //    8,
        //    9,
        //    10,
        //    J,
        //    Q,
        //    K
        //}




        internal static void  ShuffleDeck() //List<string>
        {
            LiveDeck = SortedDeck;            
            LiveDeck.Shuffle();            

            //return LiveDeck;
        }

        

    }
}
