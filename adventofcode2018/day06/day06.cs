using System;
using System.Collections.Generic;
using System.Linq;

namespace adventofcode2018
{
    using static Utils;

    public static class Day06
    {
        static int Dist(ValueTuple<int, int> a, ValueTuple<int, int> b)
        {
            return Math.Abs(a.Item1 - b.Item1) + Math.Abs(a.Item2 - b.Item2);
        }
        
        public static int p1(IEnumerable<string> input)
        {
            var points = input.Select(s => s.Split(", "))
                              .Select(s => (Int32.Parse(s[0]), Int32.Parse(s[1])));

            var maxX = points.Select(s => s.Item1).Max();
            var minX = points.Select(s => s.Item1).Min();
            var maxY = points.Select(s => s.Item2).Max();
            var minY = points.Select(s => s.Item2).Min();

            return Enumerable.Range(minX, maxX-minX)
                             .SelectMany(s => Enumerable.Range(minY, maxY-minY)
                                                    .Select(s2 => (s, s2)))
                             .AsParallel()
                             .Select(s => new {Key = s, Value = points.Select(s2 => new {Point = s2, Dist = Dist(s, s2)})
                                                            .OrderBy(o => o.Dist)})
                             .Where(x => x.Value.First().Dist != x.Value.Skip(1).First().Dist)
                             .Select(s => new {s.Key, Value = s.Value.First().Point})
                             .Where(x => x.Value.Item1 > minX && x.Value.Item1 < maxX && x.Value.Item2 > minY && x.Value.Item2 < maxY)
                             .GroupBy(g => g.Value)
                             .Select(s => s.Count())
                             .Max();
        }

        public static int p2(IEnumerable<string> input, int desiredDist)
        {
            var points = input.Select(s => s.Split(", "))
                              .Select(s => (Int32.Parse(s[0]), Int32.Parse(s[1])));

            var maxX = points.Select(s => s.Item1).Max();
            var minX = points.Select(s => s.Item1).Min();
            var maxY = points.Select(s => s.Item2).Max();
            var minY = points.Select(s => s.Item2).Min();

            return Enumerable.Range(minX, maxX-minX)
                             .SelectMany(s => Enumerable.Range(minY, maxY-minY)
                                                        .Select(s2 => (s, s2)))
                             .AsParallel()
                             .Select(s => points.Select(s2 => Dist(s, s2)).Sum())
                             .Where(x => x < desiredDist)
                             .Count();
        }

        public static void Solution()
        {
//             var testInput = @"1, 1
// 1, 6
// 8, 3
// 3, 4
// 5, 5
// 8, 9".Split("\n");
//             Console.WriteLine(Day06.p1(testInput));
//             Console.WriteLine(Day06.p2(testInput, 32));

            var input = GetFromFile(6);
            Print(p1(input), p2(input, 10000));
        }
    }
}