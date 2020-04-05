using System;
using System.Collections.Generic;
using System.Linq;
using static PokerApp.Deck;

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

            Players = new List<Player>() { Tyrion, Ford, Sherlock };





            Tyrion.FullHouseFound();


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
            Console.WriteLine("In PlayHand");

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

        

        //not sure if I will actualy need to use this string to differentiate between rounds but seems more than likely
        public static void PlayRound(string round)
        {

            Tyrion.ChipsBetThisRound = 0;
            Ford.ChipsBetThisRound = 0;
            Sherlock.ChipsBetThisRound = 0;

            while (true)
            {                
                foreach (var player in Players)
                {                    
                    if (player.HasCards && player.Chips > 0)
                    {
                        //Console.Clear();
                        Console.WriteLine($"{player.Name}'s Turn");

                        
                        //Determine if check/call and bet/raise should be shown
                        //loop through each player and check if player.ChipsBetThisRound > 0. (Although, blinds could potentially throws a spanner in the works here depending on how I handle them. If I make blinds completely seperate to player.ChipsBetThisRound then I should be fine
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
                                player.Fold(player);
                                break;

                            case "Check":
                            case "CH":
                                player.Check(player);
                                break;

                            case "Call":
                            case "CA":
                                player.Call(player);
                                break;

                            case "Bet":
                            case "B":
                                player.Bet(player, Convert.ToInt32(value));
                                break;

                            case "Raise":
                            case "R":
                                player.Raise(player, Convert.ToInt32(value));
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
                Console.WriteLine($"The Winner of the hand is {PlayersInHand[0].Name}");
            }
            else
            {
                //var app = new App();
                var winner = Dealer.GetPlayerWithBestHand(PlayersInHand);
                Console.WriteLine($"The Winner of the hand is {winner.Name}");
            }


        }



    }
}
