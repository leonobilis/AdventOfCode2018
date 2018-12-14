using System;
using System.Collections.Generic;

namespace adventofcode2018
{
    class Program
    {
        static void Main(string[] args)
        {
            var days = new Dictionary<int, Action>
            {
                {1,  Day01.Solution },
                {2,  Day02.Solution },
                {3,  Day03.Solution },
                {4,  Day04.Solution },
                {5,  Day05.Solution },
                {6,  Day06.Solution },
                {7,  Day07.Solution },
                {8,  Day08.Solution },
                {9,  Day09.Solution },
                {10, Day10.Solution },
                {11, Day11.Solution },
                {12, Day12.Solution },
                {13, Day13.Solution },
                {14, Day14.Solution },
            };

            days[Int32.Parse(args[0])]();
        }
    }
}
