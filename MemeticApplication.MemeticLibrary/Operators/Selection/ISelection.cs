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
    public interface ISelection
    {
        List<Chromosome> Run(List<Chromosome> chromosomes, Parameters parameters);

        Population Run(Population population, Parameters parameters);
    }
}
