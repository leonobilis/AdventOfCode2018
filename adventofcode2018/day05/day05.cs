using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace adventofcode2018
{
    using static Utils;

    public class Day05
    {
        public static int p1(string input)
        {
            var list = new LinkedList<char>(input.ToCharArray());
            var change = true;
            var i = list.First;
            while(i != null)
            {
                change = false;
                if(i.Next !=null && Math.Abs(i.Value - i.Next.Value) == 32)
                {
                    list.Remove(i.Next);
                    change = true;
                }
                if(i.Previous !=null && Math.Abs(i.Value - i.Previous.Value) == 32)
                {
                    list.Remove(i.Previous);
                    change = true;
                }
                if(change)
                {
                    list.Remove(i);
                    i = list.First;
                }
                else
                {
                    i = i.Next;
                }
            }
            return list.Count();
        }

        public static int p2(string input)
        {           
            var min = Int32.MaxValue;

            foreach(var c in Enumerable.Range(97, 25))
            {
                var list = new LinkedList<char>(input.Where(c2 => c2!=c && c2!=c-32));
                var change = true;
                var i = list.First;
                
                while(i != null)
                {
                    change = false;
                    if(i.Next != null && Math.Abs(i.Value - i.Next.Value) == 32)
                    {
                        list.Remove(i.Next);
                        change = true;
                    }
                    if(i.Previous != null && Math.Abs(i.Value - i.Previous.Value) == 32)
                    {
                        list.Remove(i.Previous);
                        change = true;
                    }
                    if(change)
                    {
                        list.Remove(i);
                        i = list.First;
                    }
                    else
                    {
                        i = i.Next;
                    }
                }
                min = Math.Min(min, list.Count());
            }
            return min;
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