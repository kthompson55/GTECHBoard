using System;

namespace Collection_Game_Tool.Services
{
	/// <summary>
	/// A static random instance
	/// </summary>
    internal static class SRandom
    {
		/// <summary>
		/// The random instance
		/// </summary>
        private static Random _rand = new Random();
		/// <summary>
		/// Generate a random int.
		/// </summary>
		/// <param name="min">Minimum value</param>
		/// <param name="max">Maxumum value</param>
		/// <returns>A random int</returns>
        public static int NextInt(int min, int max){
            return _rand.Next(min, max);
        }
		/// <summary>
		/// Generate a random int
		/// </summary>
		/// <returns>A random int</returns>
		public static int NextInt()
		{
			return _rand.Next();
		}
    }
}
