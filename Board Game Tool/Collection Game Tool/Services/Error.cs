using System;

namespace Collection_Game_Tool.Services
{
	/// <summary>
	/// An error class
	/// </summary>
    internal class Error
    {
		/// <summary>
		/// The sender ID
		/// </summary>
		private string SenderId { get; set; }
		/// <summary>
		/// The error code
		/// </summary>
		private string ErrorCode { get; set; }

		/// <summary>
		/// Construct a new warning using the senderId and error code
		/// </summary>
		/// <param name="senderId">The sender ID</param>
		/// <param name="errorCode">The error code</param>
        public Error(string senderId, string errorCode)
        {
            this.SenderId = senderId;
            this.ErrorCode = errorCode;
        }

		/// <summary>
		/// Determines whether the specified object is equal to the current Warning.
		/// </summary>
		/// <param name="obj">The object to compare with the current warning.</param>
		/// <returns>True if the specified object is equal to the current warning; otherwise, false.</returns>
        public override bool Equals(Object obj) {
            Error er = obj as Error;
            if(this == obj)
                return true;
            if(obj == null)
                return false;
            if (ErrorCode != er.ErrorCode)
            {
                return false;
            }
            if (SenderId != er.SenderId)
            {
                return false;
            }
           
            return true;
        }

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>A hash code for the current Warning.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((SenderId != null ? SenderId.GetHashCode() : 0) * 397) ^ (ErrorCode != null ? ErrorCode.GetHashCode() : 0);
            }
        }
    }
}
