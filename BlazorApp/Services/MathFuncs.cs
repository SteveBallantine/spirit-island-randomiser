using System;
using System.Collections.Generic;
using System.Linq;

namespace SiRandomizer.Services
{
    public static class MathFuncs
    {
        /// <summary>
        /// Calculate the factorial of the specified value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double Factorial(int value) 
        {
            if(value < 0) 
            { 
                throw new ArgumentException(
                    $"Value must be >= 0 (is {value})", nameof(value)); 
            }

            double result = 1;
            if(value > 1)
            {
                var factorial = Factorial(value - 1);
                result = value * factorial;
            }
            if(double.IsInfinity(result))
            {
                throw new ArgumentException(
                    $"Result too large for {value}", nameof(value)); 
            }
            return result;
        }
    }
}