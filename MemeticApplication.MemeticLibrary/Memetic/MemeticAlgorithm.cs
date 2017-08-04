using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MemeticApplication.MemeticLibrary.Entropy;
using MemeticApplication.MemeticLibrary.Factories;
using MemeticApplication.MemeticLibrary.Generators;
using MemeticApplication.MemeticLibrary.Genetic;
using MemeticApplication.MemeticLibrary.Heuristics;
using MemeticApplication.MemeticLibrary.Model;
using MemeticApplication.MemeticLibrary.Operators.Crossover;
using MemeticApplication.MemeticLibrary.Operators.Selection;

namespace MemeticApplication.MemeticLibrary.Memetic
{
    /// <summary>
    /// The general class for executing memetic algorithm on the problem.
    /// </summary>
    public class MemeticAlgorithm
    {
        AbstractChromosomeFactory chromosomeFactory = null;

        IHeuristics heuristics = null;

        bool stop;

        public object ShannonEntrophy { get; private set; }

        CancellationTokenSource source = new CancellationTokenSource();

        public void Stop()
        {
            source.Cancel();
        }

        /// <summary>
        /// Runs the memetic algorithm to solve the problem.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The best found solution.</returns>
        public Chromosome Run(IProblem problem, Parameters parameters)
        {
            chromosomeFactory = parameters.ChromosomeFactory;
            heuristics = parameters.Heuristics;
            var population = Initialization(problem, parameters);
            int counter = 0;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            watch.Start();
            bool conv = false;
            Chromosome theBestOne = population[0], localBest;
            do
            {
                population = Evolve(problem, parameters, population);
                population = Improve(problem, parameters, population);
                if (conv = IsConvergent(population, parameters))
                    population = Restart(problem, population, parameters);
                localBest = GetTheBestChromosome(population);
                theBestOne = theBestOne.CompareTo(localBest) < 0 ? theBestOne : localBest;
                /*int currentLineCursor = Console.CursorTop;
                Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
                if (counter % 1 == 0)
                    Console.WriteLine("Iteration " + counter + ": " + localBest.ToString() + ", time elapsed: " + watch.ElapsedMilliseconds + " ms"
                        + (conv ? " Restart!" : ""));
                Console.Write("Iteration: " + counter);*/
            } while (!StopCondition(++counter, parameters));
            Chromosome theBestSolution = GetTheBestChromosome(population);
            Console.WriteLine("\nThe best solution: " + theBestOne.ToString());
            return theBestSolution;
        }

        public async Task<Chromosome> Run(IProblem problem, Parameters parameters, Action<string, Chromosome, int, float, long, int> callback, string id)
        {
            chromosomeFactory = parameters.ChromosomeFactory;
            heuristics = parameters.Heuristics;
            var population = Initialization(problem, parameters);
            int iteration = 0, restarts = 0;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            watch.Start();
            Chromosome theBestOne = population[0], localBest;
            await Task.Run(() =>
            {
                do
                {
                    if (source.IsCancellationRequested)
                        break;
                    population = Evolve(problem, parameters, population);
                    population = Improve(problem, parameters, population);
                    if (IsConvergent(population, parameters))
                    {
                        population = Restart(problem, population, parameters);
                        ++restarts;
                    }
                    localBest = GetTheBestChromosome(population);
                    theBestOne = theBestOne.CompareTo(localBest) < 0 ? theBestOne : localBest;
                    callback(id, theBestOne, iteration, (theBestOne as Solution).TotalDistance(),
                        watch.ElapsedMilliseconds, restarts);
                } while (!StopCondition(++iteration, parameters));
            }, source.Token);
            Chromosome theBestSolution = GetTheBestChromosome(population);
            source.Dispose();
            source = new CancellationTokenSource();
            return theBestSolution;
        }

        /// <summary>
        /// Creates the first population of random and then improved population
        /// of solutions.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        protected Population Initialization(IProblem problem, Parameters parameters)
        {
            Population population = new Population();
            Chromosome solution = null;
            while(population.Size < parameters.PopulationSize)
            {
                solution = parameters.ChromosomeFactory.RandomSolution(parameters.GeneCount, problem);
                solution = RunMetaheuristics(solution, parameters);
                if (!population.Contains(solution))
                    population.AddChromosome(solution);
            }
            return population;
        }

        /// <summary>
        /// Runs the metaheuristics on a single solution.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The new solution.</returns>
        protected Chromosome RunMetaheuristics(Chromosome solution, Parameters parameters)
        {
            Chromosome newSolution = heuristics.Run(solution, parameters);
            return newSolution;
        }

        /// <summary>
        /// Invokes the process of evolution.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="population">The population.</param>
        /// <returns>The new solution population.</returns>
        protected Population Evolve(IProblem problem, Parameters parameters, Population population)
        {
            Population nextPopulation = (Population)population.Clone();
            Population newPopulation = Crossover(problem, nextPopulation, parameters);
            Mutation(newPopulation, parameters);
            return newPopulation;
        }

        /// <summary>
        /// Invokes the process of improvement with use of metaheuristics.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="population">The population.</param>
        /// <returns></returns>
        protected Population Improve(IProblem problem, Parameters parameters, Population population)
        {
            List<Thread> heuristicsThreads = new List<Thread>();
            int threadNumber = Environment.ProcessorCount;
            int tasksPerThread = population.Size / threadNumber;
            for (int i = 0; i < threadNumber; ++i)
            {
                int start = i * tasksPerThread;
                int end = (i + 1) * tasksPerThread;
                if (i == threadNumber - 1)
                    end += population.Size % threadNumber;
                Thread thread = new Thread(() => {
                    Chromosome solution;
                    for (int j = start; j < end; ++j)
                    {
                        solution = population[j];
                        solution = heuristics.Run(solution, parameters);
                        population[j] = solution;
                    }
                });
                heuristicsThreads.Add(thread);
                thread.Start();
            }
            foreach (Thread thread in heuristicsThreads)
                thread.Join();
            return population;
        }

        /// <summary>
        /// Determines whether the specified population is convergent.
        /// </summary>
        /// <param name="population">The population.</param>
        /// <returns>
        ///   <c>true</c> if the specified population is convergent; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsConvergent(Population population, Parameters parameters)
        {
            float avgEntropy = ShannonEntropy.GetEntropy(population);
            float convergence = parameters.ConvergenceLimit;
            if (avgEntropy < convergence)
                return true;
            return false;
        }

        protected Population Restart(IProblem problem, Population population, Parameters parameters)
        {
            Population newPopulation = new Population();
            Chromosome chromosome;
            for (int i = 0; i < parameters.PreservedChromosomesNumber; ++i)
            {
                chromosome = PopTheBestChromosome(population);
                newPopulation.AddChromosome(chromosome);
            }
            while (newPopulation.Size < parameters.PopulationSize)
            {
                chromosome = parameters.ChromosomeFactory.RandomSolution(parameters.GeneCount, problem);
                chromosome = RunMetaheuristics(chromosome, parameters);
                if (!newPopulation.Contains(chromosome))
                    newPopulation.AddChromosome(chromosome);
            }
            return newPopulation;
        }

        protected bool StopCondition(int counter, Parameters parameters)
        {
            bool stop = counter >= parameters.MaxIterations;
            return stop;
        }

        protected Chromosome GetTheBestChromosome(Population population)
        {
            Chromosome chromosome = population.Chromosomes.Min();
            return chromosome;
        }

        protected Chromosome PopTheBestChromosome(Population population)
        {
            Chromosome chromosome = GetTheBestChromosome(population);
            population.Chromosomes.Remove(chromosome);
            return chromosome;
        }

        /// <summary>
        /// Selects a new set of parents for mating.
        /// </summary>
        /// <param name="population">The population.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        protected Population Selection(Population population, Parameters parameters)
        {
            population.Sort();
            int eliteCount = parameters.EliteChildrenCount;
            int normieCount = parameters.PopulationSize - parameters.EliteChildrenCount;
            var eliteChromosomes = population.Take(eliteCount);
            var normieChromosomes = population.Take(eliteCount, normieCount);
            normieChromosomes = parameters.Selection.Run(normieChromosomes, parameters);
            var newChromosomes = new List<Chromosome>(parameters.PopulationSize);
            newChromosomes.AddRange(eliteChromosomes);
            newChromosomes.AddRange(normieChromosomes);
            var newPopulation = new Population(newChromosomes);
            return newPopulation;
        }

        /// <summary>
        /// Invokes the crossover operations on the next generation.
        /// </summary>
        /// <param name="population">The population.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        protected Population Crossover(IProblem problem, Population population, Parameters parameters)
        {
            Population newPopulation = population, childPopulation;
            Chromosome parent1, parent2, child1, child2;
            IGene[] child1Genes, child2Genes;
            double nextCrossoverProb;
            foreach (var crossover in parameters.CrossoverOperators)
            {
                newPopulation = Selection(newPopulation, parameters);
                newPopulation.Randomize();
                childPopulation = new Population();
                for (int i = 0; i < newPopulation.Size; i += 2)
                {
                    nextCrossoverProb = RandomGenerator.NextDouble();
                    parent1 = newPopulation[i];
                    parent2 = newPopulation[i + 1];
                    if (nextCrossoverProb > parameters.CrossoverProbability)
                    {
                        childPopulation.AddChromosome(parent1);
                        childPopulation.AddChromosome(parent2);
                        continue;
                    }
                    crossover.Run(parent1.Genes, parent2.Genes, out child1Genes, out child2Genes);
                    child1 = chromosomeFactory.MakeChromosome(problem, parent1.Genes);
                    child2 = chromosomeFactory.MakeChromosome(problem, parent2.Genes);
                    childPopulation.AddChromosome(child1);
                    childPopulation.AddChromosome(child2);
                }
                newPopulation = childPopulation;
            }
            return newPopulation;
        }

        /// <summary>
        /// Invokes the mutation operations on the next generation.
        /// </summary>
        /// <param name="population">The population.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        protected void Mutation(Population population, Parameters parameters)
        {
            double nextMutationProb;
            foreach (var mutation in parameters.MutationOperators)
            {
                for (int i = 0; i < population.Size; ++i)
                {
                    nextMutationProb = RandomGenerator.NextDouble();
                    if (nextMutationProb > parameters.MutationProbability)
                        continue;
                    Chromosome chromosome = population[i];
                    mutation.Run(ref chromosome);
                }
            }
        }
    }
}
