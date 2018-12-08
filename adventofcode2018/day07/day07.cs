using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace adventofcode2018
{
    using static Utils;

    public static class Day07
    {
        public static string p1(IEnumerable<string> input)
        {

            Regex inputRx = new Regex(@"Step ([A-Z]) must be finished before step ([A-Z]) can begin.", RegexOptions.Compiled);
            var output = new HashSet<string>();
            
            var steps = input.Select(s => inputRx.Matches(s).Select(m => m.Groups).First())
                             .Select(s => (s[2].Value, s[1].Value))
                             .GroupBy(g => g.Item1)
                             .Select(s => new {s.Key, dependencies = s.Select(s2 => s2.Item2)
                                                                      .OrderBy(o => o)})
                             .ToDictionary(k => k.Key, v => v.dependencies.ToList());

            var steps2 = input.Select(s => inputRx.Matches(s).Select(m => m.Groups).First())
                                .Select(s => (s[1].Value, s[2].Value))
                                .GroupBy(g => g.Item1)
                                .Select(s => new {s.Key, Values = s.Select(s2 => steps[s2.Item2])})
                                .ToDictionary(k => k.Key, v => v.Values.ToList());
            
            steps = steps.Concat(steps2.Where(x => !steps.ContainsKey(x.Key)).ToDictionary(k => k.Key, v => new List<string>()))
                 .OrderBy(o => o.Key)
                 .ToDictionary(k => k.Key, v => v.Value);

            while (steps.Count > 0)
            {
                var key = steps.Where(x => x.Value.Count == 0).First().Key;
                if (steps2.ContainsKey(key))
                    foreach(var step2 in steps2[key])
                    {
                        step2.Remove(key);
                    }
                output.Add(key);
                steps.Remove(key);
                    
            }

            return output.Aggregate("", (acc, s) => acc+s);
        }

        public static int p2(IEnumerable<string> input, int numWorkers, Func<int, int, int> calcTime)
        {

            Regex inputRx = new Regex(@"Step ([A-Z]) must be finished before step ([A-Z]) can begin.", RegexOptions.Compiled);
            var output = new HashSet<char>();
            
            var steps = input.Select(s => inputRx.Matches(s).Select(m => m.Groups).First())
                             .Select(s => (s[2].Value[0], s[1].Value[0]))
                             .GroupBy(g => g.Item1)
                             .Select(s => new {s.Key, dependencies = s.Select(s2 => s2.Item2)
                                                                      .OrderBy(o => o)})
                             .ToDictionary(k => k.Key, v => v.dependencies.ToList());

            var steps2 = input.Select(s => inputRx.Matches(s).Select(m => m.Groups).First())
                                .Select(s => (s[1].Value[0], s[2].Value[0]))
                                .GroupBy(g => g.Item1)
                                .Select(s => new {s.Key, Values = s.Select(s2 => steps[s2.Item2])})
                                .ToDictionary(k => k.Key, v => v.Values.ToList());
            
            steps = steps.Concat(steps2.Where(x => !steps.ContainsKey(x.Key)).ToDictionary(k => k.Key, v => new List<char>()))
                 .OrderBy(o => o.Key)
                 .ToDictionary(k => k.Key, v => v.Value);

            var workers = Enumerable.Range(0, numWorkers).Select(s => new {wait = 0, letter = '\0'});
            var time = 0;

            while (steps.Count > 0)
            {
                var toProcess = workers.Where(x => x.wait == 0).Zip(steps.Where(x => x.Value.Count == 0), (worker, step) => (worker, step)).ToList();

                if (toProcess.Count() == 0)
                {
                    time = workers.Where(x => x.wait > 0).Select(s => s.wait).Min();
                    foreach (var worker in workers.Where(x => x.wait == time).ToList())
                    {
                        var key = worker.letter;
                        if (steps2.ContainsKey(key))
                            foreach (var step2 in steps2[key])
                            {
                                step2.Remove(key);
                            }
                    }
                    workers = workers.Where(x => x.wait > time);
                }
                else
                {   
                    foreach (var key in toProcess.Select(s => s.Item2.Key))
                    {
                        steps.Remove(key);
                    }
                    workers = workers.Where(x => x.wait > 0).Concat(toProcess.Select(p => new { wait = calcTime(time, p.Item2.Key), letter = p.Item2.Key }));
                }
                workers = workers.Concat(Enumerable.Range(0, numWorkers - workers.Count()).Select(s => new { wait = 0, letter = '\0' })).ToList();
            }

            return workers.First().wait;
        }

        public static void Solution()
        {
//             var testInput = @"Step C must be finished before step A can begin.
// Step C must be finished before step F can begin.
// Step A must be finished before step B can begin.
// Step A must be finished before step D can begin.
// Step B must be finished before step E can begin.
// Step D must be finished before step E can begin.
// Step F must be finished before step E can begin.".Split("\n");
            // Console.WriteLine(Day07.p1(testInput));
            // Console.WriteLine(Day07.p2(testInput, 2, (time, letter) => time + letter - 64));

            var input = GetFromFile(7);
            Print(p1(input), p2(input, 5, (time, letter) => time + letter - 4));
        }
    }
}