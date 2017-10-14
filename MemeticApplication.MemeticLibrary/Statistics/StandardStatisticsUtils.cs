using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemeticApplication.MemeticLibrary.Statistics
{
    public static class StandardStatisticsUtils
    {
        /// <summary>
        /// Returns the variance of a sequence.
        /// </summary>
        /// <param name="data">Input data.</param>
        /// <param name="digitsAfterComma">Number of important digits after comma.</param>
        /// <returns></returns>
        public static float Variance(float[] data, int digitsAfterComma)
        {
            float[] poweredData = data.Select(f => f * f).ToArray();
            float expectedValue = ExpectedValue(data, digitsAfterComma);
            float expectedValuePower = expectedValue * expectedValue;
            float variance = ExpectedValue(poweredData, digitsAfterComma) - expectedValuePower;
            return variance;
        }

        /// <summary>
        /// Returns the expected value of a sequence.
        /// </summary>
        /// <param name="data">Input data.</param>
        /// <param name="numbersAfterComma">Number of important digits after comma.</param>
        /// <returns></returns>
        public static float ExpectedValue(float[] data, int numbersAfterComma)
        {
            var occurences = new Dictionary<decimal, int>();
            foreach (var fitness in data)
            {
                decimal fit = Math.Round((Decimal)fitness, numbersAfterComma, MidpointRounding.AwayFromZero);
                if (!occurences.ContainsKey(fit))
                    occurences.Add(fit, 1);
                else
                    ++occurences[fit];
            }
            float expectedValue = 0;
            foreach (var pair in occurences)
            {
                expectedValue += (float)pair.Key * (pair.Value / (float)data.Length);
            }
            return expectedValue;
        }
    }
}
