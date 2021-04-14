using System;
using System.Text.RegularExpressions;

namespace Octopus.Versioning.Octopus
{
    public class OctopusVersionParser
    {
        const string Prefix = "prefix";
        const string Major = "major";
        const string Minor = "minor";
        const string Patch = "patch";
        const string Revision = "revision";
        const string Prerelease = "prerelease";
        const string PrereleasePrefix = "prereleaseprefix";
        const string PrereleaseCounter = "prereleasecounter";
        const string Meta = "buildmetadata";

        /// <summary>
        /// Versions can appear in URLs, and these characters are hard to work with in URLs, so we exclude them from any part of the version
        /// </summary>
        static readonly string[] IllegalChars = { "/", "%", "?", "#", "&" };

        static readonly Regex VersionRegex = new Regex(@"^(?:" +
            // Versions can start with an optional V
            @$"(?<{Prefix}>v|V)?" +
            // Get the major version number
            @$"(?<{Major}>\d+)" +
            // Get the minor version number, delimited by a period, comma, dash or underscore
            @$"(?:\.(?<{Minor}>\d+))?" +
            // Get the patch version number, delimited by a period, comma, dash or underscore
            @$"(?:\.(?<{Patch}>\d+))?" +
            // Get the revision version number, delimited by a period, comma, dash or underscore
            @$"(?:\.(?<{Revision}>\d+))?)?" +
            // Everything after the last digit and before the plus is the prerelease
            @$"(?:[.\-_\\])?(?<{Prerelease}>(?<{PrereleasePrefix}>[A-Za-z0-9]*?)([.\-_\\](?<{PrereleaseCounter}>[A-Za-z0-9.\-_\\]*?)?)?)?" +
            // The metadata is everything after the plus
            $@"(?:\+(?<{Meta}>[A-Za-z0-9_\-.\\+]*?))?$");

        public OctopusVersion Parse(string? version)
        {
            try
            {
                if ((version?.Trim() ?? string.Empty) == string.Empty)
                    throw new ArgumentException("The version can not be an empty string");

                var result = VersionRegex.Match((version ?? string.Empty).Trim());

                if (!result.Success)
                    throw new ArgumentException("The supplied version was not valid");

                return new OctopusVersion(
                    result.Groups[Prefix].Success ? result.Groups[Prefix].Value : string.Empty,
                    result.Groups[Major].Success ? int.Parse(result.Groups[Major].Value) : 0,
                    result.Groups[Minor].Success ? int.Parse(result.Groups[Minor].Value) : 0,
                    result.Groups[Patch].Success ? int.Parse(result.Groups[Patch].Value) : 0,
                    result.Groups[Revision].Success ? int.Parse(result.Groups[Revision].Value) : 0,
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

        public bool TryParse(string version, out OctopusVersion parsedVersion)
        {
            try
            {
                parsedVersion = Parse(version);
                return true;
            }
            catch
            {
                parsedVersion = new OctopusVersion(
                    string.Empty,
                    0,
                    0,
                    0,
                    0,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    "0.0.0");
                return false;
            }
        }
    }
}