using MemeticApplication.MemeticLibrary.Factories;
using MemeticApplication.MemeticLibrary.Fitness;
using MemeticApplication.MemeticLibrary.Landscapes;
using MemeticApplication.MemeticLibrary.Measures;
using MemeticApplication.MemeticLibrary.Model;
using MemeticApplication.MemeticLibrary.Operators.Crossover;
using MemeticApplication.MemeticLibrary.Operators.Mutation;
using MemeticApplication.MemeticLibrary.Readers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemeticApplication.Tests.Landscapes
{
    [TestClass]
    public class ResearchTests
    {
        private static readonly string FILE_PATH = @"E:\Uczelnia\Sem 10\MemeticApplication\MemeticApplication\Data\Homberger";

        private static readonly string RESULT_PATH = @"E:\Uczelnia\Sem 10\MemeticApplication\MemeticApplication\Results\Homberger";

        private static readonly string SAMPLE_INSTANCE_PATH = @"E:\Uczelnia\Sem 10\MemeticApplication\MemeticApplication\Data\Homberger\C1\C1_2_1.txt";

        private static readonly int NUMBERS_AFTER_COMMA = 3;
        private static readonly float DELTA = 0.01f;

        private VrptwProblemReader reader = null;

        [TestInitialize]
        public void Setup()
        {
            reader = new VrptwProblemReader();
        }

        [TestMethod]
        public void Test1()
        {
            AbstractChromosomeFactory factory = new SolutionFactory();
            int[] routeWeights = new int[]
            {
                20000, 50000, 120000, 200000, 350000
            };
            int distanceWeight = 1;
            string[] customerTypes = new string[] { "C1", "C2", "R1", "R2", "RC1", "RC2" };
            Dictionary<string, int> customerNumbers = new Dictionary<string, int>()
            {
                { "2", 20000 }, { "4", 50000 }, {  "6", 120000 }, { "8", 200000 }, { "10", 350000 }
            };
            string[] customerInstances = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };

            CrossoverOperator[] crossoverOps = new CrossoverOperator[]
            {
                new OrderCrossover(), new PartiallyMatchedCrossover(), new CycleCrossover(), new UniformBasedOrderCrossover()
            };

            MutationOperator[] mutationOps = new MutationOperator[]
            {
                new SwapOperator(), new InsertionOperator(), new InversionOperator(), new DisplacementOperator()
            };

            int randomWalkNumber = 2000, randomWalkSteps = 5000;

            string floatPattern = "0.000", separator = ",";
            float epsilon = 0.05f;

            foreach (var type in customerTypes)
            {
                foreach (var number in customerNumbers)
                {
                    foreach (var instance in customerInstances)
                    {
                        string instanceId = type + '_' + number.Key + '_' + instance;
                        VrptwProblem problem = reader.ReadFromFile(FILE_PATH + @"\" + instanceId + ".txt");
                        FitnessFunction ff = new FitnessFunction(number.Value, distanceWeight);
                        Landscape landscape = new Landscape(problem, factory, ff);
                        foreach (var op in crossoverOps)
                        {
                            string path = RESULT_PATH + @"\" + instanceId + "_" + op.GetId() + ".csv";
                            if (!File.Exists(path))
                            {
                                File.Create(path).Close();
                                File.ReadAllText(path);
                                using (TextWriter tw = new StreamWriter(path))
                                {
                                    tw.WriteLine("AC, IC, PIC, DBI");
                                    for (int i = 0; i < randomWalkNumber; ++i)
                                    {
                                        var rwResult = landscape.RandomWalk(randomWalkSteps, op);
                                        float ac  = Autocorrelation.Run(rwResult);
                                        float ic  = InformationContent.Run(rwResult, epsilon);
                                        float pic = PartialInformationContent.Run(rwResult, epsilon);
                                        float dbi = DensityBasinInformation.Run(rwResult, epsilon);
                                        string line = 
                                            ac.ToString(floatPattern)  + separator +
                                            ic.ToString(floatPattern)  + separator + 
                                            pic.ToString(floatPattern) + separator + 
                                            dbi.ToString(floatPattern);
                                        tw.WriteLine(line);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ExpectedValueShouldBeCorrect()
        {
            float[] diceValues = new float[] { 1, 2, 3, 4, 5, 6 };
            float expectedExpectedValue = 3.500f;
            RandomWalk rw = new RandomWalk(diceValues);
            float expectedValue = rw.ExpectedValue(NUMBERS_AFTER_COMMA);
            Assert.AreEqual(expectedExpectedValue, expectedValue, DELTA);
        }

        [TestMethod]
        public void VarianceShouldBeCorrect()
        {
            float[] diceValues = new float[] { 1, 2, 3, 4, 5, 6 };
            float expectedVarianceValue = 2.91666667f;
            RandomWalk rw = new RandomWalk(diceValues);
            float variance = rw.Variance(NUMBERS_AFTER_COMMA);
            Assert.AreEqual(expectedVarianceValue, variance, DELTA);
        }
    }
}
