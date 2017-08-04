using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemeticApplication.MemeticLibrary.Genetic;
using MemeticApplication.MemeticLibrary.Model;

namespace MemeticApplication.MemeticLibrary.Operators.Mutation
{
    public interface IMutationOperator
    {
        /// <summary>
        /// Invokes the specified mutation operator on the solution.
        /// The operator performs a single mutation operation and returns
        /// a new solution.
        /// </summary>
        /// <param name="solution">The solution.</param>
        void Run(ref Chromosome solution);
    }
}
