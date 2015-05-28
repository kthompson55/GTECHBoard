using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Collection_Game_Tool.Services;
using Collection_Game_Tool.Services.Tiles;

namespace Collection_Game_Tool_Test.ServicesTests
{
    [TestClass]
    public class boadGenTest
    {
        [TestMethod]
        public void personalGenBoardTest()
        {
            BoardGeneration bg = new BoardGeneration();
            int boardSize = 20;
            ITile firstTile = bg.genBoard(boardSize, 1, 5, 3, true, true, true, 1, 2);
            int numberOfExpectedCollection = 3;
            int numberOfExpectedMoveBack = 5;
            int numberOfExpectedMoveForward = 5;
            int numberOfExpectedextraGames = 3;
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

        }
    }
}
