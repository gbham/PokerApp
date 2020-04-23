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
            Ford = new Player("Ford");
            Tyrion = new Player("Tyrion");
            Sherlock = new Player("Sherlock");

            Players = new List<Player>() { Ford, Tyrion, Sherlock }; // 




            //This will have to come after Dealer.GetNumberOfPlayersAndNames();
          



            //Ford.HasCards = true;
            //Tyrion.HasCards = true;
            //Sherlock.HasCards = true;


            //Ford.CardOne = "JC";
            //Ford.CardTwo = "2D";

            //Tyrion.CardOne = "JC";
            //Tyrion.CardTwo = "2D";

            //Sherlock.CardOne = "6D";
            //Sherlock.CardTwo = "2H";


            //Board.FlopSlot1 = "9C";
            //Board.FlopSlot2 = "10C";
            //Board.FlopSlot3 = "JC";
            //Board.TurnSlot = "9C";
            //Board.RiverSlot = "10C";

            //DetermineWinnerOfHand();


            //Dealer.GetNumberOfPlayersAndNames();

            while (!GameOver)
            {
                Dealer.DealHoleCards();

                PlayHand();

                Dealer.CheckForGameWinner();
            }
        }


        public static void PlayHand()
        {
            Dealer.TakeTheBlinds();            

            PlayPhase("PreFlop");

            if (Board.HandIsLive)
            {
                Dealer.DealFlop();
                PlayPhase("Flop");
            }

            if (Board.HandIsLive)
            {
                Dealer.DealTurn();
                PlayPhase("Turn");
            }

            if (Board.HandIsLive)
            {
                Dealer.DealRiver();
                PlayPhase("River");
            }

            DetermineWinnerOfHand();

        }

        

        public static void PlayPhase(string round) //not sure if I will actualy need to use this string to differentiate between rounds but seems more than likely
        {
            foreach(var player in Players)
            {
                player.ChipsBetThisRound = 0;
                player.ChipsNeededToCall = 0; //not sure that ChipsNeededToCall needs to be reset here anymore. some changes may have made this redundant.
            }

            while (true)
            {
                //would be nicer to use a foreach loop but I need to be able to incrememnt once back in the loop if the user wants to show/hide their cards or if they try an illegal move
                for (var i = 0; i < Players.Count; i++)
                {
                    var PlayersInHand = Board.GetPlayersInHand();
                                                                                            //Dealer.IsActionOver() does what i want but i think the only problem is telling the difference between the start of a hand and everyone checking
                    if (Players[i].HasCards && Players[i].Chips > 0 && PlayersInHand.Count > 1 ) 
                    {                      

                        Output.PrintGameInformation();

                        Output.PrintPlayersTurnIsReady(Players[i]);

                        //blinds could potentially throws a spanner in the works for GetMoveOPtions depending on how I handle them. If I make blinds completely seperate to player.ChipsBetThisRound then I should be fine. - Actually, I might want to include them in player.ChipsBetThisRound. Now I think about it, they will need to be taken into consideration 

                        var moveOptions = Players[i].GetMoveOptions();

                        Output.PrintPlayersMoveOptions(moveOptions);

                        //Need to add some instructions on format of user input and a help menu at some point
                        var input = Console.ReadLine();

                        Players[i].LastMove = Utils.GetUserInputtedCommand(input);
                        var value = Utils.GetUserInputtedValue(input);

                        if (Players[i].IsIllegalMoveUsed(moveOptions, value)) { i -= 1; }
                        else
                        {
                            switch (Players[i].LastMove)
                            {
                                case "fold":
                                    Players[i].Fold();
                                    break;

                                case "check":
                                case "c":
                                    Players[i].Check();
                                    break;

                                case "call":                                
                                    Players[i].Call();
                                    break;

                                case "bet":                                
                                    Players[i].Bet(Convert.ToInt32(value));
                                    break;

                                case "raise":                                
                                    Players[i].Raise(Convert.ToInt32(value));
                                    break;

                                case "all":
                                    Players[i].AllIn();
                                    break;

                                case "show":                               
                                    Players[i].RevealCards();
                                    i -= 1;
                                    break;

                                //Need to repeat this stage of the for loop if no valid move is made
                                default:
                                    i -= 1;
                                    break;
                            }
                        }
                    }

                    //If betting for the round is over then break. It is similiar to Dealer.IsActionOver() but they serve distinct purposes. For example, within IsActionOver(), if all players have bet 0 chips this round then that is fine, it means everyone has checked. However, if I tried to run the same code inside the for loop, the code would break out the for loop prematurely if the first player in the loop checks. IsActionOver() handles stuff to do with All-ins and other conditions but PlayersHaveBetEqualAmounts() can be simplier, since for example, if the player is all-in then they wont be able to read the code inside the for loop, then once the for loop is over, IsActionOver() can be executed
                    if (Dealer.PlayersHaveBetEqualAmounts()) { break; }
                }

                //Checks first if only one person has cards left THEN if the betting for the round is done - both are reasons to back out of the while loop (the current round, e.g the flop )
                if (Dealer.IsActionOver()) { break; }
            }
        }

        private static void DetermineWinnerOfHand()
        {
            var PlayersInHand = Board.GetPlayersInHand();

            if (PlayersInHand.Count < 2)
            {           
                Dealer.HandWinner = PlayersInHand[0];
                Dealer.HandWinner.BestHandType = Dealer.HandWinner.GetStrongestHandType();
                Output.PrintHandResult();
            }
            else
            {
                Dealer.DeterminePlayerWithBestHand();
                Output.PrintHandResult();
            }

            AssignChipsToWinnerOfHand();

            Console.WriteLine($"\nPress any button to continue to next hand...");
            Console.ReadKey();            
        }

        private static void AssignChipsToWinnerOfHand()
        {
            if(Dealer.IsSplitPot)
            {
                foreach(var player in Dealer.SplitPotPlayers)
                {
                    player.Chips += Board.ChipsInPot / Dealer.SplitPotPlayers.Count;
                }
            }
            else
            {
                Dealer.HandWinner.Chips += Board.ChipsInPot;
            }
           
            Board.ChipsInPot = 0;
        }
    }
}
