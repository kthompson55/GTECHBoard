using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collection_Game_Tool.PrizeLevels
{
    /// <summary>
    /// Represents the collection of prize levels in the current generation setup
    /// </summary>
    [Serializable]
    public class PrizeLevels
    {
        /// <summary>
        /// The number of prize levels in the current setup
        /// </summary>
        static public int numPrizeLevels;
        /// <summary>
        /// All the collection spaces necessary for all of the prize levels as total sum
        /// </summary>
        static public int totalCollections;
        /// <summary>
        /// The collection of PrizeLevels
        /// </summary>
        public List<PrizeLevel> prizeLevels = new List<PrizeLevel>();

        /// <summary>
        /// Gets the prize level at the index assigned
        /// </summary>
        /// <param name="index">The index of the prize level that will be retrieved</param>
        /// <returns>The prize level at the index put in</returns>
        public PrizeLevel getPrizeLevel(int index)
        {
            if (index >= prizeLevels.Count || index < 0)
                return null;
            return prizeLevels.ElementAt(index);
        }

        /// <summary>
        /// Adds a new prize level
        /// </summary>
        /// <param name="obj">The PrizeLevel to be added</param>
        public void addPrizeLevel(PrizeLevel obj)
        {
            if(obj!=null)
                prizeLevels.Add(obj);

            numPrizeLevels = prizeLevels.Count;
            calculateTotalCollections();
        }

        /// <summary>
        /// Removes PrizeLevel at the index provided
        /// </summary>
        /// <param name="index">Index of the PrizeLevel to be removed</param>
        public void removePrizeLevel(int index)
        {
            if (!(index >= prizeLevels.Count || index < 0))
                prizeLevels.RemoveAt(index);

            numPrizeLevels = prizeLevels.Count;
            calculateTotalCollections();
        }

        /// <summary>
        /// Adds a new PrizeLevel at the index provided
        /// </summary>
        /// <param name="obj">The PrizeLevel being added</param>
        /// <param name="index">Where the new PrizeLevel will be placed</param>
        public void addPrizeLevelAt(PrizeLevel obj, int index)
        {
            if (!(index >= prizeLevels.Count || index < 0) && obj!=null)
                prizeLevels.Insert(index, obj);

            numPrizeLevels = prizeLevels.Count;
        }

        /// <summary>
        /// Get the number of PrizeLevels in the current setup
        /// </summary>
        /// <returns>The total number of prize levels in the current setup</returns>
        public int getNumPrizeLevels()
        {
            return prizeLevels.Count;
        }

        /// <summary>
        /// Organize the PrizeLevels based on their value
        /// </summary>
        public void sortPrizeLevels()
        {
            prizeLevels.Sort();
        }

        /// <summary>
        /// Get the index of the PrizeLevel being passed in
        /// </summary>
        /// <param name="obj">The PrizeLevel that you want to know the index of</param>
        /// <returns>The index of the desired PrizeLevel</returns>
        public int getLevelOfPrize(PrizeLevel obj)
        {
            for (int i = 0; i < prizeLevels.Count; i++)
            {
                if (prizeLevels[i].Equals(obj))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Determine the total number of collections between all of the current PrizeLevels
        /// </summary>
        public void calculateTotalCollections()
        {
            int count = 0;
            foreach (PrizeLevel pl in prizeLevels)
            {
                count += pl.numCollections;
            }
            totalCollections = count;
        }
    }
}
