using NUnit.Framework;
using Octopus.Core.Versioning.Maven;
using Octopus.Core.Versioning.Ranges.Maven;

namespace Octopus.Server.Core.Versioning.Tests.Versions
{
    [TestFixture]
    public class CustomRangeTests
    {
        [Test]
        public void testVersionsWithQualifiers()
        {
            MavenVersionRange range = MavenVersionRange.CreateFromVersionSpec("[1.5,20)");
            Assert.IsFalse(range.ContainsVersion(new MavenVersionParser().Parse("23.4-jre")));
            Assert.IsTrue(range.ContainsVersion(new MavenVersionParser().Parse("19.4-jre")));
        }
        
        [Test]
        public void testVersionsWithKnownQualifiers()
        {
            MavenVersionRange range = MavenVersionRange.CreateFromVersionSpec("[2.0.0-alpha.1,2.0.0]");
            Assert.IsTrue(range.ContainsVersion(new MavenVersionParser().Parse("2.0.0.alpha.1")));
            Assert.IsTrue(range.ContainsVersion(new MavenVersionParser().Parse("2.0.0.beta1")));
            Assert.IsTrue(range.ContainsVersion(new MavenVersionParser().Parse("2.0.0.milestone.1")));
            Assert.IsTrue(range.ContainsVersion(new MavenVersionParser().Parse("2.0.0")));
            Assert.IsFalse(range.ContainsVersion(new MavenVersionParser().Parse("1.9.9")));
        }
    }
}