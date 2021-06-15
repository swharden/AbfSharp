using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbfSharpTests
{
    class AbfAPI
    {
        [Test]
        public void Test_ToString_IsCustom()
        {
            string abfFilePath = SampleData.GetAbfPath("17n16016-ic-steps.abf");
            var abf = new AbfSharp.ABF(abfFilePath);
            Assert.IsNotNull(abf.ToString());
            Assert.That(abf.ToString().Contains("sweep"));
        }
    }
}
