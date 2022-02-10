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
        public void TestMatchingVersionsAreGroupedCorrectly()
        {
            const string version = "1.2.3.4-PreRelease.987";
            var ver1 = VersionFactory.CreateDockerTag(version);
            var ver2 = VersionFactory.CreateDockerTag(version);

            var items = new List<IVersion> {ver1, ver2}.GroupBy(i => i).ToList();
            Assert.AreEqual(1, items.Count);
        }

        [Test]
        [TestCase("1.2.3.4", "1.2.3.4", 0)]
        public void TestVersionsMatchSem(string version1, string version2, int result)
        {
            var ver1 = VersionFactory.CreateSemanticVersion(version1);
            var ver2 = VersionFactory.CreateSemanticVersion(version2);

            var items = new List<IVersion> {ver1, ver2}.GroupBy(i => i).ToList();
            Assert.AreEqual(1, items.Count);
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
    }
}