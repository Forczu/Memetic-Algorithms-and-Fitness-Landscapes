using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemeticApplication.MemeticLibrary.Factories;
using MemeticApplication.MemeticLibrary.Generators;
using MemeticApplication.MemeticLibrary.Genetic;
using MemeticApplication.MemeticLibrary.Memetic;
using MemeticApplication.MemeticLibrary.Model;

namespace MemeticApplication.MemeticLibrary.Heuristics
{
    public class SimulatedAnnealing : IHeuristics
    {
        protected Parameters Parameters { get; set; }

        public Chromosome Run(Chromosome initialSolution, Parameters parameters)
        {
            Parameters = parameters;

            Chromosome currentSolution = initialSolution;
            Chromosome bestSolution = initialSolution;

            double temperature = parameters.HeuristicsParameters[0];
            double beta = parameters.HeuristicsParameters[1];
            int iterations = (int)parameters.HeuristicsParameters[2];

            for (int i = 0; i < iterations; ++i)
            {
                AnnealingStep(ref currentSolution, ref bestSolution, temperature);
                temperature *= beta;
            }
            return bestSolution;
        }

        protected void AnnealingStep(ref Chromosome currentSolution, ref Chromosome bestSolution, double temperature)
        {
            Chromosome newSolution = Parameters.ChromosomeFactory.RandomNeighbourSolution(currentSolution, Parameters.MutationOperators.First());
            float delta = GetDelta(newSolution, currentSolution);
            if (delta < 0)
            {
                SetNewSolutions(newSolution, ref currentSolution, ref bestSolution);
            }
            else
            {
                double x = RandomGeneratorThreadSafe.NextDouble();
                double e = Math.Exp(-delta / temperature);
                if (x < e)
                {
                    SetNewSolutions(newSolution, ref currentSolution, ref bestSolution);
                }
            }
        }

        /// <summary>
        /// Gets the delta between fitnesses of the first and second solution.
        /// </summary>
        /// <param name="solution1">The first solution.</param>
        /// <param name="solution2">The second solution.</param>
        /// <returns></returns>
        protected float GetDelta(Chromosome solution1, Chromosome solution2)
        {
            float fitness1 = Parameters.Fitness.Get(solution1);
            float fitness2 = Parameters.Fitness.Get(solution2);
            float delta = fitness1 - fitness2;
            return delta;
        }

        /// <summary>
        /// Sets the new current solution and the best one if the new one is better.
        /// </summary>
        /// <param name="newSolution">The new solution.</param>
        /// <param name="currentSolution">The current solution.</param>
        /// <param name="bestSolution">The best solution.</param>
        protected void SetNewSolutions(Chromosome newSolution, ref Chromosome currentSolution, ref Chromosome bestSolution)
        {
            currentSolution = newSolution;
            float delta = GetDelta(newSolution, bestSolution);
            if (delta < 0)
                bestSolution = newSolution;
        }
    }
}
