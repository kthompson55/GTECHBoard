using System;
using System.Collections.Generic;
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
        
        /// <summary>
        /// Board generation creates the board for play. The board uses a doubly linked list for its design.
        /// </summary>
        /// <param name="boardSize">Board size is the total size of the board</param>
        /// <param name="minMove">The min movement a player can make in a turn</param>
        /// <param name="maxMove">The max movement a player can make in a turn</param>
        /// <param name="moveBackInclude">Includ move back tiles</param>
        /// <param name="moveForwardInclude">Include move forward tiles</param>
        /// <param name="extraGameInclude">Include extra game tiles</param>
        /// <param name="prizes">All the game prizes</param>
        /// <param name="moveForward">How far move forward tiles will move you</param>
        /// <param name="moveBack"><How far move back tiles will move you/param>
        /// <returns>Returns the first tile of the board.</returns>
        public Tiles.ITile genBoard(int boardSize,
            int minMove,
            int maxMove,
            int moveBackCount,
            int moveForwardCount,
            int extraGameCount,
            PrizeLevels.PrizeLevels prizes,
            int moveForward = 1,
            int moveBack =  1)
        {
            int numberOfCollectionSpots = 0;
            foreach (PrizeLevels.PrizeLevel p in prizes.prizeLevels)
            {
                numberOfCollectionSpots += p.numCollections;
            }
            fillInBlankBoardTiles(boardSize);
            fillInTiles(boardSize, minMove, maxMove, numberOfCollectionSpots, Tiles.TileTypes.collection);
            if (moveBackCount > 0)
            {
                fillInTiles(boardSize, minMove, maxMove, (int)(Math.Round((double)(boardSize/5), MidpointRounding.AwayFromZero) + 1), Tiles.TileTypes.moveBack);

            }
            if (moveForwardCount > 0)
            {
                fillInTiles(boardSize, minMove, maxMove, (int)(Math.Round((double)(boardSize/5), MidpointRounding.AwayFromZero) + 1), Tiles.TileTypes.moveForward);

            }
            if (extraGameCount > 0)
            {
                fillInTiles(boardSize, minMove, maxMove, (int)(Math.Round((double)(boardSize/10), MidpointRounding.AwayFromZero) + 1), Tiles.TileTypes.extraGame);
            }
            connectTiles(boardSize, minMove, maxMove, moveBack, moveForward);
            return firstTile;
        }

        public Tiles.ITile genBoard(
            int boardSize,
            int minMove,
            int maxMove,
            int moveBackCount,
            int moveForwardCount,
            PrizeLevels.PrizeLevels prizes,
            int initialReachCount,
            int moveForward = 1,
            int moveBack = 1)
        {
            int numberOfCollectionSpots = 0;
            foreach (PrizeLevels.PrizeLevel p in prizes.prizeLevels)
            {
                numberOfCollectionSpots += p.numCollections;
            }
            fillInBlankBoardTiles(boardSize);
            fillInTiles(boardSize, minMove, maxMove, numberOfCollectionSpots, Tiles.TileTypes.collection);
            if (moveBackCount > 0)
            {
                fillInTiles(boardSize, minMove, maxMove, (int)(Math.Round((double)(boardSize/5), MidpointRounding.AwayFromZero) + 1), Tiles.TileTypes.moveBack);

            }
            if (moveForwardCount > 0)
            {
                fillInTiles(boardSize, minMove, maxMove, (int)(Math.Round((double)(boardSize/5), MidpointRounding.AwayFromZero) + 1), Tiles.TileTypes.moveForward);

            }
            connectTiles(boardSize, minMove, maxMove, moveBack, moveForward);
            return firstTile;
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
            for (int i = 1; i < boardSize; i++)
            {
                Tiles.ITile newTile = new Tiles.Tile();
                newTile.type = Tiles.TileTypes.blank;
                newTile.connectParentToChild(tempTile);
                tempTile = newTile;
            }
            lastTile = tempTile;
        }

        /// <summary>
        /// Fills in specail tiles throughout the board the player might hit. 
        /// </summary>
        /// <param name="boardSize">The board size. </param>
        /// <param name="minMove">The min movement a player can make</param>
        /// <param name="maxMove">The max movment a player can make</param>
        /// <param name="numTiles">The number of tiles in the board</param>
        /// <param name="type">the type of tile to fill in.</param>
        private void fillInTiles(int boardSize,
            int minMove,
            int maxMove,
            int numTiles,
            Tiles.TileTypes type)
        {
            Tiles.ITile currentTile = firstTile;
            int currentSpace = 0;
            for (int i = 0; i < numTiles; i++)
            {
                int moveAmount = SRandom.nextInt(minMove, maxMove + 1);
                for (int j = 0; j < moveAmount && currentSpace < boardSize; j++)
                {
                    currentTile = currentTile.child;
                }
                if (currentTile.type == Tiles.TileTypes.blank)
                {
                    currentTile.type = type;
                }
                currentSpace += moveAmount;
            }
        }


        ///NOT FINISHED
        /// <summary>
        /// Fills in collection tiles in the board
        /// </summary>
        /// <param name="minMove">The min movment a player can me</param>
        /// <param name="maxMove">The max movment a player can make</param>
        /// <param name="numMoves">The total amount of moves a player has</param>
        /// <param name="divisions">The divisions a player can win</param>
        /// <param name="prizes">The prizes</param>
        private void fillInCollectionTileValues(
            int minMove,
            int maxMove,
            int numMoves,
            Divisions.DivisionsModel divisions,
            PrizeLevels.PrizeLevels prizes)
        {
            int[] prizeLevels = new int[prizes.getNumPrizeLevels()];
            for (int i = 0; i < prizeLevels.Length; i++)
            {
                prizeLevels[i] = prizes.getPrizeLevel(i).numCollections;
            }
            Tiles.ITile current = firstTile;

            for (int i = numMoves; i > 0; i--)
            {
                int allowedEmptySpaces = numMoves - i;
                Divisions.DivisionsModel divs = getDivisionsWitNumCollection(i, divisions);
                for (int j = 0; j < divisions.divisions.Count; j++)
                {
                    int[] prizesAtThisDivisions = new int[divisions.divisions[j].getPrizeLevelsAtDivision().Count];
                    int placedCollectionSpots = 0;
                    while (placedCollectionSpots < i)
                    {

                    }
                } 
            }
            
           
        }

        /// <summary>
        /// Gets the number of collections a division needs to be won.
        /// </summary>
        /// <param name="numberOfDesiredCollection"></param>
        /// <param name="divisions"></param>
        /// <returns></returns>
        private Divisions.DivisionsModel getDivisionsWitNumCollection(
            int numberOfDesiredCollection,
            Divisions.DivisionsModel divisions)
        {
            Divisions.DivisionsModel divs = new Divisions.DivisionsModel();
            foreach (Divisions.DivisionModel dm in divisions.divisions)
            {
                List<PrizeLevels.PrizeLevel> prizesAtDivision = new List<PrizeLevels.PrizeLevel>();
                prizesAtDivision = dm.getPrizeLevelsAtDivision();
                int numToCollectPrizes = 0;
                foreach (PrizeLevels.PrizeLevel p in prizesAtDivision)
                {
                    numToCollectPrizes += p.numCollections;
                }
                if (numToCollectPrizes == numberOfDesiredCollection){
                    divs.addDivision(dm);
                }
            }
            return divs;
        }

        /// <summary>
        /// Fils the tiels with a list of links to other tiles that can be reached from dice rolls. 
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
            while (currentTile.child != null)
            {
                if (currentTile.type == Tiles.TileTypes.moveBack)
                {
                    Tiles.ITile targetGameFromTile = currentTile;
                    for (int j = 0; j < BackMove && targetGameFromTile != null; j++)
                    {
                        targetGameFromTile = targetGameFromTile.child;
                    }
                    if (targetGameFromTile != null)
                    {
                        currentTile.addTile(BackMove, targetGameFromTile);
                        currentTile.tileInformation = BackMove.ToString();
                    }
                } 
                else if(currentTile.type == Tiles.TileTypes.moveForward){
                    Tiles.ITile targetGameFromTile = currentTile;
                    for (int j = 0; j < ForwardMove && targetGameFromTile != null; j++)
                    {
                        targetGameFromTile = targetGameFromTile.child;
                    }
                    if (targetGameFromTile != null)
                    {
                        currentTile.addTile(ForwardMove, targetGameFromTile);
                        currentTile.tileInformation = ForwardMove.ToString();
                    }
                }
                else
                {
                    for (int i = minMove; i < maxMove + 1; i++)
                    {
                        Tiles.ITile targetGameFromTile = currentTile;
                        for (int j = 0; j < i && targetGameFromTile != null; j++)
                        {
                            targetGameFromTile = targetGameFromTile.child;
                        }
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
                sb.Append(currentTile.type + " :");
                currentTile = currentTile.child;
            }
            return sb.ToString();
        }
    }
}
