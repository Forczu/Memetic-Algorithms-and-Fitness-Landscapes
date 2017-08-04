using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemeticApplication.MemeticLibrary.Genetic;

namespace MemeticApplication.MemeticLibrary.Factories
{
    public abstract class AbstractChromosomeFactory
    {
        public abstract Chromosome MakeChromosome(IProblem problem);

        public abstract Chromosome MakeChromosome(IProblem problem, IGene[] genes);
        
        public abstract Chromosome RandomNeighbourSolution(Chromosome chromosome);

        public abstract Chromosome RandomSolution(int geneNumber, IProblem problem);
    }
}
