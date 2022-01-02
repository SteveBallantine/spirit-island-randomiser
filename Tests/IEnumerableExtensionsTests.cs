using Microsoft.VisualStudio.TestTools.UnitTesting;
using SiRandomizer.Services;
using SiRandomizer.Data;
using SiRandomizer.Exceptions;
using SiRandomizer.Extensions;
using System.Linq;
using System;
using System.Collections.Generic;

namespace SiRandomizer.tests
{
    [TestClass]
    public class IEnumerableExtensionsTests
    {
        /// <summary>
        /// Test some valid inputs to the 'GetCombinations' extension method.
        /// </summary>
        [DataTestMethod]
        [DataRow(1, 1, 1)]
        [DataRow(2, 1, 2)]
        [DataRow(2, 2, 1)]
        [DataRow(3, 1, 3)]
        [DataRow(3, 2, 3)]
        [DataRow(3, 3, 1)]
        [DataRow(4, 1, 4)]
        [DataRow(4, 2, 6)]
        [DataRow(4, 3, 4)]
        [DataRow(4, 4, 1)]
        [DataRow(10, 1, 10)]
        [DataRow(10, 2, 45)]
        [DataRow(10, 3, 120)]
        [DataRow(10, 4, 210)]
        [DataRow(10, 5, 252)]
        [DataRow(10, 6, 210)]
        [DataRow(10, 7, 120)]
        [DataRow(10, 8, 45)]
        [DataRow(10, 9, 10)]
        [DataRow(10, 10, 1)]
        public void Combinations(int itemsInList, int itemsToPick, int expectedCombinations)
        {
            var list = Enumerable.Range(1, itemsInList).ToList();            
            var result = list.GetCombinations(itemsToPick);

            Assert.AreEqual(expectedCombinations, result);
        }

    }
}
