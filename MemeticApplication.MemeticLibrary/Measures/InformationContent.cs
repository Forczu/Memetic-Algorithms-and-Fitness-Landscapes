using MemeticApplication.MemeticLibrary.Landscapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemeticApplication.MemeticLibrary.Measures
{
    /// <summary>
    /// Enables to run information content operation on random walk results.
    /// </summary>
    public static class InformationContent
    {
        /// <summary>
        /// Number of imporant digits after the comma
        /// </summary>
        private static readonly int DIGITS_AFTER_COMMA = 3;

        private static readonly int LOG_BASE = 6;

        public static float Run(RandomWalk rw, float epsilonNormalized)
        {
            float epsilon = DenormalizeEpsilon(rw, epsilonNormalized);
            ChangeSequence sequence = new ChangeSequence(rw, epsilon);
            var probabilities = GetProbabilities(sequence);
            float entropy = GetEntropy(probabilities);
            return entropy;
        }

        internal static float DenormalizeEpsilon(RandomWalk rw, float epsilonNormalized)
        {
            float minRw = rw.Min(), maxRw = rw.Max();
            float l = maxRw - minRw;
            float min = 0.0f, max = l;
            return epsilonNormalized * (max - min) + min;
        }

        internal static Dictionary<Tuple<ChangeSymbol, ChangeSymbol>, float> GetProbabilities(ChangeSequence sequence)
        {
            var occurrences = new Dictionary<Tuple<ChangeSymbol, ChangeSymbol>, int>();
            foreach (ChangeSymbol symbol1 in Enum.GetValues(typeof(ChangeSymbol)))
            {
                foreach (ChangeSymbol symbol2 in Enum.GetValues(typeof(ChangeSymbol)))
                {
                    occurrences.Add(new Tuple<ChangeSymbol, ChangeSymbol>(symbol1, symbol2), 0);
                }
            }
            for (int i = 0; i < sequence.Size - 1; ++i)
            {
                ChangeSymbol symbol1 = sequence.Get(i);
                ChangeSymbol symbol2 = sequence.Get(i + 1);
                var key = occurrences.Keys.Where(k => k.Item1 == symbol1 && k.Item2 == symbol2).Single();
                ++occurrences[key];
            }
            var probabilities = new Dictionary<Tuple<ChangeSymbol, ChangeSymbol>, float>();
            foreach (var pair in occurrences)
            {
                probabilities.Add(pair.Key, (float)pair.Value / (sequence.Size - 1));
            }
            return probabilities;
        }

        private static float GetEntropy(Dictionary<Tuple<ChangeSymbol, ChangeSymbol>, float> probabilities)
        {
            return (float)-probabilities.Where(p => p.Key.Item1 != p.Key.Item2).Sum(p => p.Value * Math.Log(p.Value, LOG_BASE));
        }
    }
}
