using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Collection_Game_Tool.PrizeLevels;

namespace Collection_Game_Tool_Test.PrizeLevelsTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test_Prize_Comparison()
        {
            PrizeLevel pl1 = new PrizeLevel();
            PrizeLevel pl2 = new PrizeLevel();

            pl1.prizeValue = 50;
            pl2.prizeValue = 100;

            Assert.IsTrue(pl1.CompareTo(pl2) < 0);
        }

        [TestMethod]
        public void Test_Prize_Comparison_Null()
        {
            PrizeLevel pl1 = new PrizeLevel();

            pl1.prizeValue = 100;

            Assert.IsTrue(pl1.CompareTo(null) > 0);
        }

        [TestMethod]
        public void Test_Prize_Level_Assignment()
        {
            PrizeLevel pl1 = new PrizeLevel();

            pl1.prizeLevel = 10;

            Assert.IsTrue(pl1.prizeLevel == 10);
        }

        [TestMethod]
        public void Test_Prize_Value_Assignment()
        {
            PrizeLevel pl1 = new PrizeLevel();

            pl1.prizeValue = 100;

            Assert.IsTrue(pl1.prizeValue == 100);
        }

        [TestMethod]
        public void Test_Prize_Collection_Assignment()
        {
            PrizeLevel pl1 = new PrizeLevel();

            pl1.numCollections = 5;

            Assert.IsTrue(pl1.numCollections == 5);
        }

        [TestMethod]
        public void Test_Prize_Win_Assignment()
        {
            PrizeLevel pl1 = new PrizeLevel();

            pl1.isInstantWin = true;

            Assert.IsTrue(pl1.isInstantWin);
        }
    }
}
