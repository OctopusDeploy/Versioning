using System;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Octopus.Versioning.Maven;
using Octopus.Versioning.Maven.Ranges;

namespace Octopus.Versioning.Tests.Maven.Ranges
{
    [TestFixture]
    public class CustomRangeTests
    {
        [Test]
        public void testVersionsWithQualifiers()
        {
            var range = MavenVersionRange.CreateFromVersionSpec("[1.5,20)");
            ClassicAssert.IsFalse(range.ContainsVersion(new MavenVersionParser().Parse("23.4-jre")));
            ClassicAssert.IsTrue(range.ContainsVersion(new MavenVersionParser().Parse("19.4-jre")));
        }

        [Test]
        public void testVersionsWithKnownQualifiers()
        {
            var range = MavenVersionRange.CreateFromVersionSpec("[2.0.0-alpha.1,2.0.0]");
            ClassicAssert.IsTrue(range.ContainsVersion(new MavenVersionParser().Parse("2.0.0.alpha.1")));
            ClassicAssert.IsTrue(range.ContainsVersion(new MavenVersionParser().Parse("2.0.0.beta1")));
            ClassicAssert.IsTrue(range.ContainsVersion(new MavenVersionParser().Parse("2.0.0.milestone.1")));
            ClassicAssert.IsTrue(range.ContainsVersion(new MavenVersionParser().Parse("2.0.0")));
            ClassicAssert.IsFalse(range.ContainsVersion(new MavenVersionParser().Parse("1.9.9")));
        }
    }
}