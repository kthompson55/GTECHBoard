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
    public class boadGenTest
    {
        [TestMethod]
        public void personalGenBoardTest()
        {
            BoardGeneration bg = new BoardGeneration();
            int boardSize = 24;
            PrizeLevels prizes = new PrizeLevels();
            PrizeLevel A1 = new PrizeLevel();
            A1.numCollections = 3;
            prizes.addPrizeLevel(A1);

            PrizeLevel A2 = new PrizeLevel();
            A2.numCollections = 3;
            prizes.addPrizeLevel(A2);

            PrizeLevel A3 = new PrizeLevel();
            A3.numCollections = 3;
            prizes.addPrizeLevel(A3);

            PrizeLevel A4 = new PrizeLevel();
            A4.numCollections = 3;
            prizes.addPrizeLevel(A4);

            int numberOfExpectedCollection = 12;
            int numberOfExpectedMoveBack = 5;
            int numberOfExpectedMoveForward = 5;
            int numberOfExpectedextraGames = 3;
            ITile firstTile = bg.genBoard(boardSize, 1, 2, true, true, true, prizes, 1, 2);
            
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
            Assert.IsTrue(numberOfCollection == numberOfExpectedCollection, "Number Of Collection Tiles not correct");
            Assert.IsTrue(numberOfextraGames <= numberOfExpectedextraGames, "Number Of Extra Game Tiles not correct");
            Assert.IsTrue(numberOfMoveBack <= numberOfExpectedMoveBack, "Number Of MoveBack Tiles not correct");
            Assert.IsTrue(numberOfMoveForward <= numberOfExpectedMoveForward, "Number Of MoveForward Tiles not correct");
            Assert.IsTrue(boardSize == numberOfTiles, "Number Of Tiles not correct");
            string board = bg.ToString();
            if (board == "")
            {

            }
        }
    }
}
