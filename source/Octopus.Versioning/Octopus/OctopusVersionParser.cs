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
        static readonly Regex VersionRegex = new Regex(@$"^(?:(v|V)?(?<{Major}>[0-9]\d*)(?:[.\-_](?<{Minor}>[0-9]\d*))?(?:[.\-_](?<{Patch}>[0-9]\d*))?((?:[.\-_])?(?<{Revision}>[0-9]\d*))?)?(?:[.\-_])?(?<{Prerelease}>(?<{PrereleasePrefix}>[^+.\-_\s]*?)([.\-_](?<{PrereleaseCounter}>[^+\s]*?)?)?)?(?:\+(?<{Meta}>[^\s]*?))?$");

        public OctopusVersion Parse(string version)
        {
            var result = VersionRegex.Match(version);
            return new OctopusVersion(
                result.Groups[Major].Success ? int.Parse(result.Groups[Major].Value) : 0,
                result.Groups[Minor].Success ? int.Parse(result.Groups[Minor].Value) : 0,
                result.Groups[Patch].Success ? int.Parse(result.Groups[Patch].Value) : 0,
                result.Groups[Revision].Success ? int.Parse(result.Groups[Revision].Value) : 0,
                result.Groups[Prerelease].Success ? result.Groups[Prerelease].Value : string.Empty,
                result.Groups[PrereleasePrefix].Success ? result.Groups[PrereleasePrefix].Value : string.Empty,
                result.Groups[PrereleaseCounter].Success ? result.Groups[PrereleaseCounter].Value : string.Empty,
                result.Groups[Meta].Success ? result.Groups[Meta].Value : string.Empty,
                version);
        }
    }
}