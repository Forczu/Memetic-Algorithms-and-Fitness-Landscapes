using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemeticApplication.MemeticLibrary.Genetic;
using MemeticApplication.MemeticLibrary.Generators;

namespace MemeticApplication.MemeticLibrary.Operators.Crossover
{
    public class CycleCrossover : ICrossoverOperator
    {
        public void Run(IGene[] parent1, IGene[] parent2, out IGene[] child1, out IGene[] child2)
        {
            int customerCount = parent1.Count();
            IGene[] c1 = new IGene[customerCount];
            IGene[] c2 = new IGene[customerCount];
            DoCycle(parent1, parent2, c1);
            DoCycle(parent2, parent1, c2);
            child1 = c1;
            child2 = c2;
        }

        private void DoCycle(IGene[] sourceParent, IGene[] secondParent, IGene[] child)
        {
            int index = 0;
            child[index] = sourceParent[index];
            while (true)
            {
                IGene geneAtParent2 = secondParent[index];
                if (child.Contains(geneAtParent2))
                    break;
                index = Array.IndexOf(sourceParent, geneAtParent2);
                child[index] = geneAtParent2;
            }
            int position = 0;
            for (int i = 0; i < child.Length; ++i)
            {
                if (child[i] == null)
                {
                    for (; position < child.Length; ++position)
                    {
                        if (!child.Contains(secondParent[position]))
                        {
                            child[position] = secondParent[position];
                            break;
                        }
                    }
                }
            }
        }
    }
}
