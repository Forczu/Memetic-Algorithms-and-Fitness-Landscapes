using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemeticApplication.MemeticLibrary.Generators;
using MemeticApplication.MemeticLibrary.Genetic;
using MemeticApplication.MemeticLibrary.Model;

namespace MemeticApplication.MemeticLibrary.Operators.Crossover
{
    /// <summary>
    /// Represents the partially matched crossover operation (PMX).
    /// </summary>
    public class PartiallyMatchedCrossover : CrossoverOperator
    {
        private static readonly string ID = "PMX";

        public override object Clone()
        {
            return new PartiallyMatchedCrossover();
        }

        public override string GetId()
        {
            return ID;
        }

        /// <summary>
        /// Runs the PMX operator.
        /// The cut points are chosen randomly.
        /// </summary>
        /// <param name="parent1">The first parent.</param>
        /// <param name="parent2">The second parent.</param>
        /// <param name="child1">The first child.</param>
        /// <param name="child2">The second child.</param>
        public override void Run(IGene[] parent1, IGene[] parent2, out IGene[] child1, out IGene[] child2)
        {
            int cutPoint1, cutPoint2;
            RandomGeneratorThreadSafe.NextTwoIntsFirstBigger(1, parent1.Count() - 1, out cutPoint1, out cutPoint2);
            Run(parent1, parent2, out child1, out child2, cutPoint1, cutPoint2);
        }

        /// <summary>
        /// Runs the PMX operator.
        /// The cut points are passed explicitly.
        /// </summary>
        /// <param name="parent1">The first parent.</param>
        /// <param name="parent2">The second parent.</param>
        /// <param name="child1">The first child.</param>
        /// <param name="child2">The second child.</param>
        /// <param name="cutPoint1">The first cut point.</param>
        /// <param name="cutPoint2">The second cut point.</param>
        public void Run(IGene[] parent1, IGene[] parent2, out IGene[] child1, out IGene[] child2, int cutPoint1, int cutPoint2)
        {
            int customerCount = parent1.Count();

            IGene[] c1 = new IGene[customerCount];
            IGene[] c2 = new IGene[customerCount];

            int middleSize = cutPoint2 - cutPoint1;
            int sourceIndex = cutPoint1 + 1;
            Array.Copy(parent1, sourceIndex, c1, sourceIndex, middleSize);
            Array.Copy(parent2, sourceIndex, c2, sourceIndex, middleSize);
            IGene[] copiedMiddle1 = new IGene[middleSize];
            IGene[] copiedMiddle2 = new IGene[middleSize];
            Array.Copy(parent1, sourceIndex, copiedMiddle1, 0, middleSize);
            Array.Copy(parent2, sourceIndex, copiedMiddle2, 0, middleSize);

            var mappings = new Tuple<IGene, IGene>[middleSize];
            for (int i = 0; i < middleSize; ++i)
                mappings[i] = new Tuple<IGene, IGene>(copiedMiddle1[i], copiedMiddle2[i]);

            int iterations = customerCount - middleSize;
            int position = cutPoint2;
            for (int i = 0; i < iterations; ++i)
            {
                position = (position + 1) % customerCount;
                FillBlankGene(parent2, position, copiedMiddle1, c1, mappings, true);
                FillBlankGene(parent1, position, copiedMiddle2, c2, mappings, false);
            }
            child1 = c1;
            child2 = c2;
        }

        private void FillBlankGene(IGene[] parent, int position, IGene[] copiedMiddle, IGene[] c, Tuple<IGene, IGene>[] mappings, bool upToDown)
        {
            IGene inner = parent[position];
            if (!copiedMiddle.Contains(inner))
            {
                c[position] = inner;
                return;
            }
            while (true)
            {
                var single = mappings.Where(m => (upToDown ? m.Item1.Equals(inner) : m.Item2.Equals(inner))).Single();
                inner = upToDown ? single.Item2 : single.Item1;
                if (!copiedMiddle.Contains(inner))
                    break;
            }
            c[position] = inner;
        }
    }
}
