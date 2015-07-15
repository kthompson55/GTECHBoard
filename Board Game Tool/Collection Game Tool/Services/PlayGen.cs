using System.Collections.Generic;

namespace Collection_Game_Tool.Services
{
	/// <summary>
	/// A generated plays object
	/// </summary>
    internal class PlayGen
    {
		/// <summary>
		/// The collections
		/// </summary>
        private Dictionary<string, int> _collections;
		/// <summary>
		/// Generates a new PlayGen object
		/// </summary>
        public PlayGen()
        {
            _collections = new Dictionary<string, int>();
        }

		/// <summary>
		/// Generates a new PlayGen object
		/// </summary>
		/// <param name="pl">The pl</param>
        public PlayGen(string pl)
        {
            this._collections = new Dictionary<string, int>();
            this._collections.Add(pl, 1);
        }
		/// <summary>
		/// Generates a new PlayGen object
		/// </summary>
		/// <param name="plCollections">The pl collections</param>
        public PlayGen(Dictionary<string, int> plCollections)
        {
            _collections = plCollections;
        }

		/// <summary>
		/// Generates a new PlayGen object
		/// </summary>
		/// <param name="playGen">The play gen</param>
        public PlayGen(PlayGen playGen)
        {
            this._collections = playGen._collections;
        }
		/// <summary>
		/// Generates a new PlayGen object
		/// </summary>
		/// <param name="playGen">The playgen</param>
		/// <param name="pl">The pl</param>
        public PlayGen(PlayGen playGen, string pl)
        {
            this._collections = playGen._collections;
            if (this._collections.ContainsKey(pl))
                this._collections[pl] += 1;
            else
                this._collections.Add(pl, 1);
        }
		/// <summary>
		/// Checks if has collection
		/// </summary>
		/// <param name="key">The key</param>
		/// <returns>An int</returns>
        public int HasCollection(string key)
        {
            if (_collections.ContainsKey(key))
            {
                return _collections[key];
            }
            else
            {
                return 0;
            }
        }

        public void addPl(string pl)
        {
            if (this._collections.ContainsKey(pl))
                this._collections[pl] += 1;
            else
                this._collections.Add(pl, 1);
        }
    }
}
