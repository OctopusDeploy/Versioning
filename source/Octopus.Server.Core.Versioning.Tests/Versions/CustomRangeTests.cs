using NUnit.Framework;
using Octopus.Core.Resources.Ranges;
using Octopus.Core.Resources.Versioning.Maven;

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
    }
}