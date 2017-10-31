using NUnit.Framework;
using Octopus.Core.Resources;
using Octopus.Core.Resources.Versioning.Factories;

namespace Octopus.Server.Core.Versioning.Tests.Versions
{
    [TestFixture]
    public class VersionTests
    {
        private static readonly IVersionFactory VersionFactory = new VersionFactory();
        
        [Test]
        public void TestMavenComparasion()
        {
            var versionOne = VersionFactory.CreateMavenVersion("1.0.0");
            
            Assert.Less(VersionFactory.CreateMavenVersion("0.0.1"), versionOne);
            Assert.Less(VersionFactory.CreateMavenVersion("1.0.0-SNAPSHOT"), versionOne);
            Assert.Less(VersionFactory.CreateMavenVersion("1.0.0-beta-2"), versionOne);
            Assert.Greater(VersionFactory.CreateMavenVersion("1.0.1"), versionOne);
            Assert.Greater(VersionFactory.CreateMavenVersion("1.0.0.1"), versionOne);
            Assert.Greater(VersionFactory.CreateMavenVersion("1.0.0.1-blah"), versionOne);                        
        }

        [Test]
        public void TestInvalidVersion()
        {
            Assert.False(VersionFactory.CanCreateVersion("1.0.*", out var version, FeedType.NuGet));            
        }
    }
}