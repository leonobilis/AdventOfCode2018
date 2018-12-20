using System;
using System.Collections.Generic;
using System.Linq;
using QuickGraph;
using QuickGraph.Algorithms;

namespace adventofcode2018
{
    using static Utils;

    using Field = ValueTuple<int, int>;
    using Edge = Edge<(int, int)>;
    using Map = UndirectedGraph<(int, int), Edge<(int, int)>>;

        // public void TryMove(IEnumerable<Unit> units)
        // {

        //     IEnumerable<Edge> path;
        //     IEnumerable<Edge> shorterPath = null;
        //     Func<Edge, double> edgeCost = e => 1;

        //     foreach (var range1 in EmptyInRange)
        //     {
        //         var tryPath = map.ShortestPathsDijkstra(edgeCost, range1);
        //         foreach (var range2 in EnemiesRanges)
        //         {
        //             if (tryPath(range2, out path))
        //                 shorterPath = GetShorterPath(shorterPath, path);
        //         }
        //     }
        // }

    public static class Day20
    {
        static (int, Field) Traverse(IEnumerable<char> input, Map map, Field current)
        {
            for(var i = 0; i < input.Count(); ++i)
            {
                switch(input.Skip(i).First())
                {
                    case 'N':
                        map.AddVerticesAndEdge(new Edge(current, (current.Item1, current.Item2 - 1)));
                        current = (current.Item1, current.Item2 + 1);
                        break;
                    case 'S':
                        map.AddVerticesAndEdge(new Edge(current, (current.Item1, current.Item2 + 1)));
                        current = (current.Item1, current.Item2 + 1);
                        break;
                    case 'E':
                        map.AddVerticesAndEdge(new Edge(current, (current.Item1 + 1, current.Item2)));
                        current = (current.Item1 + 1, current.Item2);
                        break;
                    case 'W':
                        map.AddVerticesAndEdge(new Edge(current, (current.Item1 - 1, current.Item2)));
                        current = (current.Item1 - 1, current.Item2);
                        break;
                    case '(':
                        var fieldsToTraverse = new HashSet<Field>();
                        for (int ii; input.Skip(i).First() != ')';  i += ii)
                        {
                            Field fieldToCheck;
                            (ii, fieldToCheck) = Traverse(input.Skip(i+1), map, current);
                            fieldsToTraverse.Add(fieldToCheck);
                        }
                        fieldsToTraverse.ToList().ForEach( c => Traverse(input.Skip(i+1), map, c) );
                        return (i, current);
                    case '|':
                    case ')':
                    case '$':
                        return (i+1, current);
                }
            }

            return (0, current);
        }

        static Map GetMap(IEnumerable<char> input)
        {
            var map = new Map();
            Traverse(input.Skip(1), map, (0, 0));
            
            map.AddVertexRange(filteredGrid.Select(s => s.Key));
            var edges = filteredGrid.Where(x => grid[x.Key.Item1-1][x.Key.Item2] == '.')
                                    .Select(s => new Edge(s.Key, (s.Key.Item1-1, s.Key.Item2)));
            map.AddEdgeRange(edges);
            edges = filteredGrid.Where(x => grid[x.Key.Item1][x.Key.Item2-1] == '.')
                                .Select(s => new Edge(s.Key, (s.Key.Item1, s.Key.Item2-1)));
            map.AddEdgeRange(edges);
            edges = filteredGrid.Where(x => grid[x.Key.Item1][x.Key.Item2+1] == '.')
                                    .Select(s => new Edge(s.Key, (s.Key.Item1, s.Key.Item2+1)));
            map.AddEdgeRange(edges);
            edges = filteredGrid.Where(x => grid[x.Key.Item1+1][x.Key.Item2] == '.')
                                .Select(s => new Edge(s.Key, (s.Key.Item1+1, s.Key.Item2)));
            map.AddEdgeRange(edges);

            return map;
        }

        public static int DoorsToFurthest(IEnumerable<char> input)
        {
            var map = GetMap(input);
            var round = 0;

            return 0;
        }

        public static void Solution()
        {
            var testInput = "^WNE$";
            Console.WriteLine(DoorsToFurthest(testInput));

            var testInput2 = "^ENWWW(NEEE|SSE(EE|N))$";
            Console.WriteLine(DoorsToFurthest(testInput2));

            var testInput3 = "^ENNWSWW(NEWS|)SSSEEN(WNSE|)EE(SWEN|)NNN$";
            Console.WriteLine(DoorsToFurthest(testInput3));

            var testInput4 = "^ESSWWN(E|NNENN(EESS(WNSE|)SSS|WWWSSSSE(SW|NNNE)))$";
            Console.WriteLine(DoorsToFurthest(testInput4));

            var testInput5 = "^WSSEESWWWNW(S|NENNEEEENN(ESSSSW(NWSW|SSEN)|WSWWN(E|WWS(E|SS))))$";
            Console.WriteLine(DoorsToFurthest(testInput5));
            
            var input = GetFromFile(20).First();
            Print(DoorsToFurthest(input));
        }
    }
}
