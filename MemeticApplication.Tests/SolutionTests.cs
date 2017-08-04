using System;
using MemeticApplication.MemeticLibrary.Memetic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MemeticApplication.MemeticLibrary.Readers;
using MemeticApplication.MemeticLibrary.Model;
using MemeticApplication.MemeticLibrary.Factories;
using MemeticApplication.MemeticLibrary.Heuristics;
using System.Collections.Generic;
using MemeticApplication.MemeticLibrary.Operators.Crossover;
using MemeticApplication.MemeticLibrary.Operators.Mutation;
using MemeticApplication.MemeticLibrary.Generators;
using MemeticApplication.MemeticLibrary.Operators.Selection;
using System.Diagnostics;
using MemeticApplication.MemeticLibrary.Fitness;

namespace MemeticApplication.Tests
{
    [TestClass]
    public class SolutionTests
    {
        private static readonly string FILE_PATH = @"D:\Uczelnia\Sem 10\MGR\Dane\Solomon\solomon_100\C101.txt";
        
        private SolomonProblemReader reader = null;

        private VrptwProblem problem = null;

        [TestInitialize]
        public void SetUp()
        {
            reader = new SolomonProblemReader();
            problem = reader.ReadFromFile(FILE_PATH);
        }

        [TestMethod]
        public void RandomSolutionShouldBeFeasible()
        {
            HeuristicsFactory.Register("SA", () => new SimulatedAnnealing());
            MemeticAlgorithm algorithm = new MemeticAlgorithm();
            Parameters parameters = new Parameters
            {
                PopulationSize = 30,
                Selection = new RouletteSelection(),
                CrossoverOperators = new List<ICrossoverOperator>(),
                MutationOperators = new List<IMutationOperator>(),
                CrossoverProbability = 0.75f,
                MutationProbability = 0.05f,
                Fitness = new FitnessFunction(4.0f, 1.0f),
                ChromosomeFactory = new SolutionFactory(),
                EliteChildrenCount = 3,
                GeneCount = problem.Customers.Count,
                FitnessStrategy = MemeticLibrary.Fitness.FitnessStrategy.MINIMIZE,
                ConvergenceLimit = 1.0f,
                PreservedChromosomesNumber = 10,
                MaxIterations = 5000
            };
            parameters.CrossoverOperators.Add(new PartiallyMatchedCrossover());
            parameters.MutationOperators.Add(new SwapOperator());
            algorithm.Run(problem, parameters);
        }
    }
}
