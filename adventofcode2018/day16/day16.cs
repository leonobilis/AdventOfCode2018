using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace adventofcode2018
{
    using static Utils;

    public static class Day16
    {
        static List<int> GetRegisters(string input)
        {
            return input.Skip(9).Take(10).Where(x => x != ',' && x != ' ').Select(s => s - 48).ToList();
        }

        static Dictionary<string, Action<int, int, int, List<int>>> GetInstructions()
        {
            return new Dictionary<string, Action<int, int, int, List<int>>>
            {
               { "addr" , (a, b, c, reg) => reg[c] = reg[a] + reg[b]},
               { "addi" , (a, b, c, reg) => reg[c] = reg[a] + b},
               { "mulr" , (a, b, c, reg) => reg[c] = reg[a] * reg[b]},
               { "muli" , (a, b, c, reg) => reg[c] = reg[a] * b},
               { "banr" , (a, b, c, reg) => reg[c] = reg[a] & reg[b]},
               { "bani" , (a, b, c, reg) => reg[c] = reg[a] & b},
               { "borr" , (a, b, c, reg) => reg[c] = reg[a] | reg[b]},
               { "bori" , (a, b, c, reg) => reg[c] = reg[a] | b},
               { "setr" , (a, b, c, reg) => reg[c] = reg[a]},
               { "seti" , (a, b, c, reg) => reg[c] = a},
               { "gtir" , (a, b, c, reg) => reg[c] = a > reg[b] ? 1 : 0},
               { "gtri" , (a, b, c, reg) => reg[c] = reg[a] > b ? 1 : 0},
               { "gtrr" , (a, b, c, reg) => reg[c] = reg[a] > reg[b] ? 1 : 0},
               { "eqir" , (a, b, c, reg) => reg[c] = a == reg[b] ? 1 : 0},
               { "eqri" , (a, b, c, reg) => reg[c] = reg[a] == b ? 1 : 0},
               { "eqrr" , (a, b, c, reg) => reg[c] = reg[a] == reg[b] ? 1 : 0},
            };
        }

        static bool CheckInstruction(Action<int, int, int, List<int>> instruction, List<int> args, List<int> before, List<int> after)
        {
            var tmpReg = new List<int> (before);
            instruction(args[0], args[1], args[2], tmpReg);
            return Enumerable.SequenceEqual(tmpReg, after);

        }

        public static int CheckSamples(IEnumerable<string> input)
        {
            var instructions = GetInstructions();

            var toTest = input.Select((x, i) => new { Index = i, Value = x })
                              .GroupBy(x => x.Index / 4)
                              .Select(x => x.Select(v => v.Value).Take(3))
                              .Select(s => new { before = GetRegisters(s.First()), instruction = s.Skip(1).First().Split().Select(Int32.Parse), after = GetRegisters(s.Last()) })
                              .ToList();
            
            return toTest.Select(t => instructions.Count(i => CheckInstruction(i.Value, t.instruction.Skip(1).ToList(), t.before, t.after)))
                         .Where(x => x >= 3).Count();
        }

        public static Dictionary<int, Action<int, int, int, List<int>>> DetermineInstructions(IEnumerable<string> input)
        {
            var instructions = GetInstructions();

            var toTest = input.Select((x, i) => new { Index = i, Value = x })
                              .GroupBy(x => x.Index / 4)
                              .Select(x => x.Select(v => v.Value).Take(3))
                              .Select(s => new { before = GetRegisters(s.First()), instruction = s.Skip(1).First().Split().Select(Int32.Parse), after = GetRegisters(s.Last()) })
                              .ToList();
            
            var inst = instructions.Select(i => new {instruction = i.Key, opcode = toTest.Select(t => new { Key = t.instruction.First(), Value = CheckInstruction(i.Value, t.instruction.Skip(1).ToList(), t.before, t.after) })
                                                                                         .GroupBy(x => x.Key)
                                                                                         .Where(x => x.All(x2 => x2.Value))
                                                                                         .Select(s => s.Key).ToList() });
            
            while(inst.Any(x => x.opcode.Count > 1))
            {
                var ready = inst.Where(x => x.opcode.Count == 1).Select(x => x.opcode[0]).ToHashSet();
                inst = inst.Select(s => new { s.instruction, opcode = s.opcode.Where(x => s.opcode.Count == 1 || !ready.Contains(x)).ToList() });
            }

            return inst.ToDictionary(k => k.opcode.First(), v => instructions[v.instruction]);
        }

        public static int RunProgram(IEnumerable<string> input1, IEnumerable<string> input2)
        {
            var registers = new List<int> {0, 0, 0, 0};
            var instructions = DetermineInstructions(input1);
            var program = input2.Select(s => s.Split().Select(Int32.Parse).ToArray()).ToList();

            program.ForEach( p => instructions[p[0]](p[1], p[2], p[3], registers) );

            return registers[0];
        }

        public static void Solution()
        {
            var input1 = File.ReadAllLines("adventofcode2018/day16/input1.txt");
            var input2 = File.ReadAllLines("adventofcode2018/day16/input2.txt");
            Print(CheckSamples(input1), RunProgram(input1, input2));
        }
    }
}