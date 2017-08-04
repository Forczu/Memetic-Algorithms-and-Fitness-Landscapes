using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemeticApplication.MemeticLibrary.Statistics
{
    public class UnaryFunction
    {
        private float[] data = null;

        public UnaryFunction(int xDomainSize)
        {
            data = new float[xDomainSize];
        }

        public void Set(int x, float y)
        {
            data[x] = y;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; ++i)
            {
                sb.Append(i.ToString() + ',' + data[i].ToString("0.0000", CultureInfo.CreateSpecificCulture("en-GB")) + '\n');
            }
            return sb.ToString();
        }
    }
}
