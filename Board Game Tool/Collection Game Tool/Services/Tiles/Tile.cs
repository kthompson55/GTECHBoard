using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collection_Game_Tool.Services.Tiles
{
    class Tile : ITile
    {
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

        public void tileAction()
        {
            throw new NotImplementedException();
        }

        public void connectParentToChild(ITile tile)
        {
            this.parent = tile;
            tile.child = this;
        }
    }
}
