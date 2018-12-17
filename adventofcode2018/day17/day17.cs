using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace adventofcode2018
{
    using static Utils;

    public static class Day17
    {
        public static IEnumerable<(int, int)> GetClay(GroupCollection match)
        {
            if (match[1].Value == "x")
            {
                var x = Int32.Parse(match[2].Value);
                var y = (Int32.Parse(match[3].Value), Int32.Parse(match[4].Value));
                return Enumerable.Range(y.Item1, y.Item2 - y.Item1 + 1)
                                 .Select(s => (x, s));
            }
            else
            {
                var y = Int32.Parse(match[2].Value);
                var x = (Int32.Parse(match[3].Value), Int32.Parse(match[4].Value));
                return Enumerable.Range(x.Item1, x.Item2 - x.Item1 + 1)
                                 .Select(s => (s, y));
            }
        }

        public static int CountWaterTiles(IEnumerable<string> input)
        {
            Regex inputRx = new Regex(@"([xy])=([\d]+), [xy]=([\d]+)..([\d]+)", RegexOptions.Compiled);
            
            var clay = input.Select(s => inputRx.Matches(s).Select(m => m.Groups).First())
                            .SelectMany(s => GetClay(s))
                            .GroupBy(g => g.Item2)
                            .ToDictionary(k => k.Key, v => v.Select (s => s.Item1).OrderBy(o => o).ToList());
            
            var maxY = clay.Keys.Max() + 1;
            var minX = clay.Select(s => s.Value.Min()).Min() - 1;
            var maxX = clay.Select(s => s.Value.Max()).Max() + 1;

            var water = new HashSet<(int, int)>();
            var sources = new HashSet<(int, int)> { (500, 0) };
            var sourcesComputed = new HashSet<(int, int)> { (500, 0) };

            while (sources.Count > 0)
            {
                var newSources = new HashSet<(int, int)>();
                //var result = sources.AsParallel()
                //       .Select(s => ComputeSource(s, clay, water, maxY, minX, maxX)).ToList();
                //.Aggregate((newSources, newWater), (acc, x) => (acc.Item1.Union(x.Item1), acc.Item2.Union(x.Item2)));

                foreach (var source in sources)
                {
                    var result = ComputeSource(source, clay, water, maxY, minX, maxX);
                    newSources.UnionWith(result.Item1);
                    sourcesComputed.UnionWith(sources);
                    water.UnionWith(result.Item2);
                    
                }
                sources = newSources.Except(sourcesComputed).ToHashSet();
                if (water.Count > 42000)
                  PrintResult(clay, minX, water);

            }


            //for (int i = 0; i < maxY; ++i)
            //{
            //    Console.WriteLine();
            //    for (int j = minX; j < maxX; ++j)
            //    {
            //        if (clay.Contains((j, i)))
            //            Console.Write('#');
            //        else if (water.Contains((j, i)))
            //            Console.Write('O');
            //        else
            //            Console.Write(' ');

            //    }
            //}


            return water.Count();
        }

        private static void PrintResult(Dictionary<int, List<int>> clay, int minX, HashSet<(int, int)> water)
        {
            for (int i = water.Max(m => m.Item2) - 200; i < water.Max(m => m.Item2) + 15; ++i)
            {
                Console.WriteLine();
                for (int j = minX; j < water.Max(m => m.Item1) + 5; ++j)
                {
                    if (clay.ContainsKey(i) && clay[i].Contains(j))
                        Console.Write('#');
                    else if (water.Contains((j, i)))
                        Console.Write('O');
                    else
                        Console.Write(' ');

                }
            }
            Console.WriteLine();
            Console.ReadKey();
        }

        private static (List<(int, int)>, HashSet<(int, int)>) ComputeSource((int, int) source, Dictionary<int, List<int>> clay, HashSet<(int, int)> water, int maxY, int minX, int maxX)
        {
            var newSources = new List<(int, int)>();
            var sourceEnd = clay.Where(x => x.Key > source.Item2 && x.Value.Contains(source.Item1)).Select(s => s.Key)/*.Union(water.Where(x => x.Item1 == source.Item1 && x.Item2 > source.Item2).Select(s => s.Item2)).*/.DefaultIfEmpty(maxY).Min();
            { }
            var newWater = Enumerable.Range(source.Item2 + 1, sourceEnd - source.Item2 - 1).Select(s => (source.Item1, s)).ToHashSet(); // ToDictionary(k => k, v => new List<int> { source.Item1 });

            if (clay.ContainsKey(sourceEnd))
            {
                bool noHole = true;
                for (var current = (source.Item1, sourceEnd - 1); noHole; current.Item2--)
                {
                    //var clayAndWater = clay.GetValueOrDefault(current.Item2, new List<int>()).Union(water.Where(x => x.Item2 == current.Item2).Select(s => s.Item1));
                    var left = clay.GetValueOrDefault(current.Item2, new List<int>()).Where(c => c < current.Item1).DefaultIfEmpty(minX).Max();
                    var right = clay.GetValueOrDefault(current.Item2, new List<int>()).Where(c => c > current.Item1).DefaultIfEmpty(maxX).Min();
                    left++;
                    right--;

                    //clayAndWater = clay.GetValueOrDefault(current.Item2 + 1, new List<int>()).Union(water.Where(x => x.Item2 == current.Item2 + 1).Select(s => s.Item1));

                    if (clay.ContainsKey(current.Item2 + 1))
                    {

                        var leftHole = clay[current.Item2 + 1].Where((c, i) => c <= current.Item1 && c >= left && !clay[current.Item2 + 1].Contains(c - 1)).DefaultIfEmpty(minX).Max();
                        if (leftHole > left)
                        {
                            newSources.Add((leftHole - 1, current.Item2));
                            newWater.Add((leftHole - 1, current.Item2));
                            left = leftHole - 1;
                            if (current.Item2 != source.Item2)
                            {
                                noHole = false;
                            }
                        }
                        var rightHole = clay[current.Item2 + 1].Where(c => c >= current.Item1 && c <= right && !clay[current.Item2 + 1].Contains(c + 1)).DefaultIfEmpty(maxX).Min();
                        if (rightHole < right)
                        {
                            newSources.Add((rightHole + 1, current.Item2));
                            newWater.Add((rightHole + 1, current.Item2));
                            if (current.Item2 != source.Item2)
                            {
                                right = rightHole + 1;
                                noHole = false;
                            }
                        }
                    }

                    newWater.UnionWith(Enumerable.Range(left, right - left + 1).Select(s => (s, current.Item2)));
                }
            }

            return (newSources, newWater);
        }

        public static void Solution()
        {
            var testInput = @"x=495, y=2..7
y=7, x=495..501
x=501, y=3..7
x=498, y=2..4
x=506, y=1..2
x=498, y=10..13
x=504, y=10..13
y=13, x=498..504".Split("\n");
              Console.WriteLine(CountWaterTiles(testInput));

            var input = GetFromFile(17);
            Print(CountWaterTiles(input));
        }
    }
}