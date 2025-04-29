using System;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Octopus.Versioning.Tests.Versions
{
    [TestFixture]
    public class VersionTests
    {
        [Test]
        public void TestMavenComparison()
        {
            var versionOne = VersionFactory.CreateMavenVersion("1.0.0");

            ClassicAssert.Less(VersionFactory.CreateMavenVersion("0.0.1"), versionOne);
            ClassicAssert.Less(VersionFactory.CreateMavenVersion("1.0.0-SNAPSHOT"), versionOne);
            ClassicAssert.Less(VersionFactory.CreateMavenVersion("1.0.0-beta-2"), versionOne);
            ClassicAssert.Greater(VersionFactory.CreateMavenVersion("1.0.1"), versionOne);
            ClassicAssert.Greater(VersionFactory.CreateMavenVersion("1.0.0.1"), versionOne);
            ClassicAssert.Greater(VersionFactory.CreateMavenVersion("1.0.0.1-blah"), versionOne);
            ClassicAssert.AreEqual(VersionFactory.CreateMavenVersion("1.0.0"), versionOne);
        }

        [Test]
        public void TestBunchMoreComparisons()
        {
            ClassicAssert.Less(VersionFactory.CreateMavenVersion("1.0-beta1-SNAPSHOT"),
                VersionFactory.CreateMavenVersion("1.0-beta1"));
            ClassicAssert.Less(VersionFactory.CreateMavenVersion("1.0-beta1"),
                VersionFactory.CreateMavenVersion("1.0-beta2-SNAPSHOT"));
            ClassicAssert.Less(VersionFactory.CreateMavenVersion("1.0-beta2-SNAPSHOT"),
                VersionFactory.CreateMavenVersion("1.0-rc1-SNAPSHOT"));
            ClassicAssert.Less(VersionFactory.CreateMavenVersion("1.0-rc1-SNAPSHOT"),
                VersionFactory.CreateMavenVersion("1.0-rc1"));
            ClassicAssert.Less(VersionFactory.CreateMavenVersion("1.0-rc1"),
                VersionFactory.CreateMavenVersion("1.0-SNAPSHOT"));
            ClassicAssert.Less(VersionFactory.CreateMavenVersion("1.0-SNAPSHOT"),
                VersionFactory.CreateMavenVersion("1.0"));
            ClassicAssert.AreEqual(VersionFactory.CreateMavenVersion("1.0"),
                VersionFactory.CreateMavenVersion("1"));
            ClassicAssert.Less(VersionFactory.CreateMavenVersion("1.0"),
                VersionFactory.CreateMavenVersion("1.0-sp"));
            ClassicAssert.Less(VersionFactory.CreateMavenVersion("1.0-sp"),
                VersionFactory.CreateMavenVersion("1.0-whatever"));
            ClassicAssert.Less(VersionFactory.CreateMavenVersion("1.0-whatever"),
                VersionFactory.CreateMavenVersion("1.0.1"));
        }

        [Test]
        public void TesQualifierOnly()
        {
            ClassicAssert.Less(
                VersionFactory.CreateMavenVersion("version1"),
                VersionFactory.CreateMavenVersion("version2"));

            ClassicAssert.Less(
                VersionFactory.CreateMavenVersion("VERSION1"),
                VersionFactory.CreateMavenVersion("version2"));
        }

        [Test]
        public void TestEquivalents()
        {
            ClassicAssert.AreEqual(
                VersionFactory.CreateMavenVersion("1.0.0.a1"),
                VersionFactory.CreateMavenVersion("1.0.0.alpha1"));
            ClassicAssert.AreEqual(
                VersionFactory.CreateMavenVersion("1.0.0.b10"),
                VersionFactory.CreateMavenVersion("1.0.0.beta10"));
            ClassicAssert.AreEqual(
                VersionFactory.CreateMavenVersion("1.0.0.m3"),
                VersionFactory.CreateMavenVersion("1.0.0.milestone3"));
            ClassicAssert.AreEqual(
                VersionFactory.CreateMavenVersion("1.0.0"),
                VersionFactory.CreateMavenVersion("1.0.0.0"),
                "Another `.0` after patch isn't counted");
        }

        [Test]
        public void TestSemVerEquivalents()
        {
            ClassicAssert.AreEqual(
                VersionFactory.CreateSemanticVersion("1.0.0"),
                VersionFactory.CreateSemanticVersion("1.0.0.0"),
                "Another `.0` after patch isn't counted");
        }

        [Test]
        public void TestSingleDigit()
        {
            ClassicAssert.AreEqual(
                VersionFactory.CreateSemanticVersion("0001"),
                VersionFactory.CreateSemanticVersion("1.0.0.0"));
            ClassicAssert.AreEqual(
                VersionFactory.CreateSemanticVersion("0001"),
                VersionFactory.CreateSemanticVersion("1.0"));
            ClassicAssert.AreEqual(
                VersionFactory.CreateSemanticVersion("0001"),
                VersionFactory.CreateSemanticVersion("1"));
            ClassicAssert.AreEqual(
                VersionFactory.CreateSemanticVersion("1"),
                VersionFactory.CreateSemanticVersion("1.0.0.0"));
        }

        [Test]
        public void TestInvalidVersion()
        {
            ClassicAssert.Null(VersionFactory.TryCreateVersion("1.0.*", VersionFormat.Semver));
        }

        [Test]
        public void TestDockerTagComparison()
        {
            ClassicAssert.AreEqual(VersionFactory.CreateDockerTag("latest"), VersionFactory.CreateDockerTag("latest"));
        }
    }
}