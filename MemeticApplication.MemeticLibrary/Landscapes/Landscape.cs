using MemeticApplication.MemeticLibrary.Factories;
using MemeticApplication.MemeticLibrary.Fitness;
using MemeticApplication.MemeticLibrary.Genetic;
using MemeticApplication.MemeticLibrary.Operators.Crossover;
using MemeticApplication.MemeticLibrary.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemeticApplication.MemeticLibrary.Landscapes
{
    public class Landscape
    {
        public IProblem Problem { get; protected set; }

        public AbstractChromosomeFactory ChromosomeFactory { get; protected set; }

        public ICrossoverOperator SearchOperator { get; protected set; }

        public FitnessFunction FitnessFunction { get; protected set; }

        private UnaryFunction statistics = null;

        public Landscape(IProblem problem, ICrossoverOperator op, AbstractChromosomeFactory chromosomeFactory, FitnessFunction fitnessFunction)
        {
            Problem = problem;
            SearchOperator = op;
            ChromosomeFactory = chromosomeFactory;
            FitnessFunction = fitnessFunction;
        }

        public UnaryFunction RandomWalk(int steps)
        {
            statistics = new UnaryFunction(steps);
            Chromosome currentSolution1 = ChromosomeFactory.RandomSolution(Problem.GeneCount(), Problem);
            Chromosome currentSolution2 = ChromosomeFactory.RandomSolution(Problem.GeneCount(), Problem);
            GatherData(currentSolution1, currentSolution2, 0);
            for (int i = 1; i < steps; ++i)
            {
                IGene[] childGenes1, childGenes2;
                SearchOperator.Run(currentSolution1.Genes, currentSolution2.Genes, out childGenes1, out childGenes2);
                currentSolution1 = ChromosomeFactory.MakeChromosome(Problem, childGenes1);
                currentSolution2 = ChromosomeFactory.MakeChromosome(Problem, childGenes2);
                GatherData(currentSolution1, currentSolution2, i);
            }
            return statistics;
        }

        private void GatherData(Chromosome solution1, Chromosome solution2, int step)
        {
            float fitness1 = FitnessFunction.Get(solution1);
            float fitness2 = FitnessFunction.Get(solution2);
            statistics.Set(step, fitness1 < fitness2 ? fitness1 : fitness2);
        }
    }
}
