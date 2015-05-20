using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collection_Game_Tool.Services
{
    class Warning
    {
         public string senderId;
        public string errorCode;

        public Warning(string senderId, string errorCode)
        {
            this.senderId = senderId;
            this.errorCode = errorCode;
        }
        
        public override bool Equals(Object obj) {
            Warning war = obj as Warning;
            if(this == obj)
                return true;
            if(obj == null)
                return false;
            if (errorCode != war.errorCode)
            {
                return false;
            }
            if (senderId != war.senderId)
            {
                return false;
            }
           
            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((senderId != null ? senderId.GetHashCode() : 0) * 397) ^ (errorCode != null ? errorCode.GetHashCode() : 0);
            }
        }

    }
}
