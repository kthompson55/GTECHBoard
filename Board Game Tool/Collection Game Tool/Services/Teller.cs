using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collection_Game_Tool.Services
{
    interface Teller
    {
        void shout(Object pass);

        void addListener(Listener list);
    }
}
