using System;
using System.Collections.Generic;
using System.Linq;
using static PokerApp.App;

namespace PokerApp
{
    static class Dealer
    {
        private static Player handWinner;        

        public static Player HandWinner { get { return handWinner; } set { handWinner = value; } }

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
                player.HasCards = true;
                DealCardsToPlayer(player);
            }

            foreach (var player in Players)
            {                
                Console.WriteLine($"Press any key to see [{player.Name}]'s cards.");
                Console.ReadKey();
                Console.Clear();
                Console.WriteLine($"[{player.Name}]'s cards = [{player.CardOne}] [{player.CardTwo}]");
                Console.WriteLine($"");
                Console.WriteLine($"Press any key to hide cards.");
                Console.ReadKey();
                Console.Clear();
            }

        }

        internal static void DealCardsToPlayer(Player player)
        {
            player.CardOne = Deck.LiveDeck[0];
            player.CardTwo = Deck.LiveDeck[1];

            //Console.WriteLine($"{player.Name}'s hand = [{player.CardOne}] + [{player.CardTwo}]");

            Deck.LiveDeck.RemoveRange(0, 2);
        }

        internal static void DealFlop()
        {
            Board.FlopSlot1 = Deck.LiveDeck[0];
            Board.FlopSlot2 = Deck.LiveDeck[1];
            Board.FlopSlot3 = Deck.LiveDeck[2];
            Board.CurrentPhase = "Flop";

            Deck.LiveDeck.RemoveRange(0, 3);            

            PrintGameInformation();

            Console.WriteLine($"Round = [Flop] - The Board = [{Board.FlopSlot1}], [{Board.FlopSlot2}], [{Board.FlopSlot3}]");
        }

        internal static void DealTurn()
        {
            Board.TurnSlot = Deck.LiveDeck[0];
            Board.CurrentPhase = "Turn";

            Deck.LiveDeck.RemoveAt(0);            

            PrintGameInformation();

            Console.WriteLine($"Round = [Turn] - The Board = [{Board.FlopSlot1}], [{Board.FlopSlot2}], [{Board.FlopSlot3}], [{Board.TurnSlot}]");
        }

        internal static void DealRiver()
        {
            Board.RiverSlot = Deck.LiveDeck[0];
            Board.CurrentPhase = "River";

            Deck.LiveDeck.RemoveAt(0);            

            PrintGameInformation();

            Console.WriteLine($"Round = [River] - The Board = [{Board.FlopSlot1}], [{Board.FlopSlot2}], [{Board.FlopSlot3}], [{Board.TurnSlot}], [{Board.RiverSlot}]");
        }


        //Simple and effective way to tell if bets are needing dealt with by checking if each player with cards has bet the same amount of chips this round
        //If they have cards, have more than 0 chips, have bet different amounts this round, then action must continue before the round can end (e.g flop)
        internal static bool IsActionOver()
        {            
            var PlayersInTheHand = Board.GetPlayersInHand();

            if (PlayersInTheHand.Count < 2)
            {
                Board.HandIsLive = false;
            }
            else
            {
                for (var i = 1; i < PlayersInTheHand.Count; i++)
                {
                    if (PlayersInTheHand[i].ChipsBetThisRound != PlayersInTheHand[i - 1].ChipsBetThisRound)
                    {
                        //This is needed in case the player is all in. In this scenario they would of course not have bet the same amount of chips as the player with the bigger stack
                        if (PlayersInTheHand[i].ChipsBetThisRound > PlayersInTheHand[i - 1].ChipsBetThisRound)
                        {
                            if (PlayersInTheHand[i].IsAllIn == true) { return false; }
                        }
                        else
                        {
                            if (PlayersInTheHand[i - 1].IsAllIn == true) { return false; }
                        }

                        if (PlayersInTheHand[i].Chips > 0 && PlayersInTheHand[i - 1].Chips > 0)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }


        internal static void CheckForGameWinner()
        {

            ////this current set up means only one hand is played
            //GameOver = true;
            //return;



            foreach (Player player in Players)
            {
                if (player.Chips == 3000) //replace with TotalAmountOfChipsInPlay
                {
                    GameOver = true;
                    Console.WriteLine($"GAME OVER");
                }
            }
        }



        internal static void DeterminePlayerWithBestHand(List<Player> PlayersInHand)
        {
            HandWinner = PlayersInHand[0];
            var PotentialWinners = new List<Player>();


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


            if (PotentialWinners.Count > 0)
            {
                PotentialWinners.Add(HandWinner);
                PotentialWinners = PotentialWinners.Distinct().ToList();

                DetermineBestVersionOfStrongestHandType(PotentialWinners);

            }   
        }

        private static void DetermineBestVersionOfStrongestHandType(List<Player> Players)
        {
            HandWinner = Players[0];

            //case statements to check player.BestPair(), then to check player.BestThreeOfAKind(), then player.BestKicker if it comes to that

            switch (Players[0].BestHandType)
            {
                case 0:
                    foreach (var player in Players)
                    {
                        if (player.BestKicker > HandWinner.BestKicker)
                        {
                            HandWinner = player;
                        }
                    }

                    break;
                    


                case 1:

                    GetBestOnePair(Players);

                    break;
                    

                case 2:



                    break;
                    






            }


        }

        private static void GetBestOnePair(List<Player> Players)
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
            }

            for (var i = 0; i < PotentialWinners.Count; i++)
            {
                if (PotentialWinners[i].BestKicker > HandWinner.BestKicker)
                {
                    HandWinner = PotentialWinners[i];
                }

                //Will need to eventually handle the situation where players share both same top pair and same top kicker. 

            }
        }
    }
}