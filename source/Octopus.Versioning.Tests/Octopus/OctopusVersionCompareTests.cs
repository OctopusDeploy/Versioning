﻿using System;
using NUnit.Framework;
using Octopus.Versioning.Octopus;
using Octopus.Versioning.Semver;

namespace Octopus.Versioning.Tests.Octopus
{
    [TestFixture]
    public class OctopusVersionCompareTests
    {
        static readonly OctopusVersionParser OctopusVersionParser = new OctopusVersionParser();

        [Test]
        [TestCase("1.0.0", "2.0.0", -1)]
        [TestCase("1.1.0", "1.2.0", -1)]
        [TestCase("1.1.1", "1.1.2", -1)]
        [TestCase("1.1.1.1", "1.1.1.2", -1)]
        [TestCase("1.1.0", "2.2.0", -1)]
        [TestCase("1.1.1", "2.1.2", -1)]
        [TestCase("1.1.1.1", "2.1.1.2", -1)]
        [TestCase("2.0.0", "1.0.0", 1)]
        [TestCase("1.2.0", "1.1.0", 1)]
        [TestCase("1.1.2", "1.1.1", 1)]
        [TestCase("1.1.1.2", "1.1.1.1", 1)]
        [TestCase("2.2.0", "1.1.0", 1)]
        [TestCase("2.1.2", "1.1.1", 1)]
        [TestCase("2.1.1.2", "1.1.1.1", 1)]
        [TestCase("1.0.0", "1.0.0", 0)]
        [TestCase("1.0.1", "1.0.0", 1)]
        [TestCase("1.0.1", "1.0.2", -1)]
        [TestCase("1.1.0", "1.1.0", 0)]
        [TestCase("1.1.1", "1.1.0", 1)]
        [TestCase("1.1.1", "1.1.2", -1)]
        [TestCase("1.0.0.0", "1.0.0.0", 0)]
        [TestCase("1.0.1.0", "1.0.0.0", 1)]
        [TestCase("1.0.1.0", "1.0.2.0", -1)]
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
        [TestCase("1.0.0.1", "1.0.0-2", 1)]
        public void TestVersionComparisons(string version1, string version2, int result)
        {
            Assert.AreEqual(result, OctopusVersionParser.Parse(version1).CompareTo(OctopusVersionParser.Parse(version2)));
        }

        /// <summary>
        /// This test validates the prerelease rules shown on https://semver.org/.
        /// It verifies that valid semver versions compare in the same way with
        /// the generic octopus version.
        /// </summary>
        [Test]
        [TestCase("1.0.0-test.1", "1.0.0-test.2")]
        [TestCase("1.0.0-alpha", "1.0.0-alpha.1")]
        [TestCase("1.0.0-alpha.1", "1.0.0-alpha.beta")]
        [TestCase("1.0.0-alpha.beta", "1.0.0-beta")]
        [TestCase("1.0.0-beta", "1.0.0-beta.2")]
        [TestCase("1.0.0-beta.2", "1.0.0-beta.11")]
        [TestCase("1.0.0-beta.11", "1.0.0-rc.1")]
        [TestCase("1.0.0-rc.1", "1.0.0")]
        public void TestSemverCompare(string version1, string version2)
        {
            var semver1 = SemVerFactory.CreateVersionOrNone(version1);
            var semver2 = SemVerFactory.CreateVersionOrNone(version2);

            Assert.LessOrEqual(semver1.CompareTo(semver2), -1);

            var octopus1 = OctopusVersionParser.Parse(version1);
            var octopus2 = OctopusVersionParser.Parse(version2);

            Assert.LessOrEqual(octopus1.CompareTo(octopus2), -1);
        }

        /// <summary>
        /// Like the TestSemverCompare above, but allowing splits on underscores
        /// and dashes.
        /// </summary>
        [Test]
        [TestCase("1.0.0-test.1", "1.0.0-test-2")]
        [TestCase("1.0.0-test.1", "1.0.0-test_2")]
        [TestCase("1.0.0-test-1", "1.0.0-test-2")]
        [TestCase("1.0.0-test-1", "1.0.0-test_2")]
        [TestCase("1.0.0-test_1", "1.0.0-test-2")]
        [TestCase("1.0.0-test_1", "1.0.0-test_2")]
        [TestCase("1.0.0-alpha", "1.0.0-alpha_1")]
        [TestCase("1.0.0-beta.2", "1.0.0-beta-11")]
        [TestCase("1.0.0-beta.2", "1.0.0-beta_11")]
        [TestCase("1.0.0-beta-2", "1.0.0-beta-11")]
        [TestCase("1.0.0-beta-2", "1.0.0-beta_11")]
        [TestCase("1.0.0-beta_2", "1.0.0-beta-11")]
        [TestCase("1.0.0-beta_2", "1.0.0-beta_11")]
        public void TestOctopusPrereleaseCompare(string version1, string version2)
        {
            var octopus1 = OctopusVersionParser.Parse(version1);
            var octopus2 = OctopusVersionParser.Parse(version2);

            Assert.LessOrEqual(octopus1.CompareTo(octopus2), -1);
        }

        [Test]
        [TestCase("1.1.1-prerelease_-.\\", "1.1.1-prerelease\\.-_", 0, Description = "Non alphanumeric chars compare the same")]
        [TestCase("1.1.1-\\_.10", "1.1.1._\\.11", -1, Description = "prerelease tags are compared")]
        public void CompareVersionsWithEquivalentChars(string version1, string version2, int expected)
        {
            var result = OctopusVersionParser.Parse(version1).CompareTo(OctopusVersionParser.Parse(version2));
            if (expected < 0)
                Assert.LessOrEqual(result, -1);
            else if (expected > 0)
                Assert.GreaterOrEqual(result, 1);
            else
                Assert.AreEqual(0, result);
        }
    }
}