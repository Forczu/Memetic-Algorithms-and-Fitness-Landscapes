using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemeticApplication.MemeticLibrary.Genetic;
using MemeticApplication.MemeticLibrary.Generators;

namespace MemeticApplication.MemeticLibrary.Operators.Mutation
{
    public class InversionOperator : IMutationOperator
    {
        public void Run(ref Chromosome solution)
        {
            int startPosition = RandomGenerator.NextInt(solution.Genes.Length - 1);
            int length = RandomGenerator.NextInt(solution.Genes.Length - startPosition - 2) + 2;
            Inverse(solution.Genes, startPosition, length);
        }

        public void Run(IGene[] genes, int startPosition, int length)
        {
            Inverse(genes, startPosition, length);
        }

        private void Inverse(IGene[] genes, int startPosition, int length)
        {
            int index1, index2;
            for (int i = 0; i < length / 2; ++i)
            {
                index1 = startPosition + i;
                index2 = startPosition + length - i - 1;
                IGene tmp = genes[index1];
                genes[index1] = genes[index2];
                genes[index2] = tmp;
            }
        }
    }
}
