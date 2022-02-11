using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Octopus.Versioning.Tests.Semver
{
    public class SemVerCompareTests
    {
        [Test]
        public void TestMatchingVersionsAreGroupedCorrectly()
        {
            const string version = "1.2.3.4-PreRelease.987";
            var ver1 = VersionFactory.CreateSemanticVersion(version);
            var ver2 = VersionFactory.CreateSemanticVersion(version);

            var items = new List<IVersion> {ver1, ver2}.GroupBy(i => i).ToList();
            Assert.AreEqual(1, items.Count);
        }

        [Test]
        public void TestMismatchingVersionsHashCodesAreDifferent()
        {
            var ver1 = VersionFactory.CreateSemanticVersion("1.2.3-PreRelease.987");
            var ver2 = VersionFactory.CreateSemanticVersion("1.2.3-PreRelease.456");

            Assert.AreNotEqual(ver1.GetHashCode(), ver2.GetHashCode());
        }
    }
}