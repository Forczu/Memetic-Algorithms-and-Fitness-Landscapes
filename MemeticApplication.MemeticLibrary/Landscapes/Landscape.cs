using MemeticApplication.MemeticLibrary.Factories;
using MemeticApplication.MemeticLibrary.Fitness;
using MemeticApplication.MemeticLibrary.Generators;
using MemeticApplication.MemeticLibrary.Genetic;
using MemeticApplication.MemeticLibrary.Operators;
using MemeticApplication.MemeticLibrary.Operators.Crossover;
using MemeticApplication.MemeticLibrary.Operators.Mutation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MemeticApplication.MemeticLibrary.Landscapes
{
    /// <summary>
    /// Represents the fitness landscape defined by the problem istance and fitness function.
    /// </summary>
    public class Landscape
    {
        /// <summary>
        /// The problem instance.
        /// </summary>
        public IProblem Problem { get; protected set; }

        /// <summary>
        /// A factory for generating new chromosomes along the research.
        /// </summary>
        public AbstractChromosomeFactory ChromosomeFactory { get; protected set; }

        /// <summary>
        /// The fitness function for evaluating the quality of a solution.
        /// </summary>
        public FitnessFunction FitnessFunction { get; protected set; }

        /// <summary>
        /// Creates a new fitness landscape.
        /// </summary>
        /// <param name="problem">The problem instance.</param>
        /// <param name="chromosomeFactory">A chromosome factory.</param>
        /// <param name="fitnessFunction">The fitness function.</param>
        public Landscape(IProblem problem, AbstractChromosomeFactory chromosomeFactory, FitnessFunction fitnessFunction)
        {
            Problem = problem;
            ChromosomeFactory = chromosomeFactory;
            FitnessFunction = fitnessFunction;
        }

        /// <summary>
        /// Runs a random walk of n steps along a landscape defined by a crossover operator and returns its results.
        /// </summary>
        /// <param name="steps">Number of steps.</param>
        /// <param name="searchOperator">Operator defining a neighbourhood.</param>
        /// <returns></returns>
        public RandomWalk RandomWalk(int steps, IOperator searchOperator)
        {
            if (searchOperator is CrossoverOperator)
                return RandomWalk(steps, (CrossoverOperator)searchOperator);
            if (searchOperator is MutationOperator)
                return RandomWalk(steps, (MutationOperator)searchOperator);
            return null;
        }

        /// <summary>
        /// Runs a random walk of n steps along a landscape defined by a crossover operator and returns its results.
        /// </summary>
        /// <param name="steps">Number of steps.</param>
        /// <param name="searchOperator">Operator defining a neighbourhood.</param>
        /// <returns></returns>
        public RandomWalk RandomWalk(int steps, CrossoverOperator searchOperator)
        {
            RandomWalk statistics = new RandomWalk(steps);
            Chromosome parent1 = ChromosomeFactory.RandomSolution(Problem.GeneCount(), Problem);
            Chromosome parent2 = ChromosomeFactory.RandomSolution(Problem.GeneCount(), Problem);
            bool firstParent = RandomGeneratorThreadSafe.NextBool();
            Chromosome parent = (firstParent ? parent1 : parent2), child;
            GatherData(parent, 0, statistics);

            const int minPopSize = 8, maxPopSize = 15;
            int popSize = RandomGeneratorThreadSafe.NextInt(minPopSize, maxPopSize);
            Chromosome[] supportPopulation = new Chromosome[popSize];
            for (int i = 0; i < popSize; ++i)
                supportPopulation[i] = ChromosomeFactory.RandomSolution(Problem.GeneCount(), Problem);

            IGene[] childGenes1, childGenes2;
            for (int i = 0; i < steps; ++i)
            {
                searchOperator.Run(parent1.Genes, parent2.Genes, out childGenes1, out childGenes2);
                child = ChromosomeFactory.MakeChromosome(Problem, RandomGeneratorThreadSafe.NextBool() ? childGenes1 : childGenes2);
                GatherData(child, i, statistics);
                parent1 = child;
                parent2 = supportPopulation[RandomGeneratorThreadSafe.NextInt(popSize)];
            }

            return statistics;
        }

        /// <summary>
        /// Runs a random walk of n steps along a landscape defined by a mutation operator and returns its results.
        /// </summary>
        /// <param name="steps">Number of steps.</param>
        /// <param name="searchOperator">Operator defining a neighbourhood.</param>
        /// <returns></returns>
        public RandomWalk RandomWalk(int steps, MutationOperator searchOperator)
        {
            RandomWalk statistics = new RandomWalk(steps);
            Chromosome currentSolution = ChromosomeFactory.RandomSolution(Problem.GeneCount(), Problem);
            GatherData(currentSolution, 0, statistics);
            for (int i = 1; i < steps; ++i)
            {
                searchOperator.Run(ref currentSolution);
                GatherData(currentSolution, i, statistics);
            }
            return statistics;
        }

        /// <summary>
        /// Gathers an information about random walk step (an index and fitness function value)
        /// on landscape defined by a crossover operator.
        /// </summary>
        /// <param name="solution1">First solution.</param>
        /// <param name="solution2">Second solution.</param>
        /// <param name="step">Index of a step.</param>
        /// <param name="statistics">Statistics of the walk.</param>
        protected void GatherData(Chromosome solution1, Chromosome solution2, int step, RandomWalk statistics)
        {
            float fitness1 = FitnessFunction.Get(solution1);
            float fitness2 = FitnessFunction.Get(solution2);
            statistics[step] = fitness1;
            statistics[step + 1] = fitness2;
        }

        /// <summary>
        /// Gathers an information about random walk step (an index and fitness function value)
        /// on landscape defined by a mutation operator.
        /// </summary>
        /// <param name="solution">A solution.</param>
        /// <param name="step">Index of a step.</param>
        /// <param name="statistics">Statistics of the walk.</param>
        protected void GatherData(Chromosome solution, int step, RandomWalk statistics)
        {
            float fitness = FitnessFunction.Get(solution);
            statistics[step] = fitness;
        }
    }
}
