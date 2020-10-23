using System;
using System.Linq;

namespace Octopus.Versioning.Semver
{
    /// <summary>
    /// This partial class contains all the log required to create SemanticVersion
    /// and StrictSemanticVerion classes.
    /// </summary>
    public class SemVerFactory
    {
        static readonly ISemanticVersionUtils utils = new SemanticVersionUtils();

        public static SemanticVersion CreateVersion(string input, bool preserveMissingComponents = false)
        {
            var ver = TryCreateVersion(input, preserveMissingComponents);
            if (ver == null)
                throw new ArgumentException($"'{input}' is not a valid version string", nameof(input));

            return ver;
        }

        public static IVersion? CreateVersionOrNone(string input, bool preserveMissingComponents = false)
        {
            return TryCreateVersion(input, preserveMissingComponents);
        }

        /// <summary>
        /// Creates a NuGetVersion from a string representing the semantic version.
        /// </summary>
        public static SemanticVersion Parse(string value, bool preserveMissingComponents = false)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Value cannot be null or an empty string", nameof(value));

            var ver = TryCreateVersion(value, preserveMissingComponents);
            if (ver == null)
                throw new ArgumentException($"'{value}' is not a valid version string", nameof(value));

            return ver;
        }

        /// <summary>
        /// Parses a version string using loose semantic versioning rules that allows 2-4 version components followed
        /// by an optional special version.
        /// </summary>
        public static SemanticVersion? TryCreateVersion(string value, bool preserveMissingComponents = false)
        {
            SemanticVersion? version = null;

            if (value != null)
            {
                Version? systemVersion = null;

                // trim the value before passing it in since we not strict here
                var sections = utils.ParseSections(value.Trim());

                // null indicates the string did not meet the rules
                if (sections != null
                    && !string.IsNullOrEmpty(sections.Item1))
                {
                    var versionPart = sections.Item1;

                    if (versionPart.IndexOf('.') < 0)
                        // System.Version requires at least a 2 part version to parse.
                        versionPart += ".0";

                    if (Version.TryParse(versionPart, out systemVersion))
                    {
                        // labels
                        if (sections.Item2 != null
                            && !sections.Item2.All(s => utils.IsValidPart(s, true)))
                            return null;

                        // build metadata
                        if (sections.Item3 != null
                            && !utils.IsValid(sections.Item3, true))
                            return null;

                        var ver = preserveMissingComponents
                            ? systemVersion
                            : utils.NormalizeVersionValue(systemVersion);

                        var originalVersion = value;

                        if (originalVersion.IndexOf(' ') > -1)
                            originalVersion = value.Replace(" ", "");

                        version = new SemanticVersion(version: ver,
                            releaseLabels: sections.Item2,
                            metadata: sections.Item3 ?? string.Empty,
                            originalVersion: originalVersion);

                        return version;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Parses a version string using strict SemVer rules.
        /// </summary>
        public static SemanticVersion? TryParseStrict(string value)
        {
            var semVer = TryCreateVersion(value);
            return semVer != null
                ? new SemanticVersion((int)semVer.Major,
                    (int)semVer.Minor,
                    (int)semVer.Patch,
                    0,
                    semVer.ReleaseLabels,
                    semVer.Metadata)
                : null;
        }
    }
}