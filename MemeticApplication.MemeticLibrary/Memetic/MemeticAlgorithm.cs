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

        protected Parameters Parameters { get; set; }

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
        public async Task<Chromosome> Run(IProblem problem, Parameters parameters, Action<string, Chromosome, int, float, long, int> callback, string id, bool genetic)
        {
            Parameters = parameters;
            chromosomeFactory = parameters.ChromosomeFactory;
            heuristics = parameters.Heuristics;
            
            Population population = new Population(parameters.PopulationSize);
            Chromosome solution = null;
            int index = 0;
            while (index < parameters.PopulationSize)
            {
                solution = parameters.ChromosomeFactory.RandomSolution(parameters.GeneCount, problem);
                if (!population.Contains(solution))
                {
                    population[index] = solution;
                    ++index;
                }
            }
            var watch = System.Diagnostics.Stopwatch.StartNew();
            watch.Start();
            Chromosome theBestOne = population[0], localBest;
            int iteration = 0, restarts = 0;
            await Task.Run(() =>
            {
                do
                {
                    if (source.IsCancellationRequested)
                        break;
                    population = Evolve(problem, parameters, population);
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
            source.Dispose();
            source = new CancellationTokenSource();
            return theBestOne;
        }

        public Chromosome Run(IProblem problem, Parameters parameters)
        {
            Parameters = parameters;
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
            } while (!StopCondition(++counter, parameters));
            //Chromosome theBestSolution = GetTheBestChromosome(population);
            Console.WriteLine("\nThe best solution: " + theBestOne.ToString());
            return theBestOne;
        }

        public Task<Chromosome> Run(IProblem problem, Parameters parameters, Action<string, Chromosome, int, float, long, int> callback, string id)
        {
            chromosomeFactory = parameters.ChromosomeFactory;
            heuristics = parameters.Heuristics;
            var population = Initialization(problem, parameters);
            int iteration = 0, restarts = 0;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            watch.Start();
            Chromosome theBestOne = population[0], localBest;
            var result = Task.Run(() =>
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
                return theBestOne;
            }, source.Token);
            //Chromosome theBestSolution = GetTheBestChromosome(population);
            source.Dispose();
            source = new CancellationTokenSource();
            return result;
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
            Population population = new Population(parameters.PopulationSize);
            Chromosome solution = null;
            int index = 0;
            while(index < parameters.PopulationSize)
            {
                solution = parameters.ChromosomeFactory.RandomSolution(parameters.GeneCount, problem);
                solution = RunMetaheuristics(solution, parameters);
                if (!population.Contains(solution))
                {
                    population[index] = solution;
                    ++index;
                }
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
            //Population nextPopulation = (Population)population.Clone();
            Population newPopulation = Crossover(problem, population, parameters);
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
            float avgEntropy = ShannonEntropy.GetGenesEntropy(population);
            float convergence = parameters.ConvergenceLimit;
            if (avgEntropy < convergence)
                return true;
            return false;
        }

        protected Population Restart(IProblem problem, Population population, Parameters parameters)
        {
            Population newPopulation = new Population(parameters.PopulationSize);
            Chromosome chromosome;
            for (int i = 0; i < parameters.PreservedChromosomesNumber; ++i)
            {
                chromosome = PopTheBestChromosome(population);
                newPopulation[i] = chromosome;
            }
            int index = 0;
            while (index < parameters.PopulationSize)
            {
                chromosome = parameters.ChromosomeFactory.RandomSolution(parameters.GeneCount, problem);
                //chromosome = RunMetaheuristics(chromosome, parameters);
                if (!newPopulation.Contains(chromosome))
                {
                    newPopulation[index] = chromosome;
                    ++index;
                }
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
            int index = Array.IndexOf(population.Chromosomes, chromosome);
            population.Chromosomes[index] = null;
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
            var newPopulation = new Population(newChromosomes.ToArray());
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
            foreach (var crossover in parameters.CrossoverOperators)
            {
                List<Thread> crossoverThreads = new List<Thread>();
                int threadNumber = Environment.ProcessorCount;
                int tasksPerThread = (population.Size / threadNumber);
                tasksPerThread += tasksPerThread % 2;

                newPopulation = Selection(newPopulation, parameters);
                newPopulation.Randomize();
                childPopulation = new Population(population.Size);
                for (int i = 0; i < threadNumber; ++i)
                {
                    int start = i * tasksPerThread;
                    int end;
                    if (i == threadNumber - 1)
                        end = population.Size;
                    else
                        end = (i + 1) * tasksPerThread;
                    Thread thread = new Thread(() => {
                        Chromosome parent1, parent2, child1, child2;
                        IGene[] child1Genes, child2Genes;
                        double nextCrossoverProb;
                        for (int j = start; j < end; j += 2)
                        {
                            nextCrossoverProb = RandomGeneratorThreadSafe.NextDouble();
                            parent1 = newPopulation[j];
                            parent2 = newPopulation[j + 1];
                            if (nextCrossoverProb > parameters.CrossoverProbability)
                            {
                                childPopulation[j] = parent1;
                                childPopulation[j + 1] = parent2;
                                continue;
                            }
                            crossover.Run(parent1.Genes, parent2.Genes, out child1Genes, out child2Genes);
                            child1 = chromosomeFactory.MakeChromosome(problem, parent1.Genes);
                            child2 = chromosomeFactory.MakeChromosome(problem, parent2.Genes);
                            childPopulation[j] = child1;
                            childPopulation[j + 1] = child2;
                        }
                    });
                    crossoverThreads.Add(thread);
                    thread.Start();
                }
                foreach (Thread thread in crossoverThreads)
                    thread.Join();
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
            foreach (var mutation in parameters.MutationOperators)
            {
                List<Thread> mutationThreads = new List<Thread>();
                int threadNumber = Environment.ProcessorCount;
                int tasksPerThread = population.Size / threadNumber;
                for (int i = 0; i < threadNumber; ++i)
                {
                    int start = i * tasksPerThread;
                    int end = (i + 1) * tasksPerThread;
                    if (i == threadNumber - 1)
                        end += population.Size % threadNumber;
                    Thread thread = new Thread(() => {
                        double nextMutationProb;
                        for (int j = start; j < end; ++j)
                        {
                            nextMutationProb = RandomGeneratorThreadSafe.NextDouble();
                            if (nextMutationProb > parameters.MutationProbability)
                                continue;
                            Chromosome chromosome = population[j];
                            mutation.Run(ref chromosome);
                        }
                    });
                    mutationThreads.Add(thread);
                    thread.Start();
                }
                foreach (Thread thread in mutationThreads)
                    thread.Join();
            }
        }
    }
}
