using NUnit.Framework;

namespace Octopus.Versioning.Tests.Versions
{
    [TestFixture]
    public class VersionTests
    {
        [Test]
        public void TestMavenComparison()
        {
            var versionOne = VersionFactory.CreateMavenVersion("1.0.0");

            Assert.Less(VersionFactory.CreateMavenVersion("0.0.1"), versionOne);
            Assert.Less(VersionFactory.CreateMavenVersion("1.0.0-SNAPSHOT"), versionOne);
            Assert.Less(VersionFactory.CreateMavenVersion("1.0.0-beta-2"), versionOne);
            Assert.Greater(VersionFactory.CreateMavenVersion("1.0.1"), versionOne);
            Assert.Greater(VersionFactory.CreateMavenVersion("1.0.0.1"), versionOne);
            Assert.Greater(VersionFactory.CreateMavenVersion("1.0.0.1-blah"), versionOne);
            Assert.AreEqual(VersionFactory.CreateMavenVersion("1.0.0"), versionOne);
        }

        [Test]
        public void TestBunchMoreComparisons()
        {
            Assert.Less(VersionFactory.CreateMavenVersion("1.0-beta1-SNAPSHOT"),
                VersionFactory.CreateMavenVersion("1.0-beta1"));
            Assert.Less(VersionFactory.CreateMavenVersion("1.0-beta1"),
                VersionFactory.CreateMavenVersion("1.0-beta2-SNAPSHOT"));
            Assert.Less(VersionFactory.CreateMavenVersion("1.0-beta2-SNAPSHOT"),
                VersionFactory.CreateMavenVersion("1.0-rc1-SNAPSHOT"));
            Assert.Less(VersionFactory.CreateMavenVersion("1.0-rc1-SNAPSHOT"),
                VersionFactory.CreateMavenVersion("1.0-rc1"));
            Assert.Less(VersionFactory.CreateMavenVersion("1.0-rc1"),
                VersionFactory.CreateMavenVersion("1.0-SNAPSHOT"));
            Assert.Less(VersionFactory.CreateMavenVersion("1.0-SNAPSHOT"),
                VersionFactory.CreateMavenVersion("1.0"));
            Assert.AreEqual(VersionFactory.CreateMavenVersion("1.0"),
                VersionFactory.CreateMavenVersion("1"));
            Assert.Less(VersionFactory.CreateMavenVersion("1.0"),
                VersionFactory.CreateMavenVersion("1.0-sp"));
            Assert.Less(VersionFactory.CreateMavenVersion("1.0-sp"),
                VersionFactory.CreateMavenVersion("1.0-whatever"));
            Assert.Less(VersionFactory.CreateMavenVersion("1.0-whatever"),
                VersionFactory.CreateMavenVersion("1.0.1"));
        }

        [Test]
        public void TesQualifierOnly()
        {
            Assert.Less(
                VersionFactory.CreateMavenVersion("version1"),
                VersionFactory.CreateMavenVersion("version2"));

            Assert.Less(
                VersionFactory.CreateMavenVersion("VERSION1"),
                VersionFactory.CreateMavenVersion("version2"));
        }

        [Test]
        public void TestEquivalents()
        {
            Assert.AreEqual(
                VersionFactory.CreateMavenVersion("1.0.0.a1"),
                VersionFactory.CreateMavenVersion("1.0.0.alpha1"));
            Assert.AreEqual(
                VersionFactory.CreateMavenVersion("1.0.0.b10"),
                VersionFactory.CreateMavenVersion("1.0.0.beta10"));
            Assert.AreEqual(
                VersionFactory.CreateMavenVersion("1.0.0.m3"),
                VersionFactory.CreateMavenVersion("1.0.0.milestone3"));
        }

        [Test]
        public void TestInvalidVersion()
        {
            Assert.Null(VersionFactory.TryCreateVersion("1.0.*", VersionFormat.Semver));
        }

        [Test]
        public void TestDockerTagComparison()
        {
            Assert.AreEqual(VersionFactory.CreateDockerTag("latest"), VersionFactory.CreateDockerTag("latest"));
        }
    }
}