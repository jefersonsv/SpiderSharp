using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SpiderSharp.Text
{
    [TestClass]
    public class RegexTest
    {
        [TestMethod]
        public void RemoveNthChildTest()
        {
            var clear = SpiderSharp.Helpers.RegexHelper.RemoveAllNthChild(@"$$("".list-unstyled > li:nth-child(3) > div:nth-child(2) > ul:nth-child(3) > li > a"")");
            Assert.AreEqual(clear, @"$$("".list-unstyled > li > div > ul > li > a"")");

        }
    }
}
