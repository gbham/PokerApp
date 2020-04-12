using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using static PokerApp.App;

namespace PokerApp
{
    static class Dealer
    {
        private static Player handWinner;
        private static string illegalMoveErrorMsg;
        
        public static Player HandWinner { get { return handWinner; } set { handWinner = value; } }

        public static string IllegalMoveErrorMsg { get { return illegalMoveErrorMsg; } set { illegalMoveErrorMsg = value; } }

        public static void DealHoleCards()
        {
            Deck.ShuffleDeck();
            Board.HandIsLive = true;
            Board.FlopSlot1 = "";
            Board.FlopSlot2 = "";
            Board.FlopSlot3 = "";
            Board.TurnSlot = "";
            Board.RiverSlot = "";
            Board.CurrentPhase = "PreFlop";

            foreach (var player in Players)
            {
                //will need to eventually adjust 0 to equal the blinds for some players depending on position
                if(player.Chips > 0)
                {
                    player.HasCards = true;
                    player.IsAllIn = false;
                    DealCardsToPlayer(player);
                }
                else
                {
                    player.HasCards = false;
                }
            }

            foreach (var player in Players)
            {
                Console.Clear();
                Console.WriteLine($"Press any key to see [{player.Name}]'s cards.");
                Console.ReadKey();
                Console.Clear();
                Console.WriteLine($"[{player.Name}]'s cards = [{player.CardOne}] [{player.CardTwo}]");
                Console.WriteLine($"");
                Console.WriteLine($"Press any key to hide cards.");
                Console.ReadKey();                
            }
        }

        internal static void DealCardsToPlayer(Player player)
        {
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


        //Simple and effective way to tell if bets are needing dealt with by checking if each player with cards has bet the same amount of chips this round
        //If they have cards, have more than 0 chips, have bet different amounts this round, then action must continue before the round can end (e.g flop)
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
                        //This is needed in case the player is all in. In this scenario player 1 has bet more chips this round than player2, and player 1 is all in. So action 
                        if (PlayersInTheHand[i].ChipsBetThisRound > PlayersInTheHand[i - 1].ChipsBetThisRound)
                        {
                            if (PlayersInTheHand[i].IsAllIn == true && PlayersInTheHand[i - 1].IsAllIn == false) { return false; }
                        }
                        else
                        {
                            if (PlayersInTheHand[i - 1].IsAllIn == true && PlayersInTheHand[i].IsAllIn == false) { return false; }
                        }

                        //If we are here then 2 players have bet different amounts this hand, if they both also have more than 0 chips then action must continue for the moment
                        if (PlayersInTheHand[i].Chips > 0 && PlayersInTheHand[i - 1].Chips > 0)
                        {
                            return false;
                        }
                    }                    
                }
            }            
           
            return true;
        }

        //this might be what im after, it closely resembles IsActionOver(). This solves an issue with a player in the later postions of Players getting asked to make a move after their previous bet has been called by another player. The IsActionOver() only gets called after it has looped through all players in Players. I need this because I have to break out of 2 loops, a for loop and a while loop
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
            foreach (Player player in App.Players)
            {
                if (player.Chips == 3000) //replace with TotalAmountOfChipsInPlay
                {
                    App.GameOver = true;
                    Console.WriteLine($"GAME OVER");
                }
            }
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

            for (var i = 0; i < PlayersInHand.Count; i++)
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

            if (PotentialWinners.Count > 0)
            {
                //Now that the loop above is complete and we know the BestHandType, we have to go through the list of "potentialWinners" and remove any that dont match the winner

                for (var i = 0; i < PotentialWinners.Count; i++)
                {
                    if (PotentialWinners[i].BestHandType != HandWinner.BestHandType)
                    {
                        PotentialWinners.Remove(PotentialWinners[i]);
                    }
                }
            }

            return PotentialWinners;

        }

        private static void DetermineBestVersionOfStrongestHandType(List<Player> PotentialWinners)
        {            

            //case statements to check player.BestPair(), then to check player.BestThreeOfAKind(), then player.BestKicker if it comes to that

            //I need to repeat the same process with PotentialWinners list and kickers. if they equal the same as the current highest then add them to list, at the we remove any that dont equal the highest value, then from there start to look at the next kicker, would be nice to loop this process 
            switch (PotentialWinners[0].BestHandType)
            {
                case 0:
                    
                    DeterminePlayerWithBestKickers(PotentialWinners);                    

                    break;
                    


                case 1:
                    //I SHOULD JUST ADD ALL THE PAIRS TO A LIST AS I MATCH THEM, THEN I CAN JUST SORT THE LIST AND DETERMINE THE BEST PAIR EASY WITHOUT COMPARISON
                    //I SHOULD JUST ADD ALL THE PAIRS TO A LIST AS I MATCH THEM, THEN I CAN JUST SORT THE LIST AND DETERMINE THE BEST PAIR EASY WITHOUT COMPARISON

                    DetermineBestOnePair(PotentialWinners);

                    break;
                    

                case 2:



                    break;
                    






            }


        }

        private static void DeterminePlayerWithBestKickers(List<Player> PotentialWinners)
        {
            HandWinner = PotentialWinners[0];

            //******I NEED TO PAD OUT ALL player.ListOfKicker FUNCTIONS WITH 0'S SO THESE COMPARISONS BELOW DONT THROW EXCEPTIONS
            //-***** actually, comparisons should only be done between hands of the same type. So I believe that should mean all these lists are the same sixe and wont need padding
            for (var i = 1; i < PotentialWinners.Count; i++)
            {
                for (var x = PotentialWinners[i].ListOfKickers.Count - 1; x > 0; x--)
                {
                    if (PotentialWinners[i].ListOfKickers[x] > PotentialWinners[i - 1].ListOfKickers[x])
                    {
                        HandWinner = PotentialWinners[i];
                    }
                    else if (PotentialWinners[i].ListOfKickers[x] < PotentialWinners[i - 1].ListOfKickers[x])
                    {
                        HandWinner = PotentialWinners[i - 1];
                    }
                }
            }            
        }

        //I SHOULD JUST ADD ALL THE PAIRS TO A LIST AS I MATCH THEM, THEN I CAN JUST SORT THE LIST AND DETERMINE THE BEST PAIR EASY WITHOUT COMPARISON
        //I SHOULD JUST ADD ALL THE PAIRS TO A LIST AS I MATCH THEM, THEN I CAN JUST SORT THE LIST AND DETERMINE THE BEST PAIR EASY WITHOUT COMPARISON
        //I SHOULD JUST ADD ALL THE PAIRS TO A LIST AS I MATCH THEM, THEN I CAN JUST SORT THE LIST AND DETERMINE THE BEST PAIR EASY WITHOUT COMPARISON
        private static void DetermineBestOnePair(List<Player> Players)
        {
            var PotentialWinners = new List<Player>();
            HandWinner = Players[0];

            for (var i = 0; i < Players.Count; i++)
            {
                if (Players[i].HighestPair > HandWinner.HighestPair)
                {
                    HandWinner = Players[i];
                }
                else if (Players[i].HighestPair == HandWinner.HighestPair)
                {
                    PotentialWinners.Add(Players[i]);
                }
            }

            if (PotentialWinners.Count > 0)
            { 
                //Now that the loop above is complete and we know the winner for sure, we have to go through the list of "PotentialWinners" and remove any that dont match the winner
                for (var i = 0; i < PotentialWinners.Count; i++)
                {
                    if (PotentialWinners[i].HighestPair != HandWinner.HighestPair)
                    {
                        PotentialWinners.Remove(PotentialWinners[i]);
                    }
                }

                if (PotentialWinners.Count > 0)
                { 
                    PotentialWinners.Add(HandWinner);
                    PotentialWinners = PotentialWinners.Distinct().ToList();

                    DeterminePlayerWithBestKickers(PotentialWinners);
                }                    
            }

            


            //for (var i = 0; i < PotentialWinners.Count; i++)
            //{
            //    if (PotentialWinners[i].BestKicker > HandWinner.BestKicker)
            //    {
            //        HandWinner = PotentialWinners[i];
            //    }
            //    //Will need to eventually handle the situation where players share both same top pair and same top kicker. 

            }
        }
    }
