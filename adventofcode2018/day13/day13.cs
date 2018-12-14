using System;
using System.Collections.Generic;
using System.Linq;

namespace adventofcode2018
{
    using static Utils;

    enum Direction {up, down, right, left, straight}
    
    class Cart 
    {
        public ValueTuple<int, int> position;
        Direction direction;
        Direction next;

        static Dictionary<ValueTuple<char, Direction>, Direction> changeDirection = new Dictionary<ValueTuple<char, Direction>, Direction>
            {
                { ('/', Direction.up), Direction.right },
                { ('/', Direction.down), Direction.left },
                { ('/', Direction.left), Direction.down },
                { ('/', Direction.right), Direction.up },
                { ('\\', Direction.up), Direction.left },
                { ('\\', Direction.down), Direction.right },
                { ('\\', Direction.left), Direction.up },
                { ('\\', Direction.right), Direction.down }
            };
        static Dictionary<ValueTuple<Direction, Direction>, ValueTuple<Direction, Direction>> changeDirectionIntersection = new Dictionary<ValueTuple<Direction, Direction>, ValueTuple<Direction, Direction>>
        {
            { (Direction.up, Direction.straight), (Direction.up, Direction.right) },
            { (Direction.down, Direction.straight), (Direction.down, Direction.right) },
            { (Direction.left, Direction.straight), (Direction.left, Direction.right) },
            { (Direction.right, Direction.straight), (Direction.right, Direction.right) },

            { (Direction.up, Direction.left), (Direction.left, Direction.straight) },
            { (Direction.down, Direction.left), (Direction.right, Direction.straight) },
            { (Direction.left, Direction.left), (Direction.down, Direction.straight) },
            { (Direction.right, Direction.left), (Direction.up, Direction.straight) },

            { (Direction.up, Direction.right), (Direction.right, Direction.left) },
            { (Direction.down, Direction.right), (Direction.left, Direction.left) },
            { (Direction.left, Direction.right), (Direction.up, Direction.left) },
            { (Direction.right, Direction.right), (Direction.down, Direction.left) }
        };
        
        
        public Cart (ValueTuple<int, int> pos, char c)
        {
            position = pos;
            if (c == '^') direction = Direction.up;
            else if (c == 'v') direction = Direction.down;
            else if (c == '<') direction = Direction.left;
            else if (c == '>') direction = Direction.right;
            next = Direction.left;
        }

        public void Move()
        {
            switch(direction)
            {
                case Direction.up: position.Item2 -= 1; break;
                case Direction.down: position.Item2 += 1; break;
                case Direction.left: position.Item1 -= 1; break;
                case Direction.right: position.Item1 += 1; break;
            }
        }

        public ValueTuple<int, int> PotentialMove()
        {
            switch(direction)
            {
                case Direction.up: return (position.Item1, position.Item2 - 1);
                case Direction.down: return (position.Item1, position.Item2 + 1);
                case Direction.left: return (position.Item1 - 1, position.Item2);
                case Direction.right: return (position.Item1 + 1, position.Item2);
            }

            return position;
        }

        public void CheckPath(char path)
        {
            if (path == '+')
            {
                var change = changeDirectionIntersection[(direction, next)];
                direction = change.Item1;
                next = change.Item2;
            }
            else
                direction = changeDirection.GetValueOrDefault((path, direction), direction);  
        }

        public bool IsPotencialCrash(Cart other)
        {
            return this.PotentialMove().Item1 == other.position.Item1 && this.PotentialMove().Item2 == other.position.Item2;
        }
    }

    public static class Day13
    {
        static char Path(char c)
        {
            if (c == '^' || c == 'v') return '|';
            if (c == '<' || c == '>') return '-';
            return c;
        }

        public static ValueTuple<int, int> FirstCrash(IEnumerable<string> input)
        {
            var grid = input.SelectMany((s, y) => s.Select((s2, x) =>  ((x, y), s2))
                                                  .Where(x => x.Item2 != ' '));
            var map = grid.Select(s => (s.Item1, Path(s.Item2)))
                          .ToDictionary(k => k.Item1, v => v.Item2);
            
            var carts = grid.Where(x => x.Item2 == '^' || x.Item2 == 'v' || x.Item2 == '<' || x.Item2 == '>')
                            .Select(s => new Cart(s.Item1, s.Item2))
                            .ToList();
            var crash = (-1,-1);

            while(crash.Item1 == -1)
            {
                var positions = carts.Select(s => s.position).ToList();
                carts.ForEach( c => c.Move() );
                carts.ForEach( c => c.CheckPath(map[c.position]) );
                var potential_crash = carts.SelectMany((s, i) => positions.Where((x, i2)  => i != i2 && x.Item1 == s.position.Item1 && x.Item2 == s.position.Item2));
                crash = potential_crash.Any() ? potential_crash.First() : crash; 
            }
            return crash;
        }

        static ValueTuple<int, int> LastCart(IEnumerable<string> input)
        {
            var grid = input.SelectMany((s, y) => s.Select((s2, x) =>  ((x, y), s2))
                                                  .Where(x => x.Item2 != ' '));
            var map = grid.Select(s => (s.Item1, Path(s.Item2)))
                          .ToDictionary(k => k.Item1, v => v.Item2);
            
            var carts = grid.Where(x => x.Item2 == '^' || x.Item2 == 'v' || x.Item2 == '<' || x.Item2 == '>')
                            .Select(s => new Cart(s.Item1, s.Item2))
                            .ToList();

            while(carts.Count != 1)
            {
                carts = carts.OrderBy(c => (c.position.Item2, c.position.Item1)).ToList();
                 var toRemove = new HashSet<Cart>();
                 foreach(var c in carts)
                 {
                    var potential_crash_inner = carts.Where((x => x != c && c.IsPotencialCrash(x)));
                    if (potential_crash_inner.Any())
                    {
                        toRemove.Add(c);
                        toRemove.Add(potential_crash_inner.First());
                    }
                    else
                    {
                        c.Move();
                        c.CheckPath(map[c.position]);
                    }
                }
                toRemove.ToList().ForEach( c => carts.Remove(c));
            }

            return carts.First().position;
        }

        public static void Solution()
        {
            // var testInput = File.ReadAllLines("adventofcode2018/day13/test_input.txt");
            // Console.WriteLine(FirstCrash(testInput));

            // var testInput2 = File.ReadAllLines("adventofcode2018/day13/test_input2.txt");
            // Console.WriteLine(LastCart(testInput2));

            var input = GetFromFile(13);
            Print(FirstCrash(input), LastCart(input));
        }
    }
}
