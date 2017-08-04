using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemeticApplication.MemeticLibrary.Genetic;
using MemeticApplication.MemeticLibrary.Model;

namespace MemeticApplication.MemeticLibrary.Entropy
{
    public class ShannonEntropy
    {
        private static float LogTwo(float num)
        {
            return (float)(Math.Log(num) / Math.Log(2));
        }

        public static float GetEntropy(Population population)
        {
            int populationSize = population.Size;
            int chromosomeSize = population.Chromosomes.FirstOrDefault().Size();

            var table = new List<Dictionary<object, float>>();
            for (int i = 0; i < chromosomeSize; ++i)
            {
                table.Add(new Dictionary<object, float>());
            }

            List<float> entropies = new List<float>();
            for (int i = 0; i < populationSize; ++i)
            {
                for (int j = 0; j < chromosomeSize; ++j)
                {
                    var dic = table[j];
                    var value = population[i].Genes[j];
                    if (dic.ContainsKey(value))
                        ++dic[value];
                    else
                        dic.Add(value, 1.0f);
                }
            }
            
            for (int i = 0; i < chromosomeSize; ++i)
            {
                float freq, infoC = 0;
                foreach (KeyValuePair<object, float> letter in table[i])
                {
                    freq = letter.Value / populationSize;
                    infoC += freq * LogTwo(freq);
                }
                infoC *= -1;
                entropies.Add(infoC);
            }

            float avgEntropy = entropies.Average();
            return avgEntropy;
        }
    }
}
