using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbfSharpTests.Header
{
    class Tags
    {
        [TestCase("17n16012-vc-steps.abf")]
        [TestCase("17n16016-ic-ramp.abf")]
        [TestCase("17n16016-ic-steps.abf")]
        [TestCase("18808025-memtest.abf")]
        public void Test_AbfsWithoutTags(string filename)
        {
            var abf = new AbfSharp.ABF(SampleData.GetAbfPath(filename));

            Assert.IsNotNull(abf.Tags);
            Assert.IsEmpty(abf.Tags);
            Assert.False(abf.HasTags);
        }

        [TestCase("16921011-vc-memtest-tags.abf", new string[] { "+TGOT", "-TGOT" }, new double[] { 399.6672, 520.8576 })]
        public void Test_AbfsWithTags(string filename, string[] comments, double[] times)
        {
            var abf = new AbfSharp.ABF(SampleData.GetAbfPath(filename));

            Assert.IsNotNull(abf.Tags);
            Assert.IsNotEmpty(abf.Tags);
            Assert.True(abf.HasTags);

            Assert.AreEqual(comments.Length, abf.Tags.Length);
            for (int i=0; i<comments.Length; i++)
            {
                Assert.AreEqual(comments[i], abf.Tags[i].comment);
                Assert.AreEqual(times[i], abf.Tags[i].timeSec);
            }
        }
    }
}
