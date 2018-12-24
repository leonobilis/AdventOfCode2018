using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace adventofcode2018
{
    using static Utils;
 
    enum AttackType { slashing, fire, cold, bludgeoning, radiation };

    abstract class UnitGroup
    {
        public int NumUnits { get; protected set; }
        protected int HP;
        protected int AttackPower;
        protected AttackType AttackType;
        public int Initiative { get; protected set; }
        public bool Alive { get { return NumUnits > 0; } }
        public int EffectiveAttackPower { get { return NumUnits * AttackPower; } }
        protected List<AttackType> Immune;
        protected List<AttackType> Weak;
        
        public UnitGroup(int numUnits, int HP, int attackPower, AttackType attackType, int initiative, List<AttackType> immune, List<AttackType> weak)
        {
            NumUnits = numUnits;
            this.HP = HP;
            AttackPower = attackPower;
            AttackType = attackType;
            Initiative = initiative;
            Immune = immune;
            Weak = weak;
        }

        public int PotencialDamage(int attackPower, AttackType attackType)
        {
            return Immune.Contains(attackType) ? 0 : Weak.Contains(attackType) ? attackPower * 2 : attackPower;
        }

        public UnitGroup SelectEnemy(IEnumerable<UnitGroup> units)
        {
            var enemies = units.Where(s => IsMyEnemy(s) && s.PotencialDamage(EffectiveAttackPower, AttackType) > 0).OrderByDescending(o => (o.PotencialDamage(EffectiveAttackPower, AttackType), o.EffectiveAttackPower, o.Initiative));
            if (enemies.Count() > 1
                && enemies.First().PotencialDamage(EffectiveAttackPower, AttackType) == enemies.Skip(1).First().PotencialDamage(EffectiveAttackPower, AttackType)
                && enemies.First().EffectiveAttackPower == enemies.Skip(1).First().EffectiveAttackPower
                && enemies.First().Initiative == enemies.Skip(1).First().Initiative)
                return null;

            return enemies.Count() > 0 ? enemies.First() : null;
        }

        public void Attack(UnitGroup enemy)
        {
            if (Alive)
            {
                enemy.Attacked(EffectiveAttackPower, AttackType);
            }     
        }

        public void Attacked(int attackPower, AttackType attackType)
        {
            int unitsKill = PotencialDamage(attackPower, attackType) / HP;
            NumUnits -= unitsKill;
        }

        public abstract bool IsMyEnemy(UnitGroup unitGroup);
    }

    class ImmuneSystem : UnitGroup
    {
        public ImmuneSystem(int numUnits, int HP, int attackPower, AttackType attackType, int initiative, List<AttackType> immune, List<AttackType> weak) : base(numUnits,  HP, attackPower, attackType, initiative, immune, weak) {}
        public override bool IsMyEnemy(UnitGroup unitGroup)
        {
            return unitGroup is Infection;
        }   
    }

    class Infection : UnitGroup
    {
        public Infection(int numUnits, int HP, int attackPower, AttackType attackType, int initiative, List<AttackType> immune, List<AttackType> weak) : base(numUnits, HP, attackPower, attackType, initiative, immune, weak) { }
        public override bool IsMyEnemy(UnitGroup unitGroup)
        {
            return unitGroup is ImmuneSystem;
        }
    }

    public static class Day24
    {
        static AttackType GetAttackType(string s)
        {
            switch (s)
            {
                case "fire": return AttackType.fire;
                case "cold": return AttackType.cold;
                case "bludgeoning": return AttackType.bludgeoning;
                case "radiation": return AttackType.radiation;
                default: return AttackType.slashing;
            }
        }

        static List<AttackType> GetWeak(string s)
        {
            Regex weakRx = new Regex(@"( \([ a-z;]*(weak to ([ ,a-z]+))+[ ,a-z;]*\))?", RegexOptions.Compiled);
            var match = weakRx.Matches(s).First().Groups[3].Value;
            return weakRx.Matches(s).First().Groups[3].Value.Split(", ").Where(x => x != "").Select(s1 => GetAttackType(s1)).ToList();
        }

        static List<AttackType> GetImmune(string s)
        {
            Regex immuneRx = new Regex(@"( \([ a-z;]*(immune to ([ ,a-z]+))+[ ,a-z;]*\))?", RegexOptions.Compiled);
            var match = immuneRx.Matches(s).First().Groups[3].Value;
            return immuneRx.Matches(s).First().Groups[3].Value.Split(", ").Where(x => x != "").Select(s1 => GetAttackType(s1)).ToList();
        }

        static Infection CreateInfectionGroup(List<string> parameters)
        {
            return new Infection
                        (
                            Int32.Parse(parameters[0]),
                            Int32.Parse(parameters[1]),
                            Int32.Parse(parameters[3]),
                            GetAttackType(parameters[4]),
                            Int32.Parse(parameters[5]),
                            GetImmune(parameters[2]),
                            GetWeak(parameters[2])
                        );
        }

        static ImmuneSystem CreateImmuneSystemGroup(List<string> parameters, int boost = 0)
        {
            return new ImmuneSystem
                        (
                            Int32.Parse(parameters[0]),
                            Int32.Parse(parameters[1]),
                            Int32.Parse(parameters[3]) + boost,
                            GetAttackType(parameters[4]),
                            Int32.Parse(parameters[5]),
                            GetImmune(parameters[2]),
                            GetWeak(parameters[2])
                        );
        }

        static IEnumerable<UnitGroup> GetUnitGroups(IEnumerable<string> input, int immuneSystemBoost = 0)
        {
            Regex inputRx = new Regex(@"([\d]+) units each with ([\d]+) hit points( \([a-z ,;]+\))? with an attack that does ([\d]+) ([a-z]+) damage at initiative ([\d]+)", RegexOptions.Compiled);

            var immuneSystemGroups = input.Skip(1).TakeWhile(x => x != "")
                                  .Select(s => inputRx.Matches(s).Select(s1 => CreateImmuneSystemGroup(s1.Groups.Skip(1).Select(s2 => s2.Value).ToList(), immuneSystemBoost)).First());

            var infectionGroups = input.Skip(immuneSystemGroups.Count() + 3)
                                  .Select(s => inputRx.Matches(s).Select(s1 => CreateInfectionGroup(s1.Groups.Skip(1).Select(s2 => s2.Value).ToList())).First());

            return immuneSystemGroups.AsEnumerable<UnitGroup>().Concat(infectionGroups).ToList();
        }

        static IEnumerable<UnitGroup> Fight(IEnumerable<UnitGroup> units)
        {
            units = units.Where(x => x.Alive);
            var guard = units.Sum(x => x.NumUnits);

            while (units.Where(x => x is ImmuneSystem).Count() > 0 && units.Where(x => x is Infection).Count() > 0)
            {
                var targetSelection = new Dictionary<UnitGroup, UnitGroup>();

                foreach (var unit in units.OrderByDescending(o => (o.EffectiveAttackPower, o.Initiative)))
                {
                    var enemy = unit.SelectEnemy(units.Except(targetSelection.Values));
                    if (enemy != null)
                        targetSelection.Add(unit, enemy);
                }

                targetSelection.OrderByDescending(o => o.Key.Initiative).ToList().ForEach( x => x.Key.Attack(x.Value) );

                if (guard == units.Sum(x => x.NumUnits))
                    return new List<UnitGroup>();
                guard = units.Sum(x => x.NumUnits);
            }

            return units;
        }

        static int UnitsInWinningArmy(IEnumerable<string> input)
        {
            var units = GetUnitGroups(input);
            return Fight(units).Sum(s => s.NumUnits);
        }

        static int UnitsLeftAfterFightWithMinBoost(IEnumerable<string> input)
        {
            var immuneSystemBoost = 0;
            while (Fight(GetUnitGroups(input, ++immuneSystemBoost)).Count(x => x is ImmuneSystem) == 0);
            return Fight(GetUnitGroups(input, immuneSystemBoost)).Sum(s => s.NumUnits);
        }

        public static void Solution()
        {
            var testInput = @"Immune System:
17 units each with 5390 hit points (weak to radiation, bludgeoning) with an attack that does 4507 fire damage at initiative 2
989 units each with 1274 hit points (immune to fire; weak to bludgeoning, slashing) with an attack that does 25 slashing damage at initiative 3

Infection:
801 units each with 4706 hit points (weak to radiation) with an attack that does 116 bludgeoning damage at initiative 1
4485 units each with 2961 hit points (immune to radiation; weak to fire, cold) with an attack that does 12 slashing damage at initiative 4".Split('\n');

            //Console.WriteLine(UnitsInWinningArmy(testInput));
            //Console.WriteLine(UnitsLeftAfterFightWithMinBoost(testInput));

            var input = GetFromFile(24);
            Print(UnitsInWinningArmy(input), UnitsLeftAfterFightWithMinBoost(input));
        }
    }
}
