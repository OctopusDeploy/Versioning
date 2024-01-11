using System;
using System.Text.RegularExpressions;

namespace Octopus.Versioning.Maven
{
    /// <summary>
    /// A parser based on the Maven build helper VersionInformation class at
    /// https://github.com/mojohaus/build-helper-maven-plugin.
    /// </summary>
    public class MavenVersionParser
    {
        static readonly string MAJOR_MINOR_PATCH_PATTERN = "^((\\d+)(\\.(\\d+)(\\.(\\d+))?)?)";

        static readonly Regex MAJOR_MINOR_PATCH = new Regex(MAJOR_MINOR_PATCH_PATTERN);

        static readonly Regex DIGITS = new Regex(MAJOR_MINOR_PATCH_PATTERN + "(.*)$");

        static readonly Regex BUILD_NUMBER = new Regex("(?:(((\\-)(\\d+)(.*))?)|(\\.(.*))|(\\-(.*))|(.*))$");

        public int Major { get; private set; }

        public int Minor { get; private set; }

        public int Patch { get; private set; }

        public int BuildNumber { get; private set; }

        public string? Qualifier { get; private set; }

        public MavenSortableVersion Parse(string version)
        {
            var matcherDigits = DIGITS.Match(version);
            if (matcherDigits.Success)
            {
                ParseMajorMinorPatchVersion(matcherDigits.Groups[1].Value);
                ParseBuildNumber(matcherDigits.Groups[7].Value);
            }
            else
            {
                Qualifier = version;
            }

            return new MavenSortableVersion(
                Major,
                Minor,
                Patch,
                BuildNumber,
                Qualifier == null ? null : new[] { Qualifier },
                version
            );
        }

        void ParseBuildNumber(string buildNumberPart)
        {
            var matcher = BUILD_NUMBER.Match(buildNumberPart);
            if (matcher.Success)
            {
                var buildNumber = matcher.Groups[4].Value;
                var qualifier = matcher.Groups[5].Value;

                if (buildNumber.Trim().Length != 0)
                    BuildNumber = int.Parse(buildNumber);

                if (matcher.Groups[7].Value.Trim().Length != 0)
                    qualifier = matcher.Groups[7].Value;
                // Starting with "-"
                if (matcher.Groups[9].Value.Trim().Length != 0)
                    qualifier = matcher.Groups[9].Value;
                if (qualifier.Trim().Length != 0)
                {
                    if (qualifier.Trim().Length == 0)
                        Qualifier = null;
                    else
                        Qualifier = qualifier;
                }
                else
                {
                    Qualifier = null;
                }
            }
        }

        void ParseMajorMinorPatchVersion(string version)
        {
            var matcher = MAJOR_MINOR_PATCH.Match(version);
            if (matcher.Success)
            {
                var majorString = matcher.Groups[2].Value;
                var minorString = matcher.Groups[4].Value;
                var patchString = matcher.Groups[6].Value;

                if (majorString.Trim().Length != 0)
                    Major = int.Parse(majorString);
                if (minorString.Trim().Length != 0)
                    Minor = int.Parse(minorString);
                if (patchString.Trim().Length != 0)
                    Patch = int.Parse(patchString);
            }
        }
    }
}