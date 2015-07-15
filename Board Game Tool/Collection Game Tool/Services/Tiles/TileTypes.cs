namespace Collection_Game_Tool.Services.Tiles
{
	/// <summary>
	/// The tile types
	/// </summary>
    /// 
    public enum TileTypes
    {
        ///<summary>Blank space</summary>
        blank,
        /// <summary>Collect a prize level</summary>
        collection,
        /// <summary>Move forward x spaces</summary>
        moveForward,
        /// <summary>Move backward x spaces</summary>
        moveBack,
        /// <summary>Play a bonus game</summary>
        extraGame
    }
}
