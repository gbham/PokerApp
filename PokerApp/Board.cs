using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static PokerApp.App;

namespace PokerApp
{   
    static class Board
    {
        private static int chipsInPot = 0;

        private static int totalAmountOfChipsInPlay = Player.StartingStack * App.Players.Count;

        private static string flopSlot1;
        private static string flopSlot2;
        private static string flopSlot3;
        private static string turnSlot;
        private static string riverSlot;
        private static bool handIsLive;
        private static string currentPhase = "";
        

        public static int TotalAmountOfChipsInPlay { get { return totalAmountOfChipsInPlay; } set { totalAmountOfChipsInPlay = value; } }
        public static int ChipsInPot { get { return chipsInPot; } set { chipsInPot = value; } }

        public static string FlopSlot1 { get { return flopSlot1; } set { flopSlot1 = value; } }
        public static string FlopSlot2 { get { return flopSlot2; } set { flopSlot2 = value; } }
        public static string FlopSlot3 { get { return flopSlot3; } set { flopSlot3 = value; } }
        public static string TurnSlot { get { return turnSlot; }   set { turnSlot = value; } }
        public static string RiverSlot { get { return riverSlot; } set { riverSlot = value; } }
        public static bool HandIsLive { get { return handIsLive; } set { handIsLive = value; } }
        public static string CurrentPhase { get { return currentPhase; } set { currentPhase = value; } }

        

        internal static List<Player> GetPlayersInHand()
        {
            var PlayersInTheHand = new List<Player>();

            foreach(var player in Players)
            {
                if(player.HasCards)
                {
                    PlayersInTheHand.Add(player);
                }                
            }

            return PlayersInTheHand;
        }

        internal static bool IsHandLive()
        {
            var numberOfPlayersInHand = GetPlayersInHand().Count;

            if(numberOfPlayersInHand < 2)
            {
                return false;
            }

            return true;
        }
        
    }
}