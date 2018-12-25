using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace adventofcode2018
{
    using static Utils;

    public static class Day25
    {
        static int Dist((int a, int b, int c, int d) x, (int a, int b, int c, int d) y)
        {
            return Math.Abs(x.a - y.a) + Math.Abs(x.b - y.b) + Math.Abs(x.c - y.c) + Math.Abs(x.d - y.d);
        }

        static HashSet<(int a, int b, int c, int d)> GetPoints(IEnumerable<string> input)
        {
            Regex inputRx = new Regex(@"([-\d]+),([-\d]+),([-\d]+),([-\d]+)", RegexOptions.Compiled);
            return input.Select(s => inputRx.Matches(s).Select(m => m.Groups).First())
                        .Select(s => (Int32.Parse(s[1].Value), Int32.Parse(s[2].Value), Int32.Parse(s[3].Value), Int32.Parse(s[4].Value)))
                        .ToHashSet();
        }

        public static int CountConstellations(IEnumerable<string> input)
        {
            var points = GetPoints(input);
            var counter = 0;

            for (; points.Count > 0; ++counter)
            {
                var constelation = points.Where(x => Dist(x, points.First()) <= 3).ToHashSet();
                points.ExceptWith(constelation);
                for (var newConstelation = new HashSet<(int a, int b, int c, int d)>(); constelation.Count > 0; constelation = newConstelation, newConstelation = new HashSet<(int a, int b, int c, int d)>())
                    foreach (var p in constelation)
                    {
                        var pointsToCOnstelation = points.Where(x => Dist(x, p) <= 3).ToList();
                        newConstelation.UnionWith(pointsToCOnstelation);
                        points.ExceptWith(pointsToCOnstelation);
                    }
            }

            return counter;
        }

        public static void Solution()
        {
//            var testInput = @"-1,2,2,0
//0,0,2,-2
//0,0,0,-2
//-1,2,0,0
//-2,-2,-2,2
//3,0,2,-1
//-1,3,2,2
//-1,0,-1,0
//0,2,1,-2
//3,0,0,0".Split("\n");
//            Console.WriteLine(CountConstellations(testInput));

//            var testInput2 = @"1,-1,0,1
//2,0,-1,0
//3,2,-1,0
//0,0,3,1
//0,0,-1,-1
//2,3,-2,0
//-2,2,0,0
//2,-2,0,-1
//1,-1,0,-1
//3,2,0,2".Split("\n");
//            Console.WriteLine(CountConstellations(testInput2));

            var input = GetFromFile(25);
            Print(CountConstellations(input));
        }
    }
}