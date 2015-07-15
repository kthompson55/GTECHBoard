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
            //Assert.IsTrue(!gsm.IsNearWin, "IsNearWin should still be false.");
            //gsm.IsNearWin = true;
            //Assert.IsTrue(gsm.IsNearWin, "IsNearWin should have been set to true.");
        }

        [TestMethod]
        public void TestToggleIsNearWin()
        {
            ///GameSetupModel gsm = new GameSetupModel();
            ///Assert.IsTrue(!gsm.IsNearWin, "IsNearWin should still be false.");
            ///gsm.toggleNearWin();
            ///Assert.IsTrue(gsm.IsNearWin, "IsNearWin should have been set to true.");
            ///gsm.toggleNearWin();
            ///Assert.IsTrue(!gsm.IsNearWin, "IsNearWin should be false again.");
        }

        [TestMethod]
        public void TestNearWinsGetSet()
        {
            GameSetupModel gsm = new GameSetupModel();
            gsm.NearWins = 5;
            Assert.IsTrue(gsm.NearWins == 5, "NearWins should have been set to 5.");
        }

        [TestMethod]
        public void TestMaxPermutationsGetSet()
        {
            GameSetupModel gsm = new GameSetupModel();
            gsm.MaxPermutations = 5;
            Assert.IsTrue(gsm.MaxPermutations == 5, "MaxPermutations should have been set to 5.");
        }


    }
}
