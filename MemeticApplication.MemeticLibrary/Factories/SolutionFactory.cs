using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemeticApplication.MemeticLibrary.Extensions;
using MemeticApplication.MemeticLibrary.Generators;
using MemeticApplication.MemeticLibrary.Genetic;
using MemeticApplication.MemeticLibrary.Model;
using MemeticApplication.MemeticLibrary.Operators.Mutation;

namespace MemeticApplication.MemeticLibrary.Factories
{
    public class SolutionFactory : AbstractChromosomeFactory
    {
        public override Chromosome MakeChromosome(IProblem problem)
        {
            Solution solution = new Solution(problem as VrptwProblem);
            return solution;
        }

        public override Chromosome MakeChromosome(IProblem problem, IGene[] genes)
        {
            Solution solution = new Solution(problem as VrptwProblem, genes);
            return solution;
        }

        public override Chromosome RandomNeighbourSolution(Chromosome chromosome)
        {
            IGene[] newGenes;
            Chromosome newSolution, newChromosome;
            newChromosome = (Chromosome)chromosome.Clone();
            newGenes = newChromosome.Genes;
            int position1, position2;
            RandomGeneratorThreadSafe.NextTwoDifferentInts(chromosome.Size(), out position1, out position2);
            IGene swappedGene1 = newGenes[position1];
            IGene swappedGene2 = newGenes[position2];
            newGenes[position1] = swappedGene2;
            newGenes[position2] = swappedGene1;
            newSolution = new Solution((VrptwProblem)chromosome.Problem, newGenes);
            return newSolution;
        }

        public override Chromosome RandomNeighbourSolution(Chromosome chromosome, MutationOperator neighbourhood)
        {
            int size = chromosome.Size();
            IGene[] newGenes = new IGene[size];
            Array.Copy(chromosome.Genes, newGenes, size);
            neighbourhood.Run(newGenes);
            Chromosome result = new Solution(chromosome.Problem as VrptwProblem, newGenes);
            return result;
        }

        public override Chromosome RandomSolution(int geneNumber, IProblem problem)
        {
            VrptwProblem vrptwProblem = problem as VrptwProblem;
            List<int> customers = Enumerable.Range(0, geneNumber).ToList();
            customers.Shuffle();
            IGene[] genes = new IGene[geneNumber];
            for (int i = 0; i < geneNumber; ++i)
                genes[i] = new CustomerGene(vrptwProblem.Customers[customers[i]]);
            Solution solution = new Solution(vrptwProblem, genes);
            return solution;
        }
    }
}
