using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerApp
{   
    static class Board
    {
        //test comment

        private static int chipsInPot = 0;        

        internal static string flopSlot1 ="";
        internal static string flopSlot2 ="";
        internal static string flopSlot3 ="";
        internal static string turnSlot ="";
        internal static string riverSlot ="";
        private static bool handIsLive;

        public static int ChipsInPot { get { return chipsInPot; } set { chipsInPot = value; } }

        public static string FlopSlot1 { get { return flopSlot1; } set { flopSlot1 = value; } }
        public static string FlopSlot2 { get { return flopSlot2; } set { flopSlot2 = value; } }
        public static string FlopSlot3 { get { return flopSlot3; } set { flopSlot3 = value; } }

        public static string TurnSlot { get { return turnSlot; } set { turnSlot = value; } }
        public static string RiverSlot { get { return riverSlot; } set { riverSlot = value; } }

        public static bool HandIsLive { get { return handIsLive; } set { handIsLive = value; } }       




        internal static List<Player> GetPlayersInHand()
        {
            var PlayersInTheHand = new List<Player>();

            foreach(var player in App.Players)
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

        internal static string GetMoveOptions()
        {
            foreach (var player in App.Players)
            {
                if (player.ChipsBetThisRound > 0)
                {
                    return "Fold, Call, Raise";
                }
            }

            return "Fold, Check, Bet";
        }

        internal static List<int> RemoveSuitsAndReplaceFaceCardsWithInts(List<string> fullBoardList)
        {
            //Removing all suits and face cards so calculations can be done against ints
            for (var i = 0; i < fullBoardList.Count; i++)
            {
                fullBoardList[i] = fullBoardList[i].Replace("H", "").Replace("D", "").Replace("S", "").Replace("C", "").Replace("A", "0").Replace("J", "11").Replace("Q", "12").Replace("K", "13");
            }

            var onlyNumbersList = fullBoardList.Select(int.Parse).ToList();

            return onlyNumbersList;
        }


        


    }
}