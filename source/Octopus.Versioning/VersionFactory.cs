using System;
using System.Collections.Generic;
using Octopus.Versioning.Maven;
using Octopus.Versioning.Semver;
using SemanticVersion = Octopus.Versioning.Semver.SemanticVersion;

namespace Octopus.Versioning
{
    public static class VersionFactory 
    {
        public static IVersion CreateVersion(string input, VersionFormat format)
        {
            switch (format)
            {
                case VersionFormat.Maven:
                    return CreateMavenVersion(input);
                default:
                    return CreateSemanticVersion(input);
            }
        }

        public static bool TryCreateVersion(string input, out IVersion version, VersionFormat format)
        {
            switch (format)
            {
                case VersionFormat.Maven:
                    return TryCreateMavenVersion(input, out version);
                default:
                    return TryCreateSemanticVersion(input, out version);
            }
        }

        public static IVersion CreateMavenVersion(string input)
        {
            return new MavenVersionParser().Parse(input);
        }

        public static IVersion CreateSemanticVersion(string input, bool preserveMissingComponents = false)
        {
            return SemVerFactory.CreateVersion(input, preserveMissingComponents);
        }

        public static IVersion CreateSemanticVersion(int major, int minor, int patch, string releaseLabel)
        {
            return new SemanticVersion(major, minor, patch, releaseLabel);
        }

        public static IVersion CreateSemanticVersion(int major, int minor, int patch)
        {
            return new SemanticVersion(major, minor, patch);
        }

        public static IVersion CreateSemanticVersion(int major, int minor, int patch, int revision)
        {
            return new SemanticVersion(major, minor, patch, revision);
        }

        public static IVersion CreateSemanticVersion(Version version, string releaseLabel = null, string metadata = null)
        {
            return new SemanticVersion(version, releaseLabel, metadata);
        }

        public static IVersion CreateSemanticVersion(int major, int minor, int patch, int revision,
            IEnumerable<string> releaseLabels,
            string metadata, string originalVersion)
        {
            return new SemanticVersion(major, minor, patch, revision, releaseLabels, metadata);
        }

        

        public static bool TryCreateMavenVersion(string input, out IVersion version)
        {
            /*
             * Any version is valid for Maven
             */
            version = new MavenVersionParser().Parse(input);
            return true;
        }

        public static bool TryCreateSemanticVersion(string input, out IVersion version, bool preserveMissingComponents = false)
        {
            var retValue = SemVerFactory.TryCreateVersion(input, out var semVersion, preserveMissingComponents);
            version = semVersion;
            return retValue;
        }

        public static IVersion CreateSemanticVersion(Version version, IEnumerable<string> releaseLabels, string metadata,
            string originalVersion)
        {
            return new SemanticVersion(
                version,
                releaseLabels,
                metadata,
                originalVersion);
        }
    }
}