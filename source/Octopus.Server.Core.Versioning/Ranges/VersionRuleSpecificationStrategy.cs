using System;
using System.Text.RegularExpressions;
using Octopus.Core.Versioning.Maven;
using Octopus.Core.Versioning.Ranges.Maven;
using Octopus.Core.Versioning.Ranges.SemVer;
using Octopus.Core.Versioning.Semver;

namespace Octopus.Core.Versioning.Ranges
{
    /// This strategy knows how to apply version range checks based on the type of version that is being compared.
    /// </summary>
    public class VersionRuleSpecificationStrategy : IVersionRuleSpecification
    {
        static readonly IVersionRuleSpecification NuGetVersionRuleSpecification = new SemVerRuleSpecification();
        static readonly IVersionRuleSpecification MavenVersionRuleSpecification = new MavenVersionRuleSpecification();

        public bool CanParseVersionRange(VersionFormat versionFormat, string versionRange)
        {
            if (string.IsNullOrWhiteSpace(versionRange))
            {
                return true;
            }

            if (versionFormat == VersionFormat.Maven)
            {
                return MavenVersionRuleSpecification.CanParseVersionRange(versionFormat, versionRange);
            }

            return NuGetVersionRuleSpecification.CanParseVersionRange(versionFormat, versionRange);
        }

        public bool SatisfiesVersionRange(IVersion version, string versionRange)
        {
            if (string.IsNullOrWhiteSpace(versionRange))
                return true;

            if (version is MavenVersion)
            {
                return MavenVersionRuleSpecification.SatisfiesVersionRange(version, versionRange);
            }

            if (version is SemanticVersion)
            {
                return NuGetVersionRuleSpecification.SatisfiesVersionRange(version, versionRange);
            }

            throw new NotImplementedException("version was not recognised");
        }

        public bool SatisfiesPreReleaseTag(IVersion version, string preReleaseTag)
        {
            if (string.IsNullOrWhiteSpace(preReleaseTag))
                return true;

            try
            {
                if (version is MavenVersion)
                {
                    return MavenVersionRuleSpecification.SatisfiesPreReleaseTag(version, preReleaseTag);
                }

                if (version is SemanticVersion)
                {
                    return NuGetVersionRuleSpecification.SatisfiesPreReleaseTag(version, preReleaseTag);
                }

                throw new NotImplementedException("version was not recognised");
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

            if (version is MavenVersion)
            {
                return MavenVersionRuleSpecification.SatisfiesPreReleaseTag(version, preReleaseTag);
            }

            if (version is SemanticVersion)
            {
                return NuGetVersionRuleSpecification.SatisfiesPreReleaseTag(version, preReleaseTag);
            }

            throw new NotImplementedException("version was not recognised");
        }
    }
}
