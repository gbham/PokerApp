using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerApp
{
    class App
    {
        internal static bool GameOver = false;

        private static Player Tyrion;
        private static Player Ford;
        private static Player Sherlock;
        private static Player player4;
        private static Player player5;

        public static List<Player> Players;
        public static List<Player> PlayersOrderOfAction; //Position 1 is the player Under the Gun (UTG), and the final 2 positions in the list will be the players with the small and big blind

        public static void Main()
        {
            Ford = new Player("Ford");
            Tyrion = new Player("Tyrion");
            Sherlock = new Player("Sherlock");
            player4 = new Player("player4");
            player5 = new Player("player5");

            Players = new List<Player>() { Ford, Tyrion, Sherlock, player4, player5 };
            PlayersOrderOfAction = new List<Player>(Players) { };

            

            
            
            
        





            //Ford.HasCards = true;
            //Tyrion.HasCards = true;
            //Sherlock.HasCards = true;

            //Ford.CardOne = "JD";
            //Ford.CardTwo = "JH";

            //Tyrion.CardOne = "10C";
            //Tyrion.CardTwo = "2D";

            //Sherlock.CardOne = "5D";
            //Sherlock.CardTwo = "2H";

            //Board.FlopSlot1 = "9C";
            //Board.FlopSlot2 = "6C";
            //Board.FlopSlot3 = "6C";
            //Board.TurnSlot = "9H";
            //Board.RiverSlot = "10H";

            //DetermineWinnerOfHand();





            //Dealer.GetNumberOfPlayersAndNames();

            while (!GameOver)
            {
                Dealer.DealHoleCards();

                Dealer.TakeTheBlinds();                

                PlayHand();

                Dealer.CheckForGameWinner();
            }
        }


        public static void PlayHand()
        {
            Dealer.DetermineUpdatedOrderOfAction();

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
                for (var i = 0; i < PlayersOrderOfAction.Count; i++)
                {
                    var PlayersInHand = Board.GetPlayersInHand();
                                                                                            //Dealer.IsActionOver() does what i want but i think the only problem is telling the difference between the start of a hand and everyone checking
                    if (PlayersOrderOfAction[i].HasCards && PlayersOrderOfAction[i].Chips > 0 && PlayersInHand.Count > 1 ) 
                    {                      
                        Output.PrintGameInformation();

                        Output.PrintPlayersTurnIsReady(PlayersOrderOfAction[i]);

                        var moveOptions = PlayersOrderOfAction[i].GetMoveOptions();

                        Output.PrintPlayersMoveOptions(moveOptions);

                        //Need to add some instructions on format of user input and a help menu at some point
                        var input = Console.ReadLine();

                        PlayersOrderOfAction[i].LastMove = Utils.GetUserInputtedCommand(input);
                        var value = Utils.GetUserInputtedValue(input);

                        if (PlayersOrderOfAction[i].IsIllegalMoveUsed(moveOptions, value)) { i -= 1; }
                        else
                        {
                            switch (PlayersOrderOfAction[i].LastMove)
                            {
                                case "fold":
                                    PlayersOrderOfAction[i].Fold();
                                    break;

                                case "check":
                                case "c":
                                    PlayersOrderOfAction[i].Check();
                                    break;

                                case "call":
                                    PlayersOrderOfAction[i].Call();
                                    break;

                                case "bet":
                                    PlayersOrderOfAction[i].Bet(Convert.ToInt32(value));
                                    break;

                                case "raise":
                                    PlayersOrderOfAction[i].Raise(Convert.ToInt32(value));
                                    break;

                                case "all":
                                    PlayersOrderOfAction[i].AllIn();
                                    break;

                                case "show":
                                    PlayersOrderOfAction[i].RevealCards();
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

            Console.WriteLine($"\nPress any key to continue...");
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
