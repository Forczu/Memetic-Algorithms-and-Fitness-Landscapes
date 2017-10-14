using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemeticApplication.MemeticLibrary.Genetic;
using MemeticApplication.MemeticLibrary.Generators;

namespace MemeticApplication.MemeticLibrary.Operators.Crossover
{
    public class CycleCrossover : CrossoverOperator
    {
        private static readonly string ID = "CX";

        private static readonly int FIRST_CYCLE_FIRST_GENE_INDEX = 0;

        public override object Clone()
        {
            return new CycleCrossover();
        }

        public override string GetId()
        {
            return ID;
        }

        public override void Run(IGene[] parent1, IGene[] parent2, out IGene[] child1, out IGene[] child2)
        {
            int customerCount = parent1.Count();
            IGene[] c1 = new IGene[customerCount];
            IGene[] c2 = new IGene[customerCount];
            int slicePoint = RandomGeneratorThreadSafe.NextInt(customerCount);
            DoCycleCrossover(parent1, parent2, c1, c2, slicePoint, true);
            child1 = c1;
            child2 = c2;
        }

        /// <summary>
        /// Makes a cycle crossover operation to make a single child.
        /// </summary>
        /// <param name="sourceParent"></param>
        /// <param name="secondParent"></param>
        /// <param name="child"></param>
        private void DoCycleCrossover(IGene[] sourceParent, IGene[] secondParent, IGene[] child1, IGene[] child2, int cycleStartGeneIndex, bool areGenesCopied)
        {
            int lastEmptyGeneIndex = cycleStartGeneIndex;
            do
            {
                DoCycle(sourceParent, secondParent, child1, child2, lastEmptyGeneIndex, areGenesCopied);
                areGenesCopied = !areGenesCopied;
                int firstEmptyGeneIndex = GetFirstEmptyGeneIndex(child1, cycleStartGeneIndex);
                if (firstEmptyGeneIndex == cycleStartGeneIndex)
                    return;
                lastEmptyGeneIndex = firstEmptyGeneIndex;
            } while (true);
        }

        /// <summary>
        /// Makes a full cycle within one iteration of crossover.
        /// </summary>
        /// <param name="sourceParent"></param>
        /// <param name="secondParent"></param>
        /// <param name="child"></param>
        /// <param name="firstIndexOfCopiedGenes"></param>
        /// <param name="areGeneCopied"></param>
        private void DoCycle(IGene[] sourceParent, IGene[] secondParent, IGene[] child1, IGene[] child2, int firstEmptyGeneIndex, bool areGeneCopied)
        {
            IList<int> indexes = GetCycleIndexes(firstEmptyGeneIndex, sourceParent, secondParent);
            if (areGeneCopied)
                foreach (var copyIndex in indexes)
                {
                    child1[copyIndex] = sourceParent[copyIndex];
                    child2[copyIndex] = secondParent[copyIndex];
                }
            else
                foreach (var swapIndex in indexes)
                {
                    child1[swapIndex] = secondParent[swapIndex];
                    child2[swapIndex] = sourceParent[swapIndex];
                }
        }

        /// <summary>
        /// Returns all gene indexes which the next cycle consists of.
        /// </summary>
        /// <param name="firstEmptyGeneIndex">First empty gene in chromosome.</param>
        /// <param name="sourceParent">Parent which is the source.</param>
        /// <param name="secondParent">Parent which is second.</param>
        /// <returns></returns>
        private IList<int> GetCycleIndexes(int firstEmptyGeneIndex, IGene[] sourceParent, IGene[] secondParent)
        {
            IList<int> cycleIndexes = new List<int>();
            cycleIndexes.Add(firstEmptyGeneIndex);
            int index = firstEmptyGeneIndex;
            while (true)
            {
                IGene geneAtParent2 = secondParent[index];
                if (geneAtParent2.Equals(sourceParent[firstEmptyGeneIndex]))
                    return cycleIndexes;
                index = Array.IndexOf(sourceParent, geneAtParent2);
                cycleIndexes.Add(index);
            }
        }

        /// <summary>
        /// Return an index of first null gene.
        /// If its value is zero then it means that there are 
        /// no empty genes.
        /// </summary>
        /// <param name="genes"></param>
        /// <returns></returns>
        private int GetFirstEmptyGeneIndex(IGene[] genes, int lastEmptyGeneIndex)
        {
            for (int i = 0; i < genes.Length; ++i)
            {
                if (genes[i] == null)
                {
                    return i;
                }
            }
            return lastEmptyGeneIndex;
        }
    }
}
