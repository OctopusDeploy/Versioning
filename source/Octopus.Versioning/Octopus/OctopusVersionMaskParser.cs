using System;
using System.Text.RegularExpressions;

namespace Octopus.Versioning.Octopus
{
    public class OctopusVersionMaskParser
    {
        public const string PatternIncrement = "i";
        public const string PatternCurrent = "c";
        const string Prefix = "prefix";
        const string Major = "major";
        const string Minor = "minor";
        const string Patch = "patch";
        const string Revision = "revision";
        const string Prerelease = "prerelease";
        const string PrereleasePrefix = "prereleaseprefix";
        const string PrereleaseCounter = "prereleasecounter";
        const string Meta = "buildmetadata";

        static readonly Regex VersionRegex = new Regex(@"^(?:" +
            // Versions can start with an optional V
            @$"(?<{Prefix}>v|V)?" +
            // Get the major version number
            @$"(?<{Major}>\d+|{PatternIncrement}|{PatternCurrent})" +
            // Get the minor version number, delimited by a period, comma, dash or underscore
            @$"(?:\.(?<{Minor}>\d+|{PatternIncrement}|{PatternCurrent}))?" +
            // Get the patch version number, delimited by a period, comma, dash or underscore
            @$"(?:\.(?<{Patch}>\d+|{PatternIncrement}|{PatternCurrent}))?" +
            // Get the revision version number, delimited by a period, comma, dash or underscore
            @$"(?:\.(?<{Revision}>\d+|{PatternIncrement}|{PatternCurrent}))?)?" +
            // Everything after the last digit and before the plus is the prerelease
            @$"(?:-(?<{Prerelease}>(?<{PrereleasePrefix}>[^+.\-_\s]*?)([.\-_](?<{PrereleaseCounter}>[^+\s]*?)?)?)?)?" +
            // The metadata is everything after the plus
            $@"(?:\+(?<{Meta}>[^\s]*?))?$");

        public OctopusVersionMask Parse(string? version)
        {
            var result = VersionRegex.Match(version?.Trim() ?? string.Empty);
            return new OctopusVersionMask(
                result.Groups[Prefix].Success ? result.Groups[Prefix].Value : string.Empty,
                new OctopusVersionMask.Component(result.Groups[Major]),
                new OctopusVersionMask.Component(result.Groups[Minor]),
                new OctopusVersionMask.Component(result.Groups[Patch]),
                new OctopusVersionMask.Component(result.Groups[Revision]),
                new OctopusVersionMask.TagComponent(result.Groups[Prerelease]),
                new OctopusVersionMask.MetadataComponent(result.Groups[Meta]));
        }
        
        OctopusVersionMask? TryParse(string? version)
        {
            var result = VersionRegex.Match(version?.Trim() ?? string.Empty);

            return !result.Success ? null : Parse(version);
        }

        public IVersion ApplyMask(string? mask, IVersion? currentVersion)
        {
            var parsedMask = TryParse(mask);
            
            /*
             * Watch out for this! A mask can return false for IsMask, but still succeed
             * parsing here and proceed to apply the masking logic. See OctopusVersionMaskParserTests.IsMask
             * for edge cases where masks fail IsMask but still work as masks.
             */
            if (parsedMask == null)
                return new OctopusVersionParser().Parse(mask);

            return currentVersion == null
                ? parsedMask.GenerateVersionFromMask()
                : parsedMask.GenerateVersionFromCurrent(Parse(currentVersion.OriginalString));
        }
    }
}