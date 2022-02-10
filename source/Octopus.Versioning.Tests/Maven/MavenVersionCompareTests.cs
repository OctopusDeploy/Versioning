using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Octopus.Versioning.Tests.Maven
{
    public class MavenVersionCompareTests
    {
        [Test]
        public void TestMatchingVersionsAreGroupedCorrectly()
        {
            const string version = "1.2.3.4-PreRelease.987";
            var ver1 = VersionFactory.CreateMavenVersion(version);
            var ver2 = VersionFactory.CreateMavenVersion(version);

            var items = new List<IVersion> {ver1, ver2}.GroupBy(i => i).ToList();
            Assert.AreEqual(1, items.Count);
        }
    }
}