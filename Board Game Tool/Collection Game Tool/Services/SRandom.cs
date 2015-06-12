using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collection_Game_Tool.Services
{
    static class SRandom
    {
        private static Random rand = new Random();
        public static int nextInt(int min, int max){
            return rand.Next(min, max);
        }
    }
}
