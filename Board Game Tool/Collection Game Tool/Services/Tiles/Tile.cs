using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collection_Game_Tool.Services
{
    interface Tile
    {
        List<Tile> nodes;

        //Talan you should probably flesh this out how you want it
        //I doubt you will want to override this each time
        public void addTile(Tile tile);

        //Again change this how you need it
        public void tileAction();
    }
}
