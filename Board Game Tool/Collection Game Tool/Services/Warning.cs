namespace Collection_Game_Tool.Services
{
	/// <summary>
	/// A warning class
	/// </summary>
    internal class Warning
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
        public Warning(string senderId, string errorCode)
        {
            this.SenderId = senderId;
            this.ErrorCode = errorCode;
        }
        
		/// <summary>
		/// Determines whether the specified object is equal to the current Warning.
		/// </summary>
		/// <param name="obj">The object to compare with the current warning.</param>
		/// <returns>True if the specified object is equal to the current warning; otherwise, false.</returns>
		public override bool Equals( object obj )
		{
			Warning war = obj as Warning;

			return obj != null && (this == war || (ErrorCode == war.ErrorCode && SenderId == war.SenderId));
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
