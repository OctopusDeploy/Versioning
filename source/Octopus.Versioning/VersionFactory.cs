using System;
using System.Collections.Generic;
using Octopus.Versioning.Docker;
using Octopus.Versioning.Maven;
using Octopus.Versioning.Octopus;
using Octopus.Versioning.Semver;
using Octopus.Versioning.Unsortable;

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
                case VersionFormat.Docker:
                    return CreateDockerTag(input);
                case VersionFormat.Octopus:
                    return CreateOctopusVersion(input);
                case VersionFormat.Unsortable:
                    return CreateUnsortableVersion(input);
                default:
                    return CreateSemanticVersion(input);
            }
        }

        public static IVersion? TryCreateVersion(string input, VersionFormat format)
        {
            switch (format)
            {
                case VersionFormat.Maven:
                    return TryCreateMavenVersion(input);
                case VersionFormat.Docker:
                    return TryCreateDockerTag(input);
                case VersionFormat.Octopus:
                    return TryCreateOctopusVersion(input);
                case VersionFormat.Unsortable:
                    return TryCreateUnsortableVersion(input);
                default:
                    return TryCreateSemanticVersion(input);
            }
        }

        public static ISortableVersion CreateMavenVersion(string input)
        {
            return new MavenVersionParser().Parse(input);
        }

        public static ISortableVersion CreateSemanticVersion(string input, bool preserveMissingComponents = false)
        {
            return SemVerFactory.CreateVersion(input, preserveMissingComponents);
        }

        public static ISortableVersion CreateSemanticVersion(int major, int minor, int patch, string releaseLabel)
        {
            return new SemanticVersion(major, minor, patch, releaseLabel);
        }

        public static ISortableVersion CreateSemanticVersion(int major, int minor, int patch)
        {
            return new SemanticVersion(major, minor, patch);
        }

        public static ISortableVersion CreateSemanticVersion(int major, int minor, int patch, int revision)
        {
            return new SemanticVersion(major, minor, patch, revision);
        }

        public static ISortableVersion CreateSemanticVersion(Version version, string? releaseLabel = null, string? metadata = null)
        {
            return new SemanticVersion(version, releaseLabel, metadata);
        }

        public static ISortableVersion CreateSemanticVersion(int major,
            int minor,
            int patch,
            int revision,
            IEnumerable<string> releaseLabels,
            string metadata,
            string originalVersion)
        {
            return new SemanticVersion(major,
                minor,
                patch,
                revision,
                releaseLabels,
                metadata);
        }

        public static ISortableVersion TryCreateMavenVersion(string input)
        {
            /*
             * Any version is valid for Maven
             */
            return new MavenVersionParser().Parse(input);
        }

        public static ISortableVersion? TryCreateSemanticVersion(string input, bool preserveMissingComponents = false)
        {
            return SemVerFactory.TryCreateVersion(input, preserveMissingComponents);
        }

        public static ISortableVersion? CreateSemanticVersionOrNone(string input, bool preserveMissingComponents = false)
        {
            return SemVerFactory.CreateVersionOrNone(input, preserveMissingComponents);
        }

        public static ISortableVersion CreateSemanticVersion(Version version,
            IEnumerable<string> releaseLabels,
            string metadata,
            string originalVersion)
        {
            return new SemanticVersion(
                version,
                releaseLabels,
                metadata,
                originalVersion);
        }

        public static ISortableVersion CreateDockerTag(string input)
        {
            return new DockerTag(new OctopusVersionParser().Parse(input));
        }

        public static ISortableVersion? TryCreateDockerTag(string input)
        {
            try
            {
                return CreateDockerTag(input);
            }
            catch
            {
                // Version fields that are larger than ints are not supported and will result in an exception.
                return null;
            }
        }

        public static ISortableVersion CreateOctopusVersion(string input)
        {
            return new OctopusVersionParser().Parse(input);
        }

        public static ISortableVersion? TryCreateOctopusVersion(string input)
        {
            try
            {
                return CreateOctopusVersion(input);
            }
            catch
            {
                // Version fields that are larger than ints are not supported and will result in an exception.
                return null;
            }
        }
        
        public static IVersion CreateUnsortableVersion(string input)
        {
            return new UnsortableVersionParser().Parse(input);
        }

        public static IVersion? TryCreateUnsortableVersion(string input)
        {
            try
            {
                return CreateUnsortableVersion(input);
            }
            catch
            {
                return null;
            }
        }
    }
}