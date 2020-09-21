using NUnit.Framework;
using Octopus.Versioning.Octopus;
using Octopus.Versioning.Semver;

namespace Octopus.Versioning.Tests.Versions.Octopus
{
    [TestFixture]
    public class OctopusVersionTests
    {
        static readonly OctopusVersionParser OctopusVersionParser = new OctopusVersionParser();
        static readonly SemVerFactory SemVerFactory = new SemVerFactory();

        [Test]
        [TestCase("0.0.4", 0, 0, 4, 0, "", "", "", "")]
        [TestCase("0.0.4.1", 0, 0, 4, 1, "", "", "", "")]
        [TestCase("1.2.3", 1, 2, 3, 0, "", "", "", "")]
        [TestCase("1.2.3.4", 1, 2, 3, 4, "", "", "", "")]
        [TestCase("10.20.30", 10, 20, 30, 0, "", "", "", "")]
        [TestCase("1.1.2-prerelease+meta", 1, 1, 2, 0, "prerelease", "prerelease", "", "meta")]
        [TestCase("1.1.2+meta", 1, 1, 2, 0, "", "", "", "meta")]
        [TestCase("1.1.2+meta-valid", 1, 1, 2, 0, "", "", "", "meta-valid")]
        [TestCase("1.0.0-alpha", 1, 0,0, 0, "alpha", "alpha", "", "")]
        [TestCase("1.0.0-alpha.beta", 1, 0,0, 0, "alpha.beta", "alpha", "beta", "")]
        [TestCase("1.0.0-alpha.beta1", 1, 0,0, 0, "alpha.beta1", "alpha", "beta1", "")]
        [TestCase("1.0.0-alpha.1", 1, 0,0, 0, "alpha.1", "alpha", "1", "")]
        [TestCase("1.0.0-alpha0.valid", 1, 0,0, 0, "alpha0.valid", "alpha0", "valid", "")]
        [TestCase("1.0.0-alpha.0valid", 1, 0,0, 0, "alpha.0valid", "alpha", "0valid", "")]
        [TestCase("1.0.0-alpha-a.b-c-somethinglong+build.1-aef.1-its-okay", 1, 0,0, 0, "alpha-a.b-c-somethinglong", "alpha", "a.b-c-somethinglong", "build.1-aef.1-its-okay")]
        [TestCase("1.0.0-rc.1+build.1", 1, 0,0, 0, "rc.1", "rc", "1", "build.1")]
        [TestCase("2.0.0-rc.1+build.123", 2, 0, 0, 0, "rc.1", "rc", "1", "build.123")]
        [TestCase("1.2.3-beta", 1, 2, 3, 0, "beta", "beta", "", "")]
        [TestCase("10.2.3-DEV-SNAPSHOT", 10, 2, 3, 0, "DEV-SNAPSHOT", "DEV", "SNAPSHOT", "")]
        [TestCase("1.2.3-SNAPSHOT-123", 1, 2, 3, 0, "SNAPSHOT-123", "SNAPSHOT", "123", "")]
        [TestCase("1.0.0", 1, 0, 0, 0, "", "", "", "")]
        [TestCase("2.0.0", 2, 0, 0, 0, "", "", "", "")]
        [TestCase("1.1.7", 1, 1, 7, 0, "", "", "", "")]
        [TestCase("2.0.0+build.1848", 2, 0, 0, 0, "", "", "", "build.1848")]
        [TestCase("2.0.1-alpha.1227", 2, 0, 1, 0, "alpha.1227", "alpha", "1227", "")]
        [TestCase("1.0.0-alpha+beta", 1, 0, 0, 0, "alpha", "alpha", "", "beta")]
        [TestCase("1.2.3----RC-SNAPSHOT.12.9.1--.12+788", 1, 2, 3, 0, "---RC-SNAPSHOT.12.9.1--.12", "", "--RC-SNAPSHOT.12.9.1--.12", "788")]
        [TestCase("1.2.3----R-S.12.9.1--.12+meta", 1, 2, 3, 0, "---R-S.12.9.1--.12", "", "--R-S.12.9.1--.12", "meta")]
        [TestCase("1.2.3----RC-SNAPSHOT.12.9.1--.12", 1, 2, 3, 0, "---RC-SNAPSHOT.12.9.1--.12", "", "--RC-SNAPSHOT.12.9.1--.12", "")]
        [TestCase("1.0.0+0.build.1-rc.10000aaa-kk-0.1", 1, 0, 0, 0, "", "", "", "0.build.1-rc.10000aaa-kk-0.1")]
        [TestCase("99999999.99999999.99999999", 99999999, 99999999, 99999999, 0, "", "", "", "")]
        [TestCase("0.0.0-foo", 0, 0, 0, 0, "foo", "foo", "", "")]
        [TestCase("0.0.0", 0, 0, 0, 0, "", "", "", "")]
        public void TestSemverVersions(string version, int major, int minor, int patch, int revision, string prerelease, string prereleasePrefix, string prereleaseCounter, string metadata)
        {
            var parsed = OctopusVersionParser.Parse(version);
            var semverParsed = SemVerFactory.Parse(version);

            Assert.AreEqual(major, parsed.Major);
            Assert.AreEqual(major, semverParsed.Major);
            Assert.AreEqual(minor, parsed.Minor);
            Assert.AreEqual(minor, semverParsed.Minor);
            Assert.AreEqual(patch, parsed.Patch);
            Assert.AreEqual(patch, semverParsed.Patch);
            Assert.AreEqual(revision, parsed.Revision);
            Assert.AreEqual(revision, semverParsed.Revision);
            Assert.AreEqual(prerelease, parsed.Release);
            Assert.AreEqual(prerelease, semverParsed.Release);
            Assert.AreEqual(metadata, parsed.Metadata);
            Assert.AreEqual(metadata, semverParsed.Metadata);

            Assert.AreEqual(prereleasePrefix, parsed.ReleasePrefix);
            Assert.AreEqual(prereleaseCounter, parsed.ReleaseCounter);

        }

        [Test]
        [TestCase("1.0.0-0A.is.legal", 1, 0, 0, 0, "0A.is.legal", "A.is.legal", "A", "is.legal", "")]
        public void TestIncompatibilitiesVersions(string version, int major, int minor, int patch, int revision, string semverPrerelease, string prerelease, string prereleasePrefix, string prereleaseCounter, string metadata)
        {
            var parsed = OctopusVersionParser.Parse(version);
            var semverParsed = SemVerFactory.Parse(version);

            Assert.AreEqual(major, parsed.Major);
            Assert.AreEqual(major, semverParsed.Major);
            Assert.AreEqual(minor, parsed.Minor);
            Assert.AreEqual(minor, semverParsed.Minor);
            Assert.AreEqual(patch, parsed.Patch);
            Assert.AreEqual(patch, semverParsed.Patch);
            Assert.AreEqual(revision, parsed.Revision);
            Assert.AreEqual(revision, semverParsed.Revision);
            Assert.AreEqual(prerelease, parsed.Release);
            Assert.AreEqual(semverPrerelease, semverParsed.Release);
            Assert.AreEqual(metadata, parsed.Metadata);
            Assert.AreEqual(metadata, semverParsed.Metadata);

            Assert.AreEqual(prereleasePrefix, parsed.ReleasePrefix);
            Assert.AreEqual(prereleaseCounter, parsed.ReleaseCounter);

        }

        [Test]
        [TestCase("1.0.0-beta.", 1, 0, 0, 0, "beta.", "beta", "", "")]
        [TestCase("v0.0.0", 0, 0, 0, 0, "", "", "", "")]
        [TestCase("V0.0.0", 0, 0, 0, 0, "", "", "", "")]
        [TestCase("v0.0.1", 0, 0, 1, 0, "", "", "", "")]
        [TestCase("V0.0.1", 0, 0, 1, 0, "", "", "", "")]
        [TestCase("v1.0.0", 1, 0, 0, 0, "", "", "", "")]
        [TestCase("v0.0.0-foo", 0, 0, 0, 0, "foo", "foo", "", "")]
        [TestCase("V0.0.0-foo", 0, 0, 0, 0, "foo", "foo", "", "")]
        public void TestInvalidSemverVersions(string version,
            int major,
            int minor,
            int patch,
            int revision,
            string prerelease,
            string prereleasePrefix,
            string prereleaseCounter,
            string metadata)
        {
            var parsed = OctopusVersionParser.Parse(version);

            Assert.AreEqual(major, parsed.Major);
            Assert.AreEqual(minor, parsed.Minor);
            Assert.AreEqual(patch, parsed.Patch);
            Assert.AreEqual(revision, parsed.Revision);
            Assert.AreEqual(prerelease, parsed.Release);
            Assert.AreEqual(metadata, parsed.Metadata);

            Assert.AreEqual(prereleasePrefix, parsed.ReleasePrefix);
            Assert.AreEqual(prereleaseCounter, parsed.ReleaseCounter);
        }
    }
}