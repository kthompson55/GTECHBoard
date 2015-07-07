using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Collection_Game_Tool.Services;
using Collection_Game_Tool.Services.Tiles;
using Collection_Game_Tool.PrizeLevels;
namespace Collection_Game_Tool_Test.ServicesTests
{
    [TestClass]
    public class BoardGenTest
    {
        [TestMethod]
        public void personalGenBoardTest()
        {
            BoardGeneration bg = new BoardGeneration();
            int boardSize = 26;
            int initialReachable = 20;
            PrizeLevels prizes = new PrizeLevels();
            PrizeLevel A1 = new PrizeLevel();
            A1.numCollections = 3;
            A1.prizeLevel = 1;
            A1.isBonusGame = true;
            prizes.addPrizeLevel(A1);

            PrizeLevel A2 = new PrizeLevel();
            A2.numCollections = 3;
            A2.prizeLevel = 2;
            prizes.addPrizeLevel(A2);

            PrizeLevel A3 = new PrizeLevel();
            A3.numCollections = 3;
            A3.prizeLevel = 3;
            A3.isInstantWin = true;
            prizes.addPrizeLevel(A3);

            PrizeLevel A4 = new PrizeLevel();
            A4.numCollections = 3;
            A4.prizeLevel = 4;
            prizes.addPrizeLevel(A4);

            int numberOfExpectedCollection = 12;
            int numberOfExpectedMoveBack = 3;
            int numberOfExpectedMoveForward = 2;
            ITile firstTile = bg.genBoard(boardSize, initialReachable, 2, 6, numberOfExpectedMoveBack, numberOfExpectedMoveForward, prizes, 1, 2);

            int numberOfCollection = 0;
            int numberOfMoveBack = 0;
            int numberOfMoveForward = 0;
            int numberOfextraGames = 0;
            int numberOfTiles = 0;
            ITile currentTile = firstTile;
            while (currentTile != null)
            {
                if (currentTile.type == TileTypes.collection)
                {
                    numberOfCollection++;
                }
                else if (currentTile.type == TileTypes.moveForward)
                {
                    numberOfMoveForward++;
                }
                else if (currentTile.type == TileTypes.moveBack)
                {
                    numberOfMoveBack++;
                }
                else if (currentTile.type == TileTypes.extraGame)
                {
                    numberOfextraGames++;
                }
                numberOfTiles++;
                currentTile = currentTile.child;
            }
            string board = bg.ToString();
            Assert.IsTrue(numberOfCollection == numberOfExpectedCollection, "Number Of Collection Tiles not correct");
            Assert.IsTrue(numberOfMoveBack == numberOfExpectedMoveBack, "Number Of MoveBack Tiles not correct");
            Assert.IsTrue(numberOfMoveForward == numberOfExpectedMoveForward, "Number Of MoveForward Tiles not correct");
            Assert.IsTrue(boardSize == numberOfTiles, "Number Of Tiles not correct");
            if (board == "")
            {

            }
        }
    }
}
