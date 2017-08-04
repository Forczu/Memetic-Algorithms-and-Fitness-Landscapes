using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemeticApplication.MemeticLibrary.Extensions;
using MemeticApplication.MemeticLibrary.Generators;
using MemeticApplication.MemeticLibrary.Genetic;

namespace MemeticApplication.MemeticLibrary.Model
{
    public class Population : ICloneable
    {
        public List<Chromosome> Chromosomes { get; protected set; }

        public Population()
        {
            Chromosomes = new List<Chromosome>();
        }

        public Population(List<Chromosome> chromosomes)
        {
            Chromosomes = chromosomes;
        }

        public int Size { get { return Chromosomes.Count; } }

        public void AddChromosome(Chromosome chromosome)
        {
            Chromosomes.Add(chromosome);
        }

        public void AddChromosome(IEnumerable<Chromosome> chromosomes)
        {
            Chromosomes.AddRange(chromosomes);
        }

        public List<Chromosome> Take(int size)
        {
            var list = Chromosomes.Take(size).ToList();
            return list;
        }

        public List<Chromosome> Take(int start, int size)
        {
            var list = Chromosomes.GetRange(start, size);
            return list;
        }

        public bool Contains(Chromosome chromosome)
        {
            return Chromosomes.Contains(chromosome);
        }

        public Chromosome this[int index]
        {
            get
            {
                return Chromosomes[index];
            }
            set
            {
                Chromosomes[index] = value;
            }
        }

        public void Sort()
        {
            Chromosomes.Sort();
        }

        public void Randomize()
        {
            Chromosomes.Shuffle();
        }

        public object Clone()
        {
            Population copy = new Population();
            foreach (var chromosome in Chromosomes)
            {
                copy.AddChromosome((Chromosome)chromosome.Clone());
            }
            return copy;
        }
    }
}
