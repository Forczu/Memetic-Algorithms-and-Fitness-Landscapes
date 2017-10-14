using MemeticApplication.MemeticLibrary.Landscapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemeticApplication.MemeticLibrary.Measures
{
    /// <summary>
    /// Symbols representing changes.
    /// One is an improvement of a fitness value.
    /// Zero is lack of changes in fitness value.
    /// BarOne is an reduction of a fitness value.
    /// </summary>
    public enum ChangeSymbol
    {
        One, Zero, BarOne
    }

    /// <summary>
    /// Represents the change of values a during random walk
    /// </summary>
    public class ChangeSequence
    {
        IList<ChangeSymbol> symbols = null;

        public int Size { get { return symbols.Count; } }

        protected float Epsilon { get; set; }

        public ChangeSequence(float epsilon = 0.0f)
        {
            Epsilon = epsilon;
            symbols = new List<ChangeSymbol>();
        }
        public ChangeSequence(RandomWalk rw, float epsilon) : this(epsilon)
        {
            float iFitness, iMinusOneFitness;
            for (int i = 1; i < rw.Size; ++i)
            {
                iFitness = rw[i];
                iMinusOneFitness = rw[i - 1];
                Add(iFitness, iMinusOneFitness);
            }
        }

        public void Add(ChangeSymbol symbol)
        {
            symbols.Add(symbol);
        }

        public ChangeSymbol Get(int index)
        {
            return symbols[index];
        }

        public void Add(float iFitness, float iMinusOnefitness)
        {
            ChangeSymbol symbolToAdd;
            if (iFitness - iMinusOnefitness < -Epsilon)
                symbolToAdd = ChangeSymbol.BarOne;
            else if (iFitness - iMinusOnefitness > Epsilon)
                symbolToAdd = ChangeSymbol.One;
            else
                symbolToAdd = ChangeSymbol.Zero;
            symbols.Add(symbolToAdd);
        }
    }
}
