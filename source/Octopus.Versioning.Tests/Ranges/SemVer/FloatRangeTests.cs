// This class is based on NuGet's FloatRangeTests class from https://github.com/NuGetArchive/NuGet.Versioning
// NuGet is licensed under the Apache license: https://github.com/NuGetArchive/NuGet.Versioning/blob/dev/LICENSE.txt
using System.Collections.Generic;
using NUnit.Framework;
using Octopus.Versioning.Factories;
using Octopus.Versioning.Ranges.SemVer;
using Octopus.Versioning.Semver;

namespace Octopus.Server.Core.Versioning.Tests.Ranges.SemVer
{
    [TestFixture]
    public class FloatingRangeTests
    {
        [Test]
        public void FloatRange_OutsideOfRange()
        {
            VersionRange range = VersionRange.Parse("[1.0.*, 2.0.0)");

            var versions = new List<SemanticVersion>()
            {
                SemVerFactory.Parse("0.1.0"),
                SemVerFactory.Parse("1.0.0-alpha.2"),
                SemVerFactory.Parse("2.0.0"),
                SemVerFactory.Parse("2.2.0"),
                SemVerFactory.Parse("3.0.0"),
            };

            Assert.Null(range.FindBestMatch(versions));
        }

       
        [Test]
        public void FloatRange_OutsideOfRangeLower()
        {
            VersionRange range = VersionRange.Parse("[1.0.*, 2.0.0)");

            var versions = new List<SemanticVersion>()
            {
                SemVerFactory.Parse("0.1.0"),
                SemVerFactory.Parse("0.2.0"),
                SemVerFactory.Parse("1.0.0-alpha.2")
            };

            Assert.Null(range.FindBestMatch(versions));
        }
 
        [Test]
        public void FloatRange_OutsideOfRangeHigher()
        {
            VersionRange range = VersionRange.Parse("[1.0.*, 2.0.0)");

            var versions = new List<SemanticVersion>()
            {
                SemVerFactory.Parse("2.0.0"),
                SemVerFactory.Parse("2.0.0-alpha.2"),
                SemVerFactory.Parse("3.1.0"),
            };

            Assert.Null(range.FindBestMatch(versions));
        }

        [Test]
        public void FloatRange_OutsideOfRangeOpen()
        {
            VersionRange range = VersionRange.Parse("[1.0.*, )");

            var versions = new List<SemanticVersion>()
            {
                SemVerFactory.Parse("0.1.0"),
                SemVerFactory.Parse("0.2.0"),
                SemVerFactory.Parse("1.0.0-alpha.2")
            };

            Assert.Null(range.FindBestMatch(versions));
        }

        [Test]
        public void FloatRange_RangeOpen()
        {
            VersionRange range = VersionRange.Parse("[1.0.*, )");

            var versions = new List<SemanticVersion>()
            {
                SemVerFactory.Parse("0.1.0"),
                SemVerFactory.Parse("0.2.0"),
                SemVerFactory.Parse("1.0.0-alpha.2"),
                SemVerFactory.Parse("101.0.0")
            };

            Assert.AreEqual("101.0.0", range.FindBestMatch(versions).ToNormalizedString());
        }

        [Test]
        [TestCase("1.0.0")]
        public void FloatRange_ParseBasic(string version)
        {
            FloatRange range = FloatRange.Parse(version);

            Assert.AreEqual(range.MinVersion, range.MinVersion);
            Assert.AreEqual(range.FloatBehavior, NuGetVersionFloatBehavior.None);
        }

        [Test]
        public void FloatRange_ParsePrerelease()
        {
            FloatRange range = FloatRange.Parse("1.0.0-*");

            Assert.True(range.Satisfies(SemVerFactory.Parse("1.0.0-alpha")));
            Assert.True(range.Satisfies(SemVerFactory.Parse("1.0.0-beta")));
            Assert.True(range.Satisfies(SemVerFactory.Parse("1.0.0")));

            Assert.False(range.Satisfies(SemVerFactory.Parse("1.0.1-alpha")));
            Assert.False(range.Satisfies(SemVerFactory.Parse("1.0.1")));
        }

        [Test]
        public void FloatingRange_FloatNone()
        {
            FloatRange range = FloatRange.Parse("1.0.0");

            Assert.AreEqual("1.0.0", range.MinVersion.ToNormalizedString());
            Assert.AreEqual(NuGetVersionFloatBehavior.None, range.FloatBehavior);
        }

        [Test]
        public void FloatingRange_FloatPre()
        {
            FloatRange range = FloatRange.Parse("1.0.0-*");

            Assert.AreEqual("1.0.0--", range.MinVersion.ToNormalizedString());
            Assert.AreEqual(NuGetVersionFloatBehavior.Prerelease, range.FloatBehavior);
        }

        [Test]
        public void FloatingRange_FloatPrePrefix()
        {
            FloatRange range = FloatRange.Parse("1.0.0-alpha-*");

            Assert.AreEqual("1.0.0-alpha-", range.MinVersion.ToNormalizedString());
            Assert.AreEqual(NuGetVersionFloatBehavior.Prerelease, range.FloatBehavior);
        }

        [Test]
        public void FloatingRange_FloatRev()
        {
            FloatRange range = FloatRange.Parse("1.0.0.*");

            Assert.AreEqual("1.0.0", range.MinVersion.ToNormalizedString());
            Assert.AreEqual(NuGetVersionFloatBehavior.Revision, range.FloatBehavior);
        }

        [Test]
        public void FloatingRange_FloatPatch()
        {
            FloatRange range = FloatRange.Parse("1.0.*");

            Assert.AreEqual("1.0.0", range.MinVersion.ToNormalizedString());
            Assert.AreEqual(NuGetVersionFloatBehavior.Patch, range.FloatBehavior);
        }

        [Test]
        public void FloatingRange_FloatMinor()
        {
            FloatRange range = FloatRange.Parse("1.*");

            Assert.AreEqual("1.0.0", range.MinVersion.ToNormalizedString());
            Assert.AreEqual(NuGetVersionFloatBehavior.Minor, range.FloatBehavior);
        }

        [Test]
        public void FloatingRange_FloatMajor()
        {
            FloatRange range = FloatRange.Parse("*");

            Assert.AreEqual("0.0.0", range.MinVersion.ToNormalizedString());
            Assert.AreEqual(NuGetVersionFloatBehavior.Major, range.FloatBehavior);
        }


        [Test]
        public void FloatingRange_FloatNoneBest()
        {
            VersionRange range = VersionRange.Parse("1.0.0");

            var versions = new List<SemanticVersion>()
            {
                SemVerFactory.Parse("1.0.0"),
                SemVerFactory.Parse("1.0.1"),
                SemVerFactory.Parse("2.0.0"),
            };

            Assert.AreEqual("1.0.0", range.FindBestMatch(versions).ToNormalizedString());
        }

        [Test]
        public void FloatingRange_FloatMinorBest()
        {
            VersionRange range = VersionRange.Parse("1.*");

            var versions = new List<SemanticVersion>()
            {
                SemVerFactory.Parse("0.1.0"),
                SemVerFactory.Parse("1.0.0"),
                SemVerFactory.Parse("1.2.0"),
                SemVerFactory.Parse("2.0.0"),
            };

            Assert.AreEqual("1.2.0", range.FindBestMatch(versions).ToNormalizedString());
        }

        [Test]
        public void FloatingRange_FloatMinorPrefixNotFoundBest()
        {
            VersionRange range = VersionRange.Parse("1.*");

            var versions = new List<SemanticVersion>()
            {
                SemVerFactory.Parse("0.1.0"),
                SemVerFactory.Parse("2.0.0"),
                SemVerFactory.Parse("2.5.0"),
                SemVerFactory.Parse("3.3.0"),
            };

            // take the nearest when the prefix is not matched
            Assert.AreEqual("2.0.0", range.FindBestMatch(versions).ToNormalizedString());
        }

        [Test]
        public void FloatingRange_FloatAllBest()
        {
            VersionRange range = VersionRange.Parse("*");

            var versions = new List<SemanticVersion>()
            {
                SemVerFactory.Parse("0.1.0"),
                SemVerFactory.Parse("2.0.0"),
                SemVerFactory.Parse("2.5.0"),
                SemVerFactory.Parse("3.3.0"),
            };

            Assert.AreEqual("3.3.0", range.FindBestMatch(versions).ToNormalizedString());
        }

        [Test]
        public void FloatingRange_FloatPrereleaseBest()
        {
            VersionRange range = VersionRange.Parse("1.0.0-*");

            var versions = new List<SemanticVersion>()
            {
                SemVerFactory.Parse("0.1.0-alpha"),
                SemVerFactory.Parse("1.0.0-alpha01"),
                SemVerFactory.Parse("1.0.0-alpha02"),
                SemVerFactory.Parse("2.0.0-beta"),
                SemVerFactory.Parse("2.0.1"),
            };

            Assert.AreEqual("1.0.0-alpha02", range.FindBestMatch(versions).ToNormalizedString());
        }

        [Test]
        public void FloatingRange_FloatPrereleaseNotFoundBest()
        {
            // "1.0.0-*"
            VersionRange range = VersionRange.Parse("1.0.0-*");

            var versions = new List<SemanticVersion>()
            {
                SemVerFactory.Parse("0.1.0-alpha"),
                SemVerFactory.Parse("1.0.1-alpha01"),
                SemVerFactory.Parse("1.0.1-alpha02"),
                SemVerFactory.Parse("2.0.0-beta"),
                SemVerFactory.Parse("2.0.1"),
            };

            Assert.AreEqual("1.0.1-alpha01", range.FindBestMatch(versions).ToNormalizedString());
        }

        [Test]
        public void FloatingRange_FloatPrereleasePartialBest()
        {
            VersionRange range = VersionRange.Parse("1.0.0-alpha*");

            var versions = new List<SemanticVersion>()
            {
                SemVerFactory.Parse("0.1.0-alpha"),
                SemVerFactory.Parse("1.0.0-alpha01"),
                SemVerFactory.Parse("1.0.0-alpha02"),
                SemVerFactory.Parse("2.0.0-beta"),
                SemVerFactory.Parse("2.0.1"),
            };

            Assert.AreEqual("1.0.0-alpha02", range.FindBestMatch(versions).ToNormalizedString());
        }

        [Test]
        public void FloatingRange_ToStringPre()
        {
            VersionRange range = VersionRange.Parse("1.0.0-*");

            Assert.AreEqual("[1.0.0-*, )", range.ToNormalizedString());
        }

        [Test]
        public void FloatingRange_ToStringPrePrefix()
        {
            VersionRange range = VersionRange.Parse("1.0.0-alpha.*");

            Assert.AreEqual("[1.0.0-alpha.*, )", range.ToNormalizedString());
        }

        [Test]
        public void FloatingRange_ToStringRev()
        {
            VersionRange range = VersionRange.Parse("1.0.0.*");

            Assert.AreEqual("[1.0.0.*, )", range.ToNormalizedString());
        }

        [Test]
        public void FloatingRange_ToStringPatch()
        {
            VersionRange range = VersionRange.Parse("1.0.*");

            Assert.AreEqual("[1.0.*, )", range.ToNormalizedString());
        }

        [Test]
        public void FloatingRange_ToStringMinor()
        {
            VersionRange range = VersionRange.Parse("1.*");

            Assert.AreEqual("[1.*, )", range.ToNormalizedString());
        }

        // TODO: fix this one the proper syntax is determined
        //[Test]
        //public void FloatingRange_ToStringMajor()
        //{
        //    VersionRange range = VersionRange.Parse("*");

        //    Assert.AreEqual("[*, )", range.ToNormalizedString());
        //}
    }
}
