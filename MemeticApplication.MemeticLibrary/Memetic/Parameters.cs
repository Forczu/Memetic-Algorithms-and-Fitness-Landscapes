using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemeticApplication.MemeticLibrary.Factories;
using MemeticApplication.MemeticLibrary.Fitness;
using MemeticApplication.MemeticLibrary.Generators;
using MemeticApplication.MemeticLibrary.Heuristics;
using MemeticApplication.MemeticLibrary.Operators.Crossover;
using MemeticApplication.MemeticLibrary.Operators.Mutation;
using MemeticApplication.MemeticLibrary.Operators.Selection;

namespace MemeticApplication.MemeticLibrary.Memetic
{
    public class ParametersWebsite
    {
        public int Iterations { get; set; }

        public int PopulationSize { get; set; }

        public int EliteChildrenCount { get; set; }

        public string Selection { get; set; }

        public string Crossover { get; set; }

        public string Mutation { get; set; }

        public string Heuristics { get; set; }

        public float CrossoverProbability { get; set; }

        public float MutationProbability { get; set; }

        public float FitnessRoutes { get; set; }

        public float FitnesDistance { get; set; }

        public float ConvergenceLimit { get; set; }

        public int PreservedChromosomeNumber { get; set; }

        public float[] HeuristicsParameters { get; set; }
    }

    public class Parameters
    {
        public int PopulationSize { get; set; }

        public IHeuristics Heuristics { get; set; }

        public Selection Selection { get; set; }
        
        public IList<CrossoverOperator> CrossoverOperators { get; set; }

        public IList<MutationOperator> MutationOperators { get; set; }

        public float CrossoverProbability { get; set; }

        public float MutationProbability { get; set; }

        public FitnessFunction Fitness { get; set; }

        public AbstractChromosomeFactory ChromosomeFactory { get; set; }

        public int GeneCount { get; set; }

        public int EliteChildrenCount { get; set; }

        public FitnessStrategy FitnessStrategy { get; set; }

        public float ConvergenceLimit { get; set; }

        public int PreservedChromosomesNumber { get; set; }

        public int MaxIterations { get; set; }

        public float[] HeuristicsParameters { get; set; }
    }
}
