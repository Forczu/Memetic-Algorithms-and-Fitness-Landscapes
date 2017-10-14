using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemeticApplication.MemeticLibrary.Generators;
using MemeticApplication.MemeticLibrary.Genetic;
using MemeticApplication.MemeticLibrary.Model;

namespace MemeticApplication.MemeticLibrary.Operators.Mutation
{
    public class SwapOperator : MutationOperator
    {
        private static readonly string ID = "SM";

        public override object Clone()
        {
            return new SwapOperator();
        }

        public override string GetId()
        {
            return ID;
        }

        public override void Run(IGene[] genes)
        {
            int index1, index2, customerCount = genes.Count();
            RandomGeneratorThreadSafe.NextTwoDifferentInts(customerCount, out index1, out index2);
            Swap(genes, index1, index2);
        }

        /// <summary>
        /// Runs the swap operator.
        /// </summary>
        /// <param name="solution">The referenced solution for a change.</param>
        public override void Run(ref Chromosome solution)
        {
            var genes = solution.Genes;
            int index1, index2, customerCount = genes.Count();
            RandomGeneratorThreadSafe.NextTwoDifferentInts(customerCount, out index1, out index2);
            Swap(genes, index1, index2);
            solution.Refresh();
        }

        /// <summary>
        /// Runs the swap operator.
        /// </summary>
        /// <param name="solution">The referenced solution for a change.</param>
        /// <param name="index1">The first swapped element.</param>
        /// <param name="index2">The second swapped element.</param>
        public void Run(IGene[] genes, int index1, int index2)
        {
            Swap(genes, index1, index2);
        }

        /// <summary>
        /// Swaps two customer in the specified vector.
        /// </summary>
        /// <param name="genes">The vector.</param>
        /// <param name="index1">The index of the first element.</param>
        /// <param name="index2">The index of the second element.</param>
        private void Swap(IGene[] genes, int index1, int index2)
        {
            var tmpCustomer = genes[index1];
            genes[index1] = genes[index2];
            genes[index2] = tmpCustomer;
        }
    }
}
