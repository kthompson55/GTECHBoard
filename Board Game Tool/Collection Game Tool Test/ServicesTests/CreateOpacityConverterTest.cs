using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Collection_Game_Tool.Services;

namespace Collection_Game_Tool_Test.ServicesTests
{
    [TestClass]
    public class CreateOpacityConverterTest
    {
        [TestMethod]
        public void Test_Convert()
        {
            CreateOpacityConverter coc = new CreateOpacityConverter();

            Assert.IsTrue((double)coc.Convert(true,typeof(double), null, new System.Globalization.CultureInfo("en-us"))==1.0);
            Assert.IsTrue((double)coc.Convert(false, typeof(double), null, new System.Globalization.CultureInfo("en-us")) == 0.3);
        }

        [TestMethod]
        public void Test_Convert_Back()
        {
            CreateOpacityConverter coc = new CreateOpacityConverter();

            Assert.IsTrue((bool)coc.ConvertBack(1.0, typeof(bool), null, new System.Globalization.CultureInfo("en-us")));
            Assert.IsFalse((bool)coc.ConvertBack(0.3, typeof(bool), null, new System.Globalization.CultureInfo("en-us")));
        }

        [TestMethod]
        public void Test_Invalid_Convert()
        {
            CreateOpacityConverter coc = new CreateOpacityConverter();

            Assert.IsTrue((double)coc.Convert(null, typeof(double), null, new System.Globalization.CultureInfo("en-us")) == 0.3);
        }

        [TestMethod]
        public void Test_Invalid_Convert_Back()
        {
            CreateOpacityConverter coc = new CreateOpacityConverter();

            Assert.IsFalse((bool)coc.ConvertBack(null, typeof(bool), null, new System.Globalization.CultureInfo("en-us")));
        }
    }
}
