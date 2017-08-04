using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemeticApplication.MemeticLibrary.Genetic;
using MemeticApplication.MemeticLibrary.Generators;

namespace MemeticApplication.MemeticLibrary.Operators.Mutation
{
    public class DisplacementOperator : IMutationOperator
    {
        public void Run(ref Chromosome solution)
        {
            int startIndex, destIndex, length;
            RandomGenerator.NextTwoDifferentInts(solution.Genes.Length, out startIndex, out destIndex);
            length = RandomGenerator.NextInt(solution.Genes.Length - startIndex);
            Displace(solution.Genes, startIndex, destIndex, length);
        }

        public void Run(IGene[] genes, int startIndex, int destIndex, int length)
        {
            Displace(genes, startIndex, destIndex, length);
        }

        private void Displace(IGene[] genes, int startIndex, int destIndex, int length)
        {
            IGene[] replaced = new IGene[length];
            Array.Copy(genes, destIndex, replaced, 0, length);
            Array.Copy(genes, startIndex, genes, destIndex, length);
            Array.Copy(replaced, 0, genes, startIndex, length);
        }
    }
}
