using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collection_Game_Tool.Services
{
    class PlayGen
    {
        Dictionary<String, int> collections;
        public PlayGen()
        {
            collections = new Dictionary<string, int>();
        }

        public PlayGen(String pl)
        {
            this.collections = new Dictionary<string, int>();
            this.collections.Add(pl, 1);
        }

        public PlayGen(Dictionary<String, int> plCollections)
        {
            collections = plCollections;
        }

        public PlayGen(PlayGen pg)
        {
            this.collections = pg.collections;
        }

        public PlayGen(PlayGen pg, String pl)
        {
            this.collections = pg.collections;
            if (this.collections.ContainsKey(pl))
                this.collections[pl] += 1;
            else
                this.collections.Add(pl, 1);
        }

        public int hasCollection(String key)
        {
            return collections[key];
        }
    }
}
