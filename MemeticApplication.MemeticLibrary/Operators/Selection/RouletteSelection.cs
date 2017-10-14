using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemeticApplication.MemeticLibrary.Fitness;
using MemeticApplication.MemeticLibrary.Generators;
using MemeticApplication.MemeticLibrary.Genetic;
using MemeticApplication.MemeticLibrary.Memetic;
using MemeticApplication.MemeticLibrary.Model;

namespace MemeticApplication.MemeticLibrary.Operators.Selection
{
    /// <summary>
    /// Represents the roulette selection operator.
    /// </summary>
    public class RouletteSelection : Selection
    {
        public override List<Chromosome> Run(List<Chromosome> chromosomes, Parameters parameters)
        {
            chromosomes.Sort();
            List<float> distribution = new List<float>(chromosomes.Count);
            float partialSum = 0.0f;
            var chosenOnes = new List<Chromosome>();
            for (int i = 0; i < chromosomes.Count; ++i)
            {
                switch (parameters.FitnessStrategy)
                {
                    case FitnessStrategy.MINIMIZE:
                        partialSum += 1.0f / parameters.Fitness.Get(chromosomes[i]);
                        break;
                    case FitnessStrategy.MAXIMIZE:
                        partialSum += parameters.Fitness.Get(chromosomes[i]);
                        break;
                }
                distribution.Add(partialSum);
            }
            for (int i = 0; i < chromosomes.Count; ++i)
            {
                float nextRand = (float)RandomGeneratorThreadSafe.NextDouble(partialSum);
                float chosenOne = distribution.First(d => d >= nextRand);
                int index = distribution.IndexOf(chosenOne);
                Chromosome chosenSolution = chromosomes[index];
                chosenOnes.Add(chosenSolution);
            }
            return chosenOnes;
        }
    }
}
