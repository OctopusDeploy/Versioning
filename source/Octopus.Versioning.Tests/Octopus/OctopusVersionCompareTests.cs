using System;
using NUnit.Framework;
using Octopus.Versioning.Octopus;

namespace Octopus.Versioning.Tests.Versions.Octopus
{
    [TestFixture]
    public class OctopusVersionCompareTests
    {
        [Test]
        [TestCase("1.0.0", "1.0.0", 0)]
        [TestCase("1-0-0", "1.0.0", 0)]
        [TestCase("1_0_0", "1.0.0", 0)]
        [TestCase("1.0.0", "1.0.0.0", 0)]
        [TestCase("1.0.0-prerelease", "1.0.0", -1)]
        [TestCase("1.0.0", "1.0.0-prerelease", 1)]
        [TestCase("1.0.0", "1.0.0-prerelease", 1)]
        [TestCase("1.0.0-prerelease", "1.0.0-prerelease", 0)]
        [TestCase("1.0.0-prerelease.1", "1.0.0-prerelease.1", 0)]
        [TestCase("1.0.0-prerelease-1", "1.0.0-prerelease.1", 0)]
        [TestCase("1.0.0-prerelease_1", "1.0.0-prerelease.1", 0)]
        [TestCase("1.0.0-prerelease_1", "1.0.0-prerelease.2", -1)]
        [TestCase("1.0.0-prerelease_3", "1.0.0-prerelease.2", 1)]
        [TestCase("1.0.0-prerelease1", "1.0.0-prerelease2", -1)]
        [TestCase("1.0.0-prerelease3", "1.0.0-prerelease2", 1)]
        [TestCase("1.0.0+meta", "1.0.0+meta", 0)]
        [TestCase("1.0.0+meta1", "1.0.0+meta2", -1)]
        [TestCase("1.0.0+meta3", "1.0.0+meta2", 1)]
        public void TestSemverVersions(string version1, string version2, int result)
        {
            Assert.AreEqual(result, new OctopusVersionParser().Parse(version1).CompareTo(new OctopusVersionParser().Parse(version2)));
        }
    }
}