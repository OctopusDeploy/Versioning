using System;
using NUnit.Framework;

namespace Octopus.Versioning.Tests.Versions.Docker
{
    [TestFixture]
    public class DockerVersionCompareTests
    {
        [Test]
        [TestCase("latest", "1.0.0", 1)]
        [TestCase("1.0.0", "latest", -1)]
        [TestCase("latest", "latest", 0)]
        public void TestSemverVersions(string version1, string version2, int result)
        {
            Assert.AreEqual(result, VersionFactory.CreateDockerTag(version1).CompareTo(VersionFactory.CreateDockerTag(version2)));
        }
    }
}