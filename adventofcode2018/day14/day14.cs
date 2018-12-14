using System;
using System.Collections.Generic;
using System.Linq;

namespace adventofcode2018
{
    using static Utils;

    public static class Day14
    {
        public static IEnumerable<int> newRecipes(int a, int b)
        {
            return (a+b).ToString().ToCharArray().Select(x => x - 48);
        }

        public static string RecipesScore(int numRecipes)
        {
            var recipes = new List<int>{3, 7};
            var firstElf = 0;
            var secondElf = 1;
            
            while(recipes.Count < numRecipes + 10)
            {
                recipes.AddRange(newRecipes(recipes[firstElf], recipes[secondElf]));
                firstElf = (firstElf + recipes[firstElf] + 1) % recipes.Count;
                secondElf = (secondElf + recipes[secondElf] + 1) % recipes.Count;
            }

            return Enumerable.Range(numRecipes, 10).Aggregate("", (acc, x) => acc + recipes[x].ToString());
        }
        
        public static int RecipesAppear(int score)
        {
            var take = score.ToString().Count();
            var recipes = new List<int>{3, 7};
            var firstElf = 0;
            var secondElf = 1;

            while(true)
            {
                var skip = recipes.Count - 5;
                recipes.AddRange(newRecipes(recipes[firstElf], recipes[secondElf]));
                
                var toCheck = recipes.Skip(skip).Take(take);
                while(toCheck.Count() == take)
                {
                    if (toCheck.Reverse().Select((s, i) => s * (int)Math.Pow(10, i)).Aggregate(0, (acc, x) => acc + x) == score)
                        return skip;
                    toCheck = recipes.Skip(++skip).Take(take);
                }
                firstElf = (firstElf + recipes[firstElf] + 1) % recipes.Count;
                secondElf = (secondElf + recipes[secondElf] + 1) % recipes.Count;
            }
        }

        public static void Solution()
        {
            // Console.WriteLine(RecipesScore(9));
            // Console.WriteLine(RecipesScore(5));
            // Console.WriteLine(RecipesScore(18));
            // Console.WriteLine(RecipesScore(2018));

            // Console.WriteLine(RecipesAppear(51589));
            // Console.WriteLine(RecipesAppear(01245));
            // Console.WriteLine(RecipesAppear(92510));
            // Console.WriteLine(RecipesAppear(59414));
            
            var input = 430971;
            Print(RecipesScore(input), RecipesAppear(input));
        }
    }
}
