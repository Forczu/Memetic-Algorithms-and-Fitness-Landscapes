using MemeticApplication.MemeticLibrary.Landscapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemeticApplication.MemeticLibrary.Measures
{
    public static class PartialInformationContent
    {
        public static float Run(RandomWalk rw, float epsilonNormalized)
        {
            float epsilon = InformationContent.DenormalizeEpsilon(rw, epsilonNormalized);
            ChangeSequence sequence = new ChangeSequence(rw, epsilon);
            float mu = EstimateSlopeNumberIterative(sequence);
            //float mu = EstimateSlopeNumberRecursive(sequence, 0, 0, 0);
            float pic = mu / sequence.Size;
            return pic;
        }

        private static int EstimateSlopeNumberIterative(ChangeSequence sequence)
        {
            int slopes = 0;
            ChangeSymbol lastSymbol = ChangeSymbol.Zero, currentSymbol;
            for (int i = 0; i < sequence.Size; ++i)
            {
                currentSymbol = sequence.Get(i);
                if ((currentSymbol == ChangeSymbol.One && lastSymbol != ChangeSymbol.One) ||
                    (currentSymbol == ChangeSymbol.BarOne && lastSymbol != ChangeSymbol.BarOne))
                {
                    ++slopes;
                    lastSymbol = currentSymbol;
                }
            }
            return slopes;
        }

        private static int EstimateSlopeNumberRecursive(ChangeSequence sequence, int i, int j, int k)
        {
            if (i >= sequence.Size)
                return k;
            if (j == 0 && sequence.Get(i) != ChangeSymbol.Zero)
                return EstimateSlopeNumberRecursive(sequence, i + 1, i, k + 1);
            if (j > 0  && sequence.Get(i) != ChangeSymbol.Zero && sequence.Get(i) != sequence.Get(j))
                return EstimateSlopeNumberRecursive(sequence, i + 1, i, k + 1);
            return EstimateSlopeNumberRecursive(sequence, i + 1, j, k);
        }
    }
}
