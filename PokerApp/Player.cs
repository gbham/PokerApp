using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static PokerApp.Output;

namespace PokerApp
{
    class Player
    {
        public readonly static int StartingStack = 1000;

        public Player(string playerName)
        {
            Name = playerName;
            ListOfKickers = new List<int>() {0};
        }

        private string name;
        private bool hasCards;
        private int chips = StartingStack;
        private int bestHandType = 0;
        private bool isAllIn = false;
        private string lastMove;
        private string moveOptions;
        private int highestPair;
        private int secondHighestPair;
        private int highestThreeOfAKindValue;        
        private bool royalStraight;
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
        public int HighestPair { get { return highestPair; } set { highestPair = value; } }
        public int SecondHighestPair { get { return secondHighestPair; } set { secondHighestPair = value; } }
        public int HighestThreeOfAKindValue { get { return highestThreeOfAKindValue; } set { highestThreeOfAKindValue = value; } }
        public bool RoyalStraight { get { return royalStraight; } set { royalStraight = value; } }
        public List<int> ListOfKickers { get { return listOfKickers; } set { listOfKickers = value; } }
        public int ChipsNeededToCall { get { return chipsNeededToCall; } set { chipsNeededToCall = value; } }
        public int ChipsBetThisRound { get { return chipsBetThisRound; } set { chipsBetThisRound = value; } }        
        public string CardOne { get { return cardOne; } set { cardOne = value; } }
        public string CardTwo { get { return cardTwo; } set { cardTwo = value; } }

        
        private string ERR_INVALID_MOVE = "*** [ERROR] Illegal move made. ***";

        private string ERR_ATTEMPTING_TO_CALL_WITH_VALUE = "*** [ERROR] Cannot choose the amount of chips to [Call]. No value can be entered alongside [Call].  ***";

        private string ERR_BET_IS_MORE_THAN_CHIP_STACK = "*** [ERROR] Cannot place a bet greater than your stack size. ***";

        private string ERR_RAISE_UNDER_REQUIRED_VALUE = "*** [ERROR] The amount bet is less than the chips required for a raise. ***";

        internal void Fold() 
        {           
            this.HasCards = false;          
        }        

        internal bool IsIllegalMoveUsed(string moveOptions, int value)
        {
            moveOptions = moveOptions.ToLower();

            //These moves are always valid
            if (this.LastMove == "all" || this.LastMove == "show" || this.LastMove == "fold") { return false; }

            if (!moveOptions.Contains(this.LastMove)) 
            {
                Dealer.IllegalMoveErrorMsg = ERR_INVALID_MOVE;
                return true;             
            }  

            if (this.LastMove.Contains("call"))
            {
                if(value > 0)
                {
                    Dealer.IllegalMoveErrorMsg = ERR_ATTEMPTING_TO_CALL_WITH_VALUE;
                    return true;
                }

                return false;
            }

            if (value > this.Chips)
            {
                Dealer.IllegalMoveErrorMsg = ERR_BET_IS_MORE_THAN_CHIP_STACK;
                return true;
            }

            //Dont think this is needed anymore, the IF statement directly below now fulfills the same purpose, still have to add a min raise value to the IF statement below though
            //if(value < this.ChipsNeededToCall)
            //{
            //    Dealer.IllegalMoveErrorMsg = "*** [ERROR] The amount bet is less than the chips required to match the current bet. ***";
            //    return true;
            //}

            if(moveOptions.Contains("raise"))
            {
                var highestBetThisRound = Dealer.GetHighestBetThisRound();

                //****here is where I would do something like "highestBetThisRound + minRaiseAmount"
                if (value <= highestBetThisRound) 
                {
                    Dealer.IllegalMoveErrorMsg = ERR_RAISE_UNDER_REQUIRED_VALUE;
                    return true;
                }
            }            

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
            this.ChipsNeededToCall = 0;
        }

        //Need to ensure that a raise value is valid, code should not be able to enter here unless the raise amount is more than what has already been bet by another player
        //Also need to eventually handle how much the legal min raise value is
        internal void Raise(int value) 
        {            
            var highestBetThisRound = Dealer.GetHighestBetThisRound();

            var temp = this.ChipsNeededToCall;     
            temp += value - highestBetThisRound;

            Board.ChipsInPot += temp;

            this.Chips -= temp;
            this.ChipsBetThisRound += temp;      //i think this could just be "+= temp;" instead of using "value", would be more simple
            this.ChipsNeededToCall = 0;
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
            Output.PrintGameInformation();

            Output.PrintCardInformation(this);
            
            //Once a key is pressed then the card information will disappear and the main game information/turn details be printed and the same user will be asked to make a move
            Console.ReadKey();
        }                        
        
        internal void DetermineChipsNeededToCall()
        {
            var PlayersInHand = Board.GetPlayersInHand();

            foreach (var player in PlayersInHand)
            {
                var chipsNeededToCall = player.ChipsBetThisRound - this.ChipsBetThisRound;

                if (chipsNeededToCall > 0 && chipsNeededToCall > this.ChipsNeededToCall)
                {
                    this.ChipsNeededToCall = chipsNeededToCall;
                }
            }
        }

        //This doesnt work actually as there could be both a royal straight and a flush on the board but not a royal flush
        //will likely need to create a dedicated function or send the "fullboardList" values in as a paramter? More likley a dedicated function as currently any extra cards are removed from 
        //ListOfKickers once they have been ordered and the highest values determined. Maybe the 2 cards thrown away could have been also been used to make a flush but they were a lower in value. 
        //Well in a pure flush v flush showdown this is fine but these extra cards might mean a straight flush could be made that wouldnt be possible without them. Unless, I could maybe remove
        //the extra cards from player.ListOfKickers at a later point? not sure how practical that would be but worth looking into as it would save a lot of code
        //internal bool RoyalFlushFound()
        //{
        //    //The position of this.RoyalStraight matters as it is determined inside StraightFound()
        //    if (FlushFound() && StraightFound(straightFlushMode) && this.RoyalStraight == true)
        //    {
        //        return true;
        //    }

        //    return false;
        //}


        //This doesnt work actually as there could be both a royal straight and a flush on the board but not a royal flush
        //will likely need to create a dedicated function or send the "fullboardList" values in as a paramter? More likley a dedicated function as currently any extra cards are removed from 
        //ListOfKickers once they have been ordered and the highest values determined. Maybe the 2 cards thrown away could have been also been used to make a flush but they were a lower in value. 
        //Well in a pure flush v flush showdown this is fine but these extra cards might mean a straight flush could be made that wouldnt be possible without them. Unless, I could maybe remove
        //the extra cards from player.ListOfKickers at a later point? not sure how practical that would be but worth looking into as it would save a lot of code
        //internal bool StraightFlushFound()
        //{            
        //    if(FlushFound() && StraightFound(straightFlushMode))
        //    {
        //        return true;
        //    }

        //    return false;
        //}

        internal bool FourOfAKindFound()
        {
            var fullBoardList = new List<string>() { this.CardOne, this.CardTwo, Board.FlopSlot1, Board.FlopSlot2, Board.FlopSlot3, Board.TurnSlot, Board.RiverSlot };

            var fullBoard = string.Join("", fullBoardList);            

            var fourOfAKindRegEx = new Regex(@"([^HDCS]{1}).*\1{1}.*\1{1}.*\1{1}");

            var matches = fourOfAKindRegEx.Matches(fullBoard);

            if (matches.Count == 1) 
            {
                this.BestHandType = 7;

                var matchedChar = GetMatchedCharFrom(matches[0]);

                fullBoardList.RemoveAll(item => item.Contains(matchedChar));

                this.ListOfKickers[0] = Utils.GetBestKickerFrom(fullBoardList);

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

                var threeOfAKindMatchedCharOne = GetMatchedCharFrom(threeOfAKindMatchesOne[0]);

                this.HighestThreeOfAKindValue = Deck.ReplaceFaceCardWithInts(threeOfAKindMatchedCharOne);

                fullBoard = fullBoard.Replace(threeOfAKindMatchedCharOne, "");

                var threeOfAKindMatchesTwo = threeOfAKindRegEx.Matches(fullBoard);

                if (threeOfAKindMatchesTwo.Count == 1)
                {
                    var threeOfAKindMatchedCharTwo = GetMatchedCharFrom(threeOfAKindMatchesTwo[0]);                    

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

                var matchedSuit = GetMatchedCharFrom(matches[0]);                

                fullBoardList.RemoveAll(item => !item.Contains(matchedSuit));

                this.ListOfKickers = Utils.GetBestKickersFrom(fullBoardList);
                this.ListOfKickers.RemoveRange(5, this.ListOfKickers.Count - 5);

                return true;
            }

            return false;
        }


        internal bool StraightFound(string mode = "normal")
        {
            List<string> fullBoardList;

            //Come back to this when trying to determine Straight/Royal flush. This should work fine but have to find a way of including all cards of the flush, in case that means all 7 are spades for example
            //first impression is to take this line from inside FlushFound(): "this.ListOfKickers.RemoveRange(5, this.ListOfKickers.Count - 5);"
            //and place it somewhere later in the code, not sure how practical but would work. It would mean the player.ListOfKickers after a flush found wouldnt remove any extra cards after the 5 highest of the matching flush suit

            //if (mode == "normal")
            //{
                  fullBoardList = new List<string>() { this.CardOne, this.CardTwo, Board.FlopSlot1, Board.FlopSlot2, Board.FlopSlot3, Board.TurnSlot, Board.RiverSlot };
            //}
            //else
            //{
            //    fullBoardList = new List<string>() {this.ListOfKickers.ToString()};
            //}

                        
            
            fullBoardList = Deck.RemoveSuits(fullBoardList);

            var fullBoardListInt = Deck.ReplaceFaceCardsWithInts(fullBoardList);
            fullBoardListInt.Sort();

            fullBoardListInt = fullBoardListInt.Distinct().ToList();

            var fullBoard = string.Join("", fullBoardListInt);            
            
            //ace is "14"
            //I tried reversing the order of this but the lowest values always match first? Must be a way to have the "first" search start at the top then work its way down. If this is possible then there is no need for the extra code checking for other matches as the first match in this scenario would be the best availiable
            var straightRegEx = new Regex(@"2345.*14|23456|34567|45678|56789|678910|7891011|89101112|910111213|1011121314");            

            var matches = straightRegEx.Matches(fullBoard);

            //Need to remove the lowest value that has been matched then perform the same search again to ensure there isnt a higher available straight on the board, this may need to be done 2 times   
            if (matches.Count > 0)
            {                
                this.BestHandType = 5;

                this.ListOfKickers[0] = GetHighestCardFromSortedMatchResult(matches[0]);
                CheckForRoyalStraight(matches[0]);
                
                fullBoard = fullBoard.Replace(matches[0].ToString().Substring(0, 1), "");   

                var matches2 = straightRegEx.Matches(fullBoard);

                if (matches2.Count > 0)
                {
                    this.ListOfKickers[0] = GetHighestCardFromSortedMatchResult(matches2[0]);
                    CheckForRoyalStraight(matches2[0]);

                    fullBoard = fullBoard.Replace(matches2[0].ToString().Substring(0, 1), ""); 

                    var matches3 = straightRegEx.Matches(fullBoard);

                    if (matches3.Count > 0)
                    {
                        this.ListOfKickers[0] = GetHighestCardFromSortedMatchResult(matches3[0]);
                        CheckForRoyalStraight(matches3[0]);
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
                var matchedChar = GetMatchedCharFrom(matches[0]);               

                this.HighestThreeOfAKindValue = Deck.ReplaceFaceCardWithInts(matchedChar);                                

                fullBoardList.RemoveAll(item => item.Contains(matchedChar));

                this.ListOfKickers = Utils.GetBestKickersFrom(fullBoardList);
                this.ListOfKickers.RemoveRange(2, this.ListOfKickers.Count - 2);                

                return true;
            }

            return false;
        }


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
                var matchedChar = GetMatchedCharFrom(matches1[0]);
                fullBoard = fullBoard.Replace(matchedChar, "");

                var matches2 = OnePairRegEx.Matches(fullBoard);

                //Remove the char that has already been matched then perform the search again
                if (matches2.Count >= 1)
                {
                    this.BestHandType = 2;

                    var matchedChar2 = GetMatchedCharFrom(matches2[0]);                    
                    fullBoard = fullBoard.Replace(matchedChar2, "");

                    var matches3 = OnePairRegEx.Matches(fullBoard);
                    
                    if (matches3.Count == 1)
                    {
                        var matchedChar3 = GetMatchedCharFrom(matches3[0]);                        

                        DetermineBestHandFromBoardWith3Pairs(matchedChar, matchedChar2, matchedChar3);

                    }
                    else
                    {
                        DetermineBestHandFromBoardWith2Pairs(matchedChar, matchedChar2);
                    }

                    //Remove the matched pairs from the list of all cards on the board so the kicker can be determined from what remains
                    this.ListOfKickers.RemoveAll(item => item.Equals(this.HighestPair));
                    this.ListOfKickers.RemoveAll(item => item.Equals(this.SecondHighestPair));

                    this.ListOfKickers.Sort();
                    this.ListOfKickers.Reverse();
                    
                    //Only concerned with the best kicker, if they equal the same and so do the pairs then it is a split pot
                    this.ListOfKickers.RemoveRange(1, this.ListOfKickers.Count - 1);

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
                var matchedChar = GetMatchedCharFrom(matches[0]);

                this.HighestPair = Convert.ToInt32(Deck.ReplaceFaceCardWithInts(matchedChar));
                this.BestHandType = 1;                

                fullBoardList.RemoveAll(item => item.Contains(matchedChar));

                this.ListOfKickers = Utils.GetBestKickersFrom(fullBoardList);
                this.ListOfKickers.RemoveRange(3, this.ListOfKickers.Count - 3);

                return true; 
            }

            return false;
        }

        //Starts comparing from the best possible hand, if match found then return as we dont care about the rest
        internal int GetStrongestHandType()
        {
            //if (this.RoyalFlushFound()) { return (int)Deck.EnumPokerHands.RoyalFlush; }

            //if (this.StraightFlushFound()) { return (int)Deck.EnumPokerHands.StraightFlush; }

            if (this.FourOfAKindFound()) { return (int)Deck.EnumPokerHands.FourOfAKind; }

            //////if (this.FullHouseFound())      { return (int) Deck.EnumPokerHands.FullHouse; }

            if (this.FlushFound()) { return (int)Deck.EnumPokerHands.Flush; }

            if (this.StraightFound()) { return (int)Deck.EnumPokerHands.Straight; }

            if (this.ThreeOfAKindFound()) { return (int)Deck.EnumPokerHands.ThreeOfAKind; }

            if (this.TwoPairFound()) { return (int)Deck.EnumPokerHands.TwoPair; }

            if (this.OnePairFound()) { return (int)Deck.EnumPokerHands.OnePair; }

            //Otherwise, set the kicker (dont think I need to prefix with "this" in this situation, test it, it does make it more clear however)
            this.SetKickers(); return (int)Deck.EnumPokerHands.HighCard;
        }


        private int GetHighestCardFromSortedMatchResult(Match match)
        {
            //This condition of "match.Length - 4" is needed for when the face cards are converted into numbers. I cant just take the 5 character to find the highest value, as ace will equal "14" so i need the final 2 characters in the sorted list
            //return Convert.ToInt32(match.ToString().Substring(4, match.Length - 4));

            var highestCard = 0;


            switch(match.Length)
            {
                case 5:
                    highestCard = Convert.ToInt32(match.ToString().Substring(4, 1));
                    break;

                case 6:
                    highestCard = Convert.ToInt32(match.ToString().Substring(4, 2));

                    //If there is only 6 chars, then the final 2 chars will equal an ace or a 10. If it is an ace then this means the straight is "234514" AKA "A-2-3-4-5".
                    //In this situation I want to assign the highest value to 5 so I can properly determine the difference between a "A-2-3-4-5" straight and a "2-3-4-5-6" straight.
                    //Otherwise ace would be the kicker and the wrong straight would be incorrectly determined as better
                    if(highestCard == 14) { highestCard = 5; }

                    break;

                case 7:
                    highestCard = Convert.ToInt32(match.ToString().Substring(5, 2));
                    break;

                case 8:
                    highestCard = Convert.ToInt32(match.ToString().Substring(6, 2));
                    break;

                case 9:
                    highestCard = Convert.ToInt32(match.ToString().Substring(7, 2));
                    break;

                case 10:
                    highestCard = Convert.ToInt32(match.ToString().Substring(8, 2));
                    break;

            }


            return highestCard;
        }

        //The matchedChar is always the first position in the string that is returned from var matches = someRegEx.Matches(string)
        private string GetMatchedCharFrom(Match match)
        {
            var matchedChar = match.ToString().Substring(0, 1);
            matchedChar = Utils.HandleAnyInstancesOf10CardFound(matchedChar);

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
            this.ListOfKickers.RemoveRange(5, this.ListOfKickers.Count - 5);        

        }

        //This is needed as I only retain the highest value in a straight as that is all that is needed to determine the best straight. 
        //However, I cant just simply check for the highest value being an ace as the user may have A/2/3/4/5 straight, which will be shown as "234514"
        private void CheckForRoyalStraight(Match match)
        {
            //13 = King, I only need to check for a king at a certain position as any "match" being sent into this function is confirmed a straight and ordered
            //A royal straight will look like "1011121314" in "match"
            if (match.Value.Substring(match.Length - 4, 2) == "13")
            {
                this.RoyalStraight = true;
            }
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
                this.MoveOptions = "Fold Call Raise All in Show";
                return $"Options: [Fold] [Call {this.ChipsNeededToCall}] [Raise] [All In] [Show]";
            }

            this.MoveOptions = "Fold Check Bet All In Show";
            return "Options: [Fold] [Check] [Bet] [All In] [Show]";            
            
        }

        internal void DetermineBestHandFromBoardWith2ThreeOfAKinds(string matchedCharOne, string matchedCharTwo)
        {
            this.HighestThreeOfAKindValue = Utils.GetHighestCardFromTwoChoices(matchedCharOne, matchedCharTwo);

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
            this.HighestPair = Utils.GetHighestCardFromTwoChoices(matchedCharOne, matchedCharTwo);

            //if i have found 2 instances of pair, then whatever is not the highest, will be the value for the 2nd highest pair
            if (this.HighestPair.Equals(Deck.ReplaceFaceCardWithInts(matchedCharOne)))
            {
                this.SecondHighestPair = Deck.ReplaceFaceCardWithInts(matchedCharTwo);
            }
            else
            {
                this.SecondHighestPair = Deck.ReplaceFaceCardWithInts(matchedCharOne);
            }
        }

        internal void DetermineBestHandFromBoardWith3Pairs(string matchedCharOne, string matchedCharTwo, string matchedCharThree)
        {
            var highestChars = Utils.DetermineHighestCardsFromThreeChoices(matchedCharOne, matchedCharTwo, matchedCharThree);

            this.HighestPair = highestChars[0];
            this.SecondHighestPair = highestChars[1];
        }

        
    }
}


