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
                            .ToHashSet();
            
            var maxY = clay.Max(m => m.Item2) + 1;
            var minX = clay.Min(m => m.Item1) - 1;
            var maxX = clay.Max(m => m.Item1) - 1;
            
            var water = new HashSet<(int, int)>();
            var sources = new List<(int, int)> {(500, 0)};

            while(sources.Count > 0)
            {
                var newSources = new List<(int, int)>();
                
                foreach(var source in sources)
                {
                    var sourceEnd = clay.Where(x => x.Item1 == source.Item1 && x.Item2 > source.Item2).DefaultIfEmpty((source.Item1, maxY)).Min();
                    {}
                    water.UnionWith(Enumerable.Range(source.Item2+1, sourceEnd.Item2-source.Item2-1).Select(s => (source.Item1, s)));

                    if(clay.Contains(sourceEnd))
                    {
                        bool noHole = true;
                        var clayAndWater = clay.Union(water);
                        for(var current = (sourceEnd.Item1, sourceEnd.Item2-1); noHole; current.Item2--)
                        {
                            var left =  clayAndWater.Where(c => c.Item2 == current.Item2 && c.Item1 < current.Item1).DefaultIfEmpty((minX, current.Item2)).Max();
                            var right = clayAndWater.Where(c => c.Item2 == current.Item2 && c.Item1 > current.Item1).DefaultIfEmpty((maxX, current.Item2)).Min();
                            left.Item1++;
                            right.Item1--;

                            var leftHole = clayAndWater.Where(c => c.Item2 == current.Item2 + 1 && c.Item1 < current.Item1 && c.Item1 >= left.Item1).DefaultIfEmpty((minX, current.Item2 + 1)).Min();
                            if (leftHole.Item1 > left.Item1)
                            {
                                newSources.Add((leftHole.Item1 - 1, leftHole.Item2 - 1));
                                water.Add((leftHole.Item1 - 1, leftHole.Item2 - 1));
                                left.Item1 = leftHole.Item1 - 1;
                                noHole = false;
                            }
                            var rightHole = clayAndWater.Where(c => c.Item2 == current.Item2 + 1 && c.Item1 > current.Item1 && c.Item1 <= right.Item1).DefaultIfEmpty((maxX, current.Item2 + 1)).Max();
                            if (rightHole.Item1 < right.Item1)
                            {
                                newSources.Add((rightHole.Item1 + 1, rightHole.Item2 -1));
                                water.Add((rightHole.Item1 + 1, rightHole.Item2 -1));
                                right.Item1 = rightHole.Item1 + 1;
                                noHole = false;
                            }

                            water.UnionWith(Enumerable.Range(left.Item1, right.Item1 - left.Item1).Select(s => (s, current.Item2)));
                        }
                    }
                }
                sources = newSources;
            }


            return water.Count;
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