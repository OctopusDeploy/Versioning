using System;
using System.Text.RegularExpressions;
using Octopus.Core.Versioning.Ranges.SemVer;

namespace Octopus.Core.Versioning.Ranges.Maven
{
    /// <summary>
    /// An implementation of a Maven range checking class.
    /// Based on https://github.com/apache/maven/blob/master/maven-artifact/src/main/java/org/apache/maven/artifact/versioning/VersionRange.java.
    /// </summary>
    public class MavenVersionRuleSpecification : IVersionRuleSpecification
    {
        public bool CanParseVersionRange(VersionFormat versionFormat, string versionRange)
        {
            if (versionFormat != VersionFormat.Maven)
            {
                return false;
            }

            try
            {
                MavenVersionRange.CreateFromVersionSpec(versionRange);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SatisfiesVersionRange(IVersion version, string versionRange)
        {
            if (string.IsNullOrWhiteSpace(versionRange))
                return true;

            try
            {
                return MavenVersionRange.CreateFromVersionSpec(versionRange).ContainsVersion(version);
            }
            catch
            {
                throw new ArgumentException($"The 'versionRange' parameter '{version}' was not a valid Maven version-range (see https://maven.apache.org/enforcer/enforcer-rules/versionRanges.html)");
            }
        }

        public bool SatisfiesPreReleaseTag(IVersion version, string preReleaseTag)
        {
            if (string.IsNullOrWhiteSpace(preReleaseTag))
                return true;

            try
            {
                // Qualifiers are not case sensitive in maven
                return SatisfiesPreReleaseTag(version, new Regex(preReleaseTag, RegexOptions.IgnoreCase));
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

            return preReleaseTag.IsMatch(version.Release);
        }
    }
}