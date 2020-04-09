
using System;
using System.Collections.Generic;
using System.Text;

namespace PokerApp
{   
    public static class Utils
    {
        //Not sure if this should be in Utils
        internal static void Shuffle<T>(this IList<T> list)
        {
            var rng = new Random();

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        internal static string GetUserInputtedCommand(string input)
        {
            var action = "";

            for (var i = 0; i < input.Length; i++)
            {
                if (Char.IsLetter(input[i]))
                {
                    action += input[i];
                }
            }

            return action;
        }

        internal static string GetUserInputtedValue(string input)
        {
            var value = "";

            for (var i = 0; i < input.Length; i++)
            {
                if (Char.IsDigit(input[i]))
                {
                    value = input.Substring(i, (input.Length - i));
                    break;
                }
            }

            return value;
        }
    }
}
