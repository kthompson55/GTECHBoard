using System;
using System.Collections;

namespace Collection_Game_Tool.Services
{
   internal static class ArrayShuffler<T>
    {
        private static Random rand = new Random();
        /// <summary>
        /// Shuffles an array of T
        /// </summary>
        /// <param name="original">The original array</param>
        /// <returns>The original array shuffled</returns>
        public static T[] Shuffle(T[] original)
        {
            SortedList matrix = new SortedList();
            for (int i = 0; i <= original.GetUpperBound(0); ++i)
            {
                int j = SRandom.NextInt();
                while (matrix.ContainsKey(j)) { j = SRandom.NextInt(); }
                matrix.Add(j, original[i]);
            }

            T[] outputArray = new T[original.Length];
            matrix.Values.CopyTo(outputArray, 0);

            return outputArray;
        }
    }
}
