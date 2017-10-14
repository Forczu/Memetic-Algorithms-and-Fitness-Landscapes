using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemeticApplication.MemeticLibrary.Genetic
{
    public class UintChromosome : Chromosome
    {
        public UintChromosome() : base(null) { }

        public UintChromosome(int size) : base(null)
        {
            Genes = new IGene[size];
        }

        public UintChromosome(IProblem problem) : base(problem)
        {
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public override float[] FitnessValues()
        {
            throw new NotImplementedException();
        }

        public override void Refresh()
        {
            throw new NotImplementedException();
        }
    }
}
