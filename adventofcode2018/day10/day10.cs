using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;

namespace adventofcode2018
{
    using static Utils;

    struct Point
    {
        public struct Position
        {
            public int X, Y;
        }
        public struct Velocity
        {
            public int X, Y;
        }

        public Position position;
        public Velocity velocity;

    }

    public static class Day10
    {
        static void PrintPoints(IEnumerable<Point> points)
        {
            var pointsSet = points.Select(p => (p.position.X, p.position.Y)).ToHashSet();
            
            var minX = points.Select(p => p.position.X).Min();
            var maxX = points.Select(p => p.position.X).Max();
            var minY = points.Select(p => p.position.Y).Min();
            var maxY = points.Select(p => p.position.Y).Max();

            foreach (var y in Enumerable.Range(minY, Math.Abs(maxY - minY) + 1))
            {
                Console.WriteLine();
                foreach (var x in Enumerable.Range(minX, Math.Abs(maxX - minX) + 1))
                    Console.Write( pointsSet.Contains((x, y)) ? "#" : "." );
            }
            
            Console.WriteLine();
        }
        
        public static void Solution(IEnumerable<string> input)
        {
            Regex inputRx = new Regex(@"position=<\s?([-\d]+),\s+([-\d]+)> velocity=<\s?([-\d]+),\s+([-\d]+)>", RegexOptions.Compiled);
            var points = input.Select(s => inputRx.Matches(s).Select(m => m.Groups).First())
                              .Select(s => new Point{ position = new Point.Position{X = Int32.Parse(s[1].Value), Y = Int32.Parse(s[2].Value)}, velocity = new Point.Velocity{X = Int32.Parse(s[3].Value), Y = Int32.Parse(s[4].Value)}}).ToArray();

            var minX = points.Select(p => p.position.X).Min();
            var maxX = points.Select(p => p.position.X).Max();
            var minY = points.Select(p => p.position.Y).Min();
            var maxY = points.Select(p => p.position.Y).Max();
            var diffX = Int32.MaxValue;
            var diffY = Int32.MaxValue;
            
            var pointsSize = points.Count();
            var i = 0;
            
            while(diffX > Math.Abs(maxX - minX) && diffY > Math.Abs(maxY - minY))
            {
                diffX = Math.Abs(maxX - minX);
                diffY = Math.Abs(maxY - minY);
                ++i;
                for (int p = 0; p < pointsSize; p++)
                {
                    points[p].position.X += points[p].velocity.X;
                    points[p].position.Y += points[p].velocity.Y;
                }
                
                minX = points.Select(p => p.position.X).Min();
                maxX = points.Select(p => p.position.X).Max();
                minY = points.Select(p => p.position.Y).Min();
                maxY = points.Select(p => p.position.Y).Max();

            }

            Console.WriteLine("Iteration {0}", i);

            for (int p = 0; p < pointsSize; p++)
            {
                points[p].position.X -= points[p].velocity.X;
                points[p].position.Y -= points[p].velocity.Y;
            }

            PrintPoints(points);
        }

        public static void Solution()
        {
            var input = GetFromFile(10);
            Solution(input);
        }
    }
}