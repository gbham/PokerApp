using System;
using static PokerApp.App;

namespace PokerApp
{
    static class Output
    {
        internal static void PrintGameInformation()
        {
            Console.Clear();
            Console.WriteLine($"------------------------------------");
            Console.WriteLine($"[{Board.CurrentPhase}] {Dealer.IllegalMoveErrorMsg} ");
            Console.WriteLine($"------------------------------------");
            Console.WriteLine($"\nChip Stacks:\n");

            if (!string.IsNullOrWhiteSpace(Dealer.IllegalMoveErrorMsg)) { Dealer.IllegalMoveErrorMsg = ""; }

            foreach (var player in Players)
            {
                Console.WriteLine($"[{player.Name}]'s chips: [{player.Chips}]");
            }
            Console.WriteLine($"\nHas Cards:\n");

            foreach (var player in Players)
            {
                Console.WriteLine($"[{player.Name}]: [{player.HasCards}]");
            }

            Console.WriteLine($"\nChips bet this round:\n");
            foreach (var player in Players)
            {
                Console.WriteLine($"[{player.Name}]: [{player.ChipsBetThisRound}]");
            }

            Console.WriteLine($"\nPot Value = [{Board.ChipsInPot}]\n");            

            PrintBoard();
            
        }

        //Will only print if the "board" actually has cards on it
        private static void PrintBoard()
        {
            var FlopOutput = $"The Board = [{ Board.FlopSlot1}] [{Board.FlopSlot2}] [{Board.FlopSlot3}]";
            var TurnOutput = $" [{Board.TurnSlot}]";
            var RiverOutput = $" [{Board.RiverSlot}]";
            var TheBoard = "";

            if (!string.IsNullOrWhiteSpace(Board.FlopSlot1)) { TheBoard += FlopOutput; }
            if (!string.IsNullOrWhiteSpace(Board.TurnSlot)) { TheBoard += TurnOutput; }
            if (!string.IsNullOrWhiteSpace(Board.RiverSlot)) { TheBoard += RiverOutput; }

            if (!string.IsNullOrWhiteSpace(TheBoard))
            {
                Console.WriteLine($"------------------------------------");
                Console.WriteLine($"{TheBoard}");
                Console.WriteLine($"------------------------------------");
            }
        }

        internal static void PrintPlayersTurnIsReady(Player player)
        {
            Console.WriteLine($"\n{player.Name}'s Turn");
        }

        internal static void PrintNewlyDealtCards(Player player)
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

        internal static void PrintPlayersMoveOptions(string MoveOptions)
        {
            Console.WriteLine($"{MoveOptions}\n");
        }

        internal static void PrintHandResult()
        {
            var PlayersInHand = Board.GetPlayersInHand();

            Console.Clear();
            //sometimes I use player.BestKicker and sometimes use player.ListOfKickers depending on the hand type, need to account for this when printing out the kicker            

            if (Dealer.IsSplitPot)
            {
                Console.WriteLine($"\nThe winnings of the pot are split between {Dealer.SplitPotPlayers.Count} players: \n");
                foreach (var player in Dealer.SplitPotPlayers)
                {
                    Console.WriteLine($"[{player.Name}]");
                }
            }
            else
            {
                Console.WriteLine($"\nThe Winner of the hand is {Dealer.HandWinner.Name}.");
            }

            Console.WriteLine($"\nThey have the hand type of: {Deck.PokerHandsList[Dealer.HandWinner.BestHandType]}. With the kicker of: {Dealer.HandWinner.ListOfKickers[0]} \n"); //

            Console.WriteLine("");

            PrintBoard();  
            
            Console.WriteLine("");

            foreach(var player in PlayersInHand)
            {
                Console.WriteLine($"{player.Name}'s cards were: [{player.CardOne}] [{player.CardTwo}] ");
            }  
        }

        internal static void PrintCardInformation(Player player)
        {
            Console.WriteLine($"[{player.Name}]'s cards are: [{player.CardOne}] [{player.CardTwo}]");
            Console.WriteLine($"\npress any button to hide cards...");
        }

        internal static void PrintGameOver()
        {
            Console.Clear();
            Console.WriteLine($"GAME OVER");
        }
    }
}
