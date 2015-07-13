using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Collection_Game_Tool.Services;
using Collection_Game_Tool.PrizeLevels;
using Collection_Game_Tool.Divisions;
using Collection_Game_Tool.Services.Tiles;

namespace Collection_Game_Tool_Test.ServicesTests
{
    [TestClass]
    public class GamePlayGenTest
    {
        [TestMethod]
        public void TestFormatGameplay()
        {

        }

        [TestMethod]
        public void TestGenerateGameplay()
        {
            PrizeLevels prizeLevels = createPrizeLevels();
            List<DivisionModel> divisions = new List<DivisionModel>();
            DivisionModel div1 = new DivisionModel();
            div1.DivisionNumber = 1;
            div1.addPrizeLevel(prizeLevels.getPrizeLevel(1));
            divisions.Add(div1);

            DivisionModel div2 = new DivisionModel();
            div2.DivisionNumber = 2;
            div2.addPrizeLevel(prizeLevels.getPrizeLevel(2));
            divisions.Add(div2);


            //DivisionModel div3 = new DivisionModel();
            //div3.DivisionNumber = 3;
            //div3.addPrizeLevel(prizeLevels.getPrizeLevel(3));
            //divisions.Add(div3);

            //DivisionModel div4 = new DivisionModel();
            //div4.DivisionNumber = 4;
            //div4.addPrizeLevel(prizeLevels.getPrizeLevel(4));
            //divisions.Add(div4);


            ITile board = createBoard(prizeLevels);
            List<ITile> boards = new List<ITile>();
            boards.Add(board);
            GamePlayGeneration gp = new GamePlayGeneration(boards);
            gp.Generate(3, divisions, prizeLevels.prizeLevels, 1);
            string output = gp.GetFormattedGameplay(boards);


        }

        private PrizeLevels createPrizeLevels()
        {
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

            //PrizeLevel A3 = new PrizeLevel();
            //A3.numCollections = 3;
            //A3.prizeLevel = 3;
            //A3.isInstantWin = true;
            //prizes.addPrizeLevel(A3);

            //PrizeLevel A4 = new PrizeLevel();
            //A4.numCollections = 3;
            //A4.prizeLevel = 4;
            //prizes.addPrizeLevel(A4);

            return prizes;

        }

        private ITile createBoard(PrizeLevels prizes)
        {
            BoardGeneration bg = new BoardGeneration();
            int boardSize = 26;
            int initialReachable = 18;
            

            
            return bg.GenerateBoard(boardSize, initialReachable, 1, 6, 3, 2, prizes, 1, 2);

        }
    }
}
