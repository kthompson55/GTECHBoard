using Collection_Game_Tool.Services.Tiles;
using System.ComponentModel;
using System.Text;

namespace Collection_Game_Tool.Services
{
    /// <summary>
    /// Board Generation creates the board data to be used in game generation. 
    /// </summary>
	/// <remarks>NOT COMPLETE</remarks>
    public class BoardGeneration
    {
        /// <summary>
        /// First tile is the starting tile of the board
        /// </summary>
        private ITile _firstTile;

        /// <summary>
        /// last tile is the last tile of the board.
        /// </summary>
        private ITile _lastTile;
		/// <summary>
		/// The thread args for cancelling.
		/// </summary>
		private DoWorkEventArgs _threadCancel;
		/// <summary>
		/// Instantiates a board generation
		/// </summary>
		/// <param name="threadCancel">Optional: arguments for cancelling the thread</param>
		public BoardGeneration(DoWorkEventArgs threadCancel = null)
		{
			this._threadCancel = threadCancel;
		}
        /// <summary>
        /// Board generation creates the board for play. The board uses a doubly linked list for its design.
        /// </summary>
        /// <param name="boardSize">Board size is the total size of the board</param>
        /// <param name="initialReachable">The number of spaces that are reachable without accounting for special tiles</param>
        /// <param name="minMove">The min movement a player can make in a turn</param>
        /// <param name="maxMove">The max movement a player can make in a turn</param>
        /// <param name="moveBackCount">The number of move back spaces</param>
        /// <param name="moveForwardCount">The number of move forward spaces</param>
        /// <param name="prizes">All the game prizes</param>
        /// <param name="moveForward">How far move forward tiles will move you</param>
        /// <param name="moveBack">How far move back tiles will move you</param>
        /// <returns>Returns the first tile of the board.</returns>
        public ITile GenerateBoard(int boardSize,
            int initialReachable,
            int minMove,
            int maxMove,
            int moveBackCount,
            int moveForwardCount,
            PrizeLevels.PrizeLevels prizes,
            int moveForward = 1,
            int moveBack = 1)
        {
            int numberOfCollectionSpots = 0;
            foreach (PrizeLevels.PrizeLevel p in prizes.prizeLevels)
            {
				if ( _threadCancel != null && _threadCancel.Cancel ) return null;
                numberOfCollectionSpots += p.numCollections;
            }

            FillInBlankBoardTiles(boardSize);
			if ( _threadCancel != null && _threadCancel.Cancel ) return null;
            TileTypes[] specialTiles = new TileTypes[moveBackCount + moveForwardCount];
            TileTypes[] collectionTiles = new TileTypes[numberOfCollectionSpots];

			for ( int i = 0; i < collectionTiles.Length && !( _threadCancel != null && _threadCancel.Cancel ); ++i )
            {
                collectionTiles[i] = TileTypes.collection;
            }
			if ( _threadCancel != null && _threadCancel.Cancel ) return null;
            int index = 0;
			while ( index < specialTiles.Length && !( _threadCancel != null && _threadCancel.Cancel ) )
            {
                if (index < moveForwardCount)
                {
                    specialTiles[index] = TileTypes.moveForward;
                }
                else
                {
                    specialTiles[index] = TileTypes.moveBack;
                }
                ++index;
            }
			if ( _threadCancel != null && _threadCancel.Cancel ) return null;

            specialTiles = ArrayShuffler<TileTypes>.Shuffle(specialTiles);
			if ( _threadCancel != null && _threadCancel.Cancel ) return null;
            FillInSpecialTiles(initialReachable, minMove, maxMove, moveForward, moveBack, specialTiles);
			if ( _threadCancel != null && _threadCancel.Cancel ) return null;
            FillInSpecialTiles(initialReachable, minMove, maxMove, moveForward, moveBack, collectionTiles);
			if ( _threadCancel != null && _threadCancel.Cancel ) return null;
			FillInCollectionTileValues(minMove, numberOfCollectionSpots, prizes);
			if ( _threadCancel != null && _threadCancel.Cancel ) return null;
			ConnectTiles(boardSize, minMove, maxMove, moveBack, moveForward);


            return _firstTile;
        }

        /// <summary>
        /// Populates the board with special tiles
        /// </summary>
        /// <param name="initialReachable">The inital reachable</param>
        /// <param name="minMove">The min move</param>
        /// <param name="maxMove">The max move</param>
        /// <param name="moveForward">The move forward</param>
        /// <param name="moveBack">The move back</param>
        /// <param name="tiles">The tiles</param>
        private void FillInSpecialTiles(int initialReachable,
                int minMove,
                int maxMove,
                int moveForward,
                int moveBack,
                TileTypes[] tiles)
        {
            ITile currentTile = _firstTile;
			for ( int i = 0; i < tiles.Length && !( _threadCancel != null && _threadCancel.Cancel ); ++i )
            {
                bool tilePlaced = false;
                int currentSpace = 0;
                Tiles.TileTypes myTile = tiles[i];

                int moveAmount = SRandom.NextInt(minMove, (initialReachable / maxMove)) * (i + 1);
				for ( int j = 0; j < moveAmount && !( _threadCancel != null && _threadCancel.Cancel ); ++j )
                {
                    if (currentTile.Child != null && (currentSpace + 1) < initialReachable)
                    {
                        currentTile = currentTile.Child;
                        ++currentSpace;
                    }
                    else
                    {
                        currentTile = _firstTile;
                        currentSpace = 0;
                    }
                }
				if ( _threadCancel != null && _threadCancel.Cancel ) return;

				while ( !tilePlaced && !( _threadCancel != null && _threadCancel.Cancel ) )
                {
                    if (currentTile.Type == TileTypes.blank && (currentSpace + 1) < initialReachable)
                    {
                        if (myTile.ToString().Equals("moveForward"))
                        {
                            ITile tempTile = currentTile;
							for ( int k = 0; k < moveForward && !( _threadCancel != null && _threadCancel.Cancel ); k++ )
                            {
                                if (tempTile.Child != null)
                                {
                                    tempTile = tempTile.Child;
                                }
                            }
							if ( _threadCancel != null && _threadCancel.Cancel ) return;

                            if (tempTile.Type != TileTypes.moveBack)
                            {
                                currentTile.Type = myTile;
                                tilePlaced = true;
                            }
                            else
                            {
                                if (currentTile.Child != null)
                                {
                                    currentTile = currentTile.Child;
                                    ++currentSpace;
                                }
                                else
                                {
                                    currentTile = _firstTile;
                                    currentSpace = 0;
                                }
                            }
                        }
                        else if (myTile.ToString().Equals("moveBack"))
                        {
                            ITile tempTile = currentTile;
							for ( int k = 0; k < moveBack && !( _threadCancel != null && _threadCancel.Cancel ); ++k )
                            {
                                if (tempTile.Parent != null)
                                {
                                    tempTile = tempTile.Parent;
                                }
                            }
							if ( _threadCancel != null && _threadCancel.Cancel ) return;
                            if (tempTile.Type != TileTypes.moveForward)
                            {
                                currentTile.Type = myTile;
                                tilePlaced = true;
                            }
                            else
                            {
                                if (currentTile.Child != null)
                                {
                                    currentTile = currentTile.Child;
                                    ++currentSpace;
                                }
                                else
                                {
                                    currentTile = _firstTile;
                                    currentSpace = 0;
                                }
                            }
                        }
                        else
                        {
                            currentTile.Type = myTile;
                            string leType = "" + currentTile.Type;
                            tilePlaced = true;
                        }
                    }
                    else if (currentTile.Child != null && (currentSpace + 1) < initialReachable)
                    {
                        currentTile = currentTile.Child;
                        ++currentSpace;
                    }
                    else
                    {
                        currentTile = _firstTile;
                        currentSpace = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Fills in the collection spaces prize level values
        /// </summary>
        /// <param name="minMove">The min move</param>
        /// <param name="numberOfCollectionSpots">The number of collection spots</param>
        /// <param name="prizes">The prizes</param>
        private void FillInCollectionTileValues(
            int minMove,
            int numberOfCollectionSpots,
            PrizeLevels.PrizeLevels prizes)
        {
            int index = 0;
            string[] prizeLevel = new string[numberOfCollectionSpots];
            foreach (PrizeLevels.PrizeLevel p in prizes.prizeLevels)
            {
				if ( _threadCancel != null && _threadCancel.Cancel ) return;
				for ( int i = 0; i < p.numCollections && !( _threadCancel != null && _threadCancel.Cancel ); ++i )
                {
                    StringBuilder sb = new StringBuilder();
                    //sb.Append("CS");
                    if (p.isInstantWin)
                        sb.Append(":I");
                    if (p.isBonusGame)
                        sb.Append(":B");
                    sb.Append((":" + (char)(p.prizeLevel + 97)));
                    prizeLevel[index] = sb.ToString();
                    ++index;
                }
            }
			if ( _threadCancel != null && _threadCancel.Cancel ) return;

            int collectionValuesFilled = 0;
            Tiles.ITile current = _firstTile;
            prizeLevel = ArrayShuffler<string>.Shuffle(prizeLevel);
			if ( _threadCancel != null && _threadCancel.Cancel ) return;

			while ( collectionValuesFilled != numberOfCollectionSpots && !( _threadCancel != null && _threadCancel.Cancel ) )
            {
                if (current == null)
                {
                    prizeLevel = ArrayShuffler<string>.Shuffle(prizeLevel);
					if ( _threadCancel != null && _threadCancel.Cancel ) return;
                    current = _firstTile;
                    collectionValuesFilled = 0;
                }

                if (current.Type == Tiles.TileTypes.collection)
                {
                    Tiles.ITile backtracker = current;
					for ( int j = 1; j < minMove && !( _threadCancel != null && _threadCancel.Cancel ); ++j )
                    {
                        if (backtracker.Parent != null)
                            backtracker = backtracker.Parent;
                    }
					if ( _threadCancel != null && _threadCancel.Cancel ) return;
                    if (backtracker == current || backtracker.Type != Tiles.TileTypes.collection || (backtracker.Type == Tiles.TileTypes.collection && backtracker.TileInformation != prizeLevel[collectionValuesFilled]))
                    {
                        current.TileInformation = prizeLevel[collectionValuesFilled++];
                    }
                }

                current = current.Child;
            }
        }


        /// <summary>
        /// Creates the blank board.
        /// </summary>
        /// <param name="boardSize"> The size of the board.</param>
        private void FillInBlankBoardTiles(int boardSize)
        {
            _firstTile = new Tile();
            _firstTile.Type = TileTypes.blank;
            ITile tempTile = _firstTile;
			for ( int i = 1; i < boardSize && !( _threadCancel != null && _threadCancel.Cancel ); ++i )
            {
                ITile newTile = new Tile();
                newTile.Type = TileTypes.blank;
                newTile.ConnectParentToChild(tempTile);
                tempTile = newTile;
            }
            _lastTile = tempTile;
        }

        /// <summary>
        /// Fills the tiles with a list of links to other tiles that can be reached from dice rolls. 
        /// </summary>
        /// <param name="boardSize">The board size</param>
        /// <param name="minMove">The min move</param>
        /// <param name="maxMove">The max move</param>
        /// <param name="BackMove">The back move</param>
        /// <param name="ForwardMove">The forward move</param>
        private void ConnectTiles(int boardSize,
            int minMove,
            int maxMove,
            int BackMove,
            int ForwardMove)
        {
            ITile currentTile = _firstTile;
			while ( currentTile.Child != null && !( _threadCancel != null && _threadCancel.Cancel ) )
            {
                if (currentTile.Type == TileTypes.moveBack)
                {
                    ITile targetGameFromTile = currentTile;
					for ( int j = 0; j < BackMove && targetGameFromTile != null && !( _threadCancel != null && _threadCancel.Cancel ); ++j )
                    {
                        targetGameFromTile = targetGameFromTile.Child;
                    }
					if ( _threadCancel != null && _threadCancel.Cancel ) return;

                    if (targetGameFromTile != null)
                    {
                        currentTile.AddTile(BackMove, targetGameFromTile);
                        currentTile.TileInformation = BackMove.ToString();
                    }
                }
                else if (currentTile.Type == TileTypes.moveForward)
                {
                    ITile targetGameFromTile = currentTile;
					for ( int j = 0; j < ForwardMove && targetGameFromTile != null && !( _threadCancel != null && _threadCancel.Cancel ); ++j )
                    {
                        targetGameFromTile = targetGameFromTile.Child;
                    }
					if ( _threadCancel != null && _threadCancel.Cancel ) return;

                    if (targetGameFromTile != null)
                    {
                        currentTile.AddTile(ForwardMove, targetGameFromTile);
                        currentTile.TileInformation = ForwardMove.ToString();
                    }
                }
                else
                {
					for ( int i = minMove; i < maxMove + 1 && !( _threadCancel != null && _threadCancel.Cancel ); ++i )
                    {
                        ITile targetGameFromTile = currentTile;
                        for (int j = 0; j < i && targetGameFromTile != null; ++j)
                        {
                            targetGameFromTile = targetGameFromTile.Child;
                        }
						if ( _threadCancel != null && _threadCancel.Cancel ) return;

                        if (targetGameFromTile != null)
                        {
                            currentTile.AddTile(i, targetGameFromTile);
                        }
                    }
                }
                currentTile = currentTile.Child;
            }
        }
		/// <summary>
		/// The string representation of the board.
		/// </summary>
		/// <returns>The string representation of the board.</returns>
        public override string ToString()
        {
            ITile currentTile = _firstTile;
            StringBuilder sb = new StringBuilder();
            while (currentTile != null)
            {
                if (currentTile.Type != TileTypes.blank)
                {
                    if (currentTile.TileInformation != null)
                        sb.Append(currentTile.TileInformation + ", ");
                    else
                        sb.Append(currentTile.Type + ", ");
                }
                else
                    sb.Append("S, ");
                currentTile = currentTile.Child;
            }
            return sb.ToString();
        }
    }
}
