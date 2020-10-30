using System;
using NUnit.Framework;
using Octopus.Versioning.Octopus;

namespace Octopus.Versioning.Tests.Octopus
{
    [TestFixture]
    public class OctopusVersionCompareTests
    {
        static readonly OctopusVersionParser OctopusVersionParser = new OctopusVersionParser();

        [Test]
        [TestCase("1.0.0", "1.0.0", 0)]
        [TestCase("1-0-0", "1.0.0", 0)]
        [TestCase("1_0_0", "1.0.0", 0)]
        [TestCase("1.0.1", "1.0.0", 1)]
        [TestCase("1-0-1", "1.0.0", 1)]
        [TestCase("1_0_1", "1.0.0", 1)]
        [TestCase("1.0.1", "1.0.2", -1)]
        [TestCase("1-0-1", "1.0.2", -1)]
        [TestCase("1_0_1", "1.0.2", -1)]
        [TestCase("1.1.0", "1.1.0", 0)]
        [TestCase("1-1-0", "1.1.0", 0)]
        [TestCase("1_1_0", "1.1.0", 0)]
        [TestCase("1.1.1", "1.1.0", 1)]
        [TestCase("1-1-1", "1.1.0", 1)]
        [TestCase("1_1_1", "1.1.0", 1)]
        [TestCase("1.1.1", "1.1.2", -1)]
        [TestCase("1-1-1", "1.1.2", -1)]
        [TestCase("1_1_1", "1.1.2", -1)]
        [TestCase("1.0.0.0", "1.0.0.0", 0)]
        [TestCase("1-0-0.0", "1.0.0.0", 0)]
        [TestCase("1_0_0.0", "1.0.0.0", 0)]
        [TestCase("1.0.1.0", "1.0.0.0", 1)]
        [TestCase("1-0-1.0", "1.0.0.0", 1)]
        [TestCase("1_0_1.0", "1.0.0.0", 1)]
        [TestCase("1.0.1.0", "1.0.2.0", -1)]
        [TestCase("1-0-1.0", "1.0.2.0", -1)]
        [TestCase("1_0_1.0", "1.0.2.0", -1)]
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
        [TestCase("1.0.0+meta1", "1.0.0+meta2", 0)]
        [TestCase("1.0.0+meta3", "1.0.0+meta2", 0)]
        public void TestVersionComparisons(string version1, string version2, int result)
        {
            Assert.AreEqual(result, OctopusVersionParser.Parse(version1).CompareTo(OctopusVersionParser.Parse(version2)));
        }

        [Test]
        [TestCase("1.1.1-prerelease!@#$%^&*()[]{};':\",./<>?", "1-1-1-prerelease][)(*&^%$#@!{};':\",./<>?", 0, Description = "Non alphanumeric chars compare the same")]
        [TestCase("1.1.1-大きい", "1-1-1-小さい", -1, Description = "UTF chars have meaning")]
        [TestCase("1.1.1-!@#.10", "1-1-1-#@!.11", -1, Description = "prerelease tags are compared")]
        public void CompareVersionsWithEquivalentChars(string version1, string version2, int expected)
        {
            var result = OctopusVersionParser.Parse(version1).CompareTo(OctopusVersionParser.Parse(version2));
            if (expected < 0)
            {
                Assert.LessOrEqual(result, -1);
            }
            else if (expected > 0)
            {
                Assert.GreaterOrEqual(result, 1);
            }
            else
            {
                Assert.AreEqual(0, result);
            }
        }
    }
}