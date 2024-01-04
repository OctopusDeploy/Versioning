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
        /// Note that we don't expect to see versions with spaces around the major, minor, patch, and release.
        /// All the clients that can create an octopus version (the web ui, cli, library, and REST API) strip
        /// spaces. However, the SemVerFactory.Parse() and SemVerFactory.TryParse() methods did accept
        /// versions with these spaces. This led to cases where old Octopus instances had release versions with
        /// spaces in them. To retain compatibility with the existing parsing logic, we tolerate whitespace
        /// around version integers.
        ///
        /// See https://github.com/OctopusDeploy/Versioning/pull/20 and https://github.com/OctopusDeploy/Issues/issues/6826
        /// for more details.
        /// </summary>
        static readonly Regex VersionRegex = new Regex(@"^\s*(?:" +
            // Versions can start with an optional V
            @$"\s*(?<{Prefix}>v|V)?" +
            // Get the major version number
            @$"\s*(?<{Major}>\d+\b)\s*" +
            // Get the minor version number, delimited by a period
            @$"(?:\.\s*(?<{Minor}>\d+\b)\s*)?" +
            // Get the patch version number, delimited by a period
            @$"(?:\.\s*(?<{Patch}>\d+\b)\s*)?" +
            // Get the revision version number, delimited by a period
            @$"(?:\.\s*(?<{Revision}>\d+\b)\s*)?)?" +
            // Everything after the last digit and before the plus is the prerelease
            @$"(?:[.\-_\\])?(?<{Prerelease}>(?<{PrereleasePrefix}>[A-Za-z0-9]*?)([.\-_\\](?<{PrereleaseCounter}>[A-Za-z0-9.\-_\\]*?)?)?)?" +
            // The metadata is everything after the plus
            $@"(?:\+(?<{Meta}>[A-Za-z0-9_\-.\\+]*?))?\s*$");

        public OctopusVersion Parse(string? version)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(version))
                    throw new ArgumentException("The version can not be an empty string");

                var sanitisedVersion = version ?? string.Empty;
                // SemVerFactory treated the original string as if it had no spaces at all
                var noSpaces = sanitisedVersion.Replace(" ", "");

                // We parse on the original string. This *does not* tolerate spaces in prerelease fields or metadata
                // just like SemVerFactory.
                var result = VersionRegex.Match(sanitisedVersion);

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
                    noSpaces);
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