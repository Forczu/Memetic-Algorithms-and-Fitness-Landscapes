using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemeticApplication.MemeticLibrary.Genetic;
using MemeticApplication.MemeticLibrary.Generators;

namespace MemeticApplication.MemeticLibrary.Operators.Mutation
{
    public class DisplacementOperator : MutationOperator
    {
        private static readonly string ID = "DM";

        public override object Clone()
        {
            return new DisplacementOperator();
        }

        public override string GetId()
        {
            return ID;
        }

        public override void Run(IGene[] genes)
        {
            int startIndex, destIndex, length;
            RandomGeneratorThreadSafe.NextTwoDifferentInts(genes.Length - 1, out startIndex, out destIndex);
            int upperLimit = startIndex > destIndex ? startIndex : destIndex;
            length = RandomGeneratorThreadSafe.NextInt(1, genes.Length - upperLimit + 1);
            Displace(genes, startIndex, destIndex, length);
        }

        public override void Run(ref Chromosome solution)
        {
            int startIndex, destIndex, length;
            RandomGeneratorThreadSafe.NextTwoDifferentInts(solution.Genes.Length - 1, out startIndex, out destIndex);
            int upperLimit = startIndex > destIndex ? startIndex : destIndex;
            length = RandomGeneratorThreadSafe.NextInt(1, solution.Genes.Length - upperLimit + 1);
            Displace(solution.Genes, startIndex, destIndex, length);
            solution.Refresh();
        }

        public void Run(IGene[] genes, int startIndex, int destIndex, int length)
        {
            Displace(genes, startIndex, destIndex, length);
        }

        private void Displace(IGene[] genes, int startIndex, int destIndex, int length)
        {
            int offset = 0, copiedLength = length;
            int abs = Math.Abs(startIndex - destIndex);
            if (abs < length)
            {
                offset = length - abs;
                copiedLength = abs;
            }
            IGene[] replaced = new IGene[copiedLength];
            if (startIndex < destIndex)
            {
                Array.Copy(genes, destIndex + offset, replaced, 0, copiedLength);
                Array.Copy(genes, startIndex, genes, destIndex, length);
                Array.Copy(replaced, 0, genes, startIndex, copiedLength);
            }
            else
            {
                Array.Copy(genes, destIndex, replaced, 0, copiedLength);
                Array.Copy(genes, startIndex, genes, destIndex, length);
                Array.Copy(replaced, 0, genes, startIndex + offset, copiedLength);
            }
        }
    }
}
