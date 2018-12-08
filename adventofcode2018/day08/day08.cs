using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace adventofcode2018
{
    using static Utils;

    struct Node 
    {
        public List<Node> childrens;
        public List<int> metadata;

        public int SumMetadata()
        {
            return childrens.Aggregate(metadata.Sum(), (acc, x) => acc+x.SumMetadata());
        }

        public int SumMetadata2()
        {   
            if (childrens.Count > 0)
            {   
                var childrens = this.childrens;
                return metadata.Select(m => m -1).Where(m => m < childrens.Count).Aggregate(0, (acc, x) => acc + childrens[x].SumMetadata2());
            }
            else
                return metadata.Sum();
        }
    }

    public static class Day08
    {
        static Node GetNode(this IEnumerable<int> input)
        {
            IEnumerable<int> end;
            return input.GetNode(out end);
        }

        static Node GetNode(this IEnumerable<int> input, out IEnumerable<int> end)
        {
            var header = input.Take(2);
            var numChildren = header.First();
            var numMetadata = header.Last();
            IEnumerable<int> metadataStart;

            var node = new Node
                        {
                            childrens = input.Skip(2).GetChildrens(numChildren, out metadataStart),
                            metadata = metadataStart.Take(numMetadata).ToList()
                        };
            end = metadataStart.Skip(numMetadata);
            return node;
        }

        static List<Node> GetChildrens(this IEnumerable<int> input, int number, out IEnumerable<int> end)
        {
            var childrens = new List<Node>();
            IEnumerable<int> next = input;
            for (var i = 0; i<number; ++i)
            {
                childrens.Add(next.GetNode(out next));
            }
            end = next;
            return childrens;
        }

        public static int p1(IEnumerable<int> input)
        {
           return input.GetNode().SumMetadata();
        }

        public static int p2(IEnumerable<int> input)
        {
            
           return input.GetNode().SumMetadata2();
        }

        public static void Solution()
        {
            // var testInput = "2 3 0 3 10 11 12 1 1 0 1 99 2 1 1 2".Split().Select(Int32.Parse);
            // Console.WriteLine(Day08.p1(testInput));
            // Console.WriteLine(Day08.p2(testInput));

            var input = GetFromFile(8).First().Split().Select(Int32.Parse);
            Print(p1(input), p2(input));
        }
    }
}