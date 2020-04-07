using System;
using System.Collections.Generic;

namespace PokerApp
{
    class App
    {
        internal static bool GameOver = false;        

        static Player Tyrion;
        static Player Ford;
        static Player Sherlock;      

        public static List<Player> Players;        

        //Extract this to somewhere else
        //static int TotalAmountOfChipsInPlay = Player.StartingStack * Players.Count;

        public static void Main() 
        {
            Console.WriteLine("In Main");            
            App.Start();

        }

        public static void Start()
        {            
            Tyrion = new Player();   
            Ford = new Player();   
            Sherlock = new Player();

            Tyrion.Name = "Tyrion";
            Ford.Name = "Ford";
            Sherlock.Name = "Sherlock";

            Players = new List<Player>() { Ford, Tyrion, Sherlock };



            Ford.HasCards = true;
            Sherlock.HasCards = true;
            Tyrion.HasCards = true;

            Ford.CardOne = "9H";
            Ford.CardTwo = "9D";

            Sherlock.CardOne = "AH";
            Sherlock.CardTwo = "7S";

            Tyrion.CardOne = "9S";
            Tyrion.CardTwo = "9C";

            Board.FlopSlot1 = "6C";
            Board.FlopSlot2 = "AS";
            Board.FlopSlot3 = "3C";
            Board.TurnSlot = "4C";
            Board.RiverSlot = "5C";

            DeclareWinnerOfHand();






            //while (!GameOver)
            //{
            //    Board.ChipsInPot = 0;
            //    Dealer.DealHoleCards();

            //    PlayHand();

            //    //At the moment, this is set up to win the first time it is ran
            //    Dealer.CheckForGameWinner();
            //}
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

            //AssignChipsToWinner

        }

        

        
        public static void PlayRound(string round) //not sure if I will actualy need to use this string to differentiate between rounds but seems more than likely
        {
            Tyrion.ChipsBetThisRound = 0;
            Ford.ChipsBetThisRound = 0;
            Sherlock.ChipsBetThisRound = 0;

            while (true)
            {                
                foreach (var player in Players)
                {
                    var PlayersInHand = Board.GetPlayersInHand();

                    if (player.HasCards && player.Chips > 0 && PlayersInHand.Count > 1)
                    {
                        //Console.Clear();
                        Console.WriteLine($"------------------------------------------------------");
                        Console.WriteLine($"{player.Name}'s Turn");

                        
                        //blinds could potentially throws a spanner in the works for GetMoveOPtions depending on how I handle them. If I make blinds completely seperate to player.ChipsBetThisRound then I should be fine
                        var output = Board.GetMoveOptions();
                        Console.WriteLine(output);

                        //Need to add some instructions on format of user input and a help menu at some point
                        var input = Console.ReadLine();          

                        string action = "";
                        string value = "";

                        for (var i = 0; i < input.Length; i++)
                        {
                            if(Char.IsLetter(input[i]))
                            {
                                action += input[i];
                            }                            
                        }

                        for (var i = 0; i < input.Length; i++)
                        {
                            if (Char.IsDigit(input[i]))
                            {                                
                                value = input.Substring(i, (input.Length - i)); 
                                break;
                            }
                        }                        

                        switch (action)
                        {
                            case "Fold":
                            case "F":
                                player.Fold();
                                break;

                            case "Check":
                            case "CH":
                                player.Check();
                                break;

                            case "Call":
                            case "CA":
                                player.Call();
                                break;

                            case "Bet":
                            case "B":
                                player.Bet(Convert.ToInt32(value));
                                break;

                            case "Raise":
                            case "R":
                                player.Raise(Convert.ToInt32(value));
                                break;

                        }
                    }
                }

                //Checks if either the betting for the round is done OR if only one person has cards left - both are reasons to back out of the while loop (the current round, e.g the flop )
                if (Dealer.IsActionOver()) { break; }
            }            
        }

        private static void DeclareWinnerOfHand()
        {
            var PlayersInHand = Board.GetPlayersInHand();

            if (PlayersInHand.Count < 2)
            {
                Console.WriteLine($"The Winner of the hand is {PlayersInHand[0].Name}.");
                Dealer.HandWinner = PlayersInHand[0];
            }
            else
            {               
                Dealer.DeterminePlayerWithBestHand(PlayersInHand);
                Console.WriteLine($"The Winner of the hand is {Dealer.HandWinner.Name}. He has a hand type of: {Deck.PokerHandsList[Dealer.HandWinner.BestHandType]}. With the kicker of: {Dealer.HandWinner.BestKicker}");
                Console.WriteLine("");
                Console.WriteLine($"The Board = {Board.flopSlot1}, {Board.flopSlot2}, {Board.flopSlot3}, {Board.TurnSlot}, {Board.RiverSlot}");
                Console.WriteLine("");
                Console.WriteLine($"{Ford.Name}'s cards were: [{Ford.CardOne}] + [{Ford.CardTwo}] ");
                Console.WriteLine($"{Tyrion.Name}'s cards were: [{Tyrion.CardOne}] + [{Tyrion.CardTwo}] ");
                Console.WriteLine($"{Sherlock.Name}'s cards were: [{Sherlock.CardOne}] + [{Sherlock.CardTwo}] ");
                

            }
        }
    }
}
