using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collection_Game_Tool.Services.Tiles
{
    class Tile : ITile
    {
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
        private Dictionary<int, ITile> connections
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

        public void addTile(int diceRoll, ITile tile)
        {
            connections.Add(diceRoll, tile);
        }

        public void tileAction()
        {
            throw new NotImplementedException();
        }
    }
}
