using System;
using System.Collections.Generic;

namespace PokerApp
{
    static class Dealer
    {

        public static void DealHoleCards()
        {
            Deck.ShuffleDeck();
            Board.HandIsLive = true;

            foreach (var player in App.Players)
            {
                player.HasCards = true;
                DealCardsToPlayer(player);
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
            Board.flopSlot1 = Deck.LiveDeck[0];
            Board.flopSlot2 = Deck.LiveDeck[1];
            Board.flopSlot3 = Deck.LiveDeck[2];

            Deck.LiveDeck.RemoveRange(0, 3);

            Console.WriteLine($"Round = [Flop] - The Board = {Board.flopSlot1}, {Board.flopSlot2}, {Board.flopSlot3}");
        }

        internal static void DealTurn()
        {
            Board.TurnSlot = Deck.LiveDeck[0];

            Deck.LiveDeck.RemoveAt(0);

            Console.WriteLine($"Round = [Turn] - The Board = {Board.flopSlot1}, {Board.flopSlot2}, {Board.flopSlot3}, {Board.TurnSlot}");
        }

        internal static void DealRiver()
        {
            Board.RiverSlot = Deck.LiveDeck[0];

            Deck.LiveDeck.RemoveAt(0);

            Console.WriteLine($"Round = [River] - The Board = {Board.flopSlot1}, {Board.flopSlot2}, {Board.flopSlot3}, {Board.TurnSlot}, {Board.RiverSlot}");
        }




        internal static Player GetPlayerWithBestHand(List<Player> PlayersInHand)
        {
            var winner = new Player();
            var PotentialWinners = new List<Player>();

            foreach (var player in PlayersInHand)
            {
                //This only returns a general assessment, as in, high card equals 0, three of a kind equals 3.
                player.BestHandType = player.DetermineStrongestHandType();
            }

            for (var i = 0; i < PlayersInHand.Count - 1; i++)
            {
                if (PlayersInHand[i].BestHandType > winner.BestHandType)
                {
                    winner = PlayersInHand[i];
                }
                else if (PlayersInHand[i].BestHandType.Equals(winner.BestHandType))
                {
                    PotentialWinners.Add(PlayersInHand[i]);
                }
            }


            if (PotentialWinners.Count > 0)
            {
                //Now that the loop above is complete and we know the winner for sure, we have to go through the list of "potentialWinners" and remove any that dont match the winner
                foreach (var potentialWinner in PotentialWinners)
                {
                    if (potentialWinner.BestHandType != winner.BestHandType)
                    {
                        PotentialWinners.Remove(potentialWinner);
                    }
                }
            }
            

            if (PotentialWinners.Count > 0)
            {
                PotentialWinners.Add(winner);

                /*winner = */ DetermineBestVersionOfStrongestHandType(PotentialWinners);

            }

            return winner;

        }





        private static void DetermineBestVersionOfStrongestHandType(List<Player> PotentialWinners)
        {
            Player winner;


            //*********READ THIS BELOW
            //IT DOESNT MAKE SENSE TO FIND THE HIGHEST VALUE IN A SEPERATE FUNCTION as I think most the calculations would need to be repeated anyway
            //it would be much easier to assign player.HighestPairCard and player.ThreeOfAKindValue etc etc when the hand type is being determined in the first place
            //I could still use this function to compare but I think it would be best to assign these values before


            //foreach(var potentialWinner in PotentialWinners)
            //{
            //    switch(potentialWinner.BestHandType)
            //    {
            //        case 0:
            //            potentialWinner.HighestCardInHandType = GetPairValue(potentialWinner);                     
            //            break;

            //        case 1:
            //            var twoPairValue = GetTwoPairValues(potentialWinner);

            //            //potentialWinner.HighestPair = twoPairValue[0];
            //            //potentialWinner.SecondHighestPair = twoPairValue[1];
            //            
            //            break;





            //    }




            //}








            //return winner;
        }



        //Simple and effective way to check if bets are needing dealt with. Check if each player with cards has bet the same amount of chips this round
        //NOTE: this wont work if someone goes all in, might get stuck in a loop. - I believe I fixed this by only running this function if the player.HasCards and player.Chips > 0
        internal static bool IsActionOver()
        {
            var PlayersInTheHand = Board.GetPlayersInHand();

            if (PlayersInTheHand.Count < 2)
            {
                Board.HandIsLive = false;
            }
            else
            {
                for (var i = 0; i < PlayersInTheHand.Count - 1; i++)
                {
                    if (PlayersInTheHand[i].ChipsBetThisRound != PlayersInTheHand[i + 1].ChipsBetThisRound)
                    {
                        //This is needed in case the player is all in. In this scenario they would of course not have bet the same amount of chips as the player with the bigger stack
                        if (PlayersInTheHand[i].Chips > 0 && PlayersInTheHand[i + 1].Chips > 0)
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

            //this current set up means only one hand is played
            App.GameOver = true;
            return;




            foreach (Player player in App.Players)
            {
                if (player.Chips == 3000) //replace with TotalAmountOfChipsInPlay
                {
                    App.GameOver = true;
                }
            }
        }


    }
}