using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace adventofcode2018
{
    using static Utils;

    struct Nanobot
    {
        public (int x, int y, int z) pos;
        public int radius;
    }

    public static class Day23
    {
        static int Dist((int x, int y, int z) a, (int x, int y, int z) b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) + Math.Abs(a.z - b.z);
        }

        static List<Nanobot> GetNanobots(IEnumerable<string> input)
        {
            Regex inputRx = new Regex(@"pos=<([-\d]+),([-\d]+),([-\d]+)>, r=([-\d]+)", RegexOptions.Compiled);
            return input.Select(s => inputRx.Matches(s).Select(m => m.Groups).First())
                        .Select(s => new Nanobot { pos = (Int32.Parse(s[1].Value), Int32.Parse(s[2].Value), Int32.Parse(s[3].Value)), radius = Int32.Parse(s[4].Value) })
                        .ToList();
        }
        
        public static int InRangeOfStrongest(IEnumerable<string> input)
        {
            var nanobots = GetNanobots(input);

            var strongest = nanobots.OrderByDescending(m => m.radius).First();

            return nanobots.Where(x => Dist(strongest.pos, x.pos) <= strongest.radius).Count();
        }

        static int InRange((int x, int y, int z) position, List<Nanobot> nanobots)
        {
            return nanobots.Where(x => Dist(position, x.pos) <= x.radius).Count();
        }

        static int LookForBestPositionDist((int x, int y, int z)  min, (int x, int y, int z) max, int diff, (int x, int y, int z) bestPosition, int bestCount, List<Nanobot> nanobots)
        {
            var bestDist = Dist(bestPosition, (0, 0, 0));
            if (diff < 1)
                return bestDist;

            for (int x = min.x; x <= max.x; x += diff)
                for (int y = min.y; y <= max.y; y += diff)
                    for (int z = min.z; z <= max.z; z += diff)
                    {
                        var inRangeCount = InRange((x, y, z), nanobots);
                        if (inRangeCount == bestCount && Dist((x, y, z), (0, 0, 0)) < bestDist)
                        {
                            bestPosition = (x, y, z);
                            bestDist = Dist(bestPosition, (0, 0, 0));
                        }
                        else if (inRangeCount > bestCount)
                        {
                            bestCount = inRangeCount;
                            bestPosition = (x, y, z);
                            bestDist = Dist(bestPosition, (0, 0, 0));
                        }
                    }

            return LookForBestPositionDist((bestPosition.x - diff, bestPosition.y - diff, bestPosition.z - diff),
                                           (bestPosition.x + diff, bestPosition.y + diff, bestPosition.z + diff),
                                           diff / 2, bestPosition, bestCount, nanobots);
        }

        public static int DistToBestPosition(IEnumerable<string> input)
        {
            var nanobots = GetNanobots(input);

            var min = (x: nanobots.Min(m => m.pos.x), y: nanobots.Min(m => m.pos.y),  z: nanobots.Min(m => m.pos.z));
            var max = (x: nanobots.Max(m => m.pos.x), y: nanobots.Max(m => m.pos.y), z: nanobots.Max(m => m.pos.z));

            return LookForBestPositionDist(min, max, (max.x - min.x) / 2, max, 0, nanobots);
        }

        public static void Solution()
        {
//            var testInput = @"pos=<0,0,0>, r=4
//pos=<1,0,0>, r=1
//pos=<4,0,0>, r=3
//pos=<0,2,0>, r=1
//pos=<0,5,0>, r=3
//pos=<0,0,3>, r=1
//pos=<1,1,1>, r=1
//pos=<1,1,2>, r=1
//pos=<1,3,1>, r=1".Split("\n");
//             Console.WriteLine(InRangeOfStrongest(testInput));

//            var testInput2 = @"pos=<10,12,12>, r=2
//pos=<12,14,12>, r=2
//pos=<16,12,12>, r=4
//pos=<14,14,14>, r=6
//pos=<50,50,50>, r=200
//pos=<10,10,10>, r=5".Split("\n");
//            Console.WriteLine(DistToBestPosition(testInput2));

            var input = GetFromFile(23);
            Print(InRangeOfStrongest(input), DistToBestPosition(input));
        }
    }
}