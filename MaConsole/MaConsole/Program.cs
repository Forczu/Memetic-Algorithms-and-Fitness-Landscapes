using MemeticApplication.MemeticLibrary.Factories;
using MemeticApplication.MemeticLibrary.Fitness;
using MemeticApplication.MemeticLibrary.Genetic;
using MemeticApplication.MemeticLibrary.Heuristics;
using MemeticApplication.MemeticLibrary.Memetic;
using MemeticApplication.MemeticLibrary.Model;
using MemeticApplication.MemeticLibrary.Operators;
using MemeticApplication.MemeticLibrary.Operators.Crossover;
using MemeticApplication.MemeticLibrary.Operators.Mutation;
using MemeticApplication.MemeticLibrary.Operators.Selection;
using MemeticApplication.MemeticLibrary.Readers;
using MemeticApplication.MemeticLibrary.Research;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MaConsole
{
    class Program
    {
        public static int walkCounter = 1;
        public static int stepCounter = 1;

        private static int walkNumber = 2000;
        private static int stepNumber = 10000;

        public static Stopwatch timer = new Stopwatch();

        private static object locker = new object();

        private static float progress = 0.0f;

        private static int pairsSize;

        private static int pairsCounter;

        public static void ClearCurrentConsoleLine()
        {
            Console.Clear();
        }

        public static void Callback(string a, float percent)
        {
            lock (locker)
            {
                progress += percent;
                if (stepCounter > walkNumber)
                {
                    stepCounter = 0;
                    ++walkCounter;
                }
                ClearCurrentConsoleLine();
                long ms = timer.ElapsedMilliseconds;
                Console.Write("Instance " + walkCounter + " walk " + stepCounter + ", " + progress * 100 + "% finished, " + ((float)ms / 1000).ToString("0.000") + " s elapsed.");
                ++stepCounter;
            }
        }

        static void CallBackMa(string id, Chromosome bestOne, int iteration, float distance, long time, int restarts)
        {
            lock (locker)
            {
                ClearCurrentConsoleLine();
                Console.Write(pairsCounter + " / " + pairsSize + ", pair " + id + ", route number " + (bestOne as Solution).Routes.Count + ", distance " + distance +  
                    ", iteration " + iteration + ", restarts: " + restarts + ", time " + ((float)time / 1000).ToString("0.000") + " s elapsed.");
            }
        }

        static void RepairNaNs()
        {
            string path = @"C:\Users\Forczu\Documents\Visual Studio 2015\Projects\MaConsole\MaConsole\bin\Release\Results";
            foreach (var file in Directory.GetFiles(path))
            {
                string text = File.ReadAllText(file);
                text = text.Replace("NaN", "0.000");
                File.WriteAllText(file, text);
            }
        }

        public static async Task<Chromosome> DoMa(string filename, CrossoverOperator crossOp, MutationOperator mutOp)
        {
            var problem = new VrptwProblemReader().ReadFromFile(filename);
            var parameters = new Parameters()
            {
                ChromosomeFactory = new SolutionFactory(),
                ConvergenceLimit = 0.15f,
                CrossoverOperators = new List<CrossoverOperator> { crossOp },
                CrossoverProbability = 0.9f,
                EliteChildrenCount = 4,
                Fitness = new FitnessFunction(200000, 1),
                FitnessStrategy = FitnessStrategy.MINIMIZE,
                GeneCount = problem.GeneCount(),
                Heuristics = new SimulatedAnnealing(),
                HeuristicsParameters = new float[] { 100, 0.90f, 50 },
                MaxIterations = 300,
                MutationOperators = new List<MutationOperator> { mutOp },
                MutationProbability = 0.15f,
                PopulationSize = 200,
                PreservedChromosomesNumber = 4,
                Selection = new TournamentSelection()
            };
            MemeticAlgorithm ma = new MemeticAlgorithm();
            Chromosome sol = await ma.Run(problem, parameters, CallBackMa, crossOp.GetId() + "-" + mutOp.GetId());
            return sol;
        }

        public static async Task<Chromosome> DoMa(string filename, IOperator op)
        {
            var problem = new VrptwProblemReader().ReadFromFile(filename);
            var crossovers = new List<CrossoverOperator>();
            var mutations = new List<MutationOperator> { };
            if (op is CrossoverOperator)
                crossovers.Add(op as CrossoverOperator);
            else
                mutations.Add(op as MutationOperator);

            var parameters = new Parameters()
            {
                ChromosomeFactory = new SolutionFactory(),
                ConvergenceLimit = 0.15f,
                CrossoverOperators = crossovers,
                CrossoverProbability = 0.8f,
                EliteChildrenCount = 2,
                Fitness = new FitnessFunction(200000, 1),
                FitnessStrategy = FitnessStrategy.MINIMIZE,
                GeneCount = problem.GeneCount(),
                Heuristics = new SimulatedAnnealing(),
                HeuristicsParameters = new float[] { 100, 0.90f, 50 },
                MaxIterations = 300,
                MutationOperators = mutations,
                MutationProbability = 0.2f,
                PopulationSize = 200,
                PreservedChromosomesNumber = 2,
                Selection = new TournamentSelection()
            };
            MemeticAlgorithm ma = new MemeticAlgorithm();
            Chromosome sol = await ma.Run(problem, parameters, CallBackMa, op.GetId(), true);
            return sol;
        }


        public static void Main(string[] args)
        {
            /*
            string myLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            myLocation = System.IO.Path.GetDirectoryName(myLocation);

            var crossoverOperators = new List<CrossoverOperator> {
                new OrderCrossover(),
                new PartiallyMatchedCrossover(),
                new CycleCrossover(),
                new UniformBasedOrderCrossover()
            };

            var mutationOperators = new List<MutationOperator> {
                new SwapOperator(),
                new InsertionOperator(),
                new InversionOperator(),
                new DisplacementOperator()
            };

            int gaRuns = 5;
            pairsCounter = 1;
            pairsSize = (crossoverOperators.Count + mutationOperators.Count) * Directory.GetFiles(myLocation + @"\TestData").Count() * gaRuns;

            foreach (var file in Directory.GetFiles(myLocation + @"\TestData"))
            {
                string name = System.IO.Path.GetFileName(file);
                string crossId, mutId;
                StringBuilder sb = new StringBuilder();
                string filename = "Result_" + name + ".txt";
                string filepath = myLocation + @"\" + filename;
                if (File.Exists(filepath))
                {
                    pairsCounter += gaRuns * (crossoverOperators.Count + mutationOperators.Count);
                    continue;
                }
                foreach (var crossOp in crossoverOperators)
                {
                    crossId = crossOp.GetId();
                    sb.Append(crossId + "\n");
                    for (int i = 0; i < gaRuns; i++)
                    {
                        Task<Chromosome> sol = DoMa(file, crossOp);
                        while (true)
                        {
                            Thread.Sleep(1000);
                            if (sol.Status == TaskStatus.RanToCompletion)
                                break;
                        }
                        pairsCounter++;
                        Solution s = sol.Result as Solution;
                        sb.Append(s.Routes.Count + ", " + s.TotalDistance() + "\n");
                    }
                }
                foreach (var mutOp in mutationOperators)
                {
                    mutId = mutOp.GetId();
                    sb.Append(mutOp + "\n");
                    for (int i = 0; i < gaRuns; i++)
                    {
                        Task<Chromosome> sol = DoMa(file, mutOp);
                        while (true)
                        {
                            Thread.Sleep(1000);
                            if (sol.Status == TaskStatus.RanToCompletion)
                                break;
                        }
                        pairsCounter++;
                        Solution s = sol.Result as Solution;
                        sb.Append(s.Routes.Count + ", " + s.TotalDistance() + "\n");
                    }
                }
                using (StreamWriter sw = new StreamWriter(filepath))
                {
                    sw.Write(sb.ToString());
                }
            }*/

            /*
            string loc = @"C:\Users\Forczu\Documents\Visual Studio 2015\Projects\MaConsole\MaConsole\bin\Release";
            string probLoc = @"C:\Users\Forczu\Documents\Visual Studio 2015\Projects\MaConsole\MaConsole\bin\Release\TestData\R1_1_1.txt";
            var prob = new VrptwProblemReader().ReadFromFile(probLoc);
            List<Route> routes = new List<Route>();
            using (StreamReader sr = new StreamReader(loc))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    Route r = new Route(prob);
                    r.AddCustomer(prob.Depot);
                    Regex reg = new Regex("[0-9]+");
                    var matches = reg.Matches(line);
                    foreach (var match in matches)
                    {
                        int id = Convert.ToInt32(match);
                        Customer cust = prob.Customers.Where(c => c.Id == id).Single();
                        r.AddCustomer(cust);
                        routes.Add(r);
                    }
                    r.AddCustomer(prob.Depot);
                }
            }
            bool feasible = true;
            foreach (var route in routes)
            {
                float distance = 0.0f, capacity = 0.0f;
                Customer customer1, customer2;
                for (int i = 0; i < route.Customers.Count; i++)
                {
                    customer1 = route.Customers[i];
                    customer2 = route.Customers[i + 1];
                    distance += prob.GetDistance(customer1.Id, customer2.Id);
                    if (distance > customer2.DueDate || capacity + customer2.Demand > prob.VehicleCapacity)
                    {
                        feasible = false;
                    }
                    capacity += customer2.Demand;
                    if (distance < customer2.ReadyTime)
                    {
                        distance = customer2.ReadyTime;
                    }
                }
            }*/


            string myLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            myLocation = System.IO.Path.GetDirectoryName(myLocation);

            var crossoverOperators = new List<CrossoverOperator> {
                new OrderCrossover(),
                new PartiallyMatchedCrossover(),
                new CycleCrossover(),
                new UniformBasedOrderCrossover()
            };

            var mutationOperators = new List<MutationOperator> {
                new SwapOperator(),
                new InsertionOperator(),
                new InversionOperator(),
                new DisplacementOperator()
            };

            pairsCounter = 1;
            pairsSize = crossoverOperators.Count * mutationOperators.Count * Directory.GetFiles(myLocation + @"\TestData").Count();

            foreach (var file in Directory.GetFiles(myLocation + @"\TestData"))
            {
                string name = System.IO.Path.GetFileName(file);
                string crossId, mutId;
                foreach (var crossOp in crossoverOperators)
                {
                    crossId = crossOp.GetId();
                    foreach (var mutOp in mutationOperators)
                    {
                        string filename = "Result_" + name + "_" + crossOp.GetId() + "-" + mutOp.GetId() + ".txt";
                        string filepath = myLocation + @"\" + filename;
                        if (File.Exists(filepath))
                        {
                            pairsCounter++;
                            continue;
                        }

                        mutId = mutOp.GetId();

                        Task<Chromosome> sol = DoMa(file, crossOp, mutOp);
                        while (true)
                        {
                            Thread.Sleep(1000);
                            if (sol.Status == TaskStatus.RanToCompletion)
                                break;
                        }
                        pairsCounter++;
                        Solution s = sol.Result as Solution;
                        using (StreamWriter sw = new StreamWriter(filepath))
                        {
                            sw.WriteLine(s.Routes.Count + ", " + s.TotalDistance());
                        }
                    }
                }
            }


            Console.ReadKey();

            /*
            string myLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            myLocation = System.IO.Path.GetDirectoryName(myLocation);

            LandscapeResearch landscape = new LandscapeResearch();
            timer.Start();
            landscape.Run(new ResearchParameters()
            {
                Sensitivity = 0.04f,
                RandomWalkNumber = walkNumber,
                RandomWalkSteps = stepNumber,
                CrossoverOperators = new List<CrossoverOperator>()
                {
                    //new OrderCrossover(),
                    //new PartiallyMatchedCrossover(),
                    //new CycleCrossover(),
                    //new UniformBasedOrderCrossover()
                },
                MutationOperators = new List<MutationOperator>()
                {
                    //new SwapOperator(),
                    //new InsertionOperator(),
                    //new InversionOperator(),
                    new DisplacementOperator()
                },
                RoadWeight200 = 20000,
                RoadWeight400 = 50000,
                RoadWeight600 = 120000,
                RoadWeight800 = 200000,
                RoadWeight1000 = 350000
            },
            myLocation + @"\Data", myLocation + @"\Results",
            Callback,
            "asd");*/
        }
    }
}
