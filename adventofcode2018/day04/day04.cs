using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace adventofcode2018
{
    using static Utils;
    public class Day04
    {
        struct Guard 
        {
            public Guard(int id)
            {
                this.id = id;
                this.asleep = null;
            }
            public int id;
            public List<int> asleep;
        }

        static Guard MakeGuard(IGrouping<DateTime, (DateTime, string)> group)
        {
            Regex guardRx = new Regex(@"Guard #([\d]+) begins shift", RegexOptions.Compiled);
    
            var id = guardRx.IsMatch(group.First().Item2)?guardRx.Matches(group.First().Item2).Select(m => m.Groups).First()[1].Value:guardRx.Matches(group.Skip(1).First().Item2).Select(m => m.Groups).First()[1].Value;
            var guard = new Guard(Int32.Parse(id));
            guard.asleep = Enumerable.Zip(group.Where(x => x.Item2 == "falls asleep"), group.Where(x => x.Item2 == "wakes up"), (a, b) => (a.Item1, b.Item1))
                                     .SelectMany(s => Enumerable.Range(s.Item1.Minute, s.Item2.Minute - s.Item1.Minute)).ToList();
            
            return guard;
        }

        public static int p1(IEnumerable<string> input) {
            Regex inputRx = new Regex(@"\[([0-9\- \:]+)\] ([\w #]+)", RegexOptions.Compiled);
            return input.Select(s => inputRx.Matches(s).Select(m => m.Groups).First())
                              .Select(s => (DateTime.ParseExact(s[1].Value, "yyyy-MM-dd HH:mm", null), s[2].Value))
                              .Select(s => s.Item1.Hour==23?(s.Item1 + new TimeSpan(0, 60 - s.Item1.Minute, 0), s.Item2):s)
                              .OrderBy(x => x.Item1)
                              .GroupBy(x => x.Item1.Date)
                              .Select(s => MakeGuard(s))
                              .GroupBy(x => x.id)
                              .Select(s => new {Key = s.Key, asleep = s.SelectMany(s2 => s2.asleep)})
                              .OrderByDescending(o => o.asleep.Count())
                              .Take(1)
                              .Select(s => s.Key * s.asleep.GroupBy(g => g)
                                                            .OrderByDescending(o => o.Count())
                                                            .First().Key)
                              .First();
        }

        public static int p2(IEnumerable<string> input) {
            Regex inputRx = new Regex(@"\[([0-9\- \:]+)\] ([\w #]+)", RegexOptions.Compiled);
            return input.Select(s => inputRx.Matches(s).Select(m => m.Groups).First())
                              .Select(s => (DateTime.ParseExact(s[1].Value, "yyyy-MM-dd HH:mm", null), s[2].Value))
                              .Select(s => s.Item1.Hour==23?(s.Item1 + new TimeSpan(0, 60 - s.Item1.Minute, 0), s.Item2):s)
                              .OrderBy(x => x.Item1)
                              .GroupBy(x => x.Item1.Date)
                              .Select(s => MakeGuard(s))
                              .GroupBy(x => x.id)
                              .Select(s => new {Key = s.Key, asleep = s.SelectMany(s2 => s2.asleep)
                                                                       .GroupBy(g => g)
                                                                       .Select(s2 => new {Minute = s2.Key, Count = s2.Count()})
                                                                       .OrderByDescending(o => o.Count)
                                                                       .Take(1)})
                              .Where(x => x.asleep.Count() > 0)
                              .Select(s => new {Key = s.Key, asleep = s.asleep.First()})
                              .OrderByDescending(o => o.asleep.Count)
                              .Take(1)
                              .Select(s => s.Key * s.asleep.Minute)
                              .First();
        }

        public static void Solution() {

//             var testInput = @"[1518-11-01 00:00] Guard #10 begins shift
// [1518-11-01 00:05] falls asleep
// [1518-11-01 00:25] wakes up
// [1518-11-01 00:30] falls asleep
// [1518-11-01 00:55] wakes up
// [1518-11-01 23:58] Guard #99 begins shift
// [1518-11-02 00:40] falls asleep
// [1518-11-02 00:50] wakes up
// [1518-11-03 00:05] Guard #10 begins shift
// [1518-11-03 00:24] falls asleep
// [1518-11-03 00:29] wakes up
// [1518-11-04 00:02] Guard #99 begins shift
// [1518-11-04 00:36] falls asleep
// [1518-11-04 00:46] wakes up
// [1518-11-05 00:03] Guard #99 begins shift
// [1518-11-05 00:45] falls asleep
// [1518-11-05 00:55] wakes up".Split("\n");
//             Console.WriteLine(Day04.p1(testInput));
//             Console.WriteLine(Day04.p2(testInput));

            var input = GetFromFile(4);
            Print(p1(input), p2(input));
        }
    }
}