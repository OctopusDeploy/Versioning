using System;
using System.Collections.Generic;

namespace Octopus.Versioning.Semver
{
    public interface ISemanticVersionUtils
    {
        IEnumerable<string>? ParseReleaseLabels(string? releaseLabels);
        /// <summary>
        /// Creates a legacy version string using System.Version
        /// </summary>
        string GetLegacyString(Version version, IEnumerable<string>? releaseLabels, string? metadata);

        Version NormalizeVersionValue(Version version);

        /// <summary>
        /// Parse the version string into version/release/build
        /// The goal of this code is to take the most direct and optimized path
        /// to parsing and validating a semver. Regex would be much cleaner, but
        /// due to the number of versions created in NuGet Regex is too slow.
        /// </summary>
        Tuple<string, string[], string?> ParseSections(string value);

        bool IsValid(string s, bool allowLeadingZeros);
        bool IsValidPart(string s, bool allowLeadingZeros);
        bool IsValidPart(char[] chars, bool allowLeadingZeros);
        bool IsLetterOrDigitOrDash(char c);
        string IncrementRelease(string release);
    }
}