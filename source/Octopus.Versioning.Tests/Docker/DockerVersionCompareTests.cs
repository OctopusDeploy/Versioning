using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Octopus.Versioning.Tests.Docker
{
    [TestFixture]
    public class DockerVersionCompareTests
    {
        [Test]
        [TestCase("latest", "1.0.0", -1)]
        [TestCase("1.0.0", "latest", 1)]
        [TestCase("latest", "latest", 0)]
        public void TestLatestVersions(string version1, string version2, int result)
        {
            var ver1 = VersionFactory.CreateDockerTag(version1);
            var ver2 = VersionFactory.CreateDockerTag(version2);

            Assert.AreEqual(result, ver1.CompareTo(ver2));
        }

        [Test]
        [TestCase("latest", false)]
        [TestCase("1.0.0", false)]
        [TestCase("1.0.0-latest", true)]
        public void TestPrerelease(string version1, bool result)
        {
            var ver1 = VersionFactory.CreateDockerTag(version1);
            Assert.AreEqual(result, ver1.IsPrerelease);
        }
        [Test]
        [TestCase("latest")]
        [TestCase("1.2.3")]
        [TestCase("1.2.3-SNAPSHOT-4")]
        public void TestMatchingVersionsAreGroupedCorrectly(string version)
        {
            var ver1 = VersionFactory.CreateDockerTag(version);
            var ver2 = VersionFactory.CreateDockerTag(version);

            var items = new List<IVersion> {ver1, ver2}.GroupBy(i => i).ToList();
            Assert.AreEqual(1, items.Count);
        }

        [Test]
        [TestCase("latest", "1.0.0")]
        [TestCase("1.2.3", "4.5.6")]
        [TestCase("1.2.3-pre-4", "1.2.3-pre-5")]
        public void TestMismatchingVersionsHashCodesAreDifferent(string v1, string v2)
        {
            var ver1 = VersionFactory.CreateDockerTag(v1);
            var ver2 = VersionFactory.CreateDockerTag(v2);

            Assert.AreNotEqual(ver1.GetHashCode(), ver2.GetHashCode());
        }
    }
}