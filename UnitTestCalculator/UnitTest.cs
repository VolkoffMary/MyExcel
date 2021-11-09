using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTestCalculator
{
    [TestClass]
    public class CalculatorTests
    {
        [TestMethod]
        public void Test1()
        {
            string exp = "-2345678901234567890 - 12 / 2";
            decimal expected = -2345678901234567896;

            decimal result = MyExcel.Calculator.Evaluate(exp);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Test2()
        {
            string exp = "-inc(-3) ^ 2 * 2";
            decimal expected = -8;

            decimal result = MyExcel.Calculator.Evaluate(exp);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Test3()
        {
            string exp = "+dec(2^0^2 + 5*2)^1*2";
            decimal expected = 20;

            decimal result = MyExcel.Calculator.Evaluate(exp);

            Assert.AreEqual(expected, result);
        }
    }
}
