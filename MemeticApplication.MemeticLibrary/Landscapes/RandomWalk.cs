using MemeticApplication.MemeticLibrary.Extensions;
using MemeticApplication.MemeticLibrary.Statistics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemeticApplication.MemeticLibrary.Landscapes
{
    public class RandomWalk
    {
        public float[] Data { get; protected set; }

        public int Size { get { return Data.Length; } }

        public RandomWalk(int size)
        {
            Data = new float[size];
        }

        public RandomWalk(float[] data)
        {
            Data = data;
        }

        public float Get(int index)
        {
            return Data[index];
        }

        public void Set(int index, float value)
        {
            Data[index] = value;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Data.Length; ++i)
            {
                sb.Append(i.ToString() + ',' + Data[i].ToString("0.0000", CultureInfo.CreateSpecificCulture("en-GB")) + '\n');
            }
            return sb.ToString();
        }

        public float this[int key]
        {
            get
            {
                return Get(key);
            }
            set
            {
                Set(key, value);
            }
        }

        public float Average()
        {
            return Data.Average();
        }

        public float ExpectedValue(int numbersAfterComma)
        {
            return StandardStatisticsUtils.ExpectedValue(Data, numbersAfterComma);
        }

        public float Variance(int numbersAfterComma)
        {
            return StandardStatisticsUtils.Variance(Data, numbersAfterComma);
        }

        public float Min()
        {
            return Data.Min();
        }

        public float Max()
        {
            return Data.Max();
        }
    }
}
