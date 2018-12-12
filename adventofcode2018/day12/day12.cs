using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;

namespace adventofcode2018
{
    using static Utils;
    
    public static class Day12
    {
        public static int PotSum(IEnumerable<string> input, int iterations = 20)
        {
            Regex stateRx = new Regex(@"initial state: ([\.#]+)", RegexOptions.Compiled);
            var state = stateRx.Matches(input.First())
                               .Select(m => m.Groups)
                               .First()[1].Value
                               .Select((s, i) => (i, s == '#' ? 1 : 0))
                               .ToDictionary(x => x.Item1, x => x.Item2);

            Regex notesRx = new Regex(@"([\.#]{5}) => ([\.#])", RegexOptions.Compiled);
            var notes = input.Skip(2).Select(s => notesRx.Matches(s)
                                                         .Select(m => m.Groups)
                                                         .First())
                                     .Select(s => new {k = s[1].Value.Select(s2 => s2 == '#' ? 1 : 0).ToList(), v = s[2].Value == "#" ? 1 : 0 })
                                     .ToDictionary(x => (x.k[0], x.k[1], x.k[2], x.k[3], x.k[4]), x => x.v);
            
            for( int i = 0; i <iterations; i++)
            {
                var newState = state.Select(s => (s.Key, notes.GetValueOrDefault((state.GetValueOrDefault(s.Key-2, 0),
                                                                                  state.GetValueOrDefault(s.Key-1, 0),
                                                                                  s.Value, 
                                                                                  state.GetValueOrDefault(s.Key+1, 0), 
                                                                                  state.GetValueOrDefault(s.Key+2, 0)), 0)))
                                     .ToDictionary(x => x.Item1, x => x.Item2);

                var minKey = state.Keys.Min();
                if (notes.GetValueOrDefault((0,0,0, state[minKey], state[minKey+1]), 0) == 1)
                    newState[minKey-1] = 1;
                var maxKey = state.Keys.Max();
                if (notes.GetValueOrDefault((state[maxKey-1], state[maxKey], 0,0,0), 0) == 1)
                    newState[maxKey+1] = 1;
                state = newState;
            }

            return state.Where(x => x.Value == 1).Select(s => s.Key).Sum();
        }

        static long Part2(IEnumerable<string> input)
        {
            var firstThousand = PotSum(input, 1000);
            var secondThousand = PotSum(input, 2000);
            var diff = secondThousand - firstThousand;
            var rest = firstThousand - diff;
            return 50000000000 * (diff/1000) + rest;
        }

        public static void Solution()
        {
            // var testInput = File.ReadAllLines("adventofcode2018/day12/test_input.txt");
            // Console.WriteLine(PotSum(testInput));

            var input = GetFromFile(12);
            Print(PotSum(input), Part2(input));
        }
    }
}
