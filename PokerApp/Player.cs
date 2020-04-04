using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PokerApp
{
    class Player
    {
        public readonly static int StartingStack = 1000;
        private string name;
        private bool hasCards;
        private int chips = StartingStack;
        private int bestHandType = 0;
        private string highestCardInHandType;
        private string highestPair;
        private string secondHighestPair;
        private string threeOfAKindValue;
        private string highestStraightValue;
        private string highestFlushValue;
        //full house value.... - cant really fit this into 1 I dont think. Could maybe use threeOfAKind and Pair but that might cause more harm than good

        private string fourOfAKindValue;
        //straight flush ..... - I could also use either flush or straight, depends how I structure the code
        //royal flush ..... - I could also use either flush or straight, depends how I structure the code

        private string bestKicker;
        private int chipsNeededToCall;
        private int chipsBetThisRound;
        private string cardOne;
        private string cardTwo;
        private string cards;

        //Think I may also need a property to hold the 5 "best" cards, which I can then use to deduct the best kicker - not sure actually, determining the kicker might have to be a specific operation for each type of hand or could risk causing unwanted complexity in a single function

        
        // uncomment this once I have tested OnePairFound() etc etc ..
        //public string fullBoard { get { return this.CardOne + this.CardTwo + Board.FlopSlot1 + Board.FlopSlot2 + Board.FlopSlot3 + Board.TurnSlot + Board.RiverSlot; } }

        public string Name { get { return name; } set { name = value; } }
        public bool HasCards { get { return hasCards; } set { hasCards = value; } }
        public int Chips { get { return chips; } set { chips = value; } }
        public int BestHandType { get { return bestHandType; } set { bestHandType = value; } }

        //Im not sure what to name this or if this is the right way of doing it. Maybe better to have a separate value for each pair, 2 pair, three of a kind 
        //- basically I need somewhere to store what card their 3 of a kind is, or the highest card in the straight, or the highest card in the flush
        public string HighestCardInHandType { get { return highestCardInHandType; } set { highestCardInHandType = value; } }
        public string HighestPair { get { return highestPair; } set { highestPair = value; } }
        public string SecondHighestPair { get { return secondHighestPair; } set { secondHighestPair = value; } }
        public string ThreeOfAKindValue { get { return threeOfAKindValue; } set { threeOfAKindValue = value; } }
        public string HighestStraightValue { get { return highestStraightValue; } set { highestStraightValue = value; } }
        public string HighestFlushValue { get { return highestFlushValue; } set { highestFlushValue = value; } }
        public string FourOfAKindValue { get { return fourOfAKindValue; } set { fourOfAKindValue = value; } }
        public string BestKicker { get { return bestKicker; } set { bestKicker = value; } }


        public int ChipsNeededToCall { get { return chipsNeededToCall; } set { chipsNeededToCall = value; } }
        public int ChipsBetThisRound { get { return chipsBetThisRound; } set { chipsBetThisRound = value; } }

        //Not sure if I want to store both cards in the same property or not
        public string CardOne { get { return cardOne; } set { cardOne = value; } }
        public string CardTwo { get { return cardTwo; } set { cardTwo = value; } }
        public string Cards { get { return cards; } set { cards = value; } }

        public void Fold(Player player)
        {
            Console.WriteLine($"player [{player.name}] has folded.");
            player.HasCards = false;

            //Maybe have the code in here to determine if folding means there is only 1 player left in the hand, and therefore the hand is oer
        }

        public void Check(Player player)
        {
            Console.WriteLine($"player [{player.name}] has checked.");
            //Not sure if there is anything is to be done in here
        }

        public void Call(Player player)
        {
            Console.WriteLine($"player [{player.name}] has called the bet with [{player.ChipsNeededToCall}] chips. (Still need to assign ChipsNeededToCall)");

            Board.ChipsInPot += player.ChipsNeededToCall;
            player.Chips -= player.ChipsNeededToCall;
            player.ChipsBetThisRound += ChipsNeededToCall;
        }

        public void Bet(Player player, int value)
        {
            Console.WriteLine($"player [{player.name}] has placed a bet of [{value}] chips.");

            Board.ChipsInPot += value;
            player.Chips -= value;
            player.ChipsBetThisRound += value;
        }

        public void Raise(Player player, int value)
        {
            Console.WriteLine($"player [{player.name}] has raised [{value}] chips.");

            Board.ChipsInPot += value;
            player.Chips -= value;
            player.ChipsBetThisRound += value;
        }





        //internal static bool RoyalFlushFound(Player player)
        //{
        //    throw new NotImplementedException();
        //}

        //internal static bool StraightFlushFound(Player player)
        //{
        //    throw new NotImplementedException();
        //}

        internal bool FourOfAKindFound()
        {
            //var fullBoard = this.CardOne + this.CardTwo + Board.FlopSlot1 + Board.FlopSlot2 + Board.FlopSlot3 + Board.TurnSlot + Board.RiverSlot;
            var fullBoard = "4C" + "4H" + "6C" + "5C" + "4S" + "2H" + "3H";

            var fourOfAKindRegEx = new Regex(@"(\d{1}).*\1{1}.*\1{1}.*\1{1}");

            if (fourOfAKindRegEx.IsMatch(fullBoard)) 
            {
                this.BestHandType = 7;
                Console.WriteLine("MATCH FOUND"); 
                return true; 
            }

            return false;
        }

        //internal bool FullHouseFound(Player player)
        //{
        //    throw new NotImplementedException();
        //}

        //internal static bool FlushFound(Player player)
        //{
        //    throw new NotImplementedException();
        //}

        internal bool StraightFound()
        {

            //var fullBoard = this.CardOne + this.CardTwo + Board.FlopSlot1 + Board.FlopSlot2 + Board.FlopSlot3 + Board.TurnSlot + Board.RiverSlot;
            //var fullBoard = "3C" + "3H" + "JC" + "10C" + "7S" + "8H" + "9H";
            var fullBoardList = new List<string>() { "2C", "KH", "10C", "QC", "JS", "8H", "6H" };

            var fullBoardListInt = Board.RemoveSuitsAndReplaceFaceCardsWithInts(fullBoardList);

            fullBoardListInt.Sort();

            for (var i = 1; i < 5; i++)
            {
                var counter = 0;

                if ((fullBoardListInt[i] - 1).Equals(fullBoardListInt[i - 1]))
                {
                    counter++;

                    for (var x = i + 1; x < 7; x++)
                    {

                        //THIS IS TO HANDLE THE SITUATION I HAVE since I can only change "Ace" to a 0 or to a 14. There is probs a better, simpler way to handle this
                        if (fullBoardListInt[x].Equals(13)) { if ((fullBoardListInt[0]).Equals(0)) { counter++; } else { break; } }
                        //THIS IS TO HANDLE THE SITUATION I HAVE since I can only change "Ace" to a 0 or to a 14. There is probs a better, simpler way to handle this


                        if ((fullBoardListInt[x] - 1).Equals(fullBoardListInt[x - 1])) { counter++; } else { break; }

                    }

                    if (counter >= 4) 
                    {
                        this.BestHandType = 4;
                        Console.WriteLine($"STRAIGHT FOUND"); 
                        return true; 
                    }
                }
            }

            return false;

        }





        //The reason why there is no need to check for a 2nd "three of a kind" on the board, like the OnePairFound() function, is because in that situation the player would have a full house and this function wont be reached
        internal bool ThreeOfAKindFound()
        {
            //var fullBoard = this.CardOne + this.CardTwo + Board.FlopSlot1 + Board.FlopSlot2 + Board.FlopSlot3 + Board.TurnSlot + Board.RiverSlot;
            var fullBoard = "4C" + "7H" + "8C" + "5C" + "4S" + "8H" + "4H";

            var threeOfAKindRegEx = new Regex(@"(\d{1}).*\1{1}.*\1{1}");

            if (threeOfAKindRegEx.IsMatch(fullBoard)) 
            {
                this.BestHandType = 3; 
                return true; 
            }

            return false;
        }

        //When using regex to determine hand types, at the moment, I am only dealing with numbers, NOT "J", "Q", "K", "A", need to create a seperate but similiar reg ex for face cards
        internal bool TwoPairFound() //Player player
        {
            //var fullBoard = this.CardOne + this.CardTwo + Board.FlopSlot1 + Board.FlopSlot2 + Board.FlopSlot3 + Board.TurnSlot + Board.RiverSlot;
            var fullBoard = "4A" + "5H" + "7C" + "5C" + "10H" + "2H" + "4H";

            //Im not sure if 2 pair can be done with a reg ex. I found it was easier to find one pair, then remove that from the search string and run the same reg ex again
            var OnePairRegEx = new Regex(@"(\d{1}).*\1{1}");

            //This will match 2 pairs for "4H 4D 5H 5D", but not "4H 5H 4H 5H"
            var matches = OnePairRegEx.Matches(fullBoard);

            if (matches.Count > 1) { return true; }

            //Remove the char that has already been matched then perform the search again
            else if (matches.Count == 1)
            {
                var matchedChar = matches[0].ToString().Substring(0, 1);
                fullBoard = fullBoard.Replace(matchedChar, "");

                if (OnePairRegEx.IsMatch(fullBoard))
                {
                    this.BestHandType = 2;
                    Console.WriteLine($"2 PAIR FOUND");
                    return true;
                }

                //** will need to eventually account for the rare situation that the user can make 3 pairs with the 7 card available. Without extra code the highest value pair could be ignored
            }

            return false;
        }


        //When using regex to determine hand types, at the moment, I am only dealing with numbers, NOT "J", "Q", "K", "A", need to create a seperate but similiar reg ex for face cards
        internal bool OnePairFound() //Player player
        {
            var fullBoard = this.CardOne + this.CardTwo + Board.FlopSlot1 + Board.FlopSlot2 + Board.FlopSlot3 + Board.TurnSlot + Board.RiverSlot;

            //The regEx below isnt great, it will still match more than 1 pair found, but, at the moment anyway, this function should only be entered once I have worked my way down from the best possible hand
            // "(\d{ 1})\1(?!\1)" - this should work but I cant use the first capturing group(e.g '\1') like I have tried to - (negative lookahead: a(?!b), which is 'a' not followed by 'b')
            var OnePairRegEx = new Regex(@"(\d{1}).*\1{1}");

            if (OnePairRegEx.IsMatch(fullBoard)) 
            {
                var match = OnePairRegEx.Matches(fullBoard);
                this.HighestPair = match[0].ToString().Substring(0, 1);
                this.BestHandType = 1;

                return true; 
            }

            return false;

        }

        private void DetermineKicker()
        {
            this.BestHandType = 0;
            //loop through board values and find highest int
            
        }

        //Starts comparing from the best possible hand, if match found then return as we dont care about the rest
        internal int DetermineStrongestHandType() //Player player
        {
            //if (player.RoyalFlushFound())      { return (int) Deck.EnumPokerHands.RoyalFlush; }

            //if (player.StraightFlushFound())   { return (int) Deck.EnumPokerHands.StraightFlush;  }

            //if (player.FourOfAKindFound())     { return "Four of a Kind"; }

            //if (player.FullHouseFound())       { return "Full House"; }

            //if (player.FlushFound())           { return "Flush"; }

            //if (player.StraightFound())        { return "Straight"; }

            //if (player.ThreeOfAKindFound())    { return "Three of a Kind"; }

            //if (player.TwoPairFound())         { return "Two Pair"; }

            if (this.OnePairFound()) { return (int)Deck.EnumPokerHands.OnePair; }

            this.DetermineKicker();

            return (int)Deck.EnumPokerHands.HighCard;
        }

        
    }
}


