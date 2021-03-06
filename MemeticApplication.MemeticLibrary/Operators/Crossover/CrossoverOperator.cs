﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemeticApplication.MemeticLibrary.Factories;
using MemeticApplication.MemeticLibrary.Genetic;
using MemeticApplication.MemeticLibrary.Model;

namespace MemeticApplication.MemeticLibrary.Operators.Crossover
{
    public abstract class CrossoverOperator : IOperator
    {
        protected static readonly int INPUT_ARITY = 2;
        protected static readonly int OUTPUT_ARITY = 2;

        /// <summary>
        /// Invokes the specified operator on two parents. The operator takes
        /// parts of the parent solutions, mixes them with magic and returns two children.
        /// </summary>
        /// <param name="parent1">The first parent.</param>
        /// <param name="parent2">The second parent.</param>
        /// <param name="child1">The first child.</param>
        /// <param name="child2">The second child.</param>
        public abstract void Run(IGene[] parent1, IGene[] parent2, out IGene[] child1, out IGene[] child2);
        
        public int InputArity()
        {
            return INPUT_ARITY;
        }

        public int OutputArity()
        {
            return OUTPUT_ARITY;
        }

        public abstract string GetId();

        public abstract object Clone();
    }
}
