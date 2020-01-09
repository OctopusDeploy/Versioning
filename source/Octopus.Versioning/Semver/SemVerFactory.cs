using System;
using System.Linq;
using Octopus.CoreUtilities;

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
            SemanticVersion ver = null;
            if (!TryCreateVersion(input, out ver, preserveMissingComponents))
            {
                throw new ArgumentException($"'{input}' is not a valid version string", nameof(input));
            }

            return ver;
        }
        
        public static Maybe<IVersion> CreateVersionOrNone(string input, bool preserveMissingComponents = false)
        {
            SemanticVersion ver;
            return TryCreateVersion(input, out ver, preserveMissingComponents)
                ? ((IVersion)ver).AsSome()
                : Maybe<IVersion>.None;
        }
        
        /// <summary>
        /// Creates a NuGetVersion from a string representing the semantic version.
        /// </summary>
        public static SemanticVersion Parse(string value, bool preserveMissingComponents = false)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Value cannot be null or an empty string", nameof(value));
            }

            SemanticVersion ver = null;
            if (!TryCreateVersion(value, out ver, preserveMissingComponents))
            {
                throw new ArgumentException($"'{value}' is not a valid version string", nameof(value));
            }

            return ver;
        }

        /// <summary>
        /// Parses a version string using loose semantic versioning rules that allows 2-4 version components followed
        /// by an optional special version.
        /// </summary>
        public static bool TryCreateVersion(string value, out SemanticVersion version, bool preserveMissingComponents = false)
        {
            version = null;

            if (value != null)
            {
                System.Version systemVersion = null;

                // trim the value before passing it in since we not strict here
                var sections = utils.ParseSections(value.Trim());

                // null indicates the string did not meet the rules
                if (sections != null
                    && !string.IsNullOrEmpty(sections.Item1))
                {
                    var versionPart = sections.Item1;

                    if (versionPart.IndexOf('.') < 0)
                    {
                        // System.Version requires at least a 2 part version to parse.
                        versionPart += ".0";
                    }

                    if (System.Version.TryParse(versionPart, out systemVersion))
                    {
                        // labels
                        if (sections.Item2 != null
                            && !sections.Item2.All(s => utils.IsValidPart(s, true)))
                        {
                            return false;
                        }

                        // build metadata
                        if (sections.Item3 != null
                            && !utils.IsValid(sections.Item3, true))
                        {
                            return false;
                        }

                        var ver = preserveMissingComponents
                            ? systemVersion
                            : utils.NormalizeVersionValue(systemVersion);

                        var originalVersion = value;

                        if (originalVersion.IndexOf(' ') > -1)
                        {
                            originalVersion = value.Replace(" ", "");
                        }

                        version = new SemanticVersion(version: ver,
                            releaseLabels: sections.Item2,
                            metadata: sections.Item3 ?? string.Empty,
                            originalVersion: originalVersion);

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Parses a version string using strict SemVer rules.
        /// </summary>
        public static bool TryParseStrict(string value, out SemanticVersion version)
        {
            version = null;

            if (TryCreateVersion(value, out var semVer))
            {
                version = new SemanticVersion(semVer.Major, semVer.Minor, semVer.Patch, 0, semVer.ReleaseLabels, semVer.Metadata);
            }

            return true;
        }
    }
}