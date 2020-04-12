using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using static PokerApp.App;
using static PokerApp.Output;

namespace PokerApp
{
    class Player
    {
        public readonly static int StartingStack = 1000;

        private string name;
        private bool hasCards;
        private int chips = StartingStack;
        private int bestHandType = 0;
        private bool isAllIn = false;
        private string lastMove;
        private string moveOptions;
        private string illegalMoveErrorMsg;        

        private int highestCardInHandType;
        private int highestPair;
        private int secondHighestPair;
        private int highestThreeOfAKindValue;
        private int highestStraightValue;
        private int highestFlushValue;
        //full house value.... - cant really fit this into 1 I dont think. Could maybe use threeOfAKind and Pair but that might cause more harm than good

        private string fourOfAKindValue;
        //straight flush ..... - I could also use either flush and/or straight, depends how I structure the code
        //royal flush ..... - I could also use either flush and/or straight, depends how I structure the code

        private int bestKicker = 0;
        private List<int> listOfKickers;
        private int chipsNeededToCall;
        private int chipsBetThisRound;
        private string cardOne;
        private string cardTwo;
        

        //Think I may also need a property to hold the 5 "best" cards, which I can then use to deduct the best kicker - not sure actually, determining the kicker might have to be a specific operation for each type of hand or could risk causing unneeded complexity 

        public string Name { get { return name; } set { name = value; } }
        public bool HasCards { get { return hasCards; } set { hasCards = value; } }
        public int Chips { get { return chips; } set { chips = value; } }
        public int BestHandType { get { return bestHandType; } set { bestHandType = value; } }
        public bool IsAllIn { get { return isAllIn; } set { isAllIn = value; } }
        public string LastMove { get { return lastMove; } set { lastMove = value; } }
        public string MoveOptions { get { return moveOptions; } set { moveOptions = value; } }
        public string IllegalMoveErrorMsg { get { return illegalMoveErrorMsg; } set { illegalMoveErrorMsg = value; } }

        public int HighestCardInHandType { get { return highestCardInHandType; } set { highestCardInHandType = value; } }
        public int HighestPair { get { return highestPair; } set { highestPair = value; } }
        public int SecondHighestPair { get { return secondHighestPair; } set { secondHighestPair = value; } }
        public int HighestThreeOfAKindValue { get { return highestThreeOfAKindValue; } set { highestThreeOfAKindValue = value; } }
        public int HighestStraightValue { get { return highestStraightValue; } set { highestStraightValue = value; } }
        public int HighestFlushValue { get { return highestFlushValue; } set { highestFlushValue = value; } }
        public string FourOfAKindValue { get { return fourOfAKindValue; } set { fourOfAKindValue = value; } }
        public int BestKicker { get { return bestKicker; } set { bestKicker = value; } }

        public List<int> ListOfKickers { get { return listOfKickers; } set { listOfKickers = value; } }

        public int ChipsNeededToCall { get { return chipsNeededToCall; } set { chipsNeededToCall = value; } }
        public int ChipsBetThisRound { get { return chipsBetThisRound; } set { chipsBetThisRound = value; } }        
        public string CardOne { get { return cardOne; } set { cardOne = value; } }
        public string CardTwo { get { return cardTwo; } set { cardTwo = value; } }
        


        internal void Fold() 
        {
            //Console.Clear();
            //Console.WriteLine($"player [{this.Name}] has folded.");
            this.HasCards = false;

            Console.WriteLine($"filler");
            //Maybe have the code in here to determine if folding means there is only 1 player left in the hand, and therefore the hand is over
        }

      

        

        internal bool IsIllegalMoveUsed(string moveOptions, int value)
        {
            moveOptions = moveOptions.ToLower();

            if (this.LastMove == "all" || this.LastMove == "show" || this.LastMove == "fold") { return false; }

            if (!moveOptions.Contains(this.LastMove)) 
            {
                Dealer.IllegalMoveErrorMsg = "*** [ERROR] Illegal move made. ***";
                Console.WriteLine("ILLEGAL MOVE MADE"); 
                return true;             
            }  

            if (this.LastMove.Contains("call") || this.LastMove.Contains("c"))
            {
                if(value > 0)
                {
                    Dealer.IllegalMoveErrorMsg = "*** [ERROR] Cannot choose the amount of chips to [Call]. No value can be entered alongside [Call].  ***";
                    return true;
                }

                return false;
            }

            if (value > this.Chips)
            {
                Dealer.IllegalMoveErrorMsg = "*** [ERROR] Cannot place a bet greater than your stack sixe. ***";
                return true;
            }

            if(value < this.ChipsNeededToCall)
            {
                Dealer.IllegalMoveErrorMsg = "*** [ERROR] The amount bet is less than the chips required to match the current bet. ***";
                return true;
            }


            //need a statement ensuring when someone bets that it is above the min raise value

            return false;
        }

        internal void Check()
        {            
            //Not sure if there is anything is to be done in here
        }

        internal void Call() 
        {
            this.DetermineChipsNeededToCall();            

            Board.ChipsInPot += this.ChipsNeededToCall;
            this.Chips -= this.ChipsNeededToCall;
            this.ChipsBetThisRound += ChipsNeededToCall;
            this.ChipsNeededToCall = 0;
        }

                
        internal void Bet(int value)
        {
            Board.ChipsInPot += value;
            this.Chips -= value;
            this.ChipsBetThisRound = value;             
        }

        //Need to ensure that a raise value is valid, code should not be able to enter here unless the raise amount is more than what has already been bet by another player
        //Also need to eventually handle how much the legal min raise value is
        internal void Raise(int value) 
        {
            var Players = Board.GetPlayersInHand();

            var HighestBetThisRound = 0;

            foreach(var player in Players)
            {
                if(player.ChipsBetThisRound > HighestBetThisRound)
                {
                    HighestBetThisRound = player.ChipsBetThisRound;
                }
            }
            
            var temp = HighestBetThisRound - this.ChipsBetThisRound;
            temp += value - HighestBetThisRound;

            Board.ChipsInPot += temp;
            this.Chips -= temp;
            this.ChipsBetThisRound = value;
        }

        internal void AllIn()
        {            
            Board.ChipsInPot += this.Chips;
            this.IsAllIn = true;
            this.ChipsBetThisRound += this.Chips;
            this.Chips = 0;
        }

        internal void RevealCards()
        {
            PrintGameInformation();

            Console.WriteLine($"[{this.Name}]'s cards are: [{this.CardOne}] [{this.CardTwo}]");
            Console.WriteLine($"\npress any button to hide cards...");
            Console.ReadKey();
        }

        //come back to this and test the change, I dont think the function below caluclated ChipsNeededToCall properly if there bets and re raises
        internal void DetermineChipsNeededToCall()
        {
            var PlayersInHand = Board.GetPlayersInHand();

            foreach (var player in PlayersInHand)
            {
                if((player.ChipsBetThisRound - this.ChipsBetThisRound) > 0)
                {
                    this.ChipsNeededToCall = (player.ChipsBetThisRound - this.ChipsBetThisRound);
                }
            }
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
            var fullBoardList = new List<string>() { this.CardOne, this.CardTwo, Board.FlopSlot1, Board.FlopSlot2, Board.FlopSlot3, Board.TurnSlot, Board.RiverSlot };

            var fullBoard = string.Join("", fullBoardList);            

            var fourOfAKindRegEx = new Regex(@"(\d{1}).*\1{1}.*\1{1}.*\1{1}");

            var matches = fourOfAKindRegEx.Matches(fullBoard);

            if (matches.Count == 1) 
            {
                this.BestHandType = 7;

                var matchedChar = GetMatchedCharFrom(matches);

                fullBoardList.RemoveAll(item => item.Contains(matchedChar));

                this.BestKicker = FindBestKicker(fullBoardList);

                return true; 
            }

            return false;
        }


        //This function requires much more work. Need to consider the different combinations of:
        //DONE: 2 three of a kinds (also, the matched three of a kind will need to be removed from the string and the reg ex search executed again. Otherwise, some instances of "2, three of a kinds" being present will go undetected
        //TODO: 1 three of a kind, and one pair
        //TODO: 1 three of a kind, and two pair
        internal bool FullHouseFound()
        {
            var fullBoardList = new List<string>() { this.CardOne, this.CardTwo, Board.FlopSlot1, Board.FlopSlot2, Board.FlopSlot3, Board.TurnSlot, Board.RiverSlot };

            var fullBoard = string.Join("", fullBoardList);

            var threeOfAKindRegEx = new Regex(@"([^HDCS]{1}).*\1{1}.*\1{1}");
            var OnePairRegEx = new Regex(@"([^HDCS]{1}).*\1{1}");

            var threeOfAKindMatchesOne = threeOfAKindRegEx.Matches(fullBoard);

            
            //Remove the char that has already been matched then perform the search again to check if there is another three of a kind present
            //then check for pairs on the board
            if (threeOfAKindMatchesOne.Count == 1)
            {
                this.BestHandType = 6;

                var threeOfAKindMatchedCharOne = GetMatchedCharFrom(threeOfAKindMatchesOne);

                this.HighestThreeOfAKindValue = Deck.ReplaceFaceCardWithInts(threeOfAKindMatchedCharOne);

                fullBoard = fullBoard.Replace(threeOfAKindMatchedCharOne, "");

                var threeOfAKindMatchesTwo = threeOfAKindRegEx.Matches(fullBoard);

                if (threeOfAKindMatchesTwo.Count == 1)
                {
                    var threeOfAKindMatchedCharTwo = GetMatchedCharFrom(threeOfAKindMatchesTwo);                    

                    DetermineBestHandFromBoardWith2ThreeOfAKinds(threeOfAKindMatchedCharOne, threeOfAKindMatchedCharTwo);

                    return true;
                }






                //NOW I NEED TO HANDLE: (ABOVE ONLY HANDLES THE SITUATION THAT THE BOARD HAS 2, THREE OF A KINDS)
                ////situation: 1 three of a kind and 1 other pair on the board
                ////situation: 1 three of a kind and 2 other pairs on the board





            }



            return false;
        }

        


        

        internal bool FlushFound()
        {
            var fullBoardList = new List<string>() { this.CardOne, this.CardTwo, Board.FlopSlot1, Board.FlopSlot2, Board.FlopSlot3, Board.TurnSlot, Board.RiverSlot };

            var fullBoard = string.Join("", fullBoardList);            

            var flushRegEx = new Regex(@"([HDCS]{1}).*\1{1}.*\1{1}.*\1{1}.*\1{1}");

            var matches = flushRegEx.Matches(fullBoard);

            //If a match is found then need to remove all cards that arent in the matched suit in order to determine the kicker
            if (matches.Count == 1)
            {                
                this.BestHandType = 5;

                var matchedSuit = matches[0].ToString().Substring(0, 1);

                fullBoardList.RemoveAll(item => !item.Contains(matchedSuit));

                this.BestKicker = FindBestKicker(fullBoardList);

                return true;
            }

            return false;
        }


        internal bool StraightFound()
        {
            var fullBoardList = new List<string>() { this.CardOne, this.CardTwo, Board.FlopSlot1, Board.FlopSlot2, Board.FlopSlot3, Board.TurnSlot, Board.RiverSlot };
            
            fullBoardList = Deck.RemoveSuits(fullBoardList);
            var fullBoardListInt = Deck.ReplaceFaceCardsWithInts(fullBoardList);

            fullBoardListInt.Sort();

            var fullBoard = string.Join("", fullBoardListInt);
            
            //ace is "14"
            var straightRegEx = new Regex(@"2345.*14|23456|34567|45678|56789|678910|7891011|89101112|910111213|1011121314");

            var matches = straightRegEx.Matches(fullBoard);

            //Need to remove the lowest value that has been matched then perform the same search again to ensure there isnt a higher available straight on the board, this may need to be done 2 times   
            if (matches.Count > 0)
            {                
                this.BestHandType = 5;
                this.BestKicker = Convert.ToInt32(matches[0].ToString().Substring(4, 1));
                
                fullBoard = fullBoard.Replace(matches[0].ToString().Substring(0, 1), "");   

                var matches2 = straightRegEx.Matches(fullBoard);

                if (matches2.Count > 0)
                {
                    this.BestKicker = Convert.ToInt32(matches2[0].ToString().Substring(4, 1));                    

                    fullBoard = fullBoard.Replace(matches2[0].ToString().Substring(0, 1), ""); 

                    var matches3 = straightRegEx.Matches(fullBoard);

                    if (matches3.Count > 0)
                    {
                        this.BestKicker = Convert.ToInt32(matches3[0].ToString().Substring(4, 1));
                    }
                }

                return true;
            }

            return false;
        }


        //The reason why there is no need to check for another pair or another three of a kind, like I have in other functions above
        //is because in that situation this function wont be reached, the player would have a full house
        internal bool ThreeOfAKindFound()
        {
            var fullBoardList = new List<string>() { this.CardOne, this.CardTwo, Board.FlopSlot1, Board.FlopSlot2, Board.FlopSlot3, Board.TurnSlot, Board.RiverSlot };
            fullBoardList = Deck.RemoveSuits(fullBoardList);            

            var fullBoard = string.Join("", fullBoardList);           

            var threeOfAKindRegEx = new Regex(@"([^HDCS]{1}).*\1{1}.*\1{1}");

            var matches = threeOfAKindRegEx.Matches(fullBoard);

            if (matches.Count == 1)
            {
                var matchedChar = GetMatchedCharFrom(matches);               

                this.HighestThreeOfAKindValue = Deck.ReplaceFaceCardWithInts(matchedChar);                                

                fullBoardList.RemoveAll(item => item.Contains(matchedChar));

                this.ListOfKickers = FindBestKickers(fullBoardList);                

                return true;
            }

            return false;
        }


        //Need to determine kicker for 2 pair
        internal bool TwoPairFound()
        {
            var fullBoardList = new List<string>() { this.CardOne, this.CardTwo, Board.FlopSlot1, Board.FlopSlot2, Board.FlopSlot3, Board.TurnSlot, Board.RiverSlot };
            fullBoardList = Deck.RemoveSuits(fullBoardList);

            var tempList = new List<string>(fullBoardList); 
            tempList = Deck.RemoveSuits(tempList);
            this.ListOfKickers = Deck.ReplaceFaceCardsWithInts(tempList); //I will remove any pairs later in the function.

            var fullBoard = string.Join("", fullBoardList);

            //Im not sure if all 2 pair combinations can be done with a reg ex. I found it was easier to find one pair, then remove that from the search string and run the same reg ex again
            //This will match 2 pairs for "4H 4D 5H 5D", but not "4H 5H 4H 5H". 
            var OnePairRegEx = new Regex(@"([^HDCS]{1}).*\1{1}"); 
            var matches1 = OnePairRegEx.Matches(fullBoard);                        

            //Remove the char that has already been matched then perform the search again
            if (matches1.Count >= 1)
            {
                var matchedChar = GetMatchedCharFrom(matches1);
                fullBoard = fullBoard.Replace(matchedChar, "");

                var matches2 = OnePairRegEx.Matches(fullBoard);

                //Remove the char that has already been matched then perform the search again
                if (matches2.Count >= 1)
                {
                    this.BestHandType = 2;

                    var matchedChar2 = GetMatchedCharFrom(matches2);                    
                    fullBoard = fullBoard.Replace(matchedChar2, "");

                    var matches3 = OnePairRegEx.Matches(fullBoard);
                    
                    if (matches3.Count == 1)
                    {
                        var matchedChar3 = GetMatchedCharFrom(matches3);                        

                        DetermineBestHandFromBoardWith3Pairs(matchedChar, matchedChar2, matchedChar3);
                    }
                    else
                    {
                        DetermineBestHandFromBoardWith2Pairs(matchedChar, matchedChar2);
                    }                        

                    return true;
                }
            }

            return false;
        }

        //The reason for so many less conditions, is since this will only be reached if all the other hand types dont exist. 
        //When this regEx matches with one pair, I can be sure that it is the only pair present as any other pairs would have been caught in TwoPairFound() function
        internal bool OnePairFound() //Player player
        {
           var fullBoardList = new List<string>() { this.CardOne, this.CardTwo, Board.FlopSlot1, Board.FlopSlot2, Board.FlopSlot3, Board.TurnSlot, Board.RiverSlot };

            var fullBoard = string.Join("", fullBoardList);
           
            var OnePairRegEx = new Regex(@"([^HDCS]{1}).*\1{1}");
            var matches = OnePairRegEx.Matches(fullBoard);

            if (matches.Count == 1) 
            {
                var matchedChar = GetMatchedCharFrom(matches);

                this.HighestPair = Convert.ToInt32(Deck.ReplaceFaceCardWithInts(matchedChar));
                this.BestHandType = 1;                

                fullBoardList.RemoveAll(item => item.Contains(matchedChar));

                this.ListOfKickers = FindBestKickers(fullBoardList);

                return true; 
            }

            return false;
        }

        //The matchedChar is always the first position in the string that is returned from var matches = someRegEx.Matches(string)
        private string GetMatchedCharFrom(MatchCollection match)
        {
            var matchedChar = match[0].ToString().Substring(0, 1);
            matchedChar = HandleAnyInstancesOf10CardFound(matchedChar);

            return matchedChar;
        }

        //The kicker is set in this function when no hand types are found, this function only gets executed when not even a pair can be made by the player. At the moment kickers are being assigned in specific functions for hand types. In time I could make it a global function if I just created a list in each specific hand type function of all the cards that can be considered. If it was a pair for example, then the player would have 5 cards to pick the kicker from, if it was 3 of a kind, then the player would have 4 cards to pick the kicker from. I need to create a list of "viable" cards then run them through a general function. To determine the best kicker I could even just sort the lists and compare the lists between players as a whole, loop through it one at a time till one has a greater value (obv need to pad some positions with 0 for hands where only 4 cards are availiable)
        internal void SetKickers()
        {
            var fullBoardList = new List<string>() { this.CardOne, this.CardTwo, Board.FlopSlot1, Board.FlopSlot2, Board.FlopSlot3, Board.TurnSlot, Board.RiverSlot };
            
            fullBoardList = Deck.RemoveSuits(fullBoardList);
            var fullBoardListInt = Deck.ReplaceFaceCardsWithInts(fullBoardList);

            fullBoardListInt.Sort();
            fullBoardListInt.Reverse();

            this.ListOfKickers = fullBoardListInt;

            this.BestKicker = fullBoardListInt[0];
        }


        //Starts comparing from the best possible hand, if match found then return as we dont care about the rest
        internal int GetStrongestHandType() //Player player
        {
            ////if (player.RoyalFlushFound())      { return (int) Deck.EnumPokerHands.RoyalFlush; }

            ////if (player.StraightFlushFound())   { return (int) Deck.EnumPokerHands.StraightFlush;  }

            if (this.FourOfAKindFound()) { return (int)Deck.EnumPokerHands.FourOfAKind; }

            ////if (this.FullHouseFound())      { return (int) Deck.EnumPokerHands.FullHouse; }

            if (this.FlushFound()) { return (int)Deck.EnumPokerHands.Flush; }

            if (this.StraightFound()) { return (int)Deck.EnumPokerHands.Straight; }

            if (this.ThreeOfAKindFound()) { return (int)Deck.EnumPokerHands.ThreeOfAKind; }

            if (this.TwoPairFound()) { return (int)Deck.EnumPokerHands.TwoPair; }

            if (this.OnePairFound()) { return (int)Deck.EnumPokerHands.OnePair; }

            //Otherwise, set the kicker (dont think I need to prefix with "this" in this situation, test it)
            this.SetKickers();  return (int) Deck.EnumPokerHands.HighCard;
        }

        //Determines if check/call and bet/raise should be shown. 
        internal string GetMoveOptions()
        {            
            this.DetermineChipsNeededToCall();

            if (this.ChipsNeededToCall >= this.Chips)
            {
                this.MoveOptions = "Fold All In Show";
                return $"Options: [Fold] [All In {this.ChipsNeededToCall}] [Show]";
            }

            //you can only raise if you have enough to match the min raise value, need to handle that
            if (this.ChipsNeededToCall > 0)
            {
                this.MoveOptions = "Fold Call Raise Show";
                return $"Options: [Fold] [Call {this.ChipsNeededToCall}] [Raise] [All In] [Show]";
            }

            this.MoveOptions = "Fold Check Bet Show";
            return "Options: [Fold] [Check] [Bet] [All In] [Show]";            
            
        }

        internal void DetermineBestHandFromBoardWith2ThreeOfAKinds(string matchedCharOne, string matchedCharTwo)
        {
            this.HighestThreeOfAKindValue = GetHighestCardFromTwoChoices(matchedCharOne, matchedCharTwo);

            //if i have found 2 instances of three of a kind, then whatever is not the highest, will be the value for the pair in the "full house"
            if (this.HighestThreeOfAKindValue.Equals(Deck.ReplaceFaceCardWithInts(matchedCharOne)))
            {
                this.HighestPair = Deck.ReplaceFaceCardWithInts(matchedCharTwo);
            }
            else
            {
                this.HighestPair = Deck.ReplaceFaceCardWithInts(matchedCharOne);
            }
        }

        internal void DetermineBestHandFromBoardWith2Pairs(string matchedCharOne, string matchedCharTwo)
        {
            this.HighestPair = GetHighestCardFromTwoChoices(matchedCharOne, matchedCharTwo);

            //if i have found 2 instances of pair, then whatever is not the highest, will be the value for the 2nd highest pair
            if (this.HighestPair.Equals(Deck.ReplaceFaceCardWithInts(matchedCharOne)))
            {
                this.SecondHighestPair = Deck.ReplaceFaceCardWithInts(matchedCharTwo);
            }
            else
            {
                this.SecondHighestPair = Deck.ReplaceFaceCardWithInts(matchedCharOne);
            }

            this.ListOfKickers.RemoveAll(item => item.Equals(this.HighestPair));
            this.ListOfKickers.RemoveAll(item => item.Equals(this.SecondHighestPair));

        }

        internal void DetermineBestHandFromBoardWith3Pairs(string matchedCharOne, string matchedCharTwo, string matchedCharThree)
        {
            var highestChars = DetermineHighestCardsFromThreeChoices(matchedCharOne, matchedCharTwo, matchedCharThree);

            this.HighestPair = highestChars[0];
            this.SecondHighestPair = highestChars[1];

            this.ListOfKickers.RemoveAll(item => item.Equals(this.HighestPair));
            this.ListOfKickers.RemoveAll(item => item.Equals(this.SecondHighestPair));

        }

        internal int GetHighestCardFromTwoChoices(string matchedCharOne, string matchedCharTwo)
        {            
            matchedCharOne = matchedCharOne.Replace("A", "14").Replace("J", "11").Replace("Q", "12").Replace("K", "13");
            matchedCharTwo = matchedCharTwo.Replace("A", "14").Replace("J", "11").Replace("Q", "12").Replace("K", "13");

            var matchedChars = new List<int>() { Convert.ToInt32(matchedCharOne), Convert.ToInt32(matchedCharTwo) };
            return matchedChars.Max();
        }

        internal List<int> DetermineHighestCardsFromThreeChoices(string matchedCharOne, string matchedCharTwo, string matchedCharThree)
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

        private List<int> FindBestKickers(List<string> list)
        {
            list = Deck.RemoveSuits(list);
            var bestKickersList = Deck.ReplaceFaceCardsWithInts(list);

            bestKickersList.Sort();

            return bestKickersList;
        }

        private int FindBestKicker(List<string> list)
        {
            list = Deck.RemoveSuits(list);
            var bestKickerList = Deck.ReplaceFaceCardsWithInts(list);

            bestKickerList.Sort();

            var bestKicker = bestKickerList[bestKickerList.Count - 1];

            return bestKicker;
        }

        //"10" is the only card that has 2 digits at this stage in the code (face cards are still J,Q etc)
        private string HandleAnyInstancesOf10CardFound(string matchedChar)
        {            
            if (matchedChar == "1") { matchedChar = "10"; }  //"10" is the only card that has 2 digits at this stage in the code (face cards are still J,Q etc)

            return matchedChar;
        }




    }
}


