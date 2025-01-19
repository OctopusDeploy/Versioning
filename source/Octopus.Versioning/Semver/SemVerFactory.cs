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
        static readonly ISemanticVersionUtils Utils = new SemanticVersionUtils();

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

            // trim the value before passing it in since we're not strict here
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
            // Conditional value? call is not necessary according to the type signature,
            // but this API is old, and it's possible some old caller could supply a null.
            value = value?.Trim();
            var originalVersion = value;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            // also trim the leading v if it exists
            if (value?.FirstOrDefault() == 'v')
            {
                value = value.Substring(1);
            }

            if (!string.IsNullOrWhiteSpace(value))
            {
                var sections = Utils.ParseSections(value!);

                // null indicates the string did not meet the rules
                if (!string.IsNullOrEmpty(sections.Item1))
                {
                    var versionPart = sections.Item1;

                    if (versionPart.IndexOf('.') < 0)
                        // System.Version requires at least a 2 part version to parse.
                        versionPart += ".0";

                    if (Version.TryParse(versionPart, out var systemVersion))
                    {
                        // labels
                        if (sections.Item2 != null
                            && !sections.Item2.All(s => Utils.IsValidPart(s, true)))
                            return null;

                        // build metadata
                        if (sections.Item3 != null
                            && !Utils.IsValid(sections.Item3, true))
                            return null;

                        var ver = preserveMissingComponents
                            ? systemVersion
                            : Utils.NormalizeVersionValue(systemVersion);

                        if (originalVersion != null && originalVersion.IndexOf(' ') > -1)
                            originalVersion = value!.Replace(" ", "");

                        return new SemanticVersion(ver,
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
        public static SemanticVersion? TryParseStrict(string value)
        {
            var semVer = TryCreateVersion(value);
            return semVer != null
                ? new SemanticVersion(semVer.Major,
                    semVer.Minor,
                    semVer.Patch,
                    0,
                    semVer.ReleaseLabels,
                    semVer.Metadata)
                : null;
        }
    }
}