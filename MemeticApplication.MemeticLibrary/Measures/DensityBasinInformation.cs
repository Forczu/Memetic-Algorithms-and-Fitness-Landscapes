using MemeticApplication.MemeticLibrary.Landscapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemeticApplication.MemeticLibrary.Measures
{
    public static class DensityBasinInformation
    {
        /// <summary>
        /// Number of imporant digits after the comma
        /// </summary>
        private static readonly int DIGITS_AFTER_COMMA = 3;

        private static readonly int LOG_BASE = 3;

        public static float Run(RandomWalk rw, float epsilonNormalized)
        {
            float epsilon = InformationContent.DenormalizeEpsilon(rw, epsilonNormalized);
            ChangeSequence sequence = new ChangeSequence(rw, epsilon);
            var probabilities = InformationContent.GetProbabilities(sequence);
            float entropy = GetEntropy(probabilities);
            return entropy;
        }

        private static float GetEntropy(Dictionary<Tuple<ChangeSymbol, ChangeSymbol>, float> probabilities)
        {
            return (float)-probabilities.Where(p => p.Key.Item1 == p.Key.Item2).Sum(p => p.Value * Math.Log(p.Value, LOG_BASE));
        }
    }
}
