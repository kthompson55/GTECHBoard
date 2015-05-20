using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Collection_Game_Tool.Services;
using System.Windows.Controls;

namespace Collection_Game_Tool_Test.ServicesTests
{
    [TestClass]
    public class RangeRuleTests
    {
        [TestMethod]
        public void Test_Illegal_Characters()
        {
            RangeRule val = new RangeRule();

            val.Min = 1;
            val.Max = 20;

            ValidationResult vr = new ValidationResult(false, "Illegal characters");

            Assert.AreEqual(val.Validate("ugh", new System.Globalization.CultureInfo("en-us")), vr);
        }

        [TestMethod]
        public void Test_Out_Of_Range()
        {
            RangeRule val = new RangeRule();

            val.Min = 1;
            val.Max = 20;

            ValidationResult vr = new ValidationResult(false, "Please enter a number in the given range.");

            Assert.AreEqual(val.Validate("50", new System.Globalization.CultureInfo("en-us")), vr);
        }

        [TestMethod]
        public void Test_Validated()
        {
            RangeRule val = new RangeRule();

            val.Min = 1;
            val.Max = 20;

            ValidationResult vr = new ValidationResult(true, null);

            Assert.AreEqual(val.Validate("5", new System.Globalization.CultureInfo("en-us")), vr);
        }
    }
}
