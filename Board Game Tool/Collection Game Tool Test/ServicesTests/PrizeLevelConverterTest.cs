using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Collection_Game_Tool.Services;

namespace Collection_Game_Tool_Test.ServicesTests
{
    [TestClass]
    public class PrizeLevelConverterTest
    {
        [TestMethod]
        public void Test_Convert()
        {
            PrizeLevelConverter plc = new PrizeLevelConverter();

            Assert.IsTrue(((string)plc.Convert(1)).Equals("A"));
        }

        [TestMethod]
        public void Test_Convert_Back()
        {
            PrizeLevelConverter plc = new PrizeLevelConverter();

            Assert.IsTrue((int)plc.ConvertBack("A") == 1);
        }

        [TestMethod]
        public void Test_Invalid_Convert()
        {
            PrizeLevelConverter plc = new PrizeLevelConverter();

            Assert.IsTrue(((string)plc.Convert(null)).Equals(""));
        }

        [TestMethod]
        public void Test_Invalid_Convert_Back()
        {
            PrizeLevelConverter plc = new PrizeLevelConverter();

            Assert.IsTrue((int)plc.ConvertBack(null) == -1);
        }
    }
}
