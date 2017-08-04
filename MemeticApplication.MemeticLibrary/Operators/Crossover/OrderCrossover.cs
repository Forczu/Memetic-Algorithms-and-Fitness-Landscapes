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
    /// Represents the order crossover operation (OX).
    /// </summary>
    public class OrderCrossover : ICrossoverOperator
    {
        /// <summary>
        /// Runs the OX operator.
        /// </summary>
        /// <param name="parent1">The first parent.</param>
        /// <param name="parent2">The second parent.</param>
        /// <param name="child1">The first child.</param>
        /// <param name="child2">The second child.</param>
        public void Run(IGene[] parent1, IGene[] parent2, out IGene[] child1, out IGene[] child2)
        {
            int cutPoint1, cutPoint2, customerCount = parent1.Count();
            RandomGenerator.NextTwoIntsFirstBigger(1, customerCount - 1, out cutPoint1, out cutPoint2);
            IGene[] c1 = new IGene[customerCount];
            IGene[] c2 = new IGene[customerCount];
            Array.Copy(parent1, cutPoint1, c1, cutPoint1, cutPoint2 - cutPoint1);
            Array.Copy(parent2, cutPoint1, c2, cutPoint1, cutPoint2 - cutPoint1);

            int iterations = customerCount - (cutPoint2 - cutPoint1);
            int position2 = cutPoint2 - 1, position1 = cutPoint2 - 1;
            for (int i = 0; i < iterations; ++i)
            {
                int position = (cutPoint2 + i) % customerCount;
                for (int j = 0; j < customerCount; ++j)
                {
                    position2 = (++position2) % customerCount;
                    IGene inner = parent2[position2];
                    if (!c1.Contains(inner))
                    {
                        c1[position] = inner;
                        break;
                    }
                }
                for (int j = 0; j < customerCount; ++j)
                {
                    position1 = (++position1) % customerCount;
                    IGene inner = parent1[position1];
                    if (!c2.Contains(inner))
                    {
                        c2[position] = inner;
                        break;
                    }
                }
            }
            child1 = c1;
            child2 = c2;
        }
    }
}
