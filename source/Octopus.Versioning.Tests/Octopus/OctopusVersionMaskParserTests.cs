using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Octopus.Versioning.Octopus;
using Octopus.Versioning.Semver;
using Octopus.Versioning.Tests.PreviousImplementation;

namespace Octopus.Versioning.Tests.Octopus
{
    [TestFixture]
    public class OctopusVersionMaskParserTests
    {
        static readonly OctopusVersionMaskParser OctopusVersionMaskParser = new OctopusVersionMaskParser();

        /// <summary>
        /// Each mask supports these prefixes with the new implementation
        /// </summary>
        static readonly string[] VersionPrefixes = { "", "V", "v" };

        /// <summary>
        /// Tests here, with the exception of masks that start with a 'v' or 'V' and initial versions that are not valid semver,
        /// should also pass in the previous implementation that has been copied into the SemanticVersionMask class in this test
        /// project.
        /// </summary>
        [Test]
        [TestCase("1.2.3-alpha.i", null, "1.2.3-alpha.0", Description = "An increment of the prerelease counter.")]
        [TestCase("1.2.3-alpha.i", "1.2.3-alpha.25", "1.2.3-alpha.26", Description = "An increment of the prerelease counter.")]
        [TestCase("i.2.3-alpha.i", "1.2.3-alpha.25", "2.2.3-alpha.26", Description = "An increment of the major and prerelease counter.")]
        [TestCase("1.i.3-alpha.i", "1.2.3-alpha.25", "1.3.3-alpha.26", Description = "An increment of the minor and prerelease counter.")]
        [TestCase("1.2.i-alpha.i", "1.2.3-alpha.25", "1.2.4-alpha.26", Description = "An increment of the patch and prerelease counter.")]
        [TestCase("c.2.3-alpha.i", "1.2.3-alpha.25", "1.2.3-alpha.26", Description = "Use the current major and increment the prerelease counter.")]
        [TestCase("1.c.3-alpha.i", "1.2.3-alpha.25", "1.2.3-alpha.26", Description = "Use the current minor and increment the prerelease counter.")]
        [TestCase("1.2.c-alpha.i", "1.2.3-alpha.25", "1.2.3-alpha.26", Description = "Use the current patch and increment the prerelease counter.")]
        [TestCase("i.i.i-alpha.i", "1.2.3-alpha.25", "2.0.0-alpha.26", Description = "This particular example is not what I would expect. However this tests passed with the old logic, and so we have it pass here too.")]
        [TestCase("c.c.i-alpha.c", "1.2.3-alpha.25", "1.2.4-alpha.25", Description = "Use the current major and minor, increment the patch, and keep the current prerelease counter.")]
        [TestCase("i.c.c", "1.2.3-alpha.25", "2.0.3", Description = "Increment the major, keep the current minor and patch. This may not be an obvious result, but is consistent with the existing logic.")]
        [TestCase("c.i.c", "1.2.3-alpha.25", "1.3.0", Description = "Keep the current major and patch, and increment the minor. This may not be an obvious result, but is consistent with the existing logic.")]
        [TestCase("c.c.i", "1.2.3-alpha.25", "1.2.4", Description = "Keep the current major and minor, and increment the patch.")]
        [TestCase("c.c.c-test", "alpha.25", "0.0.0-test", Description = "Existing versions with only plain text are assumed to be 0.0.0.")]
        [TestCase("c.c.c-test", "0.0.0.0-alpha.25", "0.0.0.0-test", Description = "Same as the tests above, but explicitly setting the version to 0.0.0.0.")]
        [TestCase("i.c.c-test", "alpha.25", "1.0.0-test", Description = "Existing versions with only plain text are assumed to be 0.0.0.")]
        [TestCase("i.c.c-test", "0.0.0.0-alpha.25", "1.0.0.0-test", Description = "Same as the tests above, but explicitly setting the version to 0.0.0.0.")]
        [TestCase("c.c.i-test", "alpha.25", "0.0.1-test", Description = "Existing versions with only plain text are assumed to be 0.0.0.")]
        [TestCase("c.c.i-test", "0.0.0.0-alpha.25", "0.0.1.0-test", Description = "Same as the tests above, but explicitly setting the version to 0.0.0.0.")]
        [TestCase("0.1.0-initial", "whatever", "0.1.0-initial", Description = "Existing versions with only plain text are assumed to be 0.0.0.")]
        [TestCase("c.c.i-initial", "whatever", "0.0.1-initial", Description = "Existing versions with only plain text are assumed to be 0.0.0.")]
        [TestCase("c.c.i-current", "whatever", "0.0.1-current", Description = "Existing versions with only plain text are assumed to be 0.0.0.")]
        [TestCase("c.c.c.i-current", "whatever", "0.0.0.1-current", Description = "Existing versions with only plain text are assumed to be 0.0.0.0.")]
        [TestCase("c.c.c.i-initial.1", "whatever", "0.0.0.1-initial.1", Description = "Existing versions with only plain text are assumed to be 0.0.0.0.")]
        [TestCase("c.c.c.i-initial.c", "whatever", "0.0.0.1-initial.0", Description = "Existing versions with only plain text are assumed to be 0.0.0.0.")]
        [TestCase("c.c.c.i-initial.i", "whatever", "0.0.0.1-initial.1", Description = "Existing versions with only plain text are assumed to be 0.0.0.0.")]
        [TestCase("c.c.c-i", "whatever", "0.0.0-1", Description = "Increment the prerelease")]
        [TestCase("c.c.c-c", "whatever", "0.0.0-0", Description = "Retain the prerelease (assumed to be 0 here)")]
        [TestCase("c.c.c_i", "whatever", "c.c.c_i", Description = "Not a valid mask, so the version is passed through.")]
        [TestCase("c.c.c_c", "whatever", "c.c.c_c", Description = "Not a valid mask, so the version is passed through.")]
        [TestCase("c.c.c.i", "whatever", "0.0.0.1", Description = "Existing versions with only plain text are assumed to be 0.0.0.0.")]
        [TestCase("c.c.c.c", "whatever", "0.0.0.0", Description = "Existing versions with only plain text are assumed to be 0.0.0.0.")]
        [TestCase("1.2.3-i", "whatever", "1.2.3-1")]
        [TestCase("1.2.3-c", "whatever", "1.2.3-0")]
        [TestCase("1.2.3_i", "whatever", "1.2.3_i", Description = "Not a valid mask, so the version is passed through.")]
        [TestCase("1.2.3_c", "whatever", "1.2.3_c", Description = "Not a valid mask, so the version is passed through.")]
        [TestCase("1.2.3.i", "whatever", "1.2.3.1", Description = "Existing versions with only plain text are assumed to be 0.0.0.0.")]
        [TestCase("1.2.3.c", "whatever", "1.2.3.0", Description = "Existing versions with only plain text are assumed to be 0.0.0.0.")]
        [TestCase("1.2.3-initial.8", "1.0.0.0", "1.2.3.0-initial.8")]
        [TestCase("2021.4.0-0.121.27.202110617.1183ec5i", "2021.4.0.0-121.50.210609", "2021.4.0.0-0.121.27.202110617.1183ec5i", Description = "A specific test case documented in https://github.com/OctopusDeploy/Issues/issues/6926. Note the resulting version gains the revision of 0 because the previous version had a revision defined.")]
        [TestCase("2021.4.0-0.121.27.202110617.1183ec5i", "2021.4.0-121.50.210609", "2021.4.0-0.121.27.202110617.1183ec5i", Description = "A specific test case documented in https://github.com/OctopusDeploy/Issues/issues/6926")]
        [TestCase("c.c.i", "1.2.3.4", "1.2.4.0", Description = "The resulting version gets the a release of 0 from the latest version.")]
        [TestCase("1.2.3-hi.i", "1.1.1.1", "1.2.3.0-hi.1", Description = "The resulting version gets the a release of 0 from the latest version.")]
        [TestCase("1.2.3-i", "1.2.3-25", "1.2.3-26")]
        [TestCase("1.2.3.4-i.1", "1.2.3.4-25.2", "1.2.3.4-26.1")]
        [TestCase("1.2.3.4-c.1", "1.2.3.4-25.2", "1.2.3.4-25.1")]
        [TestCase("1.2.3.4-1.blah.i.0", "1.2.3.4-whatever.3.1", "1.2.3.4-1.blah.2.0")]
        [TestCase("1.2.3-whatever", "1.2.2.2", "1.2.3.0-whatever", Description = "The existing implementation treats any version that can be parsed by the regex defining a mask as a mask. This means versions with no mask characters are still processed as a mask. One implication of this is if the latest version has a revision, the new version also has a revision, even if it wasn't specified.")]
        [TestCase("1.2.3-i", "1. 2. 3-4", "1.2.3-5", Description = "The existing version parsing tolerated spaces inside versions")]
        [TestCase("1.2.3-i ", "1.2.3-4", "1.2.3-i", Description = "Whitespace prevents a string from being a mask")]
        [TestCase(" 1.2.3-i ", "1.2.3-4", "1.2.3-i", Description = "Whitespace prevents a string from being a mask")]
        [TestCase("1. 2.3-i", "1.2.3-4", "1.2.3-i", Description = "Whitespace prevents a string from being a mask")]
        [TestCase("1.2. 3-i", "1.2.3-4", "1.2.3-i", Description = "Whitespace prevents a string from being a mask")]
        [TestCase("1.2.3 -i", "1.2.3-4", "1.2.3-i", Description = "Whitespace prevents a string from being a mask")]
        [TestCase("c.c.c-i", "1.2.3-4", "1.2.3-5")]
        [TestCase("c.c.c-i", "1.2.3", "1.2.3-1")]
        [TestCase("0.0.1-i", "0.0.1", "0.0.1-1")]
        [TestCase("0.0.1-i", "0.0.1-blah", "0.0.1-1")]
        [TestCase("0.0.2-i", "0.0.1", "0.0.2-1")]
        public void ShouldApplyMask(string mask, string latestVersion, string expected)
        {
            var latestVersionAsSemver = SemVerFactory.TryCreateVersion(latestVersion);
            if (latestVersionAsSemver != null)
            {
                // Ensure the old and new implementations work the same way with plain masks
                var resultNewImplementationVersion = OctopusVersionMaskParser.ApplyMask(mask, latestVersionAsSemver);
                var resultOldImplementationVersion = SemanticVersionMask.ApplyMask(mask, latestVersionAsSemver);
                Assert.AreEqual(resultOldImplementationVersion.OriginalString, resultNewImplementationVersion.OriginalString);

                // We also expect the old implementation to have matching ToString and OriginalString results
                Assert.AreEqual(expected, resultOldImplementationVersion.ToString());
                Assert.AreEqual(expected, resultOldImplementationVersion.OriginalString);
            }

            // each version and mask should support a leading V
            foreach (var prefix in VersionPrefixes)
            {
                var resultNewImplementationWithPrefix = OctopusVersionMaskParser.ApplyMask(prefix + mask, latestVersion != null ? new OctopusVersionParser().Parse(latestVersion) : null);
                Assert.AreEqual(prefix + expected, resultNewImplementationWithPrefix.ToString());
                Assert.AreEqual(prefix + expected, resultNewImplementationWithPrefix.OriginalString);
            }
        }

        /// <summary>
        /// Assert that the new and old implementations fail with the same invalid input with whitespace around the prerelease field
        /// </summary>
        [TestCase("1.2.3- i")]
        [TestCase("1.2.3- 1")]
        [TestCase("1.2.3-1 .i")]
        [TestCase("1.2.3-1 .c")]
        public void ShouldFail(string mask)
        {
            var latestVersion = SemVerFactory.TryCreateVersion("1.2.3");

            Assert.Catch(() => OctopusVersionMaskParser.ApplyMask(mask, latestVersion));
            Assert.Catch(() => SemanticVersionMask.ApplyMask(mask, latestVersion));
        }

        [TestCase("1.2.3.4-i", false, Description = "This behaviour is a little confusing, as 1.2.3.4-i will function as a mask in every practical sense, but IsMask returns false. This behaviour is retained to ensure compatibility with the old masking implementation.")]
        [TestCase("1.2.3.4-c", false, Description = "This behaviour is a little confusing, as 1.2.3.4-c will function as a mask in every practical sense, but IsMask returns false. This behaviour is retained to ensure compatibility with the old masking implementation.")]
        [TestCase("1.2.3.4-i.1", false, Description = "This behaviour is a little confusing, as 1.2.3.4-i.1 will function as a mask in every practical sense, but IsMask returns false. This behaviour is retained to ensure compatibility with the old masking implementation.")]
        [TestCase("1.2.3.4-c.1", false, Description = "This behaviour is a little confusing, as 1.2.3.4-c.1 will function as a mask in every practical sense, but IsMask returns false. This behaviour is retained to ensure compatibility with the old masking implementation.")]
        [TestCase("1.2.3.4-1.blah.i.0", false, Description = "This behaviour is a little confusing, as 1.2.3.4-1.blah.i.0 will function as a mask in every practical sense, but IsMask returns false. This behaviour is retained to ensure compatibility with the old masking implementation.")]
        [TestCase("2021.4.0-0.121.27.202110617.1183ec5i", false)]
        [TestCase("1.2.3.4-blah", false)]
        [TestCase("1.2.3.4-blahi", false)]
        [TestCase("1.2.3.4-blahc", false)]
        [TestCase("1.2.3.4-iblah", false)]
        [TestCase("1.2.3.4-cblah", false)]
        [TestCase("1.2.3.4", false)]
        [TestCase("1.2.3", false)]
        [TestCase("1.2", false)]
        [TestCase("1", false)]
        [TestCase("c.i", true)]
        [TestCase("c.c.i", true)]
        [TestCase("c.c.c.i", true)]
        [TestCase("1.2.3.4-1.i", true)]
        [TestCase("1.2.3.4-1.c", true)]
        [TestCase("1.2.3.4-1.blah.i", true)]
        [TestCase("1.2.3.4-1.blah.c", true)]
        [TestCase("1.2.3.4-1.blah-c", false)]
        [TestCase("1.2.3.4-1.blah\\c", false)]
        [TestCase("1.2.3.4-1.blah_c", false)]
        [TestCase("i.2.3.4", true)]
        [TestCase("c.2.3.4", true)]
        [TestCase("1.i.3.4", true)]
        [TestCase("1.c.3.4", true)]
        [TestCase("1.2.i.4", true)]
        [TestCase("1.2.c.4", true)]
        [TestCase("1.2.3.i", true)]
        [TestCase("1.2.3.c", true)]
        [TestCase("i.i.3.4", true)]
        [TestCase("c.i.3.4", true)]
        [TestCase("1.i.i.4", true)]
        [TestCase("1.c.i.4", true)]
        [TestCase("1.2.i.i", true)]
        [TestCase("1.2.c.i", true)]
        [TestCase("i.2.3.i", true)]
        [TestCase("i.2.3.c", true)]
        [TestCase("ii.2.3.4", false)]
        [TestCase("1.ii.3.4", false)]
        [TestCase("1.cc.3.4", false)]
        [TestCase("1.2.ii.4", false)]
        [TestCase("1.2.cc.4", false)]
        [TestCase("1.2.3.ii", false)]
        [TestCase("1.3.3.cc", false)]
        [TestCase("1.3.3.z", false)]
        [TestCase("1. 2", false)]
        [TestCase("1.2. 3", false)]
        [TestCase("1.2.3 .4", false)]
        [TestCase("1.2 ", false)]
        [TestCase("1.2.3 ", false)]
        [TestCase("1.2.3.4 ", false)]
        [TestCase(" 1.2", false)]
        [TestCase("1.2 .3", false)]
        [TestCase("1.2.3 .4", false)]
        [TestCase("1.2.3.4-cc", false)]
        [TestCase("1.2.3.4-1.cc", false)]
        [TestCase("1.2.3.4-cc.1", false)]
        [TestCase("1.2.3.4-ii", false)]
        [TestCase("1.2.3.4-1.ii", false)]
        [TestCase("1.2.3.4-ii.1", false)]
        public void IsMask(string mask, bool isMask)
        {
            // Test the new implementation
            // each mask should support a leading V
            foreach (var prefix in VersionPrefixes)
                Assert.AreEqual(isMask, OctopusVersionMaskParser.Parse(prefix + mask).IsMask);

            // Test the old implementation
            Assert.AreEqual(isMask, SemanticVersionMask.IsMask(mask));
        }

        [TestCase("1.2.4", "1.2.3", null)]
        [TestCase("1.2.3-hi.i", "1.2.3", "1.2.3")]
        [TestCase("1.2.i", "1.2.3", "1.2.3")]
        [TestCase("1.i.i", "1.2.3", "1.2.3")]
        [TestCase("1.i.i", "2.0.0", null)]
        [Obsolete]
        public void GetLatestVersionMask(string version, string latestVersion, string expected)
        {
            var latestVersions = new List<IVersion>
            {
                new OctopusVersionParser().Parse(latestVersion)
            };

            // new an old implementations should be the same
            var latestMaskedVersionNewImplementation = OctopusVersionMaskParser.Parse(version).GetLatestMaskedVersion(latestVersions);
            var latestMaskedVersionOldImplementation = SemanticVersionMask.GetLatestMaskedVersion(version, latestVersions);
            Assert.AreEqual(latestMaskedVersionOldImplementation, latestMaskedVersionNewImplementation);

            // each mask should support a leading V with the new implementation
            foreach (var prefix in VersionPrefixes)
            {
                var latestMaskedVersionNewImplementationPrefix = OctopusVersionMaskParser.Parse(prefix + version).GetLatestMaskedVersion(latestVersions);
                Assert.AreEqual(expected, latestMaskedVersionNewImplementationPrefix?.ToString());
            }
        }

        [Test]
        [Obsolete]
        public void GetLatestMaskedVersionShouldUseOrderPassedInToTieBreak()
        {
            var mask = OctopusVersionMaskParser.Parse("0.0.7+branchA.c.c.i");
            var versionParser = new OctopusVersionParser();

            var versions = new[]
                {
                    "0.0.7+branchA.1",
                    "0.0.7+branchA.2", 
                    "0.0.6+branchA",
                    "0.0.5-beta.2",
                    "0.0.5-beta.1",
                    "0.0.4-beta",
                    "0.0.3",
                    "0.0.1",
                    "0.0.2"
                }.Select(v => (IVersion)versionParser.Parse(v))
                .ToList();

            // GetLatestMaskedVersion, ignores metata when comparing versions, we verify tiebreak behaviour on matching versions.
            Assert.AreEqual("0.0.7+branchA.1", mask.GetLatestMaskedVersion(versions)!.ToString(), "Because version with metadata of '+branchA.1' is first in the list passed in");

            // Now the real test; it should still return the same result even if the versions are not in the correct order
            var reversedVersions = versions.ToList();
            reversedVersions.Reverse();
            Assert.AreEqual("0.0.7+branchA.2", mask.GetLatestMaskedVersion(reversedVersions)!.ToString(),"Because we reversed the order of the list, so `+branchA.2` is now first in the list.");
        }

        [Test]
        public void GetLatestMaskedVersionsShouldReturnAllVersionsWithHighestPrecedence()
        {
            var mask = OctopusVersionMaskParser.Parse("0.0.7+branchA.c.c.i");
            var versionParser = new OctopusVersionParser();

            var versions = new[]
                {
                    "0.0.7+branchA.1",
                    "0.0.7+branchA.2", 
                    "0.0.6+branchA",
                    "0.0.5-beta.2",
                    "0.0.5-beta.1",
                    "0.0.4-beta",
                    "0.0.3",
                    "0.0.1",
                    "0.0.2"
                }.Select(v => (IVersion)versionParser.Parse(v))
                .ToList();

            var result = mask.GetLatestMaskedVersions(versions);

            // Should return both 0.0.7 versions since they have the same precedence (metadata is ignored in comparison)
            Assert.AreEqual(2, result.Count, "Should return both versions with highest precedence");
            Assert.True(result.Any(v => v.ToString() == "0.0.7+branchA.1"), "Should include 0.0.7+branchA.1");
            Assert.True(result.Any(v => v.ToString() == "0.0.7+branchA.2"), "Should include 0.0.7+branchA.2");
        }

        [Test]
        public void GetLatestMaskedVersionsShouldReturnEmptyListWhenNoVersionsMatch()
        {
            var mask = OctopusVersionMaskParser.Parse("2.0.0");
            var versionParser = new OctopusVersionParser();

            var versions = new[]
                {
                    "1.0.0",
                    "1.5.0",
                    "1.9.9"
                }.Select(v => (IVersion)versionParser.Parse(v))
                .ToList();

            var result = mask.GetLatestMaskedVersions(versions);

            Assert.AreEqual(0, result.Count, "Should return empty list when no versions match the mask");
        }

        [Test]
        public void GetLatestMaskedVersionsShouldReturnSingleVersionWhenOnlyOneHasHighestPrecedence()
        {
            var mask = OctopusVersionMaskParser.Parse("c.c.i");
            var versionParser = new OctopusVersionParser();

            var versions = new[]
                {
                    "1.2.3",
                    "1.2.2",
                    "1.1.9",
                    "0.9.0"
                }.Select(v => (IVersion)versionParser.Parse(v))
                .ToList();

            var result = mask.GetLatestMaskedVersions(versions);

            Assert.AreEqual(1, result.Count, "Should return single version when only one has highest precedence");
            Assert.AreEqual("1.2.3", result.First().ToString(), "Should return the highest version");
        }

        [Test]
        public void GetLatestMaskedVersionsShouldReturnAllVersionsWithDifferentMetadataTypes()
        {
            var mask = OctopusVersionMaskParser.Parse("1.2.3+c.c.c");
            var versionParser = new OctopusVersionParser();

            var versions = new[]
                {
                    "1.2.3+20231201.1430",           // DateTime-like metadata
                    "1.2.3+build.456",               // Build number metadata
                    "1.2.3+commit.abc123f",          // Git commit hash metadata
                    "1.2.3+feature-branch.45",       // Branch name with build number
                    "1.2.3+build.789.20231201",      // Combined build and date metadata
                    "1.2.2+anything",                // Lower version (should be filtered out)
                    "1.2.4+something"                // Higher version (should be filtered out)
                }.Select(v => (IVersion)versionParser.Parse(v))
                .ToList();

            var result = mask.GetLatestMaskedVersions(versions);

            // Should return all 1.2.3 versions since they have the same precedence (metadata is ignored in comparison)
            Assert.AreEqual(5, result.Count, "Should return all versions with matching core version 1.2.3");
            Assert.True(result.Any(v => v.ToString() == "1.2.3+20231201.1430"), "Should include datetime metadata version");
            Assert.True(result.Any(v => v.ToString() == "1.2.3+build.456"), "Should include build number metadata version");
            Assert.True(result.Any(v => v.ToString() == "1.2.3+commit.abc123f"), "Should include git commit metadata version");
            Assert.True(result.Any(v => v.ToString() == "1.2.3+feature-branch.45"), "Should include branch metadata version");
            Assert.True(result.Any(v => v.ToString() == "1.2.3+build.789.20231201"), "Should include combined metadata version");
            
            // Verify that versions with different core numbers are not included
            Assert.False(result.Any(v => v.ToString() == "1.2.2+anything"), "Should not include lower version");
            Assert.False(result.Any(v => v.ToString() == "1.2.4+something"), "Should not include higher version");
        }
    }
}