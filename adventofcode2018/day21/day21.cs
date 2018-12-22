using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace adventofcode2018
{
    using static Utils;

    public static class Day21
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

        public static (int fewest, int most) RunProgram(IEnumerable<string> input)
        {   
            var instructions = GetInstructions();         
            var registers = new List<int> {0, 0, 0, 0, 0, 0};
            var ip = input.First()[4] - 48;
            var program = input.Skip(1)
                               .Select(s => s.Split())
                               .Select(s => new {inst = s[0], args = s.Skip(1).Select(Int32.Parse).ToArray()})
                               .ToList();
            
            var reg5Values = new HashSet<int>();

            for (; registers[ip] != 28 || reg5Values.Add(registers[5]); ++registers[ip])
            {
                var p = program[registers[ip]];
                instructions[p.inst](p.args[0], p.args[1], p.args[2], registers);
            }                   
            
            return (reg5Values.First(), reg5Values.Last());
        }

        public static void Solution()
        {            
            var input = GetFromFile(21);
            var result = RunProgram(input);
            Print(result.fewest, result.most);
        }
    }
}