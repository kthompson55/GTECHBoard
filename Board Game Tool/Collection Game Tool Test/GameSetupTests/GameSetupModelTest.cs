using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Collection_Game_Tool.GameSetup;

namespace Collection_Game_Tool_Test.GameSetupTests
{
    [TestClass]
    public class GameSetupModelTest
    {
        [TestMethod]
        public void TestTotalPicksGetSet()
        {
            //GameSetupModel gsm = new GameSetupModel();
            //Assert.IsTrue(gsm.totalPicks == 0, "totalPicks should still be 0.");
            //gsm.totalPicks = 5;
            //Assert.IsTrue(gsm.totalPicks == 5, "totalPicks should have been set to 5.");
        }

        [TestMethod]
        public void TestIsNearWinGetSet()
        {
            //GameSetupModel gsm = new GameSetupModel();
            //Assert.IsTrue(!gsm.isNearWin, "isNearWin should still be false.");
            //gsm.isNearWin = true;
            //Assert.IsTrue(gsm.isNearWin, "isNearWin should have been set to true.");
        }

        [TestMethod]
        public void TestToggleIsNearWin()
        {
            ///GameSetupModel gsm = new GameSetupModel();
            ///Assert.IsTrue(!gsm.isNearWin, "isNearWin should still be false.");
            ///gsm.toggleNearWin();
            ///Assert.IsTrue(gsm.isNearWin, "isNearWin should have been set to true.");
            ///gsm.toggleNearWin();
            ///Assert.IsTrue(!gsm.isNearWin, "isNearWin should be false again.");
        }

        [TestMethod]
        public void TestNearWinsGetSet()
        {
            GameSetupModel gsm = new GameSetupModel();
            gsm.nearWins = 5;
            Assert.IsTrue(gsm.nearWins == 5, "nearWins should have been set to 5.");
        }

        [TestMethod]
        public void TestMaxPermutationsGetSet()
        {
            GameSetupModel gsm = new GameSetupModel();
            gsm.maxPermutations = 5;
            Assert.IsTrue(gsm.maxPermutations == 5, "maxPermutations should have been set to 5.");
        }


    }
}
