using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemeticApplication.MemeticLibrary.Genetic;
using MemeticApplication.MemeticLibrary.Memetic;
using MemeticApplication.MemeticLibrary.Model;

namespace MemeticApplication.MemeticLibrary.Operators.Selection
{
    public abstract class Selection
    {
        public abstract List<Chromosome> Run(List<Chromosome> chromosomes, Parameters parameters);
    }
}
