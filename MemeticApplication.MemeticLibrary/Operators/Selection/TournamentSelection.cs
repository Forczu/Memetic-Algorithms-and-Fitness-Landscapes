using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemeticApplication.MemeticLibrary.Genetic;
using MemeticApplication.MemeticLibrary.Memetic;
using MemeticApplication.MemeticLibrary.Model;
using MemeticApplication.MemeticLibrary.Generators;

namespace MemeticApplication.MemeticLibrary.Operators.Selection
{
    public class TournamentSelection : Selection
    {
        public override List<Chromosome> Run(List<Chromosome> chromosomes, Parameters parameters)
        {
            int k = 3, index;
            var chosenOnes = new Chromosome[chromosomes.Count];
            var tournament = new Chromosome[k];
            Chromosome winner = null;
            for (int i = 0; i < chromosomes.Count; ++i)
            {
                for (int j = 0; j < k; ++j)
                {
                    index = RandomGeneratorThreadSafe.NextInt(chromosomes.Count);
                    tournament[j] = chromosomes[index];
                }
                //if (parameters.FitnessStrategy == Fitness.FitnessStrategy.MAXIMIZE)
                winner = tournament[0];
                for (int j = 1; j < k; ++j)
                {
                    if (parameters.Fitness.Get(tournament[j]) < parameters.Fitness.Get(winner))
                        winner = tournament[j];
                }
               // winner = tournament.Aggregate((c1, c2) => parameters.Fitness.Get(c1) < parameters.Fitness.Get(c2) ? c1 : c2);
                chosenOnes[i] = (Chromosome)winner.Clone();
            }
            return chosenOnes.ToList();
        }
    }
}
