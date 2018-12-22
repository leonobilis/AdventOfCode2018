using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace adventofcode2018
{
    using static Utils;

    public static class Day22
    {
        public static int RiskLevel(IEnumerable<string> input)
        {   
            var depth =  Int32.Parse(input.First().Substring(7));
            var target = input.Last().Substring(8).Split(',').Select(Int32.Parse);
            var targetX = target.First();
            var targetY = target.Last();

            var grid = new int[targetX + 1, targetY + 1];

            for (int x = 1; x <= targetX; x++)
                grid[x, 0] = (x * 16807 + depth) % 20183;
            for (int y = 1; y <= targetY; y++)
                grid[0, y] = (y * 48271 + depth) % 20183;
            
            for (int y = 1; y <= targetY; ++y)
                for (int x = 1; x <= targetX; ++x)
                    grid[x, y] = (grid[x - 1, y] * grid[x, y - 1]+ depth) % 20183;
            
            grid[0, 0] = depth % 20183 % 3;
            grid[targetX, targetY] = depth % 20183 % 3;
            
            var sum = 0;

            for (int y = 0; y <= targetY; ++y)
                for (int x = 0; x <= targetX; ++x)
                    sum += grid[x, y] % 3;
            
            return sum;
        }

        public static void Solution()
        {
            var testInput = new string [] {"depth: 510", "target: 10,10"};
            Console.WriteLine(RiskLevel(testInput));

            var input = GetFromFile(22);
            Print(RiskLevel(input));
        }
    }
}