using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QuickGraph;
using QuickGraph.Algorithms;

namespace adventofcode2018
{
    using static Utils;

    using Field = ValueTuple<(int x, int y), Tool>;
    using Edge = Edge<((int x, int y) pos, Tool tool)>;
    using Map = AdjacencyGraph<((int x, int y) pos, Tool tool), Edge<((int x, int y) pos, Tool tool)>>;
    enum Region { rocky, wet, narrow };
    enum Tool { torch, climbingGear, neither }


    public static class Day22
    {

        static int[,] GetGrid(int depth, (int x, int y) target, (int x, int y) increase)
        {
            var grid = new int[target.x + 1 + increase.x, target.y + 1 + increase.y];

            for (int x = 1; x <= target.x + increase.x; x++)
                grid[x, 0] = (x * 16807 + depth) % 20183;
            for (int y = 1; y <= target.y + increase.y; y++)
                grid[0, y] = (y * 48271 + depth) % 20183;

            for (int y = 1; y <= target.y + increase.y; ++y)
                for (int x = 1; x <= target.x + increase.x; ++x)
                    grid[x, y] = (grid[x - 1, y] * grid[x, y - 1] + depth) % 20183;

            for (int y = 0; y <= target.y + increase.y; ++y)
                for (int x = 0; x <= target.x + increase.x; ++x)
                    grid[x, y] %= 3;

            grid[0, 0] = depth % 20183 % 3;
            grid[target.x, target.y] = depth % 20183 % 3;

            return grid;
        }

        static void AddBidirectionalEdge(this Map map, Field field1, Field field2)
        {
            
            map.AddVerticesAndEdge(new Edge(field1, field2));
            map.AddVerticesAndEdge(new Edge(field2, field1));
        }

        static void AddEdgeFromRocky(this Map map, (int x, int y) source, (int x, int y) target, int[,] grid)
        {
            switch (grid[target.x, target.y])
            {
                case 0:
                    map.AddBidirectionalEdge((source, Tool.climbingGear), (target, Tool.climbingGear));
                    map.AddBidirectionalEdge((source, Tool.climbingGear), (target, Tool.torch));
                    map.AddBidirectionalEdge((source, Tool.torch), (target, Tool.climbingGear));
                    map.AddBidirectionalEdge((source, Tool.torch), (target, Tool.torch));
                    break;
                case 1:
                    map.AddBidirectionalEdge((source, Tool.climbingGear), (target, Tool.climbingGear));
                    map.AddBidirectionalEdge((source, Tool.climbingGear), (target, Tool.neither));
                    map.AddBidirectionalEdge((source, Tool.torch), (target, Tool.climbingGear));
                    map.AddBidirectionalEdge((source, Tool.torch), (target, Tool.neither));
                    break;
                case 2:
                    map.AddBidirectionalEdge((source, Tool.climbingGear), (target, Tool.torch));
                    map.AddBidirectionalEdge((source, Tool.climbingGear), (target, Tool.neither));
                    map.AddBidirectionalEdge((source, Tool.torch), (target, Tool.torch));
                    map.AddBidirectionalEdge((source, Tool.torch), (target, Tool.neither));
                    break;
            }
        }

        static void AddEdgesFromRocky(this Map map, (int x, int y) pos, int[,] grid)
        {
            map.AddEdgeFromRocky(pos, (pos.x + 1, pos.y), grid);
            map.AddEdgeFromRocky(pos, (pos.x, pos.y + 1), grid);

        }

        static void AddEdgeFromWet(this Map map, (int x, int y) source, (int x, int y) target, int[,] grid)
        {
            switch (grid[target.x, target.y])
            {
                case 0:
                    map.AddBidirectionalEdge((source, Tool.climbingGear), (target, Tool.climbingGear));
                    map.AddBidirectionalEdge((source, Tool.climbingGear), (target, Tool.torch));
                    map.AddBidirectionalEdge((source, Tool.neither), (target, Tool.climbingGear));
                    map.AddBidirectionalEdge((source, Tool.neither), (target, Tool.torch));
                    break;
                case 1:
                    map.AddBidirectionalEdge((source, Tool.climbingGear), (target, Tool.climbingGear));
                    map.AddBidirectionalEdge((source, Tool.climbingGear), (target, Tool.neither));
                    map.AddBidirectionalEdge((source, Tool.neither), (target, Tool.climbingGear));
                    map.AddBidirectionalEdge((source, Tool.neither), (target, Tool.neither));
                    break;
                case 2:
                    map.AddBidirectionalEdge((source, Tool.climbingGear), (target, Tool.torch));
                    map.AddBidirectionalEdge((source, Tool.climbingGear), (target, Tool.neither));
                    map.AddBidirectionalEdge((source, Tool.neither), (target, Tool.torch));
                    map.AddBidirectionalEdge((source, Tool.neither), (target, Tool.neither));
                    break;
            }
        }

        static void AddEdgesFromWet(this Map map, (int x, int y) pos, int[,] grid)
        {
            map.AddEdgeFromWet(pos, (pos.x + 1, pos.y), grid);
            map.AddEdgeFromWet(pos, (pos.x, pos.y + 1), grid);

        }

        static void AddEdgeFromNarrow(this Map map, (int x, int y) source, (int x, int y) target, int[,] grid)
        {
            switch (grid[target.x, target.y])
            {
                case 0:
                    map.AddBidirectionalEdge((source, Tool.torch), (target, Tool.climbingGear));
                    map.AddBidirectionalEdge((source, Tool.torch), (target, Tool.torch));
                    map.AddBidirectionalEdge((source, Tool.neither), (target, Tool.climbingGear));
                    map.AddBidirectionalEdge((source, Tool.neither), (target, Tool.torch));
                    break;
                case 1:
                    map.AddBidirectionalEdge((source, Tool.torch), (target, Tool.climbingGear));
                    map.AddBidirectionalEdge((source, Tool.torch), (target, Tool.neither));
                    map.AddBidirectionalEdge((source, Tool.neither), (target, Tool.climbingGear));
                    map.AddBidirectionalEdge((source, Tool.neither), (target, Tool.neither));
                    break;
                case 2:
                    map.AddBidirectionalEdge((source, Tool.torch), (target, Tool.torch));
                    map.AddBidirectionalEdge((source, Tool.torch), (target, Tool.neither));
                    map.AddBidirectionalEdge((source, Tool.neither), (target, Tool.torch));
                    map.AddBidirectionalEdge((source, Tool.neither), (target, Tool.neither));
                    break;
            }
        }

        static void AddEdgesFromNarrow(this Map map, (int x, int y) pos, int[,] grid)
        {
            map.AddEdgeFromNarrow(pos, (pos.x + 1, pos.y), grid);
            map.AddEdgeFromNarrow(pos, (pos.x, pos.y + 1), grid);

        }

        static void AddEdgeFromMouth(this Map map, (int x, int y) source, (int x, int y) target, int[,] grid)
        {
            switch (grid[target.x, target.y])
            {
                case 0:
                    map.AddBidirectionalEdge((source, Tool.torch), (target, Tool.climbingGear));
                    map.AddBidirectionalEdge((source, Tool.torch), (target, Tool.torch));
                    break;
                case 1:
                    map.AddBidirectionalEdge((source, Tool.torch), (target, Tool.climbingGear));
                    map.AddBidirectionalEdge((source, Tool.torch), (target, Tool.neither));
                    break;
                case 2:
                    map.AddBidirectionalEdge((source, Tool.torch), (target, Tool.torch));
                    map.AddBidirectionalEdge((source, Tool.torch), (target, Tool.neither));
                    break;
            }
        }

        static void AddEdgesFromMouth(this Map map, (int x, int y) pos, int[,] grid)
        {
            map.AddEdgeFromNarrow(pos, (pos.x + 1, pos.y), grid);
            map.AddEdgeFromNarrow(pos, (pos.x, pos.y + 1), grid);

        }

        static Map ToGraphMap (this int[,] grid)
        {
            var map = new Map();

            for (int x = 0; x < grid.GetLength(0) - 1; ++x)
                for (int y = 0; y < grid.GetLength(1) - 1; ++y)
                {
                    if (x == 0 && y ==0)
                        map.AddEdgesFromMouth((x, y), grid);
                    else
                        switch (grid[x, y])
                        {
                            case 0:
                                map.AddEdgesFromRocky((x, y), grid);
                                break;
                            case 1:
                                map.AddEdgesFromWet((x, y), grid);
                                break;
                            case 2:
                                map.AddEdgesFromNarrow((x, y), grid);
                                break;
                        }
                }

            return map;
        }

        public static int RiskLevel(IEnumerable<string> input)
        {
            var depth = Int32.Parse(input.First().Substring(7));
            var target = input.Last().Substring(8).Split(',').Select(Int32.Parse);
            var grid = GetGrid(depth, (target.First(), target.Last()), (0, 0));
                        
            var sum = 0;

            for (int y = 0; y <= target.Last(); ++y)
                for (int x = 0; x <= target.First(); ++x)
                    sum += grid[x, y];
            
            return sum;
        }

        static int Rescue(IEnumerable<string> input)
        {
            var depth = Int32.Parse(input.First().Substring(7));
            var target = input.Last().Substring(8).Split(',').Select(Int32.Parse);
            var grid = GetGrid(depth, (target.First(), target.Last()), (50, 50));

            var map = grid.ToGraphMap();

            IEnumerable<Edge> path;
            Func<Edge, double> edgeCost = e =>
            {
                if (e.Target.pos.x == target.First() && e.Target.pos.y == target.Last())
                {
                    return e.Source.tool == Tool.torch ? 1 : 8;
                }

                return e.Source.tool == e.Target.tool ? 1 : 8;
            };

            var tryPath = map.ShortestPathsDijkstra(edgeCost, ((0, 0), Tool.torch));
            var a = tryPath(((target.First(), target.Last()), Tool.torch), out path);

            return path.Count() + path.Where(s => s.Source.tool != s.Target.tool).Count() * 7;
        }

        public static void Solution()
        {
            //var testInput = new string [] {"depth: 510", "target: 10,10"};
            //Console.WriteLine(RiskLevel(testInput));
            //Console.WriteLine(Rescue(testInput));

            var input = GetFromFile(22);
            Print(RiskLevel(input), Rescue(input));
        }
    }
}