using MemeticApplication.MemeticLibrary.Genetic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemeticApplication.MemeticLibrary.Operators
{
    public interface IOperator : ICloneable
    {
        int InputArity();

        int OutputArity();

        string GetId();
    }
}
