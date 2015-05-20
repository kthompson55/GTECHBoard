using Collection_Game_Tool.Divisions;
using Collection_Game_Tool.GameSetup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collection_Game_Tool.Main
{
    [Serializable]
    class ProjectData
    {
        public PrizeLevels.PrizeLevels savedPrizeLevels;
        public GameSetupModel savedGameSetup;
        public DivisionsModel savedDivisions;
    }
}
