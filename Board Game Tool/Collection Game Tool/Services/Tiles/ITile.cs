using System.Collections.Generic;

namespace Collection_Game_Tool.Services.Tiles
{
    /// <summary>
    /// ITile is the interface needed for all Tile objects to be used inside of board generation
    /// </summary>
    public interface ITile
    {
        /// <summary>
        /// Contains extra information for the tile such as collection type or move amount.
        /// </summary>
        string TileInformation { get; set; }

        /// <summary>
        /// Signifies what kind of Tile This is
        /// </summary>
        TileTypes Type { get; set; }

        /// <summary>
        /// The tile Immediately before this tile.
        /// </summary>
        ITile Parent { get; set; }
        
        /// <summary>
        /// The tile immediately after this tile.
        /// </summary>
        ITile Child { get; set; }
        
        /// <summary>
        /// The tiles that can be reached from this tile with the distance used as a key.
        /// </summary>
        Dictionary<int, ITile> Connections { get; set; }

        /// <summary>
        /// Adds a tile to the connections list with a key based on the dice roll needed to reach that tile from this tile
        /// </summary>
        /// <param name="diceRoll">The dice roll needed to reach a tile</param>
        /// <param name="tile">The tile to connect to</param>
        void AddTile(int diceRoll, ITile tile);

        /// <summary>
        /// Sets the parent of this tile to the tile before it on the board. Sets the parents child to this tile.
        /// </summary>
        /// <param name="tile"> The tile that is immediately before this tile on the board</param>
        void ConnectParentToChild(ITile tile);
        
		/// <summary>
		/// The tile action
		/// </summary>
		/// <remarks>Again change this how you need it</remarks>
		/// <returns>The tile action</returns>
        ITile TileAction();
    }
}
