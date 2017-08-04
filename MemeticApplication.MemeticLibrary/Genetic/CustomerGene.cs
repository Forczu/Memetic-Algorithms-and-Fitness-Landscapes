using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemeticApplication.MemeticLibrary.Model;

namespace MemeticApplication.MemeticLibrary.Genetic
{
    class CustomerGene : IGene
    {
        public Customer Value { get; set; }

        public CustomerGene(Customer value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            CustomerGene other = obj as CustomerGene;
            if (other == null)
                return false;
            if (!Value.Equals(other.Value))
                return false;
            return true;
        }

        public object Clone()
        {
            return new CustomerGene(Value);
        }

        public object GetValue()
        {
            return Value;
        }

        public override string ToString()
        {
            return "Customer ID: " + Value.Id;
        }
    }
}
