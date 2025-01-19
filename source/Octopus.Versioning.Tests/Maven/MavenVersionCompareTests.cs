using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Octopus.Versioning.Tests.Maven
{
    public class MavenVersionCompareTests
    {
        [Test]
        [TestCase("1.2.3")]
        [TestCase("1.2.3-SNAPSHOT-4")]
        public void TestMatchingVersionsAreGroupedCorrectly(string version)
        {
            var ver1 = VersionFactory.CreateMavenVersion(version);
            var ver2 = VersionFactory.CreateMavenVersion(version);

            var items = new List<IVersion> {ver1, ver2}.GroupBy(i => i).ToList();
            ClassicAssert.AreEqual(1, items.Count);
        }

        [Test]
        [TestCase("1.2.3", "4.5.6")]
        [TestCase("1.2.3-SNAPSHOT-4", "1.2.3-SNAPSHOT-5")]
        public void TestMismatchingVersionsHashCodesAreDifferent(string v1, string v2)
        {
            var ver1 = VersionFactory.CreateMavenVersion(v1);
            var ver2 = VersionFactory.CreateMavenVersion(v2);

            ClassicAssert.AreNotEqual(ver1.GetHashCode(), ver2.GetHashCode());
        }
    }
}