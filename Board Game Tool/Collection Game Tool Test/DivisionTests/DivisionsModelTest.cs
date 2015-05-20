using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Collection_Game_Tool.Divisions;

namespace Collection_Game_Tool_Test.DivisionTests
{
    /// <summary>
    /// Summary description for DivisionsModelTest
    /// </summary>
    [TestClass]
    public class DivisionsModelTest
    {
        int numberOfTestRuns = 100;
        public DivisionsModelTest()
        {
            //
            // TODO: Add constructor logic here
            //
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
        public void testGetNumberOfDivisions()
        {
            DivisionsModel dm = new DivisionsModel();
            for (int i = 0; i < numberOfTestRuns; i++)
            {
                DivisionModel div = new DivisionModel();
                dm.addDivision(div);
                Assert.IsTrue(dm.getNumberOfDivisions() == i +1, "Number of divisions is wrong.");
            }
        }

        [TestMethod]
        public void testGetDivision()
        {
            DivisionsModel dm = new DivisionsModel();
            for (int i = 0; i < numberOfTestRuns; i++)
            {
                DivisionModel div = new DivisionModel();
                dm.addDivision(div);
                Assert.IsTrue(dm.getDivision(i) != null, "Number of divisions is wrong.");
            }
        }
    }
}
