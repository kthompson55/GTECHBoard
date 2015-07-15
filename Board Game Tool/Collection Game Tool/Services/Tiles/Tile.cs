using System.Collections.Generic;

namespace Collection_Game_Tool.Services.Tiles
{
    /// <summary>
    /// Tile is the basic object in a board. A generic tile is blank with no special Properties. 
    /// </summary>
    internal class Tile : ITile
    {
		/// <summary>
		/// Contains extra information for the tile such as collection type or move amount.
		/// </summary>
		public string TileInformation
		{
			get;
			set;
		}

		/// <summary>
		/// Signifies what kind of Tile This is
		/// </summary>
		public TileTypes Type
		{
			get;
			set;
		}
		/// <summary>
		/// The tile Immediately before this tile.
		/// </summary>
		public ITile Parent
		{
			get;
			set;
		}
		/// <summary>
		/// The tile immediately after this tile.
		/// </summary>
		public ITile Child
		{
			get;
			set;
		}

		/// <summary>
		/// The tiles that can be reached from this tile with the distance used as a key.
		/// </summary>
		public Dictionary<int, ITile> Connections
		{
			get;
			set;
		}

		/// <summary>
		/// Constructs a new tile
		/// </summary>
        public Tile()
        {
            Connections = new Dictionary<int, ITile>();
        }

		/// <summary>
		/// Adds a tile to the connections list with a key based on the dice roll needed to reach that tile from this tile
		/// </summary>
		/// <param name="diceRoll">The dice roll needed to reach a tile</param>
		/// <param name="tile">The tile to connect to</param>
        public void AddTile(int diceRoll, ITile tile)
        {
            Connections.Add(diceRoll, tile);
        }
		/// <summary>
		/// The tile action
		/// </summary>
		/// <remarks>Again change this how you need it</remarks>
		/// <returns>The tile action</returns>
        public ITile TileAction()
        {
            if (Type == TileTypes.moveForward)
            {
                int moveAmount = int.Parse(TileInformation);
                ITile ret = this;
                for (int i = 0; i < moveAmount; i++)
                {
                    //Moves down the board (forward) moveAmount times
                    ret = ret.Child;
                }
                return ret;
            }
            else if (Type == TileTypes.moveBack)
            {
                int moveAmount = int.Parse(TileInformation);

                //Moves up the board (backward) moveAmount times
                ITile ret = this;
                for (int i = 0; i < moveAmount; i++)
                {
                    if (ret.Parent != null)
                    {
                        ret = ret.Parent;
                    }
                }
                return ret.TileAction();
            }
            else
            {
                return this;
            }
        }

		/// <summary>
		/// Sets the parent of this tile to the tile before it on the board. Sets the parents child to this tile.
		/// </summary>
		/// <param name="tile"> The tile that is immediately before this tile on the board</param>
        public void ConnectParentToChild(ITile tile)
        {
            this.Parent = tile;
            tile.Child = this;
        }
    }
}
