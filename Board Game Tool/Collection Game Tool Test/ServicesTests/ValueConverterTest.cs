using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Collection_Game_Tool.Services;

namespace Collection_Game_Tool_Test.ServicesTests
{
    [TestClass]
    public class ValueConverterTest
    {
        [TestMethod]
        public void Test_Convert()
        {
            ValueConverter vc = new ValueConverter();

            Assert.IsTrue(((string)vc.Convert(40.0,typeof(string), null, new System.Globalization.CultureInfo("en-us"))).Equals("40"));
        }

        [TestMethod]
        public void Test_Convert_Back()
        {
            ValueConverter vc = new ValueConverter();

            Assert.IsTrue((double)vc.ConvertBack("40", typeof(double), null, new System.Globalization.CultureInfo("en-us")) == 40.0);
        }

        [TestMethod]
        public void Test_Invalid_Convert()
        {
            ValueConverter vc = new ValueConverter();

            Assert.IsTrue(((string)vc.Convert(null, typeof(string), null, new System.Globalization.CultureInfo("en-us"))).Equals("0"));
        }

        [TestMethod]
        public void Test_Invalid_Convert_Back()
        {
            ValueConverter vc = new ValueConverter();

            Assert.IsTrue((double)vc.ConvertBack(null, typeof(double), null, new System.Globalization.CultureInfo("en-us")) == 0);
        }
    }
}
