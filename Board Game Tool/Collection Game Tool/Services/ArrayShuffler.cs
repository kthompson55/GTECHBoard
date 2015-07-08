using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collection_Game_Tool.Services
{
    static class ArrayShuffler<T>
    {
        private static Random rand = new Random();
        /// <summary>
        /// Shuffles an array of T
        /// </summary>
        /// <param name="original">The original array</param>
        /// <returns>The original array shuffled</returns>
        public static T[] shuffle(T[] original)
        {
            SortedList matrix = new SortedList();
            for (int i = 0; i <= original.GetUpperBound(0); i++)
            {
                int j = rand.Next();
                while (matrix.ContainsKey(j)) { j = rand.Next(); }
                matrix.Add(j, original[i]);
            }

            T[] outputArray = new T[original.Length];
            matrix.Values.CopyTo(outputArray, 0);

            return outputArray;
        }
    }
}
