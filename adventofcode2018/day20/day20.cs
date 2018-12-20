using System;
using System.Collections.Generic;
using System.Linq;
using QuickGraph;
using QuickGraph.Algorithms;
using System.Threading;

namespace adventofcode2018
{
    using static Utils;

    using Field = ValueTuple<int, int>;
    using Edge = Edge<(int, int)>;
    using Map = UndirectedGraph<(int, int), Edge<(int, int)>>;

    public static class Day20
    {
        static (IEnumerable<char>, HashSet<Field> current) Traverse(IEnumerable<char> input, Map map, HashSet<Field> current)
        {
            return current.Select(s => Traverse(input, map, s)).Aggregate((input, new HashSet<Field>()), (acc, x) => (x.Item1, acc.Item2.Union(x.current).ToHashSet()));
        }

        static (IEnumerable<char>, HashSet<Field> current) Traverse(IEnumerable<char> input, Map map, Field current)
        {
            HashSet<Field> newCurrent = null;
            var a = input.First();

            switch (input.First())
            {
                case 'N':
                    map.AddVerticesAndEdge(new Edge(current, (current.Item1, current.Item2 - 1)));
                    current = (current.Item1, current.Item2 - 1);
                    return Traverse(input.Skip(1), map, current);
                case 'S':
                    map.AddVerticesAndEdge(new Edge(current, (current.Item1, current.Item2 + 1)));
                    current = (current.Item1, current.Item2 + 1);
                    return Traverse(input.Skip(1), map, current);
                case 'E':
                    map.AddVerticesAndEdge(new Edge(current, (current.Item1 + 1, current.Item2)));
                    current = (current.Item1 + 1, current.Item2);
                    return Traverse(input.Skip(1), map, current);
                case 'W':
                    map.AddVerticesAndEdge(new Edge(current, (current.Item1 - 1, current.Item2)));
                    current = (current.Item1 - 1, current.Item2);
                    return Traverse(input.Skip(1), map, current);
                case '(':
                    newCurrent = new HashSet<Field>();
                    while (input.First() != ')')
                    {
                        HashSet<Field> fieldsToCheck;
                        (input, fieldsToCheck) = Traverse(input.Skip(1), map, current);
                        newCurrent.UnionWith(fieldsToCheck);
                    }
                    return Traverse(input.Skip(1), map, newCurrent);
                case '|':
                case ')':
                    return (input, new HashSet<Field> { current });
                case '$':
                    return (input, new HashSet<Field> {});
                    
            }

            return Traverse(input.Skip(1), map, newCurrent);
        }

        static Map GetMap(IEnumerable<char> input)
        {
            var map = new Map();
            int stackSize = 1024 * 1024 * 64;
            Thread th = new Thread(() => Traverse(input.Skip(1), map, (0, 0)), stackSize);
            th.Start();
            th.Join();
            return map;
        }

        static (int max, int paths1k) DoorsToFurthest(IEnumerable<char> input)
        {
            var map = GetMap(input);

            IEnumerable<Edge> path;
            Func<Edge, double> edgeCost = e => 1;

            var tryPath = map.ShortestPathsDijkstra(edgeCost, (0, 0));
            var max = 1000;
            var paths1k = 0;

            foreach (var room in map.Vertices)
            {
                if (tryPath(room, out path) && path.Count() >= 1000)
                {
                    ++paths1k;
                    max = path.Count() > max ? path.Count() : max;
                }
            }

            return (max, paths1k);
        }

        public static void Solution()
        {
            //var testInput = "^WNE$";
            //Console.WriteLine(DoorsToFurthest(testInput));

            //var testInput2 = "^ENWWW(NEEE|SSE(EE|N))$";
            //Console.WriteLine(DoorsToFurthest(testInput2));

            //var testInput3 = "^ENNWSWW(NEWS|)SSSEEN(WNSE|)EE(SWEN|)NNN$";
            //Console.WriteLine(DoorsToFurthest(testInput3));

            //var testInput4 = "^ESSWWN(E|NNENN(EESS(WNSE|)SSS|WWWSSSSE(SW|NNNE)))$";
            //Console.WriteLine(DoorsToFurthest(testInput4));

            //var testInput5 = "^WSSEESWWWNW(S|NENNEEEENN(ESSSSW(NWSW|SSEN)|WSWWN(E|WWS(E|SS))))$";
            //Console.WriteLine(DoorsToFurthest(testInput5));
            
            var input = GetFromFile(20).First();
            var result = DoorsToFurthest(input);
            Print(result.max, result.paths1k);
        }
    }
}
