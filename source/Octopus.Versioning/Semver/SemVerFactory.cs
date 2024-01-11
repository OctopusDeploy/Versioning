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

        public static SemanticSortableVersion CreateVersion(string input, bool preserveMissingComponents = false)
        {
            var ver = TryCreateVersion(input, preserveMissingComponents);
            if (ver == null)
                throw new ArgumentException($"'{input}' is not a valid version string", nameof(input));

            return ver;
        }

        public static ISortableVersion? CreateVersionOrNone(string input, bool preserveMissingComponents = false)
        {
            return TryCreateVersion(input, preserveMissingComponents);
        }

        /// <summary>
        /// Creates a NuGetVersion from a string representing the semantic version.
        /// </summary>
        public static SemanticSortableVersion Parse(string value, bool preserveMissingComponents = false)
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
        public static SemanticSortableVersion? TryCreateVersion(string value, bool preserveMissingComponents = false)
        {
            // trim the value before passing it in since we're not strict here
            value = value?.Trim();
            var originalVersion = value;

            // also trim the leading v if it exists
            if (value?.FirstOrDefault() == 'v')
            {
                value = value.Substring(1);
            }

            if (!string.IsNullOrWhiteSpace(value))
            {
                var sections = utils.ParseSections(value);

                // null indicates the string did not meet the rules
                if (sections != null
                    && !string.IsNullOrEmpty(sections.Item1))
                {
                    var versionPart = sections.Item1;

                    if (versionPart.IndexOf('.') < 0)
                        // System.Version requires at least a 2 part version to parse.
                        versionPart += ".0";

                    if (Version.TryParse(versionPart, out var systemVersion))
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

                        if (originalVersion.IndexOf(' ') > -1)
                            originalVersion = value.Replace(" ", "");

                        return new SemanticSortableVersion(ver,
                            sections.Item2,
                            sections.Item3 ?? string.Empty,
                            originalVersion);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Parses a version string using strict SemVer rules.
        /// </summary>
        public static SemanticSortableVersion? TryParseStrict(string value)
        {
            var semVer = TryCreateVersion(value);
            return semVer != null
                ? new SemanticSortableVersion(semVer.Major,
                    semVer.Minor,
                    semVer.Patch,
                    0,
                    semVer.ReleaseLabels,
                    semVer.Metadata)
                : null;
        }
    }
}