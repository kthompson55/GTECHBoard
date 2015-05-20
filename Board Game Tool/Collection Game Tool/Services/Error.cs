using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collection_Game_Tool.Services
{
    class Error
    {
        public string senderId;
        public string errorCode;

        public Error(string senderId, string errorCode)
        {
            this.senderId = senderId;
            this.errorCode = errorCode;
        }
        
        public override bool Equals(Object obj) {
            Error er = obj as Error;
            if(this == obj)
                return true;
            if(obj == null)
                return false;
            if (errorCode != er.errorCode)
            {
                return false;
            }
            if (senderId != er.senderId)
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
