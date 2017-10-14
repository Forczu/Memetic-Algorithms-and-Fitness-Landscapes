using MemeticApplication.MemeticLibrary.Factories;
using MemeticApplication.MemeticLibrary.Fitness;
using MemeticApplication.MemeticLibrary.Landscapes;
using MemeticApplication.MemeticLibrary.Measures;
using MemeticApplication.MemeticLibrary.Model;
using MemeticApplication.MemeticLibrary.Operators;
using MemeticApplication.MemeticLibrary.Operators.Crossover;
using MemeticApplication.MemeticLibrary.Operators.Mutation;
using MemeticApplication.MemeticLibrary.Readers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MemeticApplication.MemeticLibrary.Research
{
    public class LandscapeResearch
    {
        private static readonly string FLOAT_PATTERN = "0.000";

        private static readonly string SEPARATOR = ",";

        private static readonly string CSV_HEADER = "AC, IC, PIC, DBI";

        private static readonly string[] CUSTOMER_TYPES = new string[]
        {
            "C1","C2", "R1", "R2", "RC1", "RC2"
        };
        
        private static readonly string[] CUSTOMER_INSTANCES = new string[]
        {
            "1", "2", "3", "4", "5", "6",/* "7", "8", "9", "10"*/
        };

        private VrptwProblemReader reader = new VrptwProblemReader();

        private AbstractChromosomeFactory factory = new SolutionFactory();

        public void Run(ResearchParameters parameters, string inputPath, string outputPath, Action<string, float> callback, string connectionId)
        {
            int distanceWeight = 1;
            Dictionary<string, int> customerNumbers = new Dictionary<string, int>()
            {
                { "1", parameters.RoadWeight200 },
                { "2", parameters.RoadWeight200 },
                //{ "4", parameters.RoadWeight400 },
                //{ "6", parameters.RoadWeight600 }, { "8", parameters.RoadWeight800 },
                //{ "10", parameters.RoadWeight1000 }
            };

            float step = 1.0f /
                (CUSTOMER_TYPES.Count() * customerNumbers.Count * CUSTOMER_INSTANCES.Count() *
                 (parameters.CrossoverOperators.Count + parameters.MutationOperators.Count) * parameters.RandomWalkNumber);

            foreach (var type in CUSTOMER_TYPES)
            {
                foreach (var number in customerNumbers)
                {
                    for (int index = 0; index < CUSTOMER_INSTANCES.Count(); ++index)
                    {
                        string instanceId = type + '_' + number.Key + '_' + CUSTOMER_INSTANCES[index];
                        VrptwProblem problem = reader.ReadFromFile(inputPath + @"\" + instanceId + ".txt");
                        FitnessFunction ff = new FitnessFunction(number.Value, distanceWeight);
                        Landscape landscape = new Landscape(problem, factory, ff);
                        foreach (var op in parameters.CrossoverOperators)
                        {
                            RunSingleResearch(landscape, parameters, op, instanceId, inputPath, outputPath,
                                callback, connectionId, step);
                        }
                        foreach (var op in parameters.MutationOperators)
                        {
                            RunSingleResearch(landscape, parameters, op, instanceId, inputPath, outputPath,
                                callback, connectionId, step);
                        }
                    }
                }
            }
        }

        public void RunSingleResearch(Landscape landscape, ResearchParameters parameters, IOperator op, string instanceId,
            string inputPath, string outputPath, Action<string, float> callback, string connectionId, float step)
        {
            string path = outputPath + @"\" + instanceId + "_" + op.GetId() + ".csv";
            if (File.Exists(path))
            {
                callback(connectionId, step * (parameters.RandomWalkNumber));
                return;
            }
            StringBuilder dataBuilder = new StringBuilder();
            int threadNumber = Environment.ProcessorCount;
            Thread[] rwThreads = new Thread[threadNumber];
            int tasksPerThread = parameters.RandomWalkNumber / threadNumber;
            for (int i = 0; i < threadNumber; ++i)
            {
                int start = i * tasksPerThread;
                int end = (i + 1) * tasksPerThread;
                if (i == threadNumber - 1)
                    end += parameters.RandomWalkNumber % threadNumber;
                rwThreads[i] = new Thread(() =>
                {
                    DoRandomWalkThread(start, end, landscape, parameters, op,
                        dataBuilder, callback, connectionId, step);
                });
                rwThreads[i].Start();
            }
            foreach (Thread thread in rwThreads)
                thread.Join();
            File.Create(path).Close();
            using (TextWriter tw = new StreamWriter(path))
            {
                tw.WriteLine(CSV_HEADER);
                tw.Write(dataBuilder.ToString());
            }
        }

        public void DoRandomWalkThread(int start, int end, Landscape landscape, ResearchParameters parameters, IOperator op,
            StringBuilder dataBuilder, Action<string, float> callback, string connectionId, float step)
        {
            for (int j = start; j < end; ++j)
            {
                var rwResult = landscape.RandomWalk(parameters.RandomWalkSteps, op);
                float ac = Autocorrelation.Run(rwResult);
                float ic = InformationContent.Run(rwResult, parameters.Sensitivity);
                float pic = PartialInformationContent.Run(rwResult, parameters.Sensitivity);
                float dbi = DensityBasinInformation.Run(rwResult, parameters.Sensitivity);
                string line =
                    (float.IsNaN(ac)  ? FLOAT_PATTERN : ac.ToString(FLOAT_PATTERN)) + SEPARATOR +
                    (float.IsNaN(ic)  ? FLOAT_PATTERN : ic.ToString(FLOAT_PATTERN)) + SEPARATOR +
                    (float.IsNaN(pic) ? FLOAT_PATTERN : pic.ToString(FLOAT_PATTERN)) + SEPARATOR +
                    (float.IsNaN(dbi) ? FLOAT_PATTERN : dbi.ToString(FLOAT_PATTERN));
                dataBuilder.AppendLine(line);
                callback(connectionId, step);
            }
        }
    }
}
