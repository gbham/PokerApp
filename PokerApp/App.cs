using System;
using System.Collections.Generic;

namespace PokerApp
{
    class App
    {
        internal static bool GameOver = false;

        private static Player Tyrion;
        private static Player Ford;
        private static Player Sherlock;

        public static List<Player> Players;


        public static void Main()
        {
            Tyrion = new Player();
            Ford = new Player();
            Sherlock = new Player();

            Tyrion.Name = "Tyrion";
            Ford.Name = "Ford";
            Sherlock.Name = "Sherlock";

            Players = new List<Player>() { Ford, Tyrion, Sherlock };


            //Ford.HasCards = true;
            //Sherlock.HasCards = true;
            //Tyrion.HasCards = true;

            //Ford.CardOne = "9H";
            //Ford.CardTwo = "9D";

            //Sherlock.CardOne = "AH";
            //Sherlock.CardTwo = "7S";

            //Tyrion.CardOne = "9S";
            //Tyrion.CardTwo = "9C";

            //Board.FlopSlot1 = "6C";
            //Board.FlopSlot2 = "AS";
            //Board.FlopSlot3 = "3C";
            //Board.TurnSlot = "4C";
            //Board.RiverSlot = "5C";

            //DeclareWinnerOfHand();


            while (!GameOver)
            {
                Dealer.DealHoleCards();

                PlayHand();

                //At the moment, this is set up to win the first time it is ran
                Dealer.CheckForGameWinner();
            }
        }



        public static void PlayHand()
        {
            //TakeTheBlinds();            

            PlayRound("PreFlop");

            if (Board.HandIsLive)
            {
                Dealer.DealFlop();
                PlayRound("Flop");
            }

            if (Board.HandIsLive)
            {
                Dealer.DealTurn();
                PlayRound("Turn");
            }

            if (Board.HandIsLive)
            {
                Dealer.DealRiver();
                PlayRound("River");
            }

            DeclareWinnerOfHand();

            AssignChipsToWinnerOfHand();

        }

        private static void AssignChipsToWinnerOfHand()
        {
            Dealer.HandWinner.Chips += Board.ChipsInPot;
            Board.ChipsInPot = 0;
        }

        public static void PlayRound(string round) //not sure if I will actualy need to use this string to differentiate between rounds but seems more than likely
        {
            Tyrion.ChipsBetThisRound = 0;
            Ford.ChipsBetThisRound = 0;
            Sherlock.ChipsBetThisRound = 0;

            while (true)
            {
                //would be nicer to use a foreach loop but I need to be able to incrememnt once back in the loop if the user wants to show/hide their cards or if they try an illegal move
                for (var i = 0; i < Players.Count; i++)
                {
                    var PlayersInHand = Board.GetPlayersInHand();

                    if (Players[i].HasCards && Players[i].Chips > 0 && PlayersInHand.Count > 1)
                    {
                        PrintGameInformation();                        

                        Console.WriteLine($"\n{Players[i].Name}'s Turn");

                        //blinds could potentially throws a spanner in the works for GetMoveOPtions depending on how I handle them. If I make blinds completely seperate to player.ChipsBetThisRound then I should be fine
                        var output = Board.GetMoveOptions();
                        Console.WriteLine(output);

                        //Need to add some instructions on format of user input and a help menu at some point
                        var input = Console.ReadLine();

                        Players[i].LastMove = Utils.GetUserInputtedCommand(input);
                        string value = Utils.GetUserInputtedValue(input);

                        //if(IllegalMoveUsed(output, input)) { i -= 1; }
                        //else
                        //{
                            switch (Players[i].LastMove)
                            {
                                case "Fold":
                                case "F":
                                    Players[i].Fold();
                                    break;

                                case "Check":
                                case "c":
                                    Players[i].Check();
                                    break;

                                case "Call":
                                case "CA":
                                    Players[i].Call();
                                    break;

                                case "Bet":
                                case "B":
                                    Players[i].Bet(Convert.ToInt32(value));
                                    break;

                                case "Raise":
                                case "R":
                                    Players[i].Raise(Convert.ToInt32(value));
                                    break;

                                case "All-In":
                                case "AllIn":
                                case "All In":
                                case "All":
                                    Players[i].AllIn();
                                    break;

                                case "Show":
                                case "S":
                                    Players[i].ShowCards();
                                    i -= 1;
                                    break;

                                //Need to repeat this stage of the for loop if no valid move is made
                                default:
                                    i -= 1;
                                    break;

                            }


                        //}

                        
                    }
                }

                //Checks first if only one person has cards left THEN the betting for the round is done - both are reasons to back out of the while loop (the current round, e.g the flop )
                if (Dealer.IsActionOver()) { break; }
            }
        }



        private static void DeclareWinnerOfHand()
        {
            var PlayersInHand = Board.GetPlayersInHand();

            if (PlayersInHand.Count < 2)
            {
                Console.Clear();
                Console.WriteLine($"\nThe Winner of the hand is {PlayersInHand[0].Name}.");
                Dealer.HandWinner = PlayersInHand[0];
            }
            else
            {
                Dealer.DeterminePlayerWithBestHand(PlayersInHand);

                Console.Clear();
                Console.WriteLine($"The Winner of the hand is {Dealer.HandWinner.Name}. He has a hand type of: {Deck.PokerHandsList[Dealer.HandWinner.BestHandType]}. With the kicker of: {Dealer.HandWinner.BestKicker} (Kickers are not complete)" );
                Console.WriteLine("");
                Console.WriteLine($"The Board = [{Board.FlopSlot1}], [{Board.FlopSlot2}], [{Board.FlopSlot3}], [{Board.TurnSlot}], [{Board.RiverSlot}]");
                Console.WriteLine("");
                Console.WriteLine($"{Ford.Name}'s cards were: [{Ford.CardOne}] [{Ford.CardTwo}] ");
                Console.WriteLine($"{Tyrion.Name}'s cards were: [{Tyrion.CardOne}] [{Tyrion.CardTwo}] ");
                Console.WriteLine($"{Sherlock.Name}'s cards were: [{Sherlock.CardOne}] [{Sherlock.CardTwo}] ");

            }

            Console.WriteLine($"\nPress any button to continue to next hand.");
            Console.ReadKey();
            Console.Clear();

        }


        internal static void PrintGameInformation()
        {
            Console.Clear();
            Console.WriteLine($"-------------------------------");
            Console.WriteLine($"[{Board.CurrentPhase}]");
            Console.WriteLine($"-------------------------------");
            Console.WriteLine($"\nChip Stacks:\n");

            foreach (var player in Players)
            {
                Console.WriteLine($"[{player.Name}]'s chips: [{player.Chips}]");
            }

            Console.WriteLine($"\nHas Cards:\n");

            foreach (var player in Players)
            {
                Console.WriteLine($"[{player.Name}]: [{player.HasCards}]");
            }

            Console.WriteLine($"\nPot Value = [{Board.ChipsInPot}]\n");
            Console.WriteLine($"-------------------------------");            

            var FlopOutput = $"The Board = [{ Board.FlopSlot1}] [{Board.FlopSlot2}] [{Board.FlopSlot3}]";
            var TurnOutput = $" [{Board.TurnSlot}]";
            var RiverOutput = $" [{Board.RiverSlot}]";
            var TheBoard = ""; 

            if (!string.IsNullOrWhiteSpace(Board.FlopSlot1)) { TheBoard += FlopOutput; }
            if (!string.IsNullOrWhiteSpace(Board.TurnSlot)) { TheBoard += TurnOutput; }
            if (!string.IsNullOrWhiteSpace(Board.RiverSlot)) { TheBoard += RiverOutput; }

            if (!string.IsNullOrWhiteSpace(TheBoard)) 
            {                
                Console.WriteLine($"{TheBoard}");
                Console.WriteLine($"-------------------------------");
            }
        }
    }
}
