using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using MemeticApplication.MemeticLibrary.Model;
using MemeticApplication.MemeticLibrary.Memetic;
using System.Collections.Concurrent;
using MemeticApplication.MemeticLibrary.Genetic;
using MemeticApplication.MemeticLibrary.Operators.Selection;
using MemeticApplication.MemeticLibrary.Operators.Crossover;
using MemeticApplication.MemeticLibrary.Operators.Mutation;
using MemeticApplication.MemeticLibrary.Factories;
using MemeticApplication.MemeticLibrary.Heuristics;
using MemeticApplication.MemeticLibrary.Fitness;

namespace MemeticApplication.Hubs
{
    public class Metadata
    {
        public int Iteration { get; set; }
        public float Distance { get; set; }
        public long Time { get; set; }
        public int Restarts { get; set; }
    }

    public class MemeticHub : Hub
    {
        private static ConcurrentDictionary<string, MemeticAlgorithm> algorithms = new ConcurrentDictionary<string, MemeticAlgorithm>();

        private static readonly object locker = new object();

        private void Register()
        {
            SelectionFactory.Register("roulette", () => new RouletteSelection());
            CrossoverOperatorFactory.Register("OX", () => new OrderCrossover());
            CrossoverOperatorFactory.Register("CX", () => new CycleCrossover());
            CrossoverOperatorFactory.Register("PMX", () => new PartiallyMatchedCrossover());
            CrossoverOperatorFactory.Register("UOX", () => new UniformBasedOrderCrossover());
            MutationOperatorFactory.Register("swap", () => new SwapOperator());
            MutationOperatorFactory.Register("insertion", () => new InsertionOperator());
            MutationOperatorFactory.Register("inversion", () => new InversionOperator());
            MutationOperatorFactory.Register("displacement", () => new DisplacementOperator());
            HeuristicsFactory.Register("SA", () => new SimulatedAnnealing());
        }

        public void RegisterAlgorithm(VrptwProblem problem, ParametersWebsite webParameters)
        {
            string connecionId = Context.ConnectionId;
            Register();
            MemeticAlgorithm ma;
            if (algorithms.ContainsKey(connecionId))
            {
                ma = algorithms[connecionId];
                ma.Stop();
            }
            else
                ma = new MemeticAlgorithm();
            Parameters parameters = new Parameters
            {
                Heuristics = HeuristicsFactory.Create(webParameters.Heuristics),
                PopulationSize = webParameters.PopulationSize,
                Selection = SelectionFactory.Create(webParameters.Selection),
                CrossoverOperators = new List<ICrossoverOperator>(),
                MutationOperators = new List<IMutationOperator>(),
                CrossoverProbability = webParameters.CrossoverProbability,
                MutationProbability = webParameters.MutationProbability,
                Fitness = new FitnessFunction(webParameters.FitnessRoutes, webParameters.FitnesDistance),
                EliteChildrenCount = webParameters.EliteChildrenCount,
                GeneCount = problem.Customers.Count,
                FitnessStrategy = FitnessStrategy.MINIMIZE,
                ConvergenceLimit = webParameters.ConvergenceLimit,
                PreservedChromosomesNumber = webParameters.PreservedChromosomeNumber,
                MaxIterations = webParameters.Iterations,
                ChromosomeFactory = new SolutionFactory()
            };
            parameters.CrossoverOperators.Add(CrossoverOperatorFactory.Create(webParameters.Crossover));
            parameters.MutationOperators.Add(MutationOperatorFactory.Create(webParameters.Mutation));
            algorithms.TryAdd(connecionId, ma);
            ma.Run(problem, parameters, UpdateSolution, connecionId);
        }

        public void StopAlgorithm()
        {
            string connecionId = Context.ConnectionId;
            if (algorithms.ContainsKey(connecionId))
            {
                MemeticAlgorithm ma;
                algorithms.TryRemove(connecionId, out ma);
                ma.Stop();
            }
        }

        private void UpdateSolution(string connectionId, Chromosome data, int iteration, float distance, long time, int restarts)
        {
            Solution solution = data as Solution;
            lock (locker)
            {
                Metadata meta = new Metadata
                {
                    Distance = distance,
                    Iteration = iteration,
                    Time = time,
                    Restarts = restarts
                };
                SendNewSolution(connectionId, solution, meta);
            }
        }

        public void SendNewSolution(string connectionId, Solution solution, Metadata meta)
        {
            Clients.Client(connectionId).SendNewData(solution, meta);
        }
    }
}