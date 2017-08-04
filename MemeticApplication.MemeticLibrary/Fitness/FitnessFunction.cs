using MemeticApplication.MemeticLibrary.Genetic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemeticApplication.MemeticLibrary.Fitness
{
    public class FitnessFunction
    {
        protected float[] Constants { get; set; }

        public FitnessFunction(params float[] constants)
        {
            Constants = constants;
        }

        public float Get(Chromosome chromosome)
        {
            float[] values = chromosome.FitnessValues();
            float fitness = 0.0f;
            for (int i = 0; i < Constants.Length; ++i)
            {
                fitness += values[i] * Constants[i];
            }
            return fitness;
        }
    }
}
