using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MemeticApplication.MemeticLibrary.Generators
{
    /// <summary>
    /// A static thread-safe random numbers generator used across the library.
    /// </summary>
    public static class RandomGeneratorThreadSafe
    {
        /// <summary>
        /// An unique seed for every thread.
        /// </summary>
        private static int seed = Environment.TickCount * Thread.CurrentThread.ManagedThreadId;

        /// <summary>
        /// A random generator assigned to each thread.
        /// </summary>
        private static readonly ThreadLocal<Random> RAND =
            new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

        /// <summary>
        /// Returns a random integer number.
        /// </summary>
        /// <returns></returns>
        public static int NextInt()
        {
            return RAND.Value.Next();
        }

        /// <summary>
        /// Returns a random integer number within a range [zero, <paramref name="range"/>).
        /// </summary>
        /// <param name="range">Upper bound of the range.</param>
        /// <returns></returns>
        public static int NextInt(int range)
        {
            return RAND.Value.Next(range);
        }

        /// <summary>
        /// Returns a random integer number within a range [<paramref name="min"/>, <paramref name="max"/>).
        /// </summary>
        /// <param name="min">Lower bound of the range.</param>
        /// <param name="max">Upper bound of the range.</param>
        /// <returns></returns>
        public static int NextInt(int min, int max)
        {
            return RAND.Value.Next(max - min) + min;
        }

        /// <summary>
        /// Returns a random floating point number.
        /// </summary>
        /// <returns></returns>
        public static double NextDouble()
        {
            return RAND.Value.NextDouble();
        }

        /// <summary>
        /// Returns a random floating point number within a range [zero, <paramref name="range"/>).
        /// </summary>
        /// <param name="range">Upper bound of the range.</param>
        /// <returns></returns>
        public static double NextDouble(double range)
        {
            return NextDouble() * range;
        }

        /// <summary>
        /// Returns two different positive integer numbers within range [0, <paramref name="range"/>).
        /// </summary>
        /// <param name="range">Upper bound of the range.</param>
        /// <param name="number1">First returned integer.</param>
        /// <param name="number2">Second returned integer.</param>
        public static void NextTwoDifferentInts(int range, out int number1, out int number2)
        {
            int rand1, rand2;
            do
            {
                rand1 = RAND.Value.Next(range);
                rand2 = RAND.Value.Next(range);
            } while (rand1 == rand2);
            number1 = rand1;
            number2 = rand2;
        }

        /// <summary>
        /// Returns two different positive integer numbers within range [<paramref name="min"/>, <paramref name="max"/>).
        /// </summary>
        /// <param name="min">Lower bound of the range.</param>
        /// <param name="max">Upper bound of the range.</param>
        /// <param name="number1">First returned integer.</param>
        /// <param name="number2">Second returned integer.</param>
        public static void NextTwoDifferentInts(int min, int max, out int number1, out int number2)
        {
            int rand1, rand2;
            do
            {
                rand1 = RAND.Value.Next(max - min) + min;
                rand2 = RAND.Value.Next(max - min) + min;
            } while (rand1 != rand2);
            number1 = rand1;
            number2 = rand2;
        }

        /// <summary>
        /// Returns two different integer numbers within range [<paramref name="min"/>, <paramref name="max"/>).
        /// Guarantees that the first number, <paramref name="number1"/>, will be bigger than the second, <paramref name="number2"/>.
        /// </summary>
        /// <param name="min">Lower bound of the range.</param>
        /// <param name="max">Upper bound of the range.</param>
        /// <param name="number1">First returned integer.</param>
        /// <param name="number2">Second returned integer.</param>
        public static void NextTwoIntsFirstBigger(int min, int max, out int number1, out int number2)
        {
            int rand1, rand2;
            do
            {
                rand1 = RAND.Value.Next(max - min) + min;
                rand2 = RAND.Value.Next(max - min) + min;
            } while (rand1 >= rand2);
            number1 = rand1;
            number2 = rand2;
        }

        /// <summary>
        /// Returns true or false at random.
        /// </summary>
        /// <returns></returns>
        public static bool NextBool()
        {
            return NextDouble() > 0.500000 ? true : false;
        }

        /// <summary>
        /// Returns a random integer [<paramref name="min"/>, <paramref name="max"/>) which is
        /// not a part of <paramref name="constraints"/>.
        /// </summary>
        /// <param name="min">Lower bound of the range.</param>
        /// <param name="max">Upper bound of the range.</param>
        /// <param name="constraints">Forbidden values.</param>
        /// <returns></returns>
        public static int NextIntWithConstraints(int min, int max, int[] constraints)
        {
            if (constraints == null)
                return NextInt(min, max);
            int result;
            do
            {
                result = NextInt(min, max);
            } while (constraints.Contains(result));
            return result;
        }
    }
}
