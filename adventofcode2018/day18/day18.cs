using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;

namespace adventofcode2018
{
    using static Utils;

    public static class Day18
    {
        static List<List<char>> GetArea(IEnumerable<string> input)
        {
            var side = input.Count();
            return new List<List<char>> {Enumerable.Range(0, side +2).Select(_ => 'X').ToList()}
                       .Concat(input.Select(s => new List<char>{'X'}.Concat(s.ToList()).Concat(new List<char>{'X'}).ToList()))
                       .Concat(new List<List<char>> {Enumerable.Range(0, side +2).Select(_ => 'X').ToList()}).ToList();
        }

        static char Transform(char field, int i, int j, List<List<char>> area)
        {
            if (field == 'X')
                return 'X';
            else
            {
                var adjacent = area.Skip(i - 1).Take(3).SelectMany(s => s.Skip(j - 1).Take(3));
                if (field == '.' && adjacent.Count(c => c == '|') >= 3)
                    return '|';
                if (field == '|' && adjacent.Count(c => c == '#') >= 3)
                    return '#';
                if (field == '#' && (adjacent.Count(c => c == '#') < 2 || adjacent.Count(c => c == '|') < 1))
                    return '.';
                return field;
            }
        }    
        public static int CountAcres(IEnumerable<string> input)
        {            
            var area = GetArea(input);


            for (int minute = 0; minute < 10; minute++)
                area = area.Select((s, i) => s.Select((s2, j) => Transform(s2, i, j, area)).ToList()).ToList();
                              

            return area.Sum(s => s.Count(c => c == '|')) * area.Sum(s => s.Count(c => c == '#'));
        }

        public static int CountAcresLongTime(IEnumerable<string> input)
        {            
            var area = GetArea(input);
            var values = new HashSet<int>();

            for (int minute = 0; minute < 1000000000; minute++)
            {
                area = area.Select((s, i) => s.Select((s2, j) => Transform(s2, i, j, area)).ToList()).ToList();
                if ((minute+1) % 1000 == 0)
                {
                    if (!values.Add(area.Sum(s => s.Count(c => c == '|')) * area.Sum(s => s.Count(c => c == '#'))))
                        return values.Skip(1000000%(minute+1)-1).First();
                }
                    
            }

            return area.Sum(s => s.Count(c => c == '|')) * area.Sum(s => s.Count(c => c == '#'));
        }

        public static void Solution()
        {
//             var testInput = @".#.#...|#.
// .....#|##|
// .|..|...#.
// ..|#.....#
// #.#|||#|#|
// ...#.||...
// .|....|...
// ||...#|.#|
// |.||||..|.
// ...#.|..|.".Split("\n");
            //Console.WriteLine(CountAcres(testInput));
            
            var input = GetFromFile(18);
            Print(CountAcres(input), CountAcresLongTime(input));
        }
    }
}