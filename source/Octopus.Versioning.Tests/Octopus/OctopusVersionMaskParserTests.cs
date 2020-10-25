using System;
using NUnit.Framework;
using Octopus.Versioning.Octopus;
using Octopus.Versioning.Semver;

namespace Octopus.Versioning.Tests.Octopus
{
    [TestFixture]
    public class OctopusVersionMaskParserTests
    {
        static readonly OctopusVersionMaskParser OctopusVersionMaskParser = new OctopusVersionMaskParser();
        static readonly OctopusVersionParser OctopusVersionParser = new OctopusVersionParser();

        [Test]
        [TestCase("1.2.3-alpha.i", null, "1.2.3-alpha.0")]
        [TestCase("1.2.3-alpha.i", "1.2.3-alpha.25", "1.2.3-alpha.26")]
        [TestCase("i.2.3-alpha.i", "1.2.3-alpha.25", "2.2.3-alpha.26")]
        [TestCase("1.i.3-alpha.i", "1.2.3-alpha.25", "1.3.3-alpha.26")]
        [TestCase("1.2.i-alpha.i", "1.2.3-alpha.25", "1.2.4-alpha.26")]
        [TestCase("c.2.3-alpha.i", "1.2.3-alpha.25", "1.2.3-alpha.26")]
        [TestCase("1.c.3-alpha.i", "1.2.3-alpha.25", "1.2.3-alpha.26")]
        [TestCase("1.2.c-alpha.i", "1.2.3-alpha.25", "1.2.3-alpha.26")]
        [TestCase("i.i.i-alpha.i", "1.2.3-alpha.25", "2.0.0-alpha.26")]
        public void ShouldApplyMask(string mask, string latestVersion, string expected)
        {
            var result = OctopusVersionMaskParser.ApplyMask(mask, latestVersion != null ? new SemanticVersion(latestVersion) : null);
            Assert.AreEqual(OctopusVersionParser.Parse(expected), result);
        }
    }
}