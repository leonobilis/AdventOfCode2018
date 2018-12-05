using System;
using System.Collections.Generic;
using System.Linq;

namespace adventofcode2018
{
    using static Utils;

    public static class Day05
    {
        public static int ReducePolymers(this IEnumerable<char> input)
        {
            var list = new LinkedList<char>(input);
            var i = list.First;
            while (i != null)
            {
                if (i.Next != null && Math.Abs(i.Value - i.Next.Value) == 32)
                {
                    list.Remove(i.Next);
                    var next = i.Previous ?? i.Next;
                    list.Remove(i);
                    i = next;
                    continue;
                }
                i = i.Next;
            }
            return list.Count();
        }

        public static int p1(string input)
        {
            return input.ReducePolymers();
        }

        public static int p2(string input)
        {
            return Enumerable.Range(97, 25).AsParallel()
                                           .Select(c => input.Where(c2 => c2 != c && c2 != c - 32)
                                                              .ReducePolymers())
                                           .Min();
        }


        
        public static void Solution()
        {
             // var testInput = "dabAcCaCBAcCcaDA";
             // Console.WriteLine(Day05.p1(testInput));
             // Console.WriteLine(Day05.p2(testInput));

            var input = GetFromFile(5);
            Print(p1(input.First()), p2(input.First()));
        }
    }
}