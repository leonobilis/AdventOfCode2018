using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;

namespace adventofcode2018
{
    using static Utils;

    public static class Day19
    {  
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

        public static int RunProgram(IEnumerable<string> input)
        {   
            var instructions = GetInstructions();         
            var registers = new List<int> {0, 0, 0, 0, 0, 0};
            var ip = input.First()[4] - 48;
            var program = input.Skip(1)
                               .Select(s => s.Split())
                               .Select(s => new {inst = s[0], args = s.Skip(1).Select(Int32.Parse).ToArray()})
                               .ToList();

            for (; registers[ip] < program.Count; ++registers[ip])
            {
                var p = program[registers[ip]];
                instructions[p.inst](p.args[0], p.args[1], p.args[2], registers);
            }                   
            
            return registers[0];
        }

        public static int Part2()
        {
            var reg4 = 10551370;
            return Enumerable.Range(1, reg4/2).Where(x => reg4 % x == 0).Sum() + reg4;
        }
        public static void Solution()
        {
//             var testInput = @"#ip 0
// seti 5 0 1
// seti 6 0 2
// addi 0 1 0
// addr 1 2 3
// setr 1 0 0
// seti 8 0 4
// seti 9 0 5".Split("\n");
            //Console.WriteLine(RunProgram(testInput));
            
            var input = GetFromFile(19);
            Print(RunProgram(input), Part2());
        }
    }
}