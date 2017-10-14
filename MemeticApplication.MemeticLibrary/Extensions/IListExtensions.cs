using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemeticApplication.MemeticLibrary.Generators;

namespace MemeticApplication.MemeticLibrary.Extensions
{
    public static class IListExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count, k;
            while (n > 1)
            {
                --n;
                k = RandomGeneratorThreadSafe.NextInt(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
