using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collection_Game_Tool.Services
{
    public class BoardGeneration
    {
        private Tiles.ITile firstTile;
        private Tiles.ITile lastTile;
        //Board Generation stuff goes here
        public Tiles.ITile genBoard(int boardSize,
            int minMove,
            int maxMove,
            int minNumCollectionSpots,
            bool moveBackInclude,
            bool moveForwardInclude,
            bool extraGameInclude,
            int moveForward = 1,
            int moveBack =  1)
        {
            fillInBlankBoardTiles(boardSize);
            fillInTiles(boardSize, minMove, maxMove, minNumCollectionSpots, Tiles.TileTypes.collection);
            if (moveBackInclude)
            {
                fillInTiles(boardSize, minMove, maxMove, (int)(Math.Round((double)boardSize, 5, MidpointRounding.AwayFromZero) + 1), Tiles.TileTypes.moveBack);

            }
            if (moveForwardInclude)
            {
                fillInTiles(boardSize, minMove, maxMove, (int)(Math.Round((double)boardSize, 5, MidpointRounding.AwayFromZero) + 1), Tiles.TileTypes.moveForward);

            }
            if (extraGameInclude)
            {
                fillInTiles(boardSize, minMove, maxMove, (int)(Math.Round((double)boardSize, 10, MidpointRounding.AwayFromZero) + 1), Tiles.TileTypes.extraGame);
            }
            connectTiles(boardSize, minMove, maxMove, moveBack, moveForward);
            return firstTile;
        }

        private void fillInBlankBoardTiles(int boardSize)
        {
            firstTile = new Tiles.Tile();
            firstTile.type = Tiles.TileTypes.blank;
            Tiles.ITile tempTile = firstTile;
            for (int i = 0; i < boardSize; i++)
            {
                Tiles.ITile newTile = new Tiles.Tile();
                newTile.type = Tiles.TileTypes.blank;
                tempTile.connectParentToChild(newTile);
                tempTile = newTile;
            }
            lastTile = tempTile;
        }

        private void fillInTiles(int boardSize,
            int minMove,
            int maxMove,
            int numTiles,
            Tiles.TileTypes type)
        {
            Random rand = new Random();
            Tiles.ITile currentTile = firstTile;
            int currentSpace = 0;
            for (int i = 0; i < numTiles; i++)
            {
                int moveAmount = rand.Next(minMove, maxMove + 1);
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
    }
}
