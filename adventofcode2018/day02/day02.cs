using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace adventofcode2018
{
    using static Utils;

    public class Day02
    {
        public static int p1(IEnumerable<string> input) {
            return input.Select(s => s.ToCharArray()
                                      .GroupBy(i => i)
                                      .Select(x => x.Count())
                                      .Where(x => x >= 2  && x <= 3)
                                      .Distinct())
                        .Aggregate((acc, x) => acc.Concat(x))
                        .GroupBy(i => i)
                        .Aggregate(1, (acc, x) => acc * x.Count());
        }

        public static string p2(IEnumerable<string> input) {

            return input.Select(s1 => (s1, input.Where(s2 => s1.Where((c, i) => c != s2[i]).Count()==1)))
                        .Where(x => x.Item2.Any())
                        .Take(1)
                        .Select(x => string.Concat(x.Item2.First().Where((c, i) => c == x.Item1[i])))
                        .First();
        }

        public static void Solution() {
            // Print(Day02.p1(new string[]{"abcdef", "bababc", "abbcde", "abcccd", "aabcdd", "abcdee", "ababab"}));

            // Print(Day02.p2(new string[]{"abcde",
            //                             "fghij",
            //                             "klmno",
            //                             "pqrst",
            //                             "fguij",
            //                             "axcye",
            //                             "wvxyz"}));

            var input = GetFromFile(2);
            Print(p1(input), p2(input));
        }
    }
}