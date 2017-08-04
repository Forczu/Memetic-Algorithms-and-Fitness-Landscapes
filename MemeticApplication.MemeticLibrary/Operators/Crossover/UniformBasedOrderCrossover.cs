using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemeticApplication.MemeticLibrary.Generators;
using MemeticApplication.MemeticLibrary.Genetic;

namespace MemeticApplication.MemeticLibrary.Operators.Crossover
{
    public class UniformBasedOrderCrossover : ICrossoverOperator
    {
        public void Run(IGene[] parent1, IGene[] parent2, out IGene[] child1, out IGene[] child2)
        {
            int geneCount = parent1.Count();
            int[] ones, zeros;
            PrepareUniform(geneCount, out ones, out zeros);
            Run(parent1, parent2, out child1, out child2, ones, zeros);
        }

        public void Run(IGene[] parent1, IGene[] parent2, out IGene[] child1, out IGene[] child2, int[] ones, int[] zeros)
        {
            int geneCount = parent1.Count();
            IGene[] c1 = new IGene[geneCount];
            IGene[] c2 = new IGene[geneCount];
            // phase 1: copy the unchanged genes
            foreach (int index in ones)
            {
                c1[index] = parent1[index];
                c2[index] = parent2[index];
            }
            // phase 2: copy the rest of genes from other parent
            int firstParentIndex = 0, secondParentIndex = 0;
            foreach (int index in zeros)
            {
                MoveGenesInZeroPositions(parent2, c1, index, secondParentIndex);
                MoveGenesInZeroPositions(parent1, c2, index, firstParentIndex);
            }
            child1 = c1;
            child2 = c2;
        }

        private void PrepareUniform(int geneCount, out int[] ones, out int[] zeros)
        {
            List<int> onesList = new List<int>(), zerosList = new List<int>();
            double nextDouble;
            for (int i = 0; i < geneCount; ++i)
            {
                nextDouble = RandomGenerator.NextDouble();
                if (nextDouble > 0.50000000)
                    onesList.Add(i);
                else
                    zerosList.Add(i);
            }
            ones = onesList.ToArray();
            zeros = zerosList.ToArray();
        }

        private void MoveGenesInZeroPositions(IGene[] otherParent, IGene[] child, int childIndex, int otherParentIndex)
        {
            IGene secondParentGene = otherParent[childIndex];
            {
                if (!child.Contains(secondParentGene))
                    child[childIndex] = secondParentGene;
                else
                {
                    for (int i = otherParentIndex; i < otherParent.Length; ++i)
                    {
                        if (!child.Contains(otherParent[i]))
                        {
                            child[childIndex] = otherParent[i];
                            break;
                        }
                    }
                }
            }
        }
    }
}
