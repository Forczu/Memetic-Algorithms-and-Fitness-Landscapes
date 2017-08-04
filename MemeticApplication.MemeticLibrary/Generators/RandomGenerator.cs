using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemeticApplication.MemeticLibrary.Generators
{
    public class RandomGenerator
    {
        private static readonly Random RAND = new Random();

        public static int NextInt()
        {
            return RAND.Next();
        }

        public static int NextInt(int range)
        {
            return RAND.Next(range);
        }

        public static double NextDouble()
        {
            return RAND.NextDouble();
        }

        public static double NextDouble(double range)
        {
            return NextDouble() * range;
        }

        public static void NextTwoDifferentInts(int range, out int number1, out int number2)
        {
            int rand1, rand2;
            do
            {
                rand1 = RAND.Next(range);
                rand2 = RAND.Next(range);
            } while (rand1 == rand2);
            number1 = rand1;
            number2 = rand2;
        }

        public static void NextTwoIntsFirstBigger(int min, int max, out int number1, out int number2)
        {
            int rand1, rand2;
            do
            {
                rand1 = RAND.Next(max - min) + min;
                rand2 = RAND.Next(max - min) + min;
            } while (rand1 >= rand2);
            number1 = rand1;
            number2 = rand2;
        }
    }
}
