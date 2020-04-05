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
        private int highestCardInHandType;
        private int highestPair;
        private int secondHighestPair;
        private int threeOfAKindValue;
        private int highestStraightValue;
        private int highestFlushValue;
        //full house value.... - cant really fit this into 1 I dont think. Could maybe use threeOfAKind and Pair but that might cause more harm than good

        private string fourOfAKindValue;
        //straight flush ..... - I could also use either flush or straight, depends how I structure the code
        //royal flush ..... - I could also use either flush or straight, depends how I structure the code

        private int bestKicker;
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
        public int HighestCardInHandType { get { return highestCardInHandType; } set { highestCardInHandType = value; } }
        public int HighestPair { get { return highestPair; } set { highestPair = value; } }
        public int SecondHighestPair { get { return secondHighestPair; } set { secondHighestPair = value; } }
        public int ThreeOfAKindValue { get { return threeOfAKindValue; } set { threeOfAKindValue = value; } }
        public int HighestStraightValue { get { return highestStraightValue; } set { highestStraightValue = value; } }
        public int HighestFlushValue { get { return highestFlushValue; } set { highestFlushValue = value; } }
        public string FourOfAKindValue { get { return fourOfAKindValue; } set { fourOfAKindValue = value; } }
        public int BestKicker { get { return bestKicker; } set { bestKicker = value; } }


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

        //This function requires much more work. Need to consider the different combinations of:
        //2, three of a kinds (also, the matched three of a kind will need to be removed from the string and the reg ex search executed again. Some instances of 2, three of a kinds being present will go undetected otherwise
        //1, three of a kind, and one pair
        //1, three of a kind, and two pair
        internal bool FullHouseFound()
        {
            //var fullBoard = this.CardOne + this.CardTwo + Board.FlopSlot1 + Board.FlopSlot2 + Board.FlopSlot3 + Board.TurnSlot + Board.RiverSlot;
            var fullBoard = "9C" + "9H" + "8C" + "5C" + "5S" + "5H" + "9H";

            var threeOfAKindRegEx = new Regex(@"([^HDCS]{1}).*\1{1}.*\1{1}");
            var OnePairRegEx = new Regex(@"(\d{1}).*\1{1}");

            var threeOfAKindMatchesOne = threeOfAKindRegEx.Matches(fullBoard);

            //first I need to find if there are 2, three of a kinds, from the first search. If this is true then no need to look for another pair. However if it is false then search for a pair, then a second pair
            if (threeOfAKindMatchesOne.Count > 1)
            {
                this.BestHandType = 6;

                var threeOfAKindMatchedCharOne = threeOfAKindMatchesOne[0].ToString().Substring(0, 1);
                var threeOfAKindMatchedCharTwo = threeOfAKindMatchesOne[1].ToString().Substring(0, 1);

                DetermineKickersFromBoardWith2ThreeOfAKinds(threeOfAKindMatchedCharOne, threeOfAKindMatchedCharTwo);                               

                return true;
            }

            //Remove the char that has already been matched then perform the search again to ensure there isnt another three of a kind present
            //then check for pairs on the board
            else if (threeOfAKindMatchesOne.Count == 1)
            {
                this.BestHandType = 6;

                var threeOfAKindMatchedCharOne = threeOfAKindMatchesOne[0].ToString().Substring(0, 1);                
                this.ThreeOfAKindValue = Board.ReplaceFaceCardsWithInts(threeOfAKindMatchedCharOne);

                fullBoard = fullBoard.Replace(threeOfAKindMatchedCharOne, "");

                var threeOfAKindMatchesTwo = threeOfAKindRegEx.Matches(fullBoard);

                if (threeOfAKindMatchesTwo.Count == 1)
                {
                    var threeOfAKindMatchedCharTwo = threeOfAKindMatchesTwo[0].ToString().Substring(0, 1);

                    DetermineKickersFromBoardWith2ThreeOfAKinds(threeOfAKindMatchedCharOne, threeOfAKindMatchedCharTwo);

                    return true;
                }






                //NOW I NEED TO HANDLE: (ABOVE ONLY HANDLES THE SITUATION THAT THE BOARD HAS 2, THREE OF A KINDS)
                //situation 1: 1, three of a kind and 1 other pair on the board
                ////situation 1: 1, three of a kind and 2 other pairs on the board





            }

















            return false;
        }

        


        

        internal bool FlushFound()
        {
            var fullBoardList = new List<string>() { "AD", "AD", "3H", "AH", "5H", "7H", "6H" };

            var fullBoard = string.Join("", fullBoardList);            

            var flushRegEx = new Regex(@"([HDCS]{1}).*\1{1}.*\1{1}.*\1{1}.*\1{1}");

            var matches = flushRegEx.Matches(fullBoard);

            //If a match is found then need to remove all cards that arent in the matched suit then determine the kicker
            if (matches.Count == 1)
            {                
                this.BestHandType = 5;
                var matchedSuit = matches[0].ToString().Substring(0, 1);                                
                
                for (var i = 0; i < fullBoardList.Count; i++)
                {
                    if (!fullBoardList[i].Contains(matchedSuit))
                    {
                        fullBoardList.RemoveAt(i);
                    }
                }

                fullBoardList = Board.RemoveSuits(fullBoardList);
                var fullBoardListInt = Board.ReplaceFaceCardsWithInts(fullBoardList);

                var highestCard = 0;
                foreach (var card in fullBoardListInt)
                {
                    if(card > highestCard)
                    {
                        highestCard = card;
                    }
                }

                this.bestKicker = highestCard;

                return true;
            }





            return false;
        }




        internal bool StraightFound()
        {
            //var fullBoard = this.CardOne + this.CardTwo + Board.FlopSlot1 + Board.FlopSlot2 + Board.FlopSlot3 + Board.TurnSlot + Board.RiverSlot;
            var fullBoardList = new List<string>() { "AD", "2H", "10C", "JC", "KS", "QH", "7H" };

            fullBoardList = Board.RemoveSuits(fullBoardList);
            var fullBoardListInt = Board.ReplaceFaceCardsWithInts(fullBoardList);

            fullBoardListInt.Sort();

            var fullBoard = string.Join("", fullBoardListInt);
            
            //ace is "14"
            var straightRegEx = new Regex(@"2345.*14|23456|34567|45678|56789|678910|7891011|89101112|910111213|1011121314");

            var matches = straightRegEx.Matches(fullBoard);

            //Need to remove the lowest value that has been matched then perform the same search again to ensure there isnt a higher available straight on the board, this needs to be done 2 times   
            if (matches.Count > 0)
            {
                
                this.BestHandType = 5;
                this.bestKicker = Convert.ToInt32(matches[0].ToString().Substring(4, 1));
                
                fullBoard = fullBoard.Replace(matches[0].ToString().Substring(1, 1), "");

                var matches2 = straightRegEx.Matches(fullBoard);

                if (matches2.Count > 0)
                {
                    this.bestKicker = Convert.ToInt32(matches2[0].ToString().Substring(4, 1));

                    fullBoard = fullBoard.Replace(matches2[0].ToString().Substring(1, 1), "");

                    var matches3 = straightRegEx.Matches(fullBoard);

                    if (matches3.Count > 0)
                    {
                        this.bestKicker = Convert.ToInt32(matches3[0].ToString().Substring(4, 1));
                    }
                }

                //not sure where to handle this - need to convert the value of ace to its high value
                if (this.bestKicker == 1) this.bestKicker = 14;
                Console.WriteLine($"this.BestKicker = {this.BestKicker}");

                return true;
            }

            return false;

        }





        //The reason why there is no need to check for a 2nd "three of a kind" on the board, like the OnePairFound() function, is because in that situation the player would have a full house and this function wont be reached
        internal bool ThreeOfAKindFound()
        {
            //var fullBoard = this.CardOne + this.CardTwo + Board.FlopSlot1 + Board.FlopSlot2 + Board.FlopSlot3 + Board.TurnSlot + Board.RiverSlot;
            var fullBoard = "JC" + "7H" + "8C" + "5C" + "4S" + "8H" + "JH";

            var threeOfAKindRegEx = new Regex(@"([^HDCS]{1}).*\1{1}.*\1{1}");

            if (threeOfAKindRegEx.IsMatch(fullBoard)) 
            {
                Console.WriteLine("THREE OF A KIND");
                this.BestHandType = 3; 
                return true; 
            }

            return false;
        }

        //have to create a seperate regEx for face cards. Turning them into numbers worked for the "straight" hand type but the reg ex will get fcked as it wont be able to tell the difference between 1 and 11, 12, 13.

        //When using regex to determine hand types, at the moment, I am only dealing with numbers, NOT "J", "Q", "K", "A", need to create a seperate but similiar reg ex for face cards
        internal bool TwoPairFound() //Player player
        {
            //var fullBoard = this.CardOne + this.CardTwo + Board.FlopSlot1 + Board.FlopSlot2 + Board.FlopSlot3 + Board.TurnSlot + Board.RiverSlot;
            var fullBoard = "QA" + "JH" + "7C" + "8C" + "QH" + "JH" + "4H";

            //Im not sure if 2 pair can be done with a reg ex. I found it was easier to find one pair, then remove that from the search string and run the same reg ex again
            var OnePairRegEx = new Regex(@"([^HDCS]{1}).*\1{1}");            
            //This will match 2 pairs for "4H 4D 5H 5D", but not "4H 5H 4H 5H"
            var matches = OnePairRegEx.Matches(fullBoard);

            if (matches.Count > 1)                
            {
                //need to convert string into a string list then convert face cards into ints so I can determine kickers


                return true;
                
            }

            //Remove the char that has already been matched then perform the search again
            else if (matches.Count == 1)
            {
                var matchedChar = matches[0].ToString().Substring(0, 1);
                fullBoard = fullBoard.Replace(matchedChar, "");

                if (OnePairRegEx.IsMatch(fullBoard))
                {
                    this.BestHandType = 2;                    
                    return true;
                }

                //** will need to eventually account for the rare situation that the user can make 3 pairs with the 7 card available. Without extra code the highest value pair could be ignored
            }

            return false;
        }


        //When using regex to determine hand types, at the moment, I am only dealing with numbers, NOT "J", "Q", "K", "A", need to create a seperate but similiar reg ex for face cards
        internal bool OnePairFound() //Player player
        {
            //var fullBoard = this.CardOne + this.CardTwo + Board.FlopSlot1 + Board.FlopSlot2 + Board.FlopSlot3 + Board.TurnSlot + Board.RiverSlot;
            var fullBoard = "4A" + "5H" + "7C" + "5C" + "10H" + "2H" + "4H";
            //The regEx below isnt great, it will still match more than 1 pair found, but, at the moment anyway, this function should only be entered once I have worked my way down from the best possible hand
            // "(\d{ 1})\1(?!\1)" - this should work but I cant use the first capturing group(e.g '\1') like I have tried to - (negative lookahead: a(?!b), which is 'a' not followed by 'b')

            var OnePairRegEx = new Regex(@"([^HDCS]{1}).*\1{1}");


            if (OnePairRegEx.IsMatch(fullBoard)) 
            {                
                var match = OnePairRegEx.Matches(fullBoard);
                this.HighestPair = Convert.ToInt32(match[0].ToString().Substring(0, 1));
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

            if (this.OnePairFound()) { return (int) Deck.EnumPokerHands.OnePair; }

            this.DetermineKicker();

            return (int)Deck.EnumPokerHands.HighCard;
        }

        internal void DetermineKickersFromBoardWith2ThreeOfAKinds(string matchedCharOne, string matchedCharTwo)
        {
            this.ThreeOfAKindValue = FindKicker(matchedCharOne, matchedCharTwo);

            //if i have found 2 instances of three of a kind, then whatever is not the highest, will be the kicker for the pair in the "full house"
            if (this.ThreeOfAKindValue.Equals(Board.ReplaceFaceCardsWithInts(matchedCharOne)))
            {
                this.highestPair = Board.ReplaceFaceCardsWithInts(matchedCharTwo);
            }
            else
            {
                this.highestPair = Board.ReplaceFaceCardsWithInts(matchedCharOne);
            }

        }

        internal int FindKicker(string match1, string match2)
        {
            var matchedChar1 = match1[0].ToString().Substring(0, 1);
            var matchedChar2 = match2[0].ToString().Substring(0, 1);

            matchedChar1 = matchedChar1.Replace("A", "14").Replace("J", "11").Replace("Q", "12").Replace("K", "13");

            var matchedChars = new List<int>() { Convert.ToInt32(matchedChar1), Convert.ToInt32(matchedChar2) };            

            return matchedChars.Max();
        }



    }
}


