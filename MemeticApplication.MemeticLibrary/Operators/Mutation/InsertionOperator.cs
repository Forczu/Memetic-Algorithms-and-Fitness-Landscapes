using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemeticApplication.MemeticLibrary.Genetic;
using MemeticApplication.MemeticLibrary.Generators;

namespace MemeticApplication.MemeticLibrary.Operators.Mutation
{
    public class InsertionOperator : IMutationOperator
    {
        public void Run(ref Chromosome solution)
        {
            int indexOfElement, indexOfDestination;
            RandomGenerator.NextTwoDifferentInts(solution.Genes.Length, out indexOfElement, out indexOfDestination);
            Insert(solution.Genes, indexOfElement, indexOfDestination);
        }

        public void Run(IGene[] genes, int indexOfElement, int indexOfDestination)
        {
            Insert(genes, indexOfElement, indexOfDestination);
        }

        private void Insert(IGene[] genes, int indexOfElement, int indexOfDestination)
        {
            IGene geneToMove = genes[indexOfElement];
            if (indexOfElement > indexOfDestination)
            {
                for (int i = indexOfElement; i > indexOfDestination; --i)
                {
                    genes[i] = genes[i - 1];
                }
                genes[indexOfDestination] = geneToMove;
            }
            else
            {
                for (int i = indexOfElement; i < indexOfDestination; ++i)
                {
                    genes[i] = genes[i + 1];
                }
                genes[indexOfDestination] = geneToMove;
            }
        }
    }
}
