using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemeticApplication.MemeticLibrary.Genetic
{
    public class UintGene : IGene
    {
        public uint Value { get; set; }

        public UintGene(uint value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            UintGene other = obj as UintGene;
            if (other == null)
                return false;
            if (Value != other.Value)
                return false;
            return true;
        }

        public override int GetHashCode()
        {
            return (int)Value;
        }

        public object Clone()
        {
            return new UintGene(Value);
        }

        public object GetValue()
        {
            return Value;
        }
    }
}
