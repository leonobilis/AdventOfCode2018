using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace adventofcode2018
{
    using static Utils;
    public class Day03
    {
        static IEnumerable<(int, int)> MakeSquare(int left, int top, int width, int height)
        {
            return Enumerable.Range(left, width).SelectMany(i => Enumerable.Range(top, height).Select(j => (i, j)));
        }

        public static int p1(IEnumerable<string> input)
        {
            Regex rx = new Regex(@"#\d+ @ (\d+),(\d+): (\d+)x(\d+)", RegexOptions.Compiled);
            var output = input.Select(s => rx.Matches(s).Select(m => m.Groups).First())
                              .Select(s => MakeSquare(Int32.Parse(s[1].Value), Int32.Parse(s[2].Value), Int32.Parse(s[3].Value), Int32.Parse(s[4].Value)));

            return output.AsParallel()
                         .SelectMany((s, i) => output.Where((_, i2) => i != i2)
                                                     .SelectMany(s2 => s2.Intersect(s))
                                                     .Distinct())
                         .Distinct()
                         .Count();
        }

        public static int p2(IEnumerable<string> input)
        {
            Regex rx = new Regex(@"#(\d+) @ (\d+),(\d+): (\d+)x(\d+)", RegexOptions.Compiled);
            var output = input.Select(s => rx.Matches(s).Select(m => m.Groups).First())
                              .Select(s => new {Key = Int32.Parse(s[1].Value), Value = MakeSquare(Int32.Parse(s[2].Value), Int32.Parse(s[3].Value), Int32.Parse(s[4].Value), Int32.Parse(s[5].Value))});
            
            return output.AsParallel()
                         .Where(s1 => output.Where(s2 => s2.Key != s1.Key)
                                            .All(s2 => s1.Value.Intersect(s2.Value).Count() == 0))
                         .First()
                         .Key;
        }

        public static void Solution()
        {
            // print(Day03.p1(new string[]{"#1 @ 1,3: 4x4",
            //                             "#2 @ 3,1: 4x4",
            //                             "#3 @ 5,5: 2x2"}));

            // print(Day03.p2(new string[]{"#1 @ 1,3: 4x4",
            //                             "#2 @ 3,1: 4x4",
            //                             "#3 @ 5,5: 2x2"}));

            var input = GetFromFile(3);
            Print(p1(input), p2(input));
        }
    }
}