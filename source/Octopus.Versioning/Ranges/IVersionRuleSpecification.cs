using System.Text.RegularExpressions;

namespace Octopus.Versioning.Ranges
{
    /// <summary>
    /// A service used to check a version is within range
    /// </summary>
    public interface IVersionRuleSpecification
    {
        bool CanParseVersionRange(VersionFormat versionFormat, string versionRange);
        bool SatisfiesVersionRange(IVersion version, string versionRange);
        bool SatisfiesPreReleaseTag(IVersion version, string preReleaseTag);
        bool SatisfiesPreReleaseTag(IVersion version, Regex preReleaseTag);
    }
}