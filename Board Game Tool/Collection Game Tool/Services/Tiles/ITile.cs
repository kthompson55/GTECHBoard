﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        string tileInformation { get; set; }

        /// <summary>
        /// Signifies what kind of Tile This is
        /// </summary>
        TileTypes type { get; set; }

        /// <summary>
        /// The tile Immediately before this tile.
        /// </summary>
        ITile parent { get; set; }
        
        /// <summary>
        /// The tile immediately after this tile.
        /// </summary>
        ITile child { get; set; }
        
        /// <summary>
        /// The tiles that can be reached from this tile with the distance used as a key.
        /// </summary>
        Dictionary<int, ITile> connections { get; set; }

        /// <summary>
        /// Adds a tile to the connections list with a key based on the dice roll needed to reach that tile from this tile
        /// </summary>
        /// <param name="diceRoll">The dice roll needed to reach a tile</param>
        /// <param name="tile">The tile to connect to</param>
        void addTile(int diceRoll, ITile tile);

        /// <summary>
        /// Sets the parent of this tile to the tile before it on the board. Sets the parents child to this tile.
        /// </summary>
        /// <param name="tile"> The tile that is immediately before this tile on the board</param>
        void connectParentToChild(ITile tile);
        
        //Again change this how you need it
        ITile tileAction();
    }
}
