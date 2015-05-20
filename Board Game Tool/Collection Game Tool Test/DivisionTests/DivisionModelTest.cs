using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Collection_Game_Tool.PrizeLevels;
using Collection_Game_Tool.Divisions;

namespace Collection_Game_Tool_Test.DivisionTests
{
    /// <summary>
    /// Summary description for DivisionModelTest
    /// </summary>
    [TestClass]
    public class DivisionModelTest
    {
        int testAmount = 100;
        public DivisionModelTest()
        {

        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void testAddPirzeLevel()
        {
            DivisionModel dm = new DivisionModel();
            for (int i = 0; i < testAmount; i++)
            {
                PrizeLevel pl = new PrizeLevel();
                pl.prizeValue = 100 + i;
                dm.addPrizeLevel(pl);
                Assert.IsTrue(dm.getPrizeLevel(i) != null, "PrizeLevel not added to Division.");
                Assert.IsTrue(dm.getPrizeLevel(i).prizeValue == 100 + i, "PrizeLevel not correct value.");
            }
        }

        [TestMethod]
        public void testGetPrizeLevelsAtDivision()
        {
            DivisionModel dm = new DivisionModel();
            for (int i = 0; i < testAmount; i++)
            {
                PrizeLevel pl = new PrizeLevel();
                pl.prizeValue = 100.0f + i;
                dm.addPrizeLevel(pl);
                Assert.IsTrue(dm.getPrizeLevelsAtDivision().Count == i + 1, "incorrect amount of prizes");
            }
        }

        [TestMethod]
        public void testGetPrizeLevel()
        {
            DivisionModel dm = new DivisionModel();
            for (int i = 0; i < testAmount; i++)
            {
                PrizeLevel pl = new PrizeLevel();
                pl.prizeValue = 100 + i;
                dm.addPrizeLevel(pl);
                Assert.IsTrue(dm.getPrizeLevel(i) != null, "PrizeLevel not added to Division.");
                Assert.IsTrue(dm.getPrizeLevel(i).prizeValue == 100 + i, "PrizeLevel not correct value.");
            }
        }

        [TestMethod]
        public void testGetDivisionValue()
        {
            DivisionModel dm = new DivisionModel();
            double prizeLevel = 0.0f;
            for (int i = 0; i < testAmount; i++)
            {
                PrizeLevel pl = new PrizeLevel();
                pl.prizeValue = 100 + i;
                prizeLevel += pl.prizeValue;
                dm.addPrizeLevel(pl);
                Assert.IsTrue(dm.getPrizeLevel(i) != null, "PrizeLevel not added to Division.");
                Assert.IsTrue(dm.calculateDivisionValue() == prizeLevel, "PrizeLevel not correct value.");
            }
        }
    }
}