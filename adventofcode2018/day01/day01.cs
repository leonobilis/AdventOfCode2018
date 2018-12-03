using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace adventofcode2018
{
    using static Utils;

    public class Day01
    {
        static int p1(IEnumerable<int> input) {
            return input.Sum();
        }

        static int p2(IEnumerable<int> input) {
            var frequency = 0;
            var frequencies = new HashSet<int>{0};
            while(true) 
            {
                foreach (var i in input) {
                    frequency += i;
                    if (!frequencies.Add(frequency))
                        return frequency;
                }
            }
        }

        public static void Solution() {
            // Print(Day01.p2(new int[]{+1, -2, +3, +1}));
            // Print(Day01.p2(new int[]{+1, -1}));
            // Print(Day01.p2(new int[]{+3, +3, +4, -2, -4}));
            // Print(Day01.p2(new int[]{-6, +3, +8, +5, -6}));
            // Print(Day01.p2(new int[]{+7, +7, -2, -7, -4}));

            var input = GetFromFile(1).Select(int.Parse);
            Print(p1(input), p2(input));
        }
    }
}