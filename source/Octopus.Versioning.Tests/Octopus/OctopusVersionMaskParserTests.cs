﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using Octopus.Versioning.Octopus;

namespace Octopus.Versioning.Tests.Octopus
{
    [TestFixture]
    public class OctopusVersionMaskParserTests
    {
        static readonly OctopusVersionMaskParser OctopusVersionMaskParser = new OctopusVersionMaskParser();

        /// <summary>
        /// This test used to live in Octopus server. You can find it at commit 43e42a5769083dffa65e2c9ab4d513641c37248a
        /// in the SemanticVersionMaskFixture.cs file.
        ///
        /// Tests here, with the exception of masks that start with a 'v' or 'V' and initial versions that are not valid semver,
        /// should also pass in that old commit.
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
        [TestCase("v1.2.3-alpha.i", null, "v1.2.3-alpha.0", Description = "An example of a previous test, but with a 'v' prefix.")]
        [TestCase("v1.2.3-alpha.i", "1.2.3-alpha.25", "v1.2.3-alpha.26", Description = "An example of a previous test, but with a 'v' prefix.")]
        [TestCase("vi.2.3-alpha.i", "1.2.3-alpha.25", "v2.2.3-alpha.26", Description = "An example of a previous test, but with a 'v' prefix.")]
        [TestCase("v1.i.3-alpha.i", "1.2.3-alpha.25", "v1.3.3-alpha.26", Description = "An example of a previous test, but with a 'v' prefix.")]
        [TestCase("v1.2.i-alpha.i", "1.2.3-alpha.25", "v1.2.4-alpha.26", Description = "An example of a previous test, but with a 'v' prefix.")]
        [TestCase("vc.2.3-alpha.i", "1.2.3-alpha.25", "v1.2.3-alpha.26", Description = "An example of a previous test, but with a 'v' prefix.")]
        [TestCase("v1.c.3-alpha.i", "1.2.3-alpha.25", "v1.2.3-alpha.26", Description = "An example of a previous test, but with a 'v' prefix.")]
        [TestCase("v1.2.c-alpha.i", "1.2.3-alpha.25", "v1.2.3-alpha.26", Description = "An example of a previous test, but with a 'v' prefix.")]
        [TestCase("vi.i.i-alpha.i", "1.2.3-alpha.25", "v2.0.0-alpha.26", Description = "An example of a previous test, but with a 'v' prefix.")]
        [TestCase("vc.c.i-alpha.c", "1.2.3-alpha.25", "v1.2.4-alpha.25", Description = "An example of a previous test, but with a 'v' prefix.")]
        [TestCase("vi.c.c", "1.2.3-alpha.25", "v2.0.3", Description = "An example of a previous test, but with a 'v' prefix.")]
        [TestCase("vc.i.c", "1.2.3-alpha.25", "v1.3.0", Description = "An example of a previous test, but with a 'v' prefix.")]
        [TestCase("vc.c.i", "1.2.3-alpha.25", "v1.2.4", Description = "An example of a previous test, but with a 'v' prefix.")]
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
        [TestCase("1.2.3-i", "whatever", "1.2.3-i", Description = "Not a valid mask (a mask is only valid when the prerelease end with '.i' or '.c'), so keep the original value")]
        [TestCase("1.2.3-c", "whatever", "1.2.3-c", Description = "Not a valid mask (a mask is only valid when the prerelease end with '.i' or '.c'), so keep the original value")]
        [TestCase("1.2.3_i", "whatever", "1.2.3_i", Description = "Not a valid mask, so the version is passed through.")]
        [TestCase("1.2.3_c", "whatever", "1.2.3_c", Description = "Not a valid mask, so the version is passed through.")]
        [TestCase("1.2.3.i", "whatever", "1.2.3.1", Description = "Existing versions with only plain text are assumed to be 0.0.0.0.")]
        [TestCase("1.2.3.c", "whatever", "1.2.3.0", Description = "Existing versions with only plain text are assumed to be 0.0.0.0.")]
        [TestCase("v0.1.0-initial", "whatever", "v0.1.0-initial", Description = "An example of a previous test, but with a 'v' prefix.")]
        [TestCase("Vc.c.i-initial", "whatever", "V0.0.1-initial", Description = "An example of a previous test, but with a 'v' prefix.")]
        [TestCase("vc.c.i-current", "whatever", "v0.0.1-current", Description = "An example of a previous test, but with a 'v' prefix.")]
        [TestCase("Vc.c.c.i-current", "whatever", "V0.0.0.1-current", Description = "An example of a previous test, but with a 'v' prefix.")]
        [TestCase("vc.c.c.i-initial.1", "whatever", "v0.0.0.1-initial.1", Description = "An example of a previous test, but with a 'v' prefix.")]
        [TestCase("Vc.c.c.i-initial.c", "whatever", "V0.0.0.1-initial.0", Description = "An example of a previous test, but with a 'v' prefix.")]
        [TestCase("vc.c.c.i-initial.i", "whatever", "v0.0.0.1-initial.1", Description = "An example of a previous test, but with a 'v' prefix.")]
        [TestCase("Vc.c.c-i", "whatever", "V0.0.0-1", Description = "An example of a previous test, but with a 'v' prefix.")]
        [TestCase("vc.c.c-c", "whatever", "v0.0.0-0", Description = "An example of a previous test, but with a 'v' prefix.")]
        [TestCase("Vc.c.c_i", "whatever", "Vc.c.c_i", Description = "An example of a previous test, but with a 'v' prefix.")]
        [TestCase("vc.c.c_c", "whatever", "vc.c.c_c", Description = "An example of a previous test, but with a 'v' prefix.")]
        [TestCase("Vc.c.c.i", "whatever", "V0.0.0.1", Description = "An example of a previous test, but with a 'v' prefix.")]
        [TestCase("vc.c.c.c", "whatever", "v0.0.0.0", Description = "An example of a previous test, but with a 'v' prefix.")]
        [TestCase("V1.2.3-i", "whatever", "V1.2.3-i", Description = "An example of a previous test, but with a 'v' prefix.")]
        [TestCase("v1.2.3-c", "whatever", "v1.2.3-c", Description = "An example of a previous test, but with a 'v' prefix.")]
        [TestCase("V1.2.3_i", "whatever", "V1.2.3_i", Description = "An example of a previous test, but with a 'v' prefix.")]
        [TestCase("v1.2.3_c", "whatever", "v1.2.3_c", Description = "An example of a previous test, but with a 'v' prefix.")]
        [TestCase("V1.2.3.i", "whatever", "V1.2.3.1", Description = "An example of a previous test, but with a 'v' prefix.")]
        [TestCase("v1.2.3.c", "whatever", "v1.2.3.0", Description = "An example of a previous test, but with a 'v' prefix.")]
        [TestCase("v1-2_3.c", "whatever", "v1-2_3.0", Description = "Increment the prerelease counter")]
        [TestCase("v1-2_3-c", "whatever", "v1-2_3-c", Description = "The prerelease counter version does not use a dot separator, and so is not a mask.")]
        [TestCase("v1.2.3_c", "whatever", "v1.2.3_c", Description = "The version does not use the dot notation with a dash before the prerelease, and so is not a mask.")]
        [TestCase("v1-2_3-i", "whatever", "v1-2_3-i", Description = "The prerelease counter version does not use a dot separator, and so is not a mask.")]
        [TestCase("v1.2.3_i", "whatever", "v1.2.3_i", Description = "The version does not use the dot notation with a dash before the prerelease, and so is not a mask.")]
        [TestCase("1.2.3-initial.8", "1.0.0.0", "1.2.3-initial.8", Description = "This is not a mask, so keep the original string.")]
        [TestCase("v1.2.3-initial.8", "v1.0.0.0", "v1.2.3-initial.8", Description = "This is not a mask, so keep the original string.")]
        [TestCase("2021.4.0-0.121.27.202110617.1183ec5i", "2021.4.0.0-121.50.210609", "2021.4.0-0.121.27.202110617.1183ec5i", Description = "A specific test case documented in https://github.com/OctopusDeploy/Issues/issues/6926")]
        [TestCase("c.c.i", "1.2.3.4", "1.2.4.0", Description = "The resulting version gets the a release of 0 from the latest version.")]
        [TestCase("1.2.3-hi.i", "1.1.1.1", "1.2.3.0-hi.1", Description = "The resulting version gets the a release of 0 from the latest version.")]
        public void ShouldApplyMask(string mask, string latestVersion, string expected)
        {
            var result = OctopusVersionMaskParser.ApplyMask(mask, latestVersion != null ? new OctopusVersionParser().Parse(latestVersion) : null);
            Assert.AreEqual(expected, result.OriginalString);
            Assert.AreEqual(expected, result.ToString());
        }

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
        [TestCase("1.2.3.4-i", false)]
        [TestCase("i", true)]
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
        [TestCase("1.2.3.4-1.blah.i.0", false)]
        public void IsMask(string mask, bool isMask)
        {
            Assert.AreEqual(isMask, OctopusVersionMaskParser.Parse(mask).IsMask);
        }

        [TestCase("1.2.4", "1.2.3", null)]
        [TestCase("1.2.3-hi.i", "1.2.3", "1.2.3")]
        [TestCase("1.2.i", "1.2.3", "1.2.3")]
        [TestCase("1.i.i", "1.2.3", "1.2.3")]
        [TestCase("1.i.i", "2.0.0", null)]
        public void GetLatestVersionMask(string version, string latestVersion, string expected)
        {
            var latestVersions = new List<IVersion>();
            latestVersions.Add(new OctopusVersionParser().Parse(latestVersion));
            var latestMaskedVersion = OctopusVersionMaskParser.Parse(version).GetLatestMaskedVersion(latestVersions);
            Assert.AreEqual(expected, expected == null ? null : new OctopusVersionParser().Parse(expected).ToString());
        }
    }
}