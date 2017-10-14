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
    public class OrderCrossover : CrossoverOperator
    {
        private static readonly string ID = "OX";

        public override object Clone()
        {
            return new OrderCrossover();
        }

        public override string GetId()
        {
            return ID;
        }

        /// <summary>
        /// Runs the OX operator.
        /// </summary>
        /// <param name="parent1">The first parent.</param>
        /// <param name="parent2">The second parent.</param>
        /// <param name="child1">The first child.</param>
        /// <param name="child2">The second child.</param>
        public override void Run(IGene[] parent1, IGene[] parent2, out IGene[] child1, out IGene[] child2)
        {
            int cutPoint1, cutPoint2, customerCount = parent1.Count();
            RandomGeneratorThreadSafe.NextTwoIntsFirstBigger(1, customerCount - 1, out cutPoint1, out cutPoint2);
            IGene[] c1 = new IGene[customerCount];
            IGene[] c2 = new IGene[customerCount];
            Array.Copy(parent1, cutPoint1, c1, cutPoint1, cutPoint2 - cutPoint1);
            Array.Copy(parent2, cutPoint1, c2, cutPoint1, cutPoint2 - cutPoint1);

            int iterations = customerCount - (cutPoint2 - cutPoint1);
            int parentPosition2 = cutPoint2 - 1, parentPosition1 = cutPoint2 - 1;
            for (int i = 0; i < iterations; ++i)
            {
                int position = (cutPoint2 + i) % customerCount;
                FillEmptyGenes(position, ref parentPosition2, parent2, c1);
                FillEmptyGenes(position, ref parentPosition1, parent1, c2);
            }
            child1 = c1;
            child2 = c2;
        }

        /// <summary>
        /// Fills empty place in chromosome with genes from second parent.
        /// </summary>
        /// <param name="childPosition">Index in child chromosome.</param>
        /// <param name="sourceParentPosition">Index in parent chromosome.</param>
        /// <param name="sourceParent">Source of genes.</param>
        /// <param name="child">Output child.</param>
        private void FillEmptyGenes(int childPosition, ref int sourceParentPosition, IGene[] sourceParent, IGene[] child)
        {
            for (int j = 0; j < child.Length; ++j)
            {
                sourceParentPosition = (++sourceParentPosition) % child.Length;
                IGene inner = sourceParent[sourceParentPosition];
                if (!child.Contains(inner))
                {
                    child[childPosition] = inner;
                    break;
                }
            }
        }
    }
}
