using System;
using System.Text.RegularExpressions;

namespace Octopus.Versioning.Unsortable
{
    public class UnsortableVersionParser
    {
        const string Release = "release";
        const string Meta = "buildmetadata";

        static readonly Regex VersionRegex = new Regex($@"^(?<{Release}>([A-Za-z0-9]*?)([.\-_\\]([A-Za-z0-9.\-_\\]*?)?)?)?" +
            $@"(?:\+(?<{Meta}>[A-Za-z0-9_\-.\\+]*?))?\s*$"
        );

        public UnsortableSortableVersion Parse(string? version)
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

            return new UnsortableSortableVersion(
                result.Groups[Release].Success ? result.Groups[Release].Value : string.Empty,
                result.Groups[Meta].Success ? result.Groups[Meta].Value : string.Empty,
                noSpaces
            );
        }

        public bool TryParse(string version, out UnsortableSortableVersion parsedSortableVersion)
        {
            try
            {
                parsedSortableVersion = Parse(version);
                return true;
            }
            catch
            {
                parsedSortableVersion = new UnsortableSortableVersion(
                    string.Empty,
                    string.Empty,
                    null
                );
                return false;
            }
        }
    }
}