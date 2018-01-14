using System;
using System.Text.RegularExpressions;
using Octopus.Versioning.Semver;

namespace Octopus.Versioning.Ranges.SemVer
{
    /// <summary>
    /// A version range checking service that uses the NuGet classes.
    /// </summary>
    public class SemVerRuleSpecification : IVersionRuleSpecification
    {
        public bool CanParseVersionRange(VersionFormat versionFormat, string versionRange)
        {
            if (string.IsNullOrWhiteSpace(versionRange))
            {
                return true;
            }

            try
            {
                return VersionRange.TryParse(versionRange, out var range);
            }
            catch
            {
                return false;
            }
        }

        public bool SatisfiesVersionRange(IVersion version, string versionRange)
        {

            var semver = version as SemanticVersion;
            if (string.IsNullOrWhiteSpace(versionRange) || semver == null)
            {
                return true;
            }

            try
            {
                VersionRange.TryParse(versionRange, out var range);
                return range.Satisfies(semver);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException($"The 'versionRange' parameter '{version}' was not a valid NuGet version-range (see http://g.octopushq.com/NuGetVersioning)");
            }
        }

        public bool SatisfiesPreReleaseTag(IVersion version, string preReleaseTag)
        {
            if (string.IsNullOrWhiteSpace(preReleaseTag))
                return true;

            try
            {
                return SatisfiesPreReleaseTag(version, new Regex(preReleaseTag));
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public bool SatisfiesPreReleaseTag(IVersion version, Regex preReleaseTag)
        {
            if (preReleaseTag == null)
                return true;

            // For SemVer 2.0 versions, the regex is applied to the PreRelease component and the metadata (including the '+')
            return preReleaseTag.IsMatch(version.Release + (version.HasMetadata ? "+" + version.Metadata : ""));
        }
    }
}