using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collection_Game_Tool.Services.Tiles
{
    /// <summary>
    /// Tile is the basic object in a board. A generic tile is blank with no special Properties. 
    /// </summary>
    class Tile : ITile
    {
        private string _tileInformation;
        public string tileInformation
        {
            get
            {
                return _tileInformation;
            }
            set
            {
                _tileInformation = value;
            }
        }


        private TileTypes _type;
        public TileTypes type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        private ITile _parent;
        public ITile parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }

        private ITile _child;
        public ITile child
        {
            get
            {
                return _child;
            }
            set
            {
                _child = value;
            }
        }

        private Dictionary<int, ITile> _connections;
        public Dictionary<int, ITile> connections
        {
            get
            {
                return _connections;
            }
            set
            {
                _connections = value;
            }
        }

        public Tile()
        {
            connections = new Dictionary<int, ITile>();
        }

        public void addTile(int diceRoll, ITile tile)
        {
            connections.Add(diceRoll, tile);
        }

        public ITile tileAction()
        {
            if (type == TileTypes.moveForward)
            {
                int moveAmount = int.Parse(tileInformation);
                ITile ret = this;
                for (int i = 0; i < moveAmount; i++)
                {
                    //Moves down the board (forward) moveAmount times
                    ret = ret.child;
                }
                return ret;
            }
            else if (type == TileTypes.moveBack)
            {
                int moveAmount = int.Parse(tileInformation);

                //Moves up the board (backward) moveAmount times
                ITile ret = this;
                for (int i = 0; i < moveAmount; i++)
                {
                    ret = ret.parent;
                }
                return ret;
            }
            else
            {
                return this;
            }
        }

        public void connectParentToChild(ITile tile)
        {
            this.parent = tile;
            tile.child = this;
        }
    }
}
