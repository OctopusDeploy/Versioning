﻿using System;
using System.Linq;
using NUnit.Framework;
using Octopus.Versioning.Maven;
using Octopus.Versioning.Octopus;
using Octopus.Versioning.Semver;

namespace Octopus.Versioning.Tests.Octopus
{
    [TestFixture]
    public class OctopusVersionTests
    {
        static readonly Random Random = new Random();
        static readonly OctopusVersionParser OctopusVersionParser = new OctopusVersionParser();

        [Test]
        public void TryParseTest()
        {
            Assert.IsFalse(OctopusVersionParser.TryParse("99999999999999999999999999999999999", out var version));
            Assert.IsTrue(OctopusVersionParser.TryParse("1.1.1.1", out var version2));
        }

        [Test]
        [TestCase("0.0.4",
            0,
            0,
            4,
            0,
            "",
            "",
            "",
            "")]
        [TestCase("0.0.4.1",
            0,
            0,
            4,
            1,
            "",
            "",
            "",
            "")]
        [TestCase("1.2.3",
            1,
            2,
            3,
            0,
            "",
            "",
            "",
            "")]
        [TestCase("1.2.3.4",
            1,
            2,
            3,
            4,
            "",
            "",
            "",
            "")]
        [TestCase("10.20.30",
            10,
            20,
            30,
            0,
            "",
            "",
            "",
            "")]
        [TestCase("v0.0.0",
            0,
            0,
            0,
            0,
            "",
            "",
            "",
            "")]
        [TestCase("v0.0.1",
            0,
            0,
            1,
            0,
            "",
            "",
            "",
            "")]
        [TestCase("v1.0.0",
            1,
            0,
            0,
            0,
            "",
            "",
            "",
            "")]
        [TestCase("v0.0.0-foo",
            0,
            0,
            0,
            0,
            "foo",
            "foo",
            "",
            "")]
        [TestCase("1.1.2-prerelease+meta",
            1,
            1,
            2,
            0,
            "prerelease",
            "prerelease",
            "",
            "meta")]
        [TestCase("1.1.2+meta",
            1,
            1,
            2,
            0,
            "",
            "",
            "",
            "meta")]
        [TestCase("1.1.2+meta-valid",
            1,
            1,
            2,
            0,
            "",
            "",
            "",
            "meta-valid")]
        [TestCase("1.0.0-alpha",
            1,
            0,
            0,
            0,
            "alpha",
            "alpha",
            "",
            "")]
        [TestCase("1.0.0-alpha.beta",
            1,
            0,
            0,
            0,
            "alpha.beta",
            "alpha",
            "beta",
            "")]
        [TestCase("1.0.0-alpha.beta1",
            1,
            0,
            0,
            0,
            "alpha.beta1",
            "alpha",
            "beta1",
            "")]
        [TestCase("1.0.0-alpha.1",
            1,
            0,
            0,
            0,
            "alpha.1",
            "alpha",
            "1",
            "")]
        [TestCase("1.0.0-alpha0.valid",
            1,
            0,
            0,
            0,
            "alpha0.valid",
            "alpha0",
            "valid",
            "")]
        [TestCase("1.0.0-alpha.0valid",
            1,
            0,
            0,
            0,
            "alpha.0valid",
            "alpha",
            "0valid",
            "")]
        [TestCase("1.0.0-alpha-a.b-c-somethinglong+build.1-aef.1-its-okay",
            1,
            0,
            0,
            0,
            "alpha-a.b-c-somethinglong",
            "alpha",
            "a.b-c-somethinglong",
            "build.1-aef.1-its-okay")]
        [TestCase("1.0.0-rc.1+build.1",
            1,
            0,
            0,
            0,
            "rc.1",
            "rc",
            "1",
            "build.1")]
        [TestCase("2.0.0-rc.1+build.123",
            2,
            0,
            0,
            0,
            "rc.1",
            "rc",
            "1",
            "build.123")]
        [TestCase("1.2.3-beta",
            1,
            2,
            3,
            0,
            "beta",
            "beta",
            "",
            "")]
        [TestCase("10.2.3-DEV-SNAPSHOT",
            10,
            2,
            3,
            0,
            "DEV-SNAPSHOT",
            "DEV",
            "SNAPSHOT",
            "")]
        [TestCase("1.2.3-SNAPSHOT-123",
            1,
            2,
            3,
            0,
            "SNAPSHOT-123",
            "SNAPSHOT",
            "123",
            "")]
        [TestCase("1.0.0",
            1,
            0,
            0,
            0,
            "",
            "",
            "",
            "")]
        [TestCase("2.0.0",
            2,
            0,
            0,
            0,
            "",
            "",
            "",
            "")]
        [TestCase("1.1.7",
            1,
            1,
            7,
            0,
            "",
            "",
            "",
            "")]
        [TestCase("2.0.0+build.1848",
            2,
            0,
            0,
            0,
            "",
            "",
            "",
            "build.1848")]
        [TestCase("2.0.1-alpha.1227",
            2,
            0,
            1,
            0,
            "alpha.1227",
            "alpha",
            "1227",
            "")]
        [TestCase("1.0.0-alpha+beta",
            1,
            0,
            0,
            0,
            "alpha",
            "alpha",
            "",
            "beta")]
        [TestCase("1.2.3----RC-SNAPSHOT.12.9.1--.12+788",
            1,
            2,
            3,
            0,
            "---RC-SNAPSHOT.12.9.1--.12",
            "",
            "--RC-SNAPSHOT.12.9.1--.12",
            "788")]
        [TestCase("1.2.3----R-S.12.9.1--.12+meta",
            1,
            2,
            3,
            0,
            "---R-S.12.9.1--.12",
            "",
            "--R-S.12.9.1--.12",
            "meta")]
        [TestCase("1.2.3----RC-SNAPSHOT.12.9.1--.12",
            1,
            2,
            3,
            0,
            "---RC-SNAPSHOT.12.9.1--.12",
            "",
            "--RC-SNAPSHOT.12.9.1--.12",
            "")]
        [TestCase("1.0.0+0.build.1-rc.10000aaa-kk-0.1",
            1,
            0,
            0,
            0,
            "",
            "",
            "",
            "0.build.1-rc.10000aaa-kk-0.1")]
        [TestCase("99999999.99999999.99999999",
            99999999,
            99999999,
            99999999,
            0,
            "",
            "",
            "",
            "")]
        [TestCase("0.0.0-foo",
            0,
            0,
            0,
            0,
            "foo",
            "foo",
            "",
            "")]
        [TestCase("0.0.0",
            0,
            0,
            0,
            0,
            "",
            "",
            "",
            "")]
        [TestCase("1.2",
            1,
            2,
            0,
            0,
            "",
            "",
            "",
            "")]
        [TestCase("1",
            1,
            0,
            0,
            0,
            "",
            "",
            "",
            "")]
        [TestCase("01.1.1",
            1,
            1,
            1,
            0,
            "",
            "",
            "",
            "")]
        [TestCase("1.01.1",
            1,
            1,
            1,
            0,
            "",
            "",
            "",
            "")]
        [TestCase("1.1.01",
            1,
            1,
            1,
            0,
            "",
            "",
            "",
            "")]
        [TestCase("1.1.01-9999999999999999999999999",
            1,
            1,
            1,
            0,
            "9999999999999999999999999",
            "9999999999999999999999999",
            "",
            "")]
        [TestCase("1.1.01-9999999999999999999999999+meta ",
            1,
            1,
            1,
            0,
            "9999999999999999999999999",
            "9999999999999999999999999",
            "",
            "meta",
            Description = "Test that trailing whitespace after metadata is treated the same way in SemVerFactory and OctopusVersionParser")]
        [TestCase("1.1.01-9999999999999999999999999 ",
            1,
            1,
            1,
            0,
            "9999999999999999999999999",
            "9999999999999999999999999",
            "",
            "",
            Description = "Test that trailing whitespace is treated the same way in SemVerFactory and OctopusVersionParser")]
        [TestCase(" 1.1.01-9999999999999999999999999 ",
            1,
            1,
            1,
            0,
            "9999999999999999999999999",
            "9999999999999999999999999",
            "",
            "",
            Description = "Test that leading whitespace is treated the same way in SemVerFactory and OctopusVersionParser")]
        [TestCase(" 1 .1.01-9999999999999999999999999",
            1,
            1,
            1,
            0,
            "9999999999999999999999999",
            "9999999999999999999999999",
            "",
            "",
            Description = "Test that whitespace around the major integer is treated the same way in SemVerFactory and OctopusVersionParser")]
        [TestCase("1. 1 .01-9999999999999999999999999",
            1,
            1,
            1,
            0,
            "9999999999999999999999999",
            "9999999999999999999999999",
            "",
            "",
            Description = "Test that whitespace around the minor integer is treated the same way in SemVerFactory and OctopusVersionParser")]
        [TestCase("1.1. 01 -9999999999999999999999999",
            1,
            1,
            1,
            0,
            "9999999999999999999999999",
            "9999999999999999999999999",
            "",
            "",
            Description = "Test that whitespace around the patch integer is treated the same way in SemVerFactory and OctopusVersionParser")]
        [TestCase("1.1.01. 1 -9999999999999999999999999",
            1,
            1,
            1,
            1,
            "9999999999999999999999999",
            "9999999999999999999999999",
            "",
            "",
            Description = "Test that whitespace around the release integer is treated the same way in SemVerFactory and OctopusVersionParser")]
        public void TestSemverVersions(string version,
            int major,
            int minor,
            int patch,
            int revision,
            string prerelease,
            string prereleasePrefix,
            string prereleaseCounter,
            string metadata)
        {
            var semverParsed = SemVerFactory.Parse(version);
            var parsed = OctopusVersionParser.Parse(version);

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
        [TestCase("NotAVersionSting",
            0,
            0,
            0,
            0,
            "NotAVersionSting",
            "NotAVersionSting",
            "",
            "")]
        [TestCase("1.0-alpha",
            1,
            0,
            0,
            0,
            "alpha",
            "alpha",
            "",
            "")]
        [TestCase("1.0-alpha1",
            1,
            0,
            0,
            0,
            "alpha1",
            "alpha1",
            "",
            "")]
        [TestCase("1.0-a1",
            1,
            0,
            0,
            0,
            "a1",
            "a1",
            "",
            "")]
        [TestCase("1.0-beta",
            1,
            0,
            0,
            0,
            "beta",
            "beta",
            "",
            "")]
        [TestCase("1.0-beta1",
            1,
            0,
            0,
            0,
            "beta1",
            "beta1",
            "",
            "")]
        [TestCase("1.0-b1",
            1,
            0,
            0,
            0,
            "b1",
            "b1",
            "",
            "")]
        [TestCase("1.0-whatever",
            1,
            0,
            0,
            0,
            "whatever",
            "whatever",
            "",
            "")]
        [TestCase("1.0-SNAPSHOT",
            1,
            0,
            0,
            0,
            "SNAPSHOT",
            "SNAPSHOT",
            "",
            "")]
        [TestCase("1.0-cr1",
            1,
            0,
            0,
            0,
            "cr1",
            "cr1",
            "",
            "")]
        [TestCase("1.0-cr",
            1,
            0,
            0,
            0,
            "cr",
            "cr",
            "",
            "")]
        [TestCase("1.0-rc",
            1,
            0,
            0,
            0,
            "rc",
            "rc",
            "",
            "")]
        [TestCase("1.0-rc1",
            1,
            0,
            0,
            0,
            "rc1",
            "rc1",
            "",
            "")]
        [TestCase("19.0.0.Final",
            19,
            0,
            0,
            0,
            "Final",
            "Final",
            "",
            "",
            Description = "https://hub.docker.com/r/jboss/wildfly/tags")]
        public void TestMavenVersions(string version,
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
            var mavenParsed = new MavenVersionParser().Parse(version);

            Assert.AreEqual(major, parsed.Major);
            Assert.AreEqual(major, mavenParsed.Major);
            Assert.AreEqual(minor, parsed.Minor);
            Assert.AreEqual(minor, mavenParsed.Minor);
            Assert.AreEqual(patch, parsed.Patch);
            Assert.AreEqual(patch, mavenParsed.Patch);
            Assert.AreEqual(revision, parsed.Revision);
            Assert.AreEqual(revision, mavenParsed.Revision);
            Assert.AreEqual(prerelease, parsed.Release);
            Assert.AreEqual(prerelease, mavenParsed.Release);
            Assert.AreEqual(metadata, parsed.Metadata);
            Assert.AreEqual(metadata, mavenParsed.Metadata ?? string.Empty);

            Assert.AreEqual(prereleasePrefix, parsed.ReleasePrefix);
            Assert.AreEqual(prereleaseCounter, parsed.ReleaseCounter);
        }

        /// <summary>
        /// This test identifies examples where valid maven is processed differently from the octopus version
        /// </summary>
        [Test]
        [TestCase("1-0-alpha",
            1,
            0,
            0,
            0,
            0,
            0,
            0,
            "-alpha",
            "0-alpha",
            "0",
            "alpha",
            "")]
        [TestCase("1_0_alpha",
            1,
            0,
            0,
            0,
            0,
            0,
            0,
            "",
            "0_alpha",
            "0",
            "alpha",
            "")]
        [TestCase("1_0alpha",
            1,
            0,
            0,
            0,
            0,
            0,
            0,
            "",
            "0alpha",
            "0alpha",
            "",
            "")]
        [TestCase("1.0a1-SNAPSHOT",
            1,
            0,
            0,
            0,
            0,
            0,
            0,
            "",
            "a1-SNAPSHOT",
            "a1",
            "SNAPSHOT",
            "")]
        [TestCase("1-2.3-SNAPSHOT",
            1,
            0,
            0,
            0,
            0,
            2,
            0,
            ".3-SNAPSHOT",
            "2.3-SNAPSHOT",
            "2",
            "3-SNAPSHOT",
            "")]
        [TestCase("1.0.2.3-rc1",
            1,
            0,
            0,
            2,
            2,
            0,
            3,
            "3-rc1",
            "rc1",
            "rc1",
            "",
            "")]
        public void TestIncompatibleMavenVersions(string version,
            int major,
            int mavenMinor,
            int minor,
            int mavenPatch,
            int patch,
            int mavenRevision,
            int revision,
            string mavenPrerelease,
            string prerelease,
            string prereleasePrefix,
            string prereleaseCounter,
            string metadata)
        {
            var parsed = OctopusVersionParser.Parse(version);
            var mavenParsed = new MavenVersionParser().Parse(version);

            Assert.AreEqual(major, parsed.Major);
            Assert.AreEqual(major, mavenParsed.Major);
            Assert.AreEqual(minor, parsed.Minor);
            Assert.AreEqual(mavenMinor, mavenParsed.Minor);
            Assert.AreEqual(patch, parsed.Patch);
            Assert.AreEqual(mavenPatch, mavenParsed.Patch);
            Assert.AreEqual(revision, parsed.Revision);
            Assert.AreEqual(mavenRevision, mavenParsed.Revision);
            Assert.AreEqual(prerelease, parsed.Release);
            Assert.AreEqual(mavenPrerelease, mavenParsed.Release);
            Assert.AreEqual(metadata, parsed.Metadata);
            Assert.AreEqual(metadata, mavenParsed.Metadata ?? string.Empty);

            Assert.AreEqual(prereleasePrefix, parsed.ReleasePrefix);
            Assert.AreEqual(prereleaseCounter, parsed.ReleaseCounter);
        }

        /// <summary>
        /// Tests of versions that are invalid semver
        /// </summary>
        [Test]
        [TestCase("1.0.0-beta.",
            1,
            0,
            0,
            0,
            "beta.",
            "beta",
            "",
            "")]
        [TestCase("V0.0.0",
            0,
            0,
            0,
            0,
            "",
            "",
            "",
            "")]
        [TestCase("V0.0.1",
            0,
            0,
            1,
            0,
            "",
            "",
            "",
            "")]
        [TestCase("V0.0.0-foo",
            0,
            0,
            0,
            0,
            "foo",
            "foo",
            "",
            "")]
        [TestCase("1.1.2+.123",
            1,
            1,
            2,
            0,
            "",
            "",
            "",
            ".123")]
        [TestCase("+invalid",
            0,
            0,
            0,
            0,
            "",
            "",
            "",
            "invalid")]
        [TestCase("-invalid",
            0,
            0,
            0,
            0,
            "invalid",
            "invalid",
            "",
            "")]
        [TestCase("-invalid+invalid",
            0,
            0,
            0,
            0,
            "invalid",
            "invalid",
            "",
            "invalid")]
        [TestCase("-invalid.01",
            0,
            0,
            0,
            0,
            "invalid.01",
            "invalid",
            "01",
            "")]
        [TestCase("alpha..",
            0,
            0,
            0,
            0,
            "alpha..",
            "alpha",
            ".",
            "")]
        [TestCase("1.0.0-alpha.......1",
            1,
            0,
            0,
            0,
            "alpha.......1",
            "alpha",
            "......1",
            "")]
        [TestCase("19.0.0.Final",
            19,
            0,
            0,
            0,
            "Final",
            "Final",
            "",
            "",
            Description = "https://hub.docker.com/r/jboss/wildfly/tags")]
        [TestCase("284.0.0-debian_component_based",
            284,
            0,
            0,
            0,
            "debian_component_based",
            "debian",
            "component_based",
            "",
            Description = "https://hub.docker.com/r/google/cloud-sdk/tags")]
        [TestCase("latest",
            0,
            0,
            0,
            0,
            "latest",
            "latest",
            "",
            "")]
        [TestCase("stable",
            0,
            0,
            0,
            0,
            "stable",
            "stable",
            "",
            "")]
        [TestCase("stable-\\hi_there.how-are+you+today",
            0,
            0,
            0,
            0,
            "stable-\\hi_there.how-are",
            "stable",
            "\\hi_there.how-are",
            "you+today")]
        [TestCase(" stable-\\hi_there.how-are+you+today ",
            0,
            0,
            0,
            0,
            "stable-\\hi_there.how-are",
            "stable",
            "\\hi_there.how-are",
            "you+today")]
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

            Assert.IsNull(SemVerFactory.TryCreateVersion(version));
        }

        [TestCase("stable", false)]
        [TestCase("1.0.0", false)]
        [TestCase("1.0.0-something", false)]
        [TestCase("1.0.0-something+meta", true)]
        [TestCase("1.0.0-something+", false)]
        public void TestHasMetadata(string version, bool hasMetadata)
        {
            var parsed = OctopusVersionParser.Parse(version);
            Assert.AreEqual(hasMetadata, parsed.HasMetadata);
        }

        /// <summary>
        /// All strings should parse to something
        /// </summary>
        [Test]
        [Repeat(1000)]
        public void RandomStringTest()
        {
            try
            {
                var version = RandomString(20);
                var parsed = OctopusVersionParser.Parse(version);

                var hasMajor = parsed.Major != 0;
                var hasMinor = parsed.Minor != 0;
                var hasPatch = parsed.Patch != 0;
                var hasRevision = parsed.Revision != 0;
                var hasRelease = !string.IsNullOrEmpty(parsed.Release);
                var hasMetadata = !string.IsNullOrEmpty(parsed.Metadata);

                Assert.IsTrue(hasMajor || hasMinor || hasPatch || hasRevision || hasRelease || hasMetadata);
            }
            catch (OverflowException)
            {
                // There is a small chance we created a random string with version fields larger than an int, but this is ok.
            }
        }

        [Test]
        [TestCase("2147483648.1.1")]
        [TestCase("1.2147483648.1")]
        [TestCase("1.1.2147483648")]
        [TestCase("1.1.1.2147483648")]
        [TestCase("1.1.9999999999")]
        public void LargeVersionNumbersWillFail(string version)
        {
            try
            {
                OctopusVersionParser.Parse(version);
                Assert.Fail("Should have thrown an exception");
            }
            catch (OverflowException)
            {
                Assert.Pass("Exception was expected");
            }
        }

        [Test]
        [TestCase("1.2.3-hi'there")]
        [TestCase("1.2.3-hi\"there")]
        [TestCase("1.2.3-hi;there")]
        [TestCase("1.2.3-hi\nthere")]
        [TestCase("1.2.3-hi\tthere")]
        [TestCase("1.2.3-hi/there")]
        [TestCase(" 1.2.3-hi/there ")]
        [TestCase("1.2.3-hi%there")]
        [TestCase(" 1.2.3-hi%there ")]
        [TestCase("1.2.3-hi?there")]
        [TestCase(" 1.2.3-hi?there ")]
        [TestCase("1.2.3-hi#there")]
        [TestCase(" 1.2.3-hi#there ")]
        [TestCase("1.2.3-hi&there")]
        [TestCase("1.2.3-hi there")]
        [TestCase("1.2.3-hithere +meta", Description = "Ensure prerelease spaces are treated the same way in OctopusVersionParser and SemVerFactory")]
        [TestCase("1.2.3- hithere+meta", Description = "Ensure prerelease spaces are treated the same way in OctopusVersionParser and SemVerFactory")]
        [TestCase("1.2.3-hithere+ meta", Description = "Ensure metadata spaces are treated the same way in OctopusVersionParser and SemVerFactory")]
        [TestCase(" ")]
        [TestCase("")]
        [TestCase(null)]
        public void IllegalCharsWillFail(string version)
        {
            var octoSuccess = OctopusVersionParser.TryParse(version, out _);
            var semanticVersion = SemVerFactory.TryCreateVersion(version);

            if (octoSuccess || semanticVersion != null)
                Assert.Fail("Should have thrown an exception");
        }

        /// <summary>
        /// This test ensures the OriginalString value is the same whether parsed by OctopusVersionParser or
        /// SemVerFactory
        /// </summary>
        /// <param name="version">The version to parse</param>
        [Test]
        [TestCase("1.2.3-hithere")]
        [TestCase(" 1.2.3-hithere")]
        [TestCase("1.2.3-hithere ")]
        [TestCase(" 1.2.3-hithere ")]
        [TestCase("1 .2.3-hithere")]
        [TestCase("1. 2 .3-hithere")]
        [TestCase("1.2. 3 -hithere")]
        [TestCase("1.2.3. 1 -hithere")]
        public void TestOriginalStringIsTheSame(string version)
        {
            OctopusVersionParser.TryParse(version, out var octoVersion);
            var semanticVersion = SemVerFactory.TryCreateVersion(version);

            Assert.AreEqual(octoVersion.OriginalString, semanticVersion.OriginalString);
        }

        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-.\\+";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)])
                .ToArray());
        }
        
        [Test]
        [TestCase("1_2_3", "1_2_4")]
        [TestCase("01_2_3", "1_2_4")]
        [TestCase("1_02_3", "1_2_4")]
        [TestCase("1_02_03", "1_2_4")]
        [TestCase("1_2_03", "1_2_4")]
        public void TestUnderscores(string version, string version2)
        {
            OctopusVersionParser.TryParse(version, out var octoVersion);
            OctopusVersionParser.TryParse(version2, out var octoVersion2);

            Assert.Greater(octoVersion2, octoVersion);
        }
    }
}