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
using MemeticApplication.MemeticLibrary.Research;
using System.Web.Mvc;
using MemeticApplication.MemeticLibrary.Readers;
using System.IO;

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

        private static ConcurrentDictionary<string, LandscapeResearch> researches = new ConcurrentDictionary<string, LandscapeResearch>();

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

        public void RegisterAlgorithm(string problemName, ParametersWebsite webParameters)
        {
            string filePath = HttpContext.Current.Server.MapPath("~/Data/Homberger/" + problemName.ToUpper() + @".txt");
            VrptwProblem problem = new VrptwProblemReader().ReadFromFile(filePath);

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
                CrossoverOperators = new List<CrossoverOperator>(),
                MutationOperators = new List<MutationOperator>(),
                CrossoverProbability = webParameters.CrossoverProbability,
                MutationProbability = webParameters.MutationProbability,
                Fitness = new FitnessFunction(webParameters.FitnessRoutes, webParameters.FitnesDistance),
                EliteChildrenCount = webParameters.EliteChildrenCount,
                GeneCount = problem.Customers.Count,
                FitnessStrategy = FitnessStrategy.MINIMIZE,
                ConvergenceLimit = webParameters.ConvergenceLimit,
                PreservedChromosomesNumber = webParameters.PreservedChromosomeNumber,
                MaxIterations = webParameters.Iterations,
                ChromosomeFactory = new SolutionFactory(),
                HeuristicsParameters = webParameters.HeuristicsParameters
            };
            parameters.CrossoverOperators.Add(CrossoverOperatorFactory.Create(webParameters.Crossover));
            parameters.MutationOperators.Add(MutationOperatorFactory.Create(webParameters.Mutation));
            algorithms.TryAdd(connecionId, ma);
            var result = ma.Run(problem, parameters, UpdateSolution, connecionId);
            var solution = result.Result as Solution;
            string outputPath = HttpContext.Current.Server.MapPath("~/Results/Memetic");
            string outputFilePath = outputPath + @"/" + problem.Name + ".txt";
            File.WriteAllText(outputFilePath, solution.ToFileText());
        }

        public void RegisterResearch(WebResearchParameters webParameters)
        {
            string connecionId = Context.ConnectionId;
            Register();
            LandscapeResearch research = new LandscapeResearch();
            if (researches.ContainsKey(connecionId))
            {
                research = researches[connecionId];
                //reseach.Stop();
            }
            else
                research = new LandscapeResearch();
            ResearchParameters parameters = new ResearchParameters()
            {
                Sensitivity = webParameters.Sensitivity,
                RandomWalkNumber = webParameters.RandomWalkNumber,
                RandomWalkSteps = webParameters.RandomWalkSteps,
                CrossoverOperators = new List<CrossoverOperator>(),
                MutationOperators = new List<MutationOperator>(),
                RoadWeight200 = webParameters.RoadWeight200,
                RoadWeight400 = webParameters.RoadWeight400,
                RoadWeight600 = webParameters.RoadWeight600,
                RoadWeight800 = webParameters.RoadWeight800,
                RoadWeight1000 = webParameters.RoadWeight1000
            };
            if (webParameters.CrossoverOperators != null)
                foreach (var xOp in webParameters.CrossoverOperators)
                    parameters.CrossoverOperators.Add(CrossoverOperatorFactory.Create(xOp));
            if (webParameters.MutationOperators != null)
                foreach (var mOp in webParameters.MutationOperators)
                    parameters.MutationOperators.Add(MutationOperatorFactory.Create(mOp));
            researches.TryAdd(connecionId, research);
            string inputPath  = HttpContext.Current.Server.MapPath("~/Data/Homberger");
            string outputPath = HttpContext.Current.Server.MapPath("~/Results/Homberger");
            research.Run(parameters, inputPath, outputPath, UpdateResearch, connecionId);
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

        private void UpdateResearch(string connectionId, float percent)
        {
            lock (locker)
            {
                SendResearchData(connectionId, percent);
            }
        }

        public void SendNewSolution(string connectionId, Solution solution, Metadata meta)
        {
            Clients.Client(connectionId).SendNewData(solution, meta);
        }

        public void SendResearchData(string connectionId, float percent)
        {
            Clients.Client(connectionId).SendResearchProgress(percent);
        }
    }
}