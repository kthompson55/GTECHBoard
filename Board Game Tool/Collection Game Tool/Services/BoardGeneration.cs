using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

///NOT COMPLETE
namespace Collection_Game_Tool.Services
{
    /// <summary>
    /// Board Generation creates the board data to be used in game generation. 
    /// </summary>
    public class BoardGeneration
    {
        /// <summary>
        /// First tile is the starting tile of the board
        /// </summary>
        private Tiles.ITile firstTile;

        /// <summary>
        /// last tile is the last tile of the board.
        /// </summary>
        private Tiles.ITile lastTile;
		private DoWorkEventArgs threadCancel;
		public BoardGeneration(DoWorkEventArgs threadCancel = null)
		{
			this.threadCancel = threadCancel;
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
        public Tiles.ITile genBoard(int boardSize,
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
				if ( threadCancel != null && threadCancel.Cancel ) return null;
                numberOfCollectionSpots += p.numCollections;
            }

            fillInBlankBoardTiles(boardSize);
			if ( threadCancel != null && threadCancel.Cancel ) return null;
            Tiles.TileTypes[] specialTiles = new Tiles.TileTypes[moveBackCount + moveForwardCount];
            Tiles.TileTypes[] collectionTiles = new Tiles.TileTypes[numberOfCollectionSpots];

			for ( int i = 0; i < collectionTiles.Length && !( threadCancel != null && threadCancel.Cancel ); ++i )
            {
                collectionTiles[i] = Tiles.TileTypes.collection;
            }
			if ( threadCancel != null && threadCancel.Cancel ) return null;
            int index = 0;
			while ( index < specialTiles.Length && !( threadCancel != null && threadCancel.Cancel ) )
            {
                if (index < moveForwardCount)
                {
                    specialTiles[index] = Tiles.TileTypes.moveForward;
                }
                else
                {
                    specialTiles[index] = Tiles.TileTypes.moveBack;
                }
                ++index;
            }
			if ( threadCancel != null && threadCancel.Cancel ) return null;

            specialTiles = ArrayShuffler<Tiles.TileTypes>.shuffle(specialTiles);
			if ( threadCancel != null && threadCancel.Cancel ) return null;
            fillInSpecialTiles(initialReachable, minMove, maxMove, moveForward, moveBack, specialTiles);
			if ( threadCancel != null && threadCancel.Cancel ) return null;
            fillInSpecialTiles(initialReachable, minMove, maxMove, moveForward, moveBack, collectionTiles);
			if ( threadCancel != null && threadCancel.Cancel ) return null;
			fillInCollectionTileValues(minMove, numberOfCollectionSpots, prizes);
			if ( threadCancel != null && threadCancel.Cancel ) return null;
			connectTiles(boardSize, minMove, maxMove, moveBack, moveForward);


            return firstTile;
        }

        /// <summary>
        /// Populates the board with special tiles
        /// </summary>
        /// <param name="initialReachable"></param>
        /// <param name="minMove"></param>
        /// <param name="maxMove"></param>
        /// <param name="moveForward"></param>
        /// <param name="moveBack"></param>
        /// <param name="tiles"></param>
        private void fillInSpecialTiles(int initialReachable,
                int minMove,
                int maxMove,
                int moveForward,
                int moveBack,
                Tiles.TileTypes[] tiles)
        {
            Tiles.ITile currentTile = firstTile;
			for ( int i = 0; i < tiles.Length && !( threadCancel != null && threadCancel.Cancel ); ++i )
            {
                bool tilePlaced = false;
                int currentSpace = 0;
                Tiles.TileTypes myTile = tiles[i];

                int moveAmount = SRandom.nextInt(minMove, (initialReachable / maxMove)) * (i + 1);
				for ( int j = 0; j < moveAmount && !( threadCancel != null && threadCancel.Cancel ); j++ )
                {
                    if (currentTile.child != null && (currentSpace + 1) < initialReachable)
                    {
                        currentTile = currentTile.child;
                        currentSpace++;
                    }
                    else
                    {
                        currentTile = firstTile;
                        currentSpace = 0;
                    }
                }
				if ( threadCancel != null && threadCancel.Cancel ) return;

				while ( !tilePlaced && !( threadCancel != null && threadCancel.Cancel ) )
                {
                    if (currentTile.type == Tiles.TileTypes.blank && (currentSpace + 1) < initialReachable)
                    {
                        if (myTile.ToString().Equals("moveForward"))
                        {
                            Tiles.ITile tempTile = currentTile;
							for ( int k = 0; k < moveForward && !( threadCancel != null && threadCancel.Cancel ); k++ )
                            {
                                if (tempTile.child != null)
                                {
                                    tempTile = tempTile.child;
                                }
                            }
							if ( threadCancel != null && threadCancel.Cancel ) return;

                            if (tempTile.type != Tiles.TileTypes.moveBack)
                            {
                                currentTile.type = myTile;
                                tilePlaced = true;
                            }
                            else
                            {
                                if (currentTile.child != null)
                                {
                                    currentTile = currentTile.child;
                                    currentSpace++;
                                }
                                else
                                {
                                    currentTile = firstTile;
                                    currentSpace = 0;
                                }
                            }
                        }
                        else if (myTile.ToString().Equals("moveBack"))
                        {
                            Tiles.ITile tempTile = currentTile;
							for ( int k = 0; k < moveBack && !( threadCancel != null && threadCancel.Cancel ); k++ )
                            {
                                if (tempTile.parent != null)
                                {
                                    tempTile = tempTile.parent;
                                }
                            }
							if ( threadCancel != null && threadCancel.Cancel ) return;
                            if (tempTile.type != Tiles.TileTypes.moveForward)
                            {
                                currentTile.type = myTile;
                                tilePlaced = true;
                            }
                            else
                            {
                                if (currentTile.child != null)
                                {
                                    currentTile = currentTile.child;
                                    currentSpace++;
                                }
                                else
                                {
                                    currentTile = firstTile;
                                    currentSpace = 0;
                                }
                            }
                        }
                        else
                        {
                            currentTile.type = myTile;
                            string leType = "" + currentTile.type;
                            tilePlaced = true;
                        }
                    }
                    else if (currentTile.child != null && (currentSpace + 1) < initialReachable)
                    {
                        currentTile = currentTile.child;
                        currentSpace++;
                    }
                    else
                    {
                        currentTile = firstTile;
                        currentSpace = 0;
                    }
                }
            }
        }

        private void calulateNextSpaceToTest()
        {

        }

        /// <summary>
        /// Fills in the collection spaces prize level values
        /// </summary>
        /// <param name="minMove"></param>
        /// <param name="numberOfCollectionSpots"></param>
        /// <param name="prizes"></param>
        private void fillInCollectionTileValues(
            int minMove,
            int numberOfCollectionSpots,
            PrizeLevels.PrizeLevels prizes)
        {
            int index = 0;
            string[] prizeLevel = new string[numberOfCollectionSpots];
            foreach (PrizeLevels.PrizeLevel p in prizes.prizeLevels)
            {
				if ( threadCancel != null && threadCancel.Cancel ) return;
				for ( int i = 0; i < p.numCollections && !( threadCancel != null && threadCancel.Cancel ); ++i )
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
			if ( threadCancel != null && threadCancel.Cancel ) return;

            int collectionValuesFilled = 0;
            Tiles.ITile current = firstTile;
            prizeLevel = ArrayShuffler<string>.shuffle(prizeLevel);
			if ( threadCancel != null && threadCancel.Cancel ) return;

			while ( collectionValuesFilled != numberOfCollectionSpots && !( threadCancel != null && threadCancel.Cancel ) )
            {
                if (current == null)
                {
                    prizeLevel = ArrayShuffler<string>.shuffle(prizeLevel);
					if ( threadCancel != null && threadCancel.Cancel ) return;
                    current = firstTile;
                    collectionValuesFilled = 0;
                }

                if (current.type == Tiles.TileTypes.collection)
                {
                    Tiles.ITile backtracker = current;
					for ( int j = 1; j < minMove && !( threadCancel != null && threadCancel.Cancel ); ++j )
                    {
                        if (backtracker.parent != null)
                            backtracker = backtracker.parent;
                    }
					if ( threadCancel != null && threadCancel.Cancel ) return;
                    if (backtracker == current || backtracker.type != Tiles.TileTypes.collection || (backtracker.type == Tiles.TileTypes.collection && backtracker.tileInformation != prizeLevel[collectionValuesFilled]))
                    {
                        current.tileInformation = prizeLevel[collectionValuesFilled++];
                    }
                }

                current = current.child;
            }
        }


        /// <summary>
        /// Creates the blank board.
        /// </summary>
        /// <param name="boardSize"> The size of hte board.</param>
        private void fillInBlankBoardTiles(int boardSize)
        {
            firstTile = new Tiles.Tile();
            firstTile.type = Tiles.TileTypes.blank;
            Tiles.ITile tempTile = firstTile;
			for ( int i = 1; i < boardSize && !( threadCancel != null && threadCancel.Cancel ); ++i )
            {
                Tiles.ITile newTile = new Tiles.Tile();
                newTile.type = Tiles.TileTypes.blank;
                newTile.connectParentToChild(tempTile);
                tempTile = newTile;
            }
            lastTile = tempTile;
        }

        /// <summary>
        /// Fills the tiles with a list of links to other tiles that can be reached from dice rolls. 
        /// </summary>
        /// <param name="boardSize"></param>
        /// <param name="minMove"></param>
        /// <param name="maxMove"></param>
        /// <param name="BackMove"></param>
        /// <param name="ForwardMove"></param>
        private void connectTiles(int boardSize,
            int minMove,
            int maxMove,
            int BackMove,
            int ForwardMove)
        {
            Tiles.ITile currentTile = firstTile;
			while ( currentTile.child != null && !( threadCancel != null && threadCancel.Cancel ) )
            {
                if (currentTile.type == Tiles.TileTypes.moveBack)
                {
                    Tiles.ITile targetGameFromTile = currentTile;
					for ( int j = 0; j < BackMove && targetGameFromTile != null && !( threadCancel != null && threadCancel.Cancel ); ++j )
                    {
                        targetGameFromTile = targetGameFromTile.child;
                    }
					if ( threadCancel != null && threadCancel.Cancel ) return;

                    if (targetGameFromTile != null)
                    {
                        currentTile.addTile(BackMove, targetGameFromTile);
                        currentTile.tileInformation = BackMove.ToString();
                    }
                }
                else if (currentTile.type == Tiles.TileTypes.moveForward)
                {
                    Tiles.ITile targetGameFromTile = currentTile;
					for ( int j = 0; j < ForwardMove && targetGameFromTile != null && !( threadCancel != null && threadCancel.Cancel ); j++ )
                    {
                        targetGameFromTile = targetGameFromTile.child;
                    }
					if ( threadCancel != null && threadCancel.Cancel ) return;

                    if (targetGameFromTile != null)
                    {
                        currentTile.addTile(ForwardMove, targetGameFromTile);
                        currentTile.tileInformation = ForwardMove.ToString();
                    }
                }
                else
                {
					for ( int i = minMove; i < maxMove + 1 && !( threadCancel != null && threadCancel.Cancel ); ++i )
                    {
                        Tiles.ITile targetGameFromTile = currentTile;
                        for (int j = 0; j < i && targetGameFromTile != null; j++)
                        {
                            targetGameFromTile = targetGameFromTile.child;
                        }
						if ( threadCancel != null && threadCancel.Cancel ) return;

                        if (targetGameFromTile != null)
                        {
                            currentTile.addTile(i, targetGameFromTile);
                        }
                    }
                }
                currentTile = currentTile.child;
            }
        }

        public override string ToString()
        {
            Tiles.ITile currentTile = firstTile;
            StringBuilder sb = new StringBuilder();
            while (currentTile != null)
            {
                if (currentTile.type != Tiles.TileTypes.blank)
                {
                    if (currentTile.tileInformation != null)
                        sb.Append(currentTile.tileInformation + ", ");
                    else
                        sb.Append(currentTile.type + ", ");
                }
                else
                    sb.Append("S, ");
                currentTile = currentTile.child;
            }
            return sb.ToString();
        }
    }
}
