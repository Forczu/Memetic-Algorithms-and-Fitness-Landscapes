using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemeticApplication.MemeticLibrary.Genetic;
using MemeticApplication.MemeticLibrary.Model;

namespace MemeticApplication.MemeticLibrary.Operators.Mutation
{
    public abstract class MutationOperator : IOperator
    {
        protected static readonly int INPUT_ARITY = 1;
        protected static readonly int OUTPUT_ARITY = 1;

        /// <summary>
        /// Invokes the specified mutation operator on the solution.
        /// The operator performs a single mutation operation and returns
        /// a new solution.
        /// </summary>
        /// <param name="solution">The solution.</param>
        public abstract void Run(ref Chromosome solution);

        public abstract void Run(IGene[] genes);

        public abstract string GetId();
        public abstract object Clone();

        public int InputArity()
        {
            return INPUT_ARITY;
        }

        public int OutputArity()
        {
            return OUTPUT_ARITY;
        }
    }
}
