using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Collection_Game_Tool.PrizeLevels;

namespace Collection_Game_Tool_Test.PrizeLevelsTests
{
    [TestClass]
    public class PrizeLevelsTests
    {
        [TestMethod]
        public void Test_Add_Get_Prize_Level()
        {
            PrizeLevels pls = new PrizeLevels();

            PrizeLevel pl1 = new PrizeLevel();

            pls.addPrizeLevel(pl1);

            Assert.IsTrue(pl1.Equals(pls.getPrizeLevel(0)));
        }

        [TestMethod]
        public void Test_Invalid_Add_Prize_Level()
        {
            PrizeLevels pls = new PrizeLevels();

            pls.addPrizeLevel(null);

            Assert.IsTrue(pls.getNumPrizeLevels() == 0);
        }

        [TestMethod]
        public void Test_Invalid_Get_Prize_Level()
        {
            PrizeLevels pls = new PrizeLevels();

            Assert.IsNull(pls.getPrizeLevel(1));
        }

        [TestMethod]
        public void Test_Remove_Count_Prize_Level()
        {
            PrizeLevels pls = new PrizeLevels();

            PrizeLevel pl1 = new PrizeLevel();

            pls.addPrizeLevel(pl1);

            pls.removePrizeLevel(0);

            Assert.IsTrue(pls.getNumPrizeLevels() == 0);
        }

        [TestMethod]
        public void Test_Invalid_Remove_Prize_Level()
        {
            PrizeLevels pls = new PrizeLevels();

            pls.removePrizeLevel(5);

            Assert.IsTrue(pls.getNumPrizeLevels() == 0);
        }

        [TestMethod]
        public void Test_Level_Prize_Level()
        {
            PrizeLevels pls = new PrizeLevels();

            PrizeLevel pl1 = new PrizeLevel();
            PrizeLevel pl2 = new PrizeLevel();

            pls.addPrizeLevel(pl1);
            pls.addPrizeLevel(pl2);

            Assert.IsTrue(pls.getLevelOfPrize(pl1) == 0);
        }

        [TestMethod]
        public void Test_Invalid_Level_Prize_Level()
        {
            PrizeLevels pls = new PrizeLevels();

            PrizeLevel pl1 = new PrizeLevel();

            pls.addPrizeLevel(pl1);

            Assert.IsTrue(pls.getLevelOfPrize(null) == -1);
        }

        [TestMethod]
        public void Test_Add_At_Prize_Level()
        {
            PrizeLevels pls = new PrizeLevels();

            PrizeLevel pl1 = new PrizeLevel();
            PrizeLevel pl2 = new PrizeLevel();

            pls.addPrizeLevel(pl1);
            pls.addPrizeLevelAt(pl2, 0);

            Assert.IsTrue(pls.getLevelOfPrize(pl2) == 0);
        }

        [TestMethod]
        public void Test_Invalid_Add_At_Prize_Level()
        {
            PrizeLevels pls = new PrizeLevels();
            PrizeLevel pl1 = new PrizeLevel();

            pls.addPrizeLevelAt(null, 0);
            pls.addPrizeLevelAt(pl1, 40);

            Assert.IsTrue(pls.getNumPrizeLevels() == 0);
        }
    }
}
