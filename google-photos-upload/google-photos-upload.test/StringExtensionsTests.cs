using Microsoft.VisualStudio.TestTools.UnitTesting;
using google_photos_upload.Extensions;
using System;

namespace google_photos_upload.test
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void Test_UrlEncode()
        {
            //Setup
            string val = "http://www.deådæekxåsed.com/290479322&dlkwelnewdswå";
            string actual = val.UrlEncode();

            //Expected
            var expected = "http%3A%2F%2Fwww.de%C3%A5d%C3%A6ekx%C3%A5sed.com%2F290479322%26dlkwelnewdsw%C3%A5";

            //Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
