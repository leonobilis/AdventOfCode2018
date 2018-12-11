using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace adventofcode2018
{
    using static Utils;
    
    public static class Day11
    {
        public static ValueTuple<int, int> TotalPower(int serialNumber)
        {
            var grid = Enumerable.Range(1, 300)
                                 .SelectMany(y => Enumerable.Range(1, 300)
                                                            .Select(x => new {k = (x, y), v = (((x+10) * y + serialNumber)*(x+10)).ToString()})
                                                            .Select(s => new {s.k, v = s.v.Length > 3 ? Int32.Parse(s.v[s.v.Length - 3].ToString()) - 5 : -5}))
                                 .ToDictionary(x => x.k, x => x.v);

            return grid.Where(x => x.Key.Item1 < 298 && x.Key.Item2 < 298)
                       .Select(s => new { k = s.Key, v = s.Value + grid[(s.Key.Item1+1, s.Key.Item2)] + grid[(s.Key.Item1+2, s.Key.Item2)]
                                    + grid[(s.Key.Item1, s.Key.Item2+1)] + grid[(s.Key.Item1+1, s.Key.Item2+1)] + grid[(s.Key.Item1+2, s.Key.Item2+1)]
                                    + grid[(s.Key.Item1, s.Key.Item2+2)] + grid[(s.Key.Item1+1, s.Key.Item2+2)] + grid[(s.Key.Item1+2, s.Key.Item2+2)]})
                       .OrderByDescending(o => o.v).First().k;
        }

        public static int calcTotalPower(Dictionary<(int, int), int> grid, ValueTuple<int, int> coordinate, int size)
        {
            return Enumerable.Range(coordinate.Item1, size)
                      .SelectMany(x => Enumerable.Range(coordinate.Item2, size)
                                                 .Select(y => grid[(x, y)]))
                      .Aggregate(0, (acc, g) => acc + g);
        }
        public static ValueTuple<int, int, int> TotalPower2(int serialNumber)
        {
            var grid = Enumerable.Range(1, 300)
                                 .SelectMany(y => Enumerable.Range(1, 300)
                                                            .Select(x => new {k = (x, y), v = (((x+10)*y + serialNumber)*(x+10)).ToString()})
                                                            .Select(s => new {s.k, v = s.v.Length > 3 ? Int32.Parse(s.v[s.v.Length - 3].ToString()) - 5 : -5}))
                                 .ToDictionary(x => x.k, x => x.v);

            var max = Enumerable.Range(1, 300)
                                .AsParallel()
                                .Select(s => grid.Where(x => x.Key.Item1 <= 301-s && x.Key.Item2 <= 301-s)
                                                 .Select(s2 => new { k = s2.Key, size = s,  v = calcTotalPower(grid, s2.Key, s)})
                                                 .Aggregate((acc, g) => g.v > acc.v ? g : acc))
                                .Aggregate((acc, g) => g.v > acc.v ? g : acc);

            return (max.k.Item1, max.k.Item2, max.size);
        }

        public static void Solution()
        {
            //Console.WriteLine(Day11.TotalPower(57));

            Print(TotalPower(9445), TotalPower2(9445));
        }
    }
}
