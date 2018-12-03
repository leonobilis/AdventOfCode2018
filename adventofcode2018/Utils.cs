using System;
using System.Collections.Generic;
using System.IO;

namespace adventofcode2018
{
    public static class Utils
    {
        public static void Print<T>(T arg) 
        {
            Console.WriteLine($"Part1: {arg}");
        }

        public static void Print<T1, T2>(T1 arg1, T2 arg2) 
        {
            Console.WriteLine($"Part1: {arg1}\nPart2: {arg2}");
        }

        public static IEnumerable<string> GetFromFile(int day) {
            return File.ReadAllLines(string.Format("adventofcode2018/day{0:00}/input.txt", day));
        }
    }
    
}
