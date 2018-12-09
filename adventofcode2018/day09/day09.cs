using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace adventofcode2018
{
    using static Utils;
    
    public static class Day09
    {
        static LinkedListNode<T> CircularNext<T>(this LinkedListNode<T> current)
        {
            return current.Next ?? current.List.First;
        }

        static LinkedListNode<T> CircularPrevious<T>(this LinkedListNode<T> current)
        {
            return current.Previous ?? current.List.Last;
        }

        public static long HighScore(int numPlayers, int lastMarable)
        {
            var players = Enumerable.Range(0, numPlayers).ToDictionary(k => k, v => 0L);
            var circle = new LinkedList<int>();
            var currentMarable = circle.AddFirst(0);
            
            foreach (var i in Enumerable.Range(1, lastMarable))
            {
                if(i % 23 == 0)
                {
                    var toRemove = Enumerable.Range(0, 7).Aggregate(currentMarable, (acc, _) => acc.CircularPrevious());
                    players[i%players.Count] += i + toRemove.Value;
                    currentMarable = toRemove.CircularNext();
                    circle.Remove(toRemove);
                }
                else
                {
                    currentMarable = currentMarable.CircularNext();
                    currentMarable = circle.AddAfter(currentMarable, i);
                }
            }

            return players.Values.Max();
        }

        public static void Solution()
        {
            // Console.WriteLine(Day09.HighScore(9, 25));
            // Console.WriteLine(Day09.HighScore(10, 1618));
            // Console.WriteLine(Day09.HighScore(13, 7999));
            // Console.WriteLine(Day09.HighScore(17, 1104));
            // Console.WriteLine(Day09.HighScore(21, 6111));
            // Console.WriteLine(Day09.HighScore(30, 5807));

            Print(HighScore(452, 70784), HighScore(452, 70784*100));
        }
    }
}