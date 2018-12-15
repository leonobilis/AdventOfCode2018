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
    using Map = AdjacencyGraph<(int, int), Edge<(int, int)>>;

    abstract class Unit
    {
        protected Map map;
        public Field Position {get; protected set;}
        public int HP {get; protected set;}
        protected int AttackPower;
        public bool Alive { get { return HP > 0; } }
        protected List<Field> EmptyInRange;
        protected IEnumerable<Unit> EnemyInRange;
        protected IEnumerable<Field> EnemiesRanges;
        
        public Unit(Map map, Field position, int attackPower = 3)
        {
            this.map = map;
            Position = position;
            HP = 200;
            AttackPower = attackPower;
            EmptyInRange = new List<Field>();
            RefreshemptyInRange();
            map.VertexAdded += FieldReleased;
            map.VertexRemoved += FieldOccupied;
        }
        
        void BindUnits(IEnumerable<Unit> units)
        {
            EnemyInRange = units.Where(x => IsMyEnemy(x) && Math.Abs(Position.Item1 - x.Position.Item1) + Math.Abs(Position.Item2 - x.Position.Item2) == 1).OrderBy(o => (o.HP, o.Position));
            EnemiesRanges = units.Where(x => IsMyEnemy(x)).SelectMany(s => s.EmptyInRange).OrderBy(o => o);
        }
        void FieldReleased(Field field)
        {
            if (Math.Abs(Position.Item1 - field.Item1) + Math.Abs(Position.Item2 - field.Item2) == 1)
                EmptyInRange.Add(field);
        }

        void FieldOccupied(Field field)
        {
            if (Math.Abs(Position.Item1 - field.Item1) + Math.Abs(Position.Item2 - field.Item2) == 1)
                EmptyInRange.Remove(field);
        }

        void RefreshemptyInRange()
        {   
            EmptyInRange.Clear();
            if (map.ContainsVertex((Position.Item1 - 1, Position.Item2))) EmptyInRange.Add((Position.Item1 - 1, Position.Item2));
            if (map.ContainsVertex((Position.Item1, Position.Item2 - 1))) EmptyInRange.Add((Position.Item1, Position.Item2 - 1));
            if (map.ContainsVertex((Position.Item1, Position.Item2 + 1))) EmptyInRange.Add((Position.Item1, Position.Item2 + 1));
            if (map.ContainsVertex((Position.Item1 + 1, Position.Item2))) EmptyInRange.Add((Position.Item1 + 1, Position.Item2));
        }

        protected void TryAttack()
        {
            if (EnemyInRange.Any())
                Attack(EnemyInRange.First());
        }

        protected void Attack(Unit enemy)
        {
            enemy.Attacked(AttackPower);
        }

        void Attacked(int attackPower)
        {
            HP -= attackPower;
            if (!Alive) 
            {
                map.AddField(Position);
                EmptyInRange.Clear();
                map.VertexAdded -= FieldReleased;
                map.VertexRemoved -= FieldOccupied;
            }
        }

        void Move(Field where)
        {
            map.AddField(Position);
            Position = where;
            map.RemoveVertex(where);
            RefreshemptyInRange();
        }

        public void TryMove(IEnumerable<Unit> units)
        {
            BindUnits(units);
            
            if (!Alive)
                return;

            if (EnemyInRange.Any())
            {
                Attack(EnemyInRange.First());
                return;
            }

            foreach (var range1 in EnemiesRanges)
                foreach (var range2 in EmptyInRange)
                    if (range1.CompareTo(range2) == 0)
                    {
                        Move(range1);
                        TryAttack();
                        return;
                    }

            IEnumerable<Edge> path;
            IEnumerable<Edge> shorterPath = null;
            Func<Edge, double> edgeCost = e => 1;

            foreach (var range1 in EmptyInRange)
            {
                var tryPath = map.ShortestPathsDijkstra(edgeCost, range1);
                foreach (var range2 in EnemiesRanges)
                {
                    if (tryPath(range2, out path))
                        shorterPath = GetShorterPath(shorterPath, path);
                }
            }
            
            if (shorterPath != null)
            {
                Move(shorterPath.First().Source);
                TryAttack();
                return;
            }
        }

        static IEnumerable<Edge> GetShorterPath(IEnumerable<Edge> path1, IEnumerable<Edge> path2)
        {
            if (path1 == null)
                return path2;
            if (path1.Count() < path2.Count())
                return path1;
            if (path2.Count() < path1.Count())
                return path2;
            if(path1.Last().Target.IsLess(path2.Last().Target))
                return path1;
            if(path2.Last().Target.IsLess(path1.Last().Target))
                return path2;
            if(path1.First().Source.IsLess(path2.First().Source))
                return path1;
            return path2;
        }

        public abstract bool IsMyEnemy(Unit unit);
    }

    class Elf : Unit
    {
        public Elf(Map map, Field position, int attackPower = 3) : base(map, position, attackPower) {}
        public override bool IsMyEnemy(Unit unit)
        {
            return unit is Goblin;
        }   
    }

    class Goblin : Unit
    {
        public Goblin(Map map, Field position) : base(map, position) {}
        public override bool IsMyEnemy(Unit unit)
        {
            return unit is Elf;
        }
    }

    public static class Day15
    {
        public static bool IsLess(this Field field, Field other)
        {
            if (field.Item1 < other.Item1)
                return true;
            if (field.Item1 > other.Item1)
                return false;
            return field.Item2 < other.Item2;
        }

        static void AddBidirectionalEdge(this Map map, Field field1, Field field2)
        {
            if (map.ContainsVertex(field1) && map.ContainsVertex(field2))
            {
                map.AddEdge(new Edge(field1, field2));
                map.AddEdge(new Edge(field2, field1));
            }   
        }
        
        public static void AddField(this Map map, Field field)
        {
            map.AddVertex(field);
            map.AddBidirectionalEdge(field, (field.Item1 -1, field.Item2));
            map.AddBidirectionalEdge(field, (field.Item1, field.Item2 - 1));
            map.AddBidirectionalEdge(field, (field.Item1, field.Item2 + 1));
            map.AddBidirectionalEdge(field, (field.Item1 + 1, field.Item2));
        }

        static Map GetMap(IEnumerable<string> input)
        {
            var grid = input.ToArray();
            var map = new Map();
            var filteredGrid = grid.SelectMany((s, y) => s.Select((s2, x) => new{ Key = (y, x), Value = s2}).Where(x => x.Value == '.'));
            
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

        static IEnumerable<Unit> GetUnits(IEnumerable<string> input, Map map, int elvesAttackPower = 3)
        {
            return input.SelectMany((s, y) => s.Select((s2, x) => new{ Key = (y, x), Value = s2}))
                        .Where(x => x.Value == 'E' || x.Value == 'G')
                        .Select(s => s.Value == 'E' ? (Unit)(new Elf(map, s.Key, elvesAttackPower)) : (Unit)(new Goblin(map, s.Key)))
                        .ToList()
                        .Where(x => x.Alive).OrderBy(o => o.Position);
        }

        public static int Combat(IEnumerable<string> input)
        {
            var map = GetMap(input);
            var units = GetUnits(input, map);
            var round = 0;

            while (true)
            {
                foreach(var unit in units)
                {
                    if (!units.Where(x => unit.IsMyEnemy(x)).Any())
                        return round * units.Select(s => s.HP).Sum();
                    unit.TryMove(units);
                }
                round ++;
            }
        }

        public static bool CombatWithBoost(IEnumerable<string> input, int elvesAttackPower, out int outcome)
        {
            var map = GetMap(input);
            var units = GetUnits(input, map, elvesAttackPower);
            var round = 0;
            var elvesNum = units.Count(x => x is Elf );
            outcome = 0;

            while (true)
            {
                foreach(var unit in units)
                {
                    if (unit is Elf && !units.Where(x => x is Goblin).Any())
                    {
                        outcome = round * units.Select(s => s.HP).Sum();
                        return true;
                    }

                    unit.TryMove(units);

                    if (unit is Goblin && units.Count(x => x is Elf ) < elvesNum)
                        return false;
                }
                round ++;
            }
        }

        public static int ElvesWinCombat(IEnumerable<string> input)
        {
            var elvesAttackPower = 3;
            int outcome;
            while(!CombatWithBoost(input, ++elvesAttackPower, out outcome));
            return outcome;
        }

        public static void Solution()
        {
//             var testInput = @"#######
// #.G...#
// #...EG#
// #.#.#G#
// #..G#E#
// #.....#
// #######".Split();
//                 Console.WriteLine(Combat(testInput));
                
//                 var testInput2 = @"#######
// #G..#E#
// #E#E.E#
// #G.##.#
// #...#E#
// #...E.#
// #######".Split();
//              Console.WriteLine(Combat(testInput2));

//             var testInput3 = @"#########
// #G......#
// #.E.#...#
// #..##..G#
// #...##..#
// #...#...#
// #.G...G.#
// #.....G.#
// #########".Split();
//             Console.WriteLine(Combat(testInput3));

//             var testInput4 = @"#######
// #.G...#
// #...EG#
// #.#.#G#
// #..G#E#
// #.....#
// #######".Split();
//             Console.WriteLine(ElvesWinCombat(testInput4));
            
            var input = GetFromFile(15);
            Print(Combat(input), ElvesWinCombat(input));
        }
    }
}
