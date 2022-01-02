using Microsoft.VisualStudio.TestTools.UnitTesting;
using SiRandomizer.Services;
using SiRandomizer.Data;
using SiRandomizer.Exceptions;
using System.Linq;
using System;

namespace SiRandomizer.tests
{
    [TestClass]
    public class MathFuncTests
    {
        /// <summary>
        /// Test valid inputs for the factorial function
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="expectedResult"></param>
        [DataTestMethod]
        [DataRow(0, 1)]
        [DataRow(1, 1)]
        [DataRow(2, 2)]
        [DataRow(3, 6)]
        [DataRow(4, 24)]
        [DataRow(5, 120)]
        [DataRow(20, 2432902008176640000)]
        public void Factorial_Valid(int parameter, double expectedResult)
        {
            var result = MathFuncs.Factorial(parameter);
            Assert.AreEqual(expectedResult, result);
        }

        /// <summary>
        /// Test invalid inputs for the factorial function
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="expectedResult"></param>
        [DataTestMethod]
        [DataRow(-1)]
        [DataRow(200)]
        [ExpectedException(typeof(ArgumentException))]
        public void Factorial_Invalid(int parameter)
        {
            var result = MathFuncs.Factorial(parameter);
        }
    }
}
