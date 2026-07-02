using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Octopus.Versioning.Docker;

namespace Octopus.Versioning.Tests.Docker
{
    [TestFixture]
    public class DockerVersionCompareTests
    {
        [Test]
        [TestCase("latest", "1.0.0", -1)]
        [TestCase("1.0.0", "latest", 1)]
        [TestCase("latest", "latest", 0)]
        [TestCase("1.2.3", "1.2.4", -1)]
        [TestCase("1.2.4", "1.2.3", 1)]
        [TestCase("1.2.3", "1.2.3", 0)]
        [TestCase("1.2.3-pre-4", "1.2.3-pre-5", -1)]
        [TestCase("1.2.3-pre-5", "1.2.3-pre-4", 1)]
        [TestCase("1.2.3-pre-4", "1.2.3-pre-4", 0)]
        public void CompareToShouldReturnCorrectValue(string version1, string version2, int result)
        {
            var ver1 = VersionFactory.CreateDockerTag(version1);
            var ver2 = VersionFactory.CreateDockerTag(version2);

            Assert.AreEqual(result, ver1.CompareTo(ver2));
        }

        [Test]
        [TestCase("latest", false)]
        [TestCase("1.0.0", false)]
        [TestCase("1.0.0-latest", true)]
        public void PrereleaseVersionsShouldBeLabelledCorrectly(string version1, bool result)
        {
            var ver1 = VersionFactory.CreateDockerTag(version1);
            Assert.AreEqual(result, ver1.IsPrerelease);
        }

        [Test]
        [TestCase("latest")]
        [TestCase("1.2.3")]
        [TestCase("1.2.3-pre-4")]
        public void MatchingVersionsShouldBeGroupedCorrectly(string version)
        {
            var ver1 = VersionFactory.CreateDockerTag(version);
            var ver2 = VersionFactory.CreateDockerTag(version);

            var items = new List<IVersion> {ver1, ver2}.GroupBy(i => i).ToList();
            Assert.AreEqual(1, items.Count);
        }

        [Test]
        [TestCase("latest")]
        [TestCase("1.2.3")]
        [TestCase("1.2.3-pre-4")]
        public void MatchingVersionsShouldHaveSameHashCodes(string version)
        {
            var ver1 = VersionFactory.CreateDockerTag(version);
            var ver2 = VersionFactory.CreateDockerTag(version);

            Assert.AreEqual(ver1.GetHashCode(), ver2.GetHashCode());
        }

        [Test]
        [TestCase("latest", "1.0.0")]
        [TestCase("1.2.3", "4.5.6")]
        [TestCase("1.2.3-pre-4", "1.2.3-pre-5")]
        public void MismatchingVersionsShouldHaveDifferentHashCodes(string v1, string v2)
        {
            var ver1 = VersionFactory.CreateDockerTag(v1);
            var ver2 = VersionFactory.CreateDockerTag(v2);

            Assert.AreNotEqual(ver1.GetHashCode(), ver2.GetHashCode());
        }

        [Test]
        [TestCase("6.5@sha256:abc123def456", "6.5", "sha256:abc123def456")]
        [TestCase("latest@sha256:abc123def456", "latest", "sha256:abc123def456")]
        [TestCase("6.5", "6.5", "")]
        public void SplitDigestShouldExtractDigestCorrectly(string input, string expectedTag, string expectedDigest)
        {
            var (tag, digest) = DockerTag.SplitDigest(input);
            Assert.AreEqual(expectedTag, tag);
            Assert.AreEqual(string.IsNullOrEmpty(expectedDigest) ? null : expectedDigest, digest);
        }

        [Test]
        [TestCase("6.5@sha256:abc123def456", "sha256:abc123def456")]
        [TestCase("latest", "")]
        public void CreateDockerTagShouldPopulateDigest(string input, string expectedDigest)
        {
            var tag = (DockerTag)VersionFactory.CreateDockerTag(input);
            Assert.AreEqual(string.IsNullOrEmpty(expectedDigest) ? null : expectedDigest, tag.Digest);
        }

        [Test]
        [TestCase("6.5@sha256:abc123def456", "6.5@sha256:abc123def456")]
        [TestCase("6.5", "6.5")]
        [TestCase("latest", "latest")]
        public void ToStringShouldIncludeDigestWhenPresent(string input, string expected)
        {
            var tag = VersionFactory.CreateDockerTag(input);
            Assert.AreEqual(expected, tag.ToString());
        }

        [Test]
        [TestCase("latest", "latest", true)]
        [TestCase("latest", "1.0.0", false)]
        [TestCase("1.2.3", "1.2.3", true)]
        [TestCase("1.2.3", "1.2.4", false)]
        [TestCase("1.2.3-pre-4", "1.2.3-pre-4", true)]
        [TestCase("1.2.3-pre-4", "1.2.3-pre-5", false)]
        public void EqualsShouldReturnCorrectValue(string v1, string v2, bool expected)
        {
            var ver1 = VersionFactory.CreateDockerTag(v1);
            var ver2 = VersionFactory.CreateDockerTag(v2);

            Assert.AreEqual(expected, ver1.Equals(ver2));
        }
    }
}