using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemeticApplication.MemeticLibrary.Genetic
{
    public interface IGene : ICloneable
    {
        object GetValue();
    }
}
