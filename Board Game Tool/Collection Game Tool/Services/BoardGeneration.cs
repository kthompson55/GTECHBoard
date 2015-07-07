﻿using System;
using System.Collections;
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
                numberOfCollectionSpots += p.numCollections;
            }
            fillInBlankBoardTiles(boardSize);

            Tiles.TileTypes[] specialTiles = new Tiles.TileTypes[moveBackCount + moveForwardCount];
            Tiles.TileTypes[] collectionTiles = new Tiles.TileTypes[numberOfCollectionSpots];

            for (int i = 0; i < collectionTiles.Length; i++)
            {
                collectionTiles[i] = Tiles.TileTypes.collection;
            }

            int index = 0;
            while (index < specialTiles.Length)
            {
                if (index < moveForwardCount)
                {
                    specialTiles[index] = Tiles.TileTypes.moveForward;
                }
                else
                {
                    specialTiles[index] = Tiles.TileTypes.moveBack;
                }
                index++;
            }

            specialTiles = ArrayShuffler<Tiles.TileTypes>.shuffle(specialTiles);
            fillInSpecialTiles(initialReachable, minMove, maxMove, moveForward, moveBack, specialTiles);
            fillInSpecialTiles(initialReachable, minMove, maxMove, moveForward, moveBack, collectionTiles);
            fillInCollectionTileValues(minMove, numberOfCollectionSpots, prizes);
            connectTiles(boardSize, minMove, maxMove, moveBack, moveForward);


            return firstTile;
        }

        private void fillInSpecialTiles(int initialReachable,
                int minMove,
                int maxMove,
                int moveForward,
                int moveBack,
                Tiles.TileTypes[] tiles)
        {
            Tiles.ITile currentTile = firstTile;
            for (int i = 0; i < tiles.Length; i++)
            {
                bool tilePlaced = false;
                int currentSpace = 0;
                Tiles.TileTypes myTile = tiles[i];

                int moveAmount = SRandom.nextInt(minMove, (initialReachable / maxMove)) * (i + 1);
                for (int j = 0; j < moveAmount; j++)
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

                while (!tilePlaced)
                {
                    if (currentTile.type == Tiles.TileTypes.blank)
                    {
                        if (myTile.ToString().Equals("moveForward"))
                        {
                            Tiles.ITile tempTile = currentTile;
                            for (int k = 0; k < moveForward; k++)
                            {
                                if (tempTile.child != null)
                                {
                                    tempTile = tempTile.child;
                                }
                            }
                            if (tempTile.type != Tiles.TileTypes.moveBack)
                            {
                                currentTile.type = myTile;
                                tilePlaced = true;
                            }
                        }
                        else if (myTile.ToString().Equals("moveBack"))
                        {
                            Tiles.ITile tempTile = currentTile;
                            for (int k = 0; k < moveBack; k++)
                            {
                                if (tempTile.parent != null)
                                {
                                    tempTile = tempTile.parent;
                                }
                            }
                            if (tempTile.type != Tiles.TileTypes.moveForward)
                            {
                                currentTile.type = myTile;
                                tilePlaced = true;
                            }
                        }
                        else
                        {
                            currentTile.type = myTile;
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

        private void fillInCollectionTileValues(
            int minMove,
            int numberOfCollectionSpots,
            PrizeLevels.PrizeLevels prizes)
        {
            int index = 0;
            string[] prizeLevel = new string[numberOfCollectionSpots];
            foreach (PrizeLevels.PrizeLevel p in prizes.prizeLevels)
            {
                for (int i = 0; i < p.numCollections; i++)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("CS");
                    if (p.isInstantWin)
                        sb.Append(":IW");
                    if (p.isBonusGame)
                        sb.Append(":BG");
                    sb.Append((":" + (char)(p.prizeLevel + 97)));
                    prizeLevel[index] = sb.ToString();
                    index++;
                }
            }

            int collectionValuesFilled = 0;
            Tiles.ITile current = firstTile;
            prizeLevel = ArrayShuffler<string>.shuffle(prizeLevel);
            while (collectionValuesFilled != numberOfCollectionSpots)
            {
                if (current == null)
                {
                    prizeLevel = ArrayShuffler<string>.shuffle(prizeLevel);
                    current = firstTile;
                    collectionValuesFilled = 0;
                }

                if (current.type == Tiles.TileTypes.collection)
                {
                    Tiles.ITile backtracker = current;
                    for (int j = 1; j < minMove; j++)
                    {
                        if (backtracker.parent != null)
                            backtracker = backtracker.parent;
                    }

                    if (backtracker == current || backtracker.type != Tiles.TileTypes.collection || (backtracker.type == Tiles.TileTypes.collection && backtracker.tileInformation != prizeLevel[collectionValuesFilled]))
                    {
                        current.tileInformation = prizeLevel[collectionValuesFilled];
                        collectionValuesFilled++;
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
                if (numToCollectPrizes == numberOfDesiredCollection)
                {
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
                        currentTile.tileInformation = "MB:" + BackMove;
                    }
                }
                else if (currentTile.type == Tiles.TileTypes.moveForward)
                {
                    Tiles.ITile targetGameFromTile = currentTile;
                    for (int j = 0; j < ForwardMove && targetGameFromTile != null; j++)
                    {
                        targetGameFromTile = targetGameFromTile.child;
                    }
                    if (targetGameFromTile != null)
                    {
                        currentTile.addTile(ForwardMove, targetGameFromTile);
                        currentTile.tileInformation = "MF:" + ForwardMove;
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
                if (currentTile.tileInformation != null)
                    sb.Append(currentTile.tileInformation + ", ");
                else
                    sb.Append("BS, ");
                currentTile = currentTile.child;
            }
            return sb.ToString();
        }
    }
}
