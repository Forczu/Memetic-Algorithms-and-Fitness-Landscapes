using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemeticApplication.MemeticLibrary.Genetic;
using MemeticApplication.MemeticLibrary.Generators;

namespace MemeticApplication.MemeticLibrary.Operators.Mutation
{
    public class InversionOperator : MutationOperator
    {
        private static readonly string ID = "VM";

        public override object Clone()
        {
            return new InversionOperator();
        }

        public override string GetId()
        {
            return ID;
        }

        public override void Run(IGene[] genes)
        {
            int startPosition = RandomGeneratorThreadSafe.NextInt(genes.Length - 1);
            int length = RandomGeneratorThreadSafe.NextInt(genes.Length - startPosition - 2) + 2;
            Inverse(genes, startPosition, length);
        }

        public override void Run(ref Chromosome solution)
        {
            int startPosition = RandomGeneratorThreadSafe.NextInt(solution.Genes.Length - 1);
            int length = RandomGeneratorThreadSafe.NextInt(solution.Genes.Length - startPosition - 2) + 2;
            Inverse(solution.Genes, startPosition, length);
            solution.Refresh();
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
