using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemeticApplication.MemeticLibrary.Extensions
{
    public static class ArrayExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> value, Action<T> action)
        {
            foreach (T item in value)
            {
                action(item);
            }
        }
    }
}
