using System;
using System.Collections.Generic;
using System.Linq;
using SiRandomizer.Services;

namespace SiRandomizer.Extensions
{
    public static class IEnumerableExtensions
    {
        private static readonly Random _rng = new Random();

        public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> collection, int count) {
            var tempList = collection.ToList();
            for(int i = 0; i < count; i++) {
                var pick = _rng.Next(0, tempList.Count);
                yield return tempList.ElementAt(pick);
                tempList.RemoveAt(pick);
            }
        }

        /// <sumamry>
        /// Get count of possible combinations.
        /// This assumes that each item from the collection can only be chosen once and
        /// that choosing A, then B is the same as choosing B, then A.
        /// </sumamry>
        public static long GetCombinations<T>(this IEnumerable<T> collection, int itemsToChoose)  
        {
            // We can use Pascal's triangle to figure this out given:
            // n = number of items in the complete set (e.g. number of spirits available)
            // k = number of items to be chosen from the set   
            // possible combinations = n! / (k! * (n - k)!)
            var n = collection.Count();
            var k = itemsToChoose;
            var result = MathFuncs.Factorial(n) / (MathFuncs.Factorial(k) * MathFuncs.Factorial(n - k));
            if(result > long.MaxValue) 
            { 
                throw new ArgumentException($"Values too large. " +
                    $"Result '{result}' is > {long.MaxValue} for " +
                    $"{collection.Count()} items and {itemsToChoose} picks"); 
            }
            return (long)result;
        }

    }
}
