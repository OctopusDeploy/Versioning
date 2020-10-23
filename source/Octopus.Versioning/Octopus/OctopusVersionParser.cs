using System;
using System.Text.RegularExpressions;

namespace Octopus.Versioning.Octopus
{
    public class OctopusVersionParser
    {
        const string Major = "major";
        const string Minor = "minor";
        const string Patch = "patch";
        const string Revision = "revision";
        const string Prerelease = "prerelease";
        const string PrereleasePrefix = "prereleaseprefix";
        const string PrereleaseCounter = "prereleasecounter";
        const string Meta = "buildmetadata";
        static readonly Regex VersionRegex = new Regex(@$"^(?:" +
            // Versions can start with an optional V
            @$"(v|V)?" +
            // Get the major version number
            @$"(?<{Major}>\d+)" +
            // Get the minor version number, delimited by a period, comma, dash or underscore
            @$"(?:[.\-_](?<{Minor}>\d+))?" +
            // Get the patch version number, delimited by a period, comma, dash or underscore
            @$"(?:[.\-_](?<{Patch}>\d+))?" +
            // Get the revision version number, delimited by a period, comma, dash or underscore
            @$"(?:[.\-_](?<{Revision}>\d+))?)?" +
            // Everything after the last digit and before the plus is the prerelease
            @$"(?:[.\-_])?(?<{Prerelease}>(?<{PrereleasePrefix}>[^+.\-_\s]*?)([.\-_](?<{PrereleaseCounter}>[^+\s]*?)?)?)?" +
            // The metadata is everything after the plus
            $@"(?:\+(?<{Meta}>[^\s]*?))?$");

        public OctopusVersion Parse(string version)
        {
            try
            {
                var result = VersionRegex.Match(version?.Trim() ?? string.Empty);
                return new OctopusVersion(
                    result.Groups[Major].Success ? long.Parse(result.Groups[Major].Value) : 0,
                    result.Groups[Minor].Success ? long.Parse(result.Groups[Minor].Value) : 0,
                    result.Groups[Patch].Success ? long.Parse(result.Groups[Patch].Value) : 0,
                    result.Groups[Revision].Success ? long.Parse(result.Groups[Revision].Value) : 0,
                    result.Groups[Prerelease].Success ? result.Groups[Prerelease].Value : string.Empty,
                    result.Groups[PrereleasePrefix].Success ? result.Groups[PrereleasePrefix].Value : string.Empty,
                    result.Groups[PrereleaseCounter].Success ? result.Groups[PrereleaseCounter].Value : string.Empty,
                    result.Groups[Meta].Success ? result.Groups[Meta].Value : string.Empty,
                    version ?? string.Empty);
            }
            catch (OverflowException ex)
            {
                throw new OverflowException($"Failed to parse the version {version?.Trim()} because the major, minor, patch or revision fields were too large.", ex);
            }
        }
    }
}