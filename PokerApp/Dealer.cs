using System;
using System.Collections.Generic;
using System.Linq;
using static PokerApp.App;

namespace PokerApp
{
    static class Dealer
    {
        private static Player handWinner;
        private static string illegalMoveErrorMsg;
        private static bool isSplitPot;
        private static List<Player> splitPotPlayers;
        private static int SmallBlindCounter = 0;
        private static int BigBlindCounter = 1;

        public static Player HandWinner { get { return handWinner; } set { handWinner = value; } }
        public static string IllegalMoveErrorMsg { get { return illegalMoveErrorMsg; } set { illegalMoveErrorMsg = value; } }
        public static bool IsSplitPot { get { return isSplitPot; } set { isSplitPot = value; } }
        public static List<Player> SplitPotPlayers { get { return splitPotPlayers; } set { splitPotPlayers = value; } }

        internal static void GetNumberOfPlayersAndNames()
        {
            Console.WriteLine($"Please enter the number of players wanting dealt in");
            var userInput = Console.ReadLine();
            Console.Clear();

            var numberOfPlayers = Convert.ToInt32(userInput);

            for (var i = 0; i < numberOfPlayers; i++)
            {
                Console.WriteLine($"Please enter player number [{i + 1}]'s name");
                var name = Console.ReadLine();
                Console.Clear();

                Players.Add(new Player(name));
            }
        }

        internal static void DealHoleCards()
        {
            Board.CurrentPhase = "PreFlop";
            Deck.ShuffleDeck();

            ResetValues();                            

            foreach (var player in Players)
            {
                //"10" is enough for the big blind
                if(player.Chips > 10)
                {   
                    DealCardsToPlayer(player);
                }
                else
                {
                    player.HasCards = false;
                }
            }

            foreach (var player in Players)
            {
                Output.PrintNewlyDealtCards(player);                        
            }
        }

        //Need to properly test with more than 3 players and mix in some players with no chips so see it can skip over that postion properly 

        internal static void TakeTheBlinds()
        {            
            //Checks if the current player has cards and therefore has over 10 chips, meaning they are suitable for the small blind
            while (true)
            {
                if (SmallBlindCounter >= Players.Count) { SmallBlindCounter = 0; }

                if (Players[SmallBlindCounter].HasCards == true)
                {
                    Players[SmallBlindCounter].Chips -= Board.SmallBlind;
                    break;
                }
                else
                {
                    SmallBlindCounter++;
                    BigBlindCounter++;
                }
            }

            //Checks if the current player has cards and therefore has over 10 chips, meaning they are suitable for the big blind
            while (true)
            {
                if (BigBlindCounter >= Players.Count) { BigBlindCounter = 0; }

                if (Players[BigBlindCounter].HasCards == true)
                {
                    Players[BigBlindCounter].Chips -= Board.BigBlind;
                    break;                    
                }
                else
                {
                    BigBlindCounter++;
                }
            }
            SmallBlindCounter++;
            BigBlindCounter++;
            
        }

        private static void ResetValues()
        {
            Board.ResetBoardValues();
            Board.ResetPlayerValues();

            Board.HandIsLive = true;
            IsSplitPot = false;            
        }

        internal static void DealCardsToPlayer(Player player)
        {
            player.HasCards = true;

            player.CardOne = Deck.LiveDeck[0];
            player.CardTwo = Deck.LiveDeck[1];

            Deck.LiveDeck.RemoveRange(0, 2);
        }

        internal static void DealFlop()
        {
            Board.FlopSlot1 = Deck.LiveDeck[0];
            Board.FlopSlot2 = Deck.LiveDeck[1];
            Board.FlopSlot3 = Deck.LiveDeck[2];
            Board.CurrentPhase = "Flop";

            Deck.LiveDeck.RemoveRange(0, 3);
        }

        internal static void DealTurn()
        {
            Board.TurnSlot = Deck.LiveDeck[0];
            Board.CurrentPhase = "Turn";

            Deck.LiveDeck.RemoveAt(0); 
        }

        internal static void DealRiver()
        {
            Board.RiverSlot = Deck.LiveDeck[0];
            Board.CurrentPhase = "River";

            Deck.LiveDeck.RemoveAt(0);  
        }


        //Main condtion is to check if each player with cards has bet the same amount of chips this round
        //If they have cards, have more than 0 chips, have bet different amounts this round, then action must continue as the round is not complete (e.g the flop)
        //I do have to handle the situation where a player has bet more but is all in, so they wont have any chips in their stack as they are all in the pot, but the other player still has to call the bet so IsActionOver() equals false
        internal static bool IsActionOver()
        {            
            var PlayersInTheHand = Board.GetPlayersInHand();

            if (PlayersInTheHand.Count < 2)
            {
                Board.HandIsLive = false;
                return true;
            }
            else
            {
                for (var i = 1; i < PlayersInTheHand.Count; i++)
                {
                    if (PlayersInTheHand[i].ChipsBetThisRound != PlayersInTheHand[i - 1].ChipsBetThisRound)
                    {
                        //I JUST SWAPPED THIS IF STATEMENT WITH THE IF STATEMENT DIRECTLY BELOW, VERIFY THIS CHANGE DOES NOT BREAK ANYTHING BUT IT SEEMS FINE                       

                        //If we are here then 2 players have bet different amounts this hand, if they both also have more than 0 chips then action must continue for the moment
                        if (PlayersInTheHand[i].Chips > 0 && PlayersInTheHand[i - 1].Chips > 0)
                        {
                            return false;
                        }

                        //This is needed in case the player is all in. In this scenario player 1 has bet more chips this round than player2, but player 1 is all in. So action must continue as player 2 needs to fold or call
                        if (PlayersInTheHand[i].ChipsBetThisRound > PlayersInTheHand[i - 1].ChipsBetThisRound)
                        {
                            if (PlayersInTheHand[i].IsAllIn == true && PlayersInTheHand[i - 1].IsAllIn == false) { return false; }
                        }
                        else
                        {
                            if (PlayersInTheHand[i - 1].IsAllIn == true && PlayersInTheHand[i].IsAllIn == false) { return false; }
                        }                        
                    }                    
                }
            }            
           
            return true;
        }

        internal static void DeterminePlayerWithBestHand()
        {     
            var PotentialWinners = FindPotentialHandWinners();

            if (PotentialWinners.Count > 0)
            {
                PotentialWinners.Add(HandWinner);
                PotentialWinners = PotentialWinners.Distinct().ToList();

                DetermineBestVersionOfStrongestHandType(PotentialWinners);
            }   
        }

        private static List<Player> FindPotentialHandWinners()
        {
            var PotentialWinners = new List<Player>();
            var PlayersInHand = Board.GetPlayersInHand();

            HandWinner = PlayersInHand[0];

            foreach (var player in PlayersInHand)
            {
                //This only returns a general assessment, as in, high card equals 0, three of a kind equals 3.
                player.BestHandType = player.GetStrongestHandType();
            }

            for (var i = 1; i < PlayersInHand.Count; i++)
            {
                if (PlayersInHand[i].BestHandType > HandWinner.BestHandType)
                {
                    HandWinner = PlayersInHand[i];
                }
                else if (PlayersInHand[i].BestHandType.Equals(HandWinner.BestHandType))
                {
                    PotentialWinners.Add(PlayersInHand[i]);
                }
            }

            //Now that the loop above is complete and we know the BestHandType, we have to go through the list of "potentialWinners" and remove any that dont match the winner
            PotentialWinners.RemoveAll(item => item.BestHandType != HandWinner.BestHandType);

            return PotentialWinners;
        }

        private static void DetermineBestVersionOfStrongestHandType(List<Player> potentialWinners)
        {            
            //I need to repeat the same process with PotentialWinners list and kickers. if they equal the same as the current highest then add them to list, at the we remove any that dont equal the highest value, then from there start to look at the next kicker
            //All players in PotentialWinners should have the same BestHandType if they are at this stage in the code, so the "[0]" index is irrelevant
            switch (potentialWinners[0].BestHandType)
            {
                case 0:                          
                    DeterminePlayerWithBestKickers(potentialWinners); 
                    break; 

                case 1:
                    DetermineBestOnePair(potentialWinners);
                    break;                    

                case 2:
                    DetermineBestTwoPair(potentialWinners);
                    break;

                case 3:
                    DetermineBestThreeOfAKind(potentialWinners);
                    break;

                case 4:
                    DetermineBestStraight(potentialWinners);
                    break;

                case 5:
                    DetermineBestFlush(potentialWinners);
                    break;

                case 6:
                    //DetermineBestFullHouse(potentialWinners);
                    break;

                case 7:
                    DetermineBestFourOfAKind(potentialWinners);
                    break;

                case 8:
                    //DetermineBestStraightFlushFound(potentialWinners);
                    break;

                case 9:
                    //dont need one for royal flush as if more than 1 person has it then it is automatically a split pot as it must be the 5 cards on the board 
                    break;


            }
        }

        private static void DetermineBestOnePair(List<Player> players)
        {
            var potentialWinners = new List<Player>();
            HandWinner = players[0];

            for (var i = 1; i < Players.Count; i++)
            {
                if (Players[i].HighestPair > HandWinner.HighestPair)
                {
                    HandWinner = Players[i];
                }
                else if (Players[i].HighestPair == HandWinner.HighestPair)
                {
                    potentialWinners.Add(Players[i]);
                }
            }

            //Now that the loop above is complete and we know the winner for sure, we have to go through the list of "PotentialWinners" and remove any that dont match the winner
            potentialWinners.RemoveAll(item => item.HighestPair != HandWinner.HighestPair);

            if (potentialWinners.Count > 0)
            {
                potentialWinners.Add(HandWinner);//dont think I actually need this since HandWinner always gets assigned to players[0] to start the function now. 
                potentialWinners = potentialWinners.Distinct().ToList(); //Im not sure this is actually needed since I now start the for loop above at 1 but no harm in keeping it

                DeterminePlayerWithBestKickers(potentialWinners);
            }
        }

        //closely resembles how a single pair is determined
        private static void DetermineBestTwoPair(List<Player> players)
        {
            var potentialWinners = new List<Player>();
            HandWinner = players[0];

            for (var i = 1; i < Players.Count; i++)
            {
                if (Players[i].HighestPair > HandWinner.HighestPair)
                {
                    HandWinner = Players[i];
                }
                else if (Players[i].HighestPair == HandWinner.HighestPair)
                {
                    if (Players[i].SecondHighestPair > HandWinner.SecondHighestPair)
                    {
                        HandWinner = Players[i];
                    }
                    else if (Players[i].SecondHighestPair == HandWinner.SecondHighestPair)
                    {
                        potentialWinners.Add(Players[i]);
                    }
                }
            }

            ////Now that the loop above is complete and we know the winner for sure, we have to go through the list of "PotentialWinners" and remove any that dont match the winner            
            potentialWinners.RemoveAll(item => item.HighestPair != HandWinner.HighestPair);
            potentialWinners.RemoveAll(item => item.SecondHighestPair != HandWinner.SecondHighestPair);

            if (potentialWinners.Count > 0)
            {
                potentialWinners.Add(HandWinner); //dont think I actually need this since HandWinner always gets assigned to players[0] to start the function now. 
                potentialWinners = potentialWinners.Distinct().ToList(); // Im not sure this is actually needed since I now start the for loop above at 1 but no harm in keeping it

                DeterminePlayerWithBestKicker(potentialWinners);
            }
        }

        private static void DetermineBestThreeOfAKind(List<Player> players)
        {
            var potentialWinners = new List<Player>();
            HandWinner = players[0];

            for (var i = 1; i < Players.Count; i++)
            {
                if (Players[i].HighestThreeOfAKindValue > HandWinner.HighestThreeOfAKindValue)
                {
                    HandWinner = Players[i];
                }
                else if (Players[i].HighestThreeOfAKindValue == HandWinner.HighestThreeOfAKindValue)
                {
                    potentialWinners.Add(Players[i]);
                }
            }

            potentialWinners.RemoveAll(item => item.HighestThreeOfAKindValue != HandWinner.HighestThreeOfAKindValue);

            if (potentialWinners.Count > 0)
            {
                potentialWinners.Add(HandWinner); //dont think I actually need this since HandWinner always gets assigned to players[0] to start the function now. 
                potentialWinners = potentialWinners.Distinct().ToList(); // Im not sure this is actually needed anymore since I now start the for loop above at 1 but no harm in keeping it here

                DeterminePlayerWithBestKickers(potentialWinners);
            }
        }

        private static void DetermineBestStraight(List<Player> potentialWinners)
        {
            DeterminePlayerWithBestKickers(potentialWinners);
        }

        private static void DetermineBestFlush(List<Player> potentialWinners)
        {
            DeterminePlayerWithBestKickers(potentialWinners);
        }

        //private static void DetermineBestFullHouse(List<Player> potentialWinners)
        //{
        //    throw new NotImplementedException();
        //}

        private static void DetermineBestFourOfAKind(List<Player> potentialWinners)
        {
            DeterminePlayerWithBestKickers(potentialWinners);
        }

        //private static void DetermineBestStraightFlushFound(List<Player> potentialWinners)
        //{            
        //}

        //private static void DetermineBestRoyalFlushFound(List<Player> potentialWinners)
        //{            
        //}


        //I think this may be redundant now. DeterminePlayerWithBestKickers() is very similiar and I think it can handle the situation where we only have a single kicker
        private static void DeterminePlayerWithBestKicker(List<Player> PotentialWinners)
        {
            HandWinner = PotentialWinners[0];
            SplitPotPlayers = new List<Player>() { };

            for (var i = 1; i < PotentialWinners.Count; i++)
            {
                if (PotentialWinners[i].ListOfKickers[0] > HandWinner.ListOfKickers[0])
                {
                    HandWinner = PotentialWinners[i];
                }
                else if (PotentialWinners[i].ListOfKickers[0] == HandWinner.ListOfKickers[0])
                {
                    //come back to handling this split pot stuff. I think I may just be able to have a more general function that checks for split pots, at the very least seperate functions, now not so sure about that
                    IsSplitPot = true;
                    SplitPotPlayers.Add(PotentialWinners[i]);
                }
            }

            if (IsSplitPot) { HandleSplitPot(); }       

        }

        private static void DeterminePlayerWithBestKickers(List<Player> potentialWinners)
        {
            HandWinner = potentialWinners[0];
            SplitPotPlayers = new List<Player>() { };
            int counter;

            for (var i = 1; i < potentialWinners.Count; i++)
            {
                counter = 0;
                for (var x = 0; x < potentialWinners[i].ListOfKickers.Count; x++)
                {
                    if (potentialWinners[i].ListOfKickers[x] > HandWinner.ListOfKickers[x])
                    {                        
                        HandWinner = potentialWinners[i];
                        break;                                      //pretty sure I need this break here, test properly
                    }
                    else if (potentialWinners[i].ListOfKickers[x] < HandWinner.ListOfKickers[x])
                    {
                        break;
                    }
                    else //if they equal each other
                    {
                        counter++;
                    }
                }

                if(counter == HandWinner.ListOfKickers.Count)
                {
                    IsSplitPot = true;
                    SplitPotPlayers.Add(potentialWinners[i]);
                }
            }

            if (IsSplitPot) { HandleSplitPot(); }
            
        }

        //This ensures that there is in fact a split pot as sometimes it may just being player 1 & 2 had the same hand but player 3 beats them both.
        //However, this is not unexpected as due to the nature of the code and how it works, some false positives are expected in certain scenarios. 
        //Imagine the board has 4 aces, player 1 has Q, player 2 has Q, player 3 has K. Player 2 would be added to SplitPotPlayers but once I reach player three, I see that there is no split pot 
        private static void HandleSplitPot()
        {
            //Since I start with assigning the first player in the list as the HandWinner, there may be instances when the 2nd player is incorrectly added to SplitPotPlayers
            for (var i = 0; i < HandWinner.ListOfKickers.Count; i++)
            {
                SplitPotPlayers.RemoveAll(item => item.ListOfKickers[i] < HandWinner.ListOfKickers[i]);
            }

            SplitPotPlayers.Add(HandWinner);

            SplitPotPlayers = SplitPotPlayers.Distinct().ToList(); //not needed anymore, think about removing once confident
            
            if (SplitPotPlayers.Count < 2)
            {
                IsSplitPot = false;
            }
        }

        internal static int GetHighestBetThisRound()
        {
            var Players = Board.GetPlayersInHand();
            var highestBetThisRound = 0;

            foreach (var player in Players)
            {
                if (player.ChipsBetThisRound > highestBetThisRound)
                {
                    highestBetThisRound = player.ChipsBetThisRound;
                }
            }

            return highestBetThisRound;
        }

        //this closely resembles IsActionOver(). This solves an issue with a player in the later postions of Players List getting incorrectly asked to make a move after their bet for this round has already been called
        //The IsActionOver() gets called after the code has looped through all players in Players List. I need PlayersHaveBetEqualAmounts() because I have to break out of 2 loops, a 'for' loop in some scenarios and a 'while' loop when each round of betting is done
        internal static bool PlayersHaveBetEqualAmounts()
        {
            var PlayersInTheHand = Board.GetPlayersInHand();
            var counter = 0;

            for (var i = 1; i < PlayersInTheHand.Count; i++)
            {
                if (PlayersInTheHand[i].ChipsBetThisRound > 0 && PlayersInTheHand[i].ChipsBetThisRound == PlayersInTheHand[i - 1].ChipsBetThisRound)
                {
                    counter++;
                }
            }

            if (counter >= PlayersInTheHand.Count - 1) { return true; }

            return false;
        }

        internal static void CheckForGameWinner()
        {
            foreach (Player player in Players)
            {
                if (player.Chips == Board.TotalAmountOfChipsInPlay)
                {
                    App.GameOver = true;
                    Output.PrintGameOver();
                }
            }
        }

    }
}
