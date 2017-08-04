using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MemeticApplication.MemeticLibrary.Readers;
using MemeticApplication.MemeticLibrary.Landscapes;
using MemeticApplication.MemeticLibrary.Factories;
using MemeticApplication.MemeticLibrary.Fitness;
using System.IO;
using MemeticApplication.MemeticLibrary.Operators.Crossover;

namespace MemeticApplication.Tests
{
    [TestClass]
    public class RandomWalkTests
    {
        private SolomonProblemReader reader = null;
        private AbstractChromosomeFactory factory = null;
        private FitnessFunction fitness = null;

        private static readonly string FILE_PATH = @"D:\Uczelnia\Sem 10\MGR\Dane\Solomon\solomon_25\C101.txt";

        [TestInitialize]
        public void SetUp()
        {
            reader = new SolomonProblemReader();
            factory = new SolutionFactory();
            fitness = new FitnessFunction(5000, 1);
        }

        [TestMethod]
        public void Solomon50RandomWalk()
        {
            var problem = reader.ReadFromFile(FILE_PATH);
            var landscape = new Landscape(problem, new PartiallyMatchedCrossover(), factory, fitness);
            var data = landscape.RandomWalk(2000);
            var dataStr = data.ToString();
            string path = @"E:\Solomon_100_C101_RW.csv";
            if (!File.Exists(path))
                File.Create(path);
            using (var tw = new StreamWriter(path))
            {
                tw.Write(dataStr);
            }
        }
    }
}
