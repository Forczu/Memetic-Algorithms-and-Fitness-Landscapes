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
        public Chromosome[] Chromosomes { get; protected set; }

        public Population(int size)
        {
            Chromosomes = new Chromosome[size];
        }

        public Population(Chromosome[] chromosomes)
        {
            Chromosomes = chromosomes;
        }

        public int Size { get { return Chromosomes.Count(); } }

        public void AddChromosome(Chromosome chromosome, int index)
        {
            Chromosomes[index] = chromosome;
        }

        public List<Chromosome> Take(int size)
        {
            var list = Chromosomes.Take(size).ToList();
            return list;
        }

        public List<Chromosome> Take(int start, int size)
        {
            var list = Chromosomes.Skip(start).Take(size).ToList();
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
            Array.Sort(Chromosomes);
        }

        public void Randomize()
        {
            Chromosomes.Shuffle();
        }

        public object Clone()
        {
            Population copy = new Population(Chromosomes.Count());
            for (int i = 0; i < Chromosomes.Count(); ++i)
            {
                copy[i] = (Chromosome)Chromosomes[i].Clone();
            }
            return copy;
        }
    }
}
