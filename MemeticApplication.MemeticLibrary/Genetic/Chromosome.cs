using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemeticApplication.MemeticLibrary.Genetic
{
    public abstract class Chromosome : IComparable, ICloneable
    {
        public IProblem Problem { get; protected set; }

        public IGene[] Genes { get; set; }

        public Chromosome(IProblem problem)
        {
            Problem = problem;
            if (Problem != null)
                Genes = new IGene[problem.GeneCount()] ;
        }

        public Chromosome(IProblem problem, IGene[] genes)
        {
            Problem = problem;
            if (Problem != null)
                Genes = new IGene[problem.GeneCount()];
            Genes = genes;
        }


        public int Size()
        {
            if (Genes == null)
                return 0;
            return Genes.Count();
        }

        public override bool Equals(object obj)
        {
            Chromosome other = obj as Chromosome;
            if (other == null)
                return false;
            for (int i = 0; i < Genes.Count(); ++i)
            {
                if (!Genes[i].Equals(other.Genes[i]))
                    return false;
            }
            return true;
        }

        public abstract float[] FitnessValues();

        public abstract int CompareTo(object obj);

        public abstract object Clone();

        public abstract void Refresh();
    }
}
