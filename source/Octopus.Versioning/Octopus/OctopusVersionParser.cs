using System;
using System.Text.RegularExpressions;

namespace Octopus.Versioning.Octopus
{
    public class OctopusVersionParser
    {
        const string Prefix = "prefix";
        const string Major = "major";
        const string Minor = "minor";
        const string MinorGroup = "minorgroup";
        const string Patch = "patch";
        const string PatchGroup = "patchgroup";
        const string Revision = "revision";
        const string RevisionGroup = "revisiongroup";
        const string Prerelease = "prerelease";
        const string PrereleasePrefix = "prereleaseprefix";
        const string PrereleaseCounter = "prereleasecounter";
        const string Meta = "buildmetadata";

        static readonly Regex VersionRegex = new Regex(@"^(?:" +
            // Versions can start with an optional V
            @$"(?<{Prefix}>v|V)?" +
            // Get the major version number
            @$"(?<{Major}>\d+)" +
            // Get the minor version number, delimited by a period, comma, dash or underscore
            @$"(?<{MinorGroup}>[.\-_](?<{Minor}>\d+))?" +
            // Get the patch version number, delimited by a period, comma, dash or underscore
            @$"(?<{PatchGroup}>[.\-_](?<{Patch}>\d+))?" +
            // Get the revision version number, delimited by a period, comma, dash or underscore
            @$"(?<{RevisionGroup}>[.\-_](?<{Revision}>\d+))?)?" +
            // Everything after the last digit and before the plus is the prerelease
            @$"(?:[.\-_])?(?<{Prerelease}>(?<{PrereleasePrefix}>[^+.\-_\s]*?)([.\-_](?<{PrereleaseCounter}>[^+\s]*?)?)?)?" +
            // The metadata is everything after the plus
            $@"(?:\+(?<{Meta}>[^\s]*?))?$");

        public OctopusVersion Parse(string? version)
        {

            var result = VersionRegex.Match(version?.Trim() ?? string.Empty);
                            // Version numbers must be ints
            bool majorIsInt = result.Groups[Major].Success && int.TryParse(result.Groups[Major].Value, out _);
            bool minorIsInt = majorIsInt && result.Groups[Minor].Success && int.TryParse(result.Groups[Minor].Value, out _);
            bool patchIsInt = minorIsInt && result.Groups[Patch].Success && int.TryParse(result.Groups[Patch].Value, out _);
            bool revisionIsInt = patchIsInt && result.Groups[Revision].Success && int.TryParse(result.Groups[Revision].Value, out _);

            // The first field that isn't an int marks the beginning of the prerelease
            var fieldPreRelease = "";
            if (!revisionIsInt) fieldPreRelease = result.Groups[Revision].Value;
            if (!patchIsInt) fieldPreRelease = result.Groups[Patch].Value + result.Groups[RevisionGroup].Value;
            if (!minorIsInt) fieldPreRelease = result.Groups[Minor].Value + result.Groups[PatchGroup].Value + result.Groups[RevisionGroup].Value;
            if (!majorIsInt) fieldPreRelease = result.Groups[Major].Value + result.Groups[MinorGroup].Value + result.Groups[PatchGroup].Value + result.Groups[RevisionGroup].Value;

            return new OctopusVersion(
                string.Empty,
                majorIsInt ? int.Parse(result.Groups[Major].Value) : 0,
                minorIsInt ? int.Parse(result.Groups[Minor].Value) : 0,
                patchIsInt ? int.Parse(result.Groups[Patch].Value) : 0,
                revisionIsInt ? int.Parse(result.Groups[Revision].Value) : 0,
                fieldPreRelease + (result.Groups[Prerelease].Success ? result.Groups[Prerelease].Value : string.Empty),
                fieldPreRelease + (result.Groups[PrereleasePrefix].Success ? result.Groups[PrereleasePrefix].Value : string.Empty),
                result.Groups[PrereleaseCounter].Success ? result.Groups[PrereleaseCounter].Value : string.Empty,
                result.Groups[Meta].Success ? result.Groups[Meta].Value : string.Empty,
                version);
            
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