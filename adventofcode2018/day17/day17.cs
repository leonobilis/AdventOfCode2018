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

        public static (int, int) CountWaterTiles(IEnumerable<string> input)
        {
            Regex inputRx = new Regex(@"([xy])=([\d]+), [xy]=([\d]+)..([\d]+)", RegexOptions.Compiled);
            
            var clay = input.Select(s => inputRx.Matches(s).Select(m => m.Groups).First())
                            .SelectMany(s => GetClay(s))
                            .GroupBy(g => g.Item2)
                            .ToDictionary(k => k.Key, v => v.Select (s => s.Item1).OrderBy(o => o).ToList());
            
            var maxY = clay.Keys.Max() + 1;
            var minY = clay.Keys.Min() - 1;
            var minX = clay.Select(s => s.Value.Min()).Min() - 2;
            var maxX = clay.Select(s => s.Value.Max()).Max() + 2;

            var water = new HashSet<(int, int)>();
            var spring = new HashSet<(int, int)>();
            var sources = new HashSet<(int, int)> { (500, minY) };
            var sourcesComputed = new HashSet<(int, int)> { (500, 0) };

            while (sources.Count > 0)
            {
                var newSources = new HashSet<(int, int)>();
                (newSources, water, spring) = sources.Select(s => ComputeSource(s, clay, water, maxY, minX, maxX))
                                                     .Aggregate((newSources, water, spring), (acc, x) => (acc.Item1.Union(x.Item1).ToHashSet(),
                                                                                                          acc.Item2.Union(x.Item2).ToHashSet(),
                                                                                                          acc.Item3.Union(x.Item3).Except(acc.Item2).ToHashSet()));

                sourcesComputed.UnionWith(sources);
                sources = newSources.Except(sourcesComputed).ToHashSet();

            }

            return (water.Count + spring.Count, water.Count);
        }

        private static (HashSet<(int, int)>, HashSet<(int, int)>, HashSet<(int, int)>) ComputeSource((int, int) source, Dictionary<int, List<int>> clay, HashSet<(int, int)> water, int maxY, int minX, int maxX)
        {
            var newSources = new HashSet<(int, int)>();
            var sourceEnd = clay.Where(x => x.Key > source.Item2 && x.Value.Contains(source.Item1)).Select(s => s.Key).DefaultIfEmpty(maxY).Min();
            var spring = Enumerable.Range(source.Item2 + 1, sourceEnd - source.Item2 - 1).Select(s => (source.Item1, s)).ToHashSet();
            
            var newWater = new HashSet<(int, int)>();

            if (clay.ContainsKey(sourceEnd))
            {
                bool noHole = true;
                for (var current = (source.Item1, sourceEnd - 1); noHole; current.Item2--)
                {
                    var left = clay.GetValueOrDefault(current.Item2, new List<int>()).Where(c => c < current.Item1).DefaultIfEmpty(minX).Max() + 1;
                    var right = clay.GetValueOrDefault(current.Item2, new List<int>()).Where(c => c > current.Item1).DefaultIfEmpty(maxX).Min() - 1;

                    if (clay.ContainsKey(current.Item2 + 1))
                    {
                        var leftHole = clay[current.Item2 + 1].Where((c, i) => c <= current.Item1 && c >= left && !clay[current.Item2 + 1].Contains(c - 1)).DefaultIfEmpty(minX).Max();
                        if (leftHole > left)
                        {
                            newSources.Add((leftHole - 1, current.Item2));
                            spring.Add((leftHole - 1, current.Item2));
                            if (current.Item2 != source.Item2)
                            {
                                left = leftHole - 1;
                                noHole = false;
                            }
                        }
                        var rightHole = clay[current.Item2 + 1].Where(c => c >= current.Item1 && c <= right && !clay[current.Item2 + 1].Contains(c + 1)).DefaultIfEmpty(maxX).Min();
                        if (rightHole < right)
                        {
                            newSources.Add((rightHole + 1, current.Item2));
                            spring.Add((rightHole + 1, current.Item2));
                            if (current.Item2 != source.Item2)
                            {
                                right = rightHole + 1;
                                noHole = false;
                            }
                        }
                    }

                    if (noHole)
                        newWater.UnionWith(Enumerable.Range(left, right - left + 1).Select(s => (s, current.Item2)));
                    else
                        spring.UnionWith(Enumerable.Range(left, right - left + 1).Select(s => (s, current.Item2)));
                }
            }

            return (newSources, newWater, spring);
        }

        public static void Solution()
        {
//             var testInput = @"x=495, y=2..7
// y=7, x=495..501
// x=501, y=3..7
// x=498, y=2..4
// x=506, y=1..2
// x=498, y=10..13
// x=504, y=10..13
// y=13, x=498..504".Split("\n");
//               Console.WriteLine($"{CountWaterTiles(testInput).Item1}, {CountWaterTiles(testInput).Item2}");

            var input = GetFromFile(17);
            var result = CountWaterTiles(input);
            Print(result.Item1, result.Item2);
        }
    }
}