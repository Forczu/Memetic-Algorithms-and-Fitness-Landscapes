using MemeticApplication.MemeticLibrary.Landscapes;
using MemeticApplication.MemeticLibrary.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemeticApplication.MemeticLibrary.Measures
{
    /// <summary>
    /// Enables to run autocorrelaction operation on random walk results.
    /// </summary>
    public static class Autocorrelation
    {
        /// <summary>
        /// Number of imporant digits after the comma
        /// </summary>
        private static readonly int DIGITS_AFTER_COMMA = 3;

        /// <summary>
        /// Runs the autocorrelation and returns its length (a tau value in literature).
        /// </summary>
        /// <param name="rw">Random walk data.</param>
        /// <returns></returns>
        public static float Run(RandomWalk rw)
        {
            float mean = rw.Average();
            float var = rw.Variance(DIGITS_AFTER_COMMA);
            float e = StandardStatisticsUtils.ExpectedValue(GetNumeratorValues(rw, mean), DIGITS_AFTER_COMMA);
            float ro = e / var;
            return 1.0f / ro;
        }

        /// <summary>
        /// Prepares an array with values for calculating an expected value.
        /// </summary>
        /// <param name="rw">Random walk data.</param>
        /// <param name="mean">Average fitness in random walk.</param>
        /// <returns></returns>
        private static float[] GetNumeratorValues(RandomWalk rw, float mean)
        {
            float[] numeratorValues = new float[rw.Size - 1];
            for (int i = 0; i < rw.Size - 1; ++i)
            {
                numeratorValues[i] = (rw[i] - mean) * (rw[i + 1] - mean);
            }
            return numeratorValues;
        }
    }
}
