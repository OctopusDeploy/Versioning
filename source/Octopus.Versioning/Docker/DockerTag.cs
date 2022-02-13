using System;
using Octopus.Versioning.Octopus;
using Octopus.Versioning.Semver;

namespace Octopus.Versioning.Docker
{
    public class DockerTag : OctopusVersion
    {
        const string Latest = "latest";

        public DockerTag(OctopusVersion version)
            : base(version.Prefix,
                version.Major,
                version.Minor,
                version.Patch,
                version.Revision,
                version.Release,
                version.ReleasePrefix,
                version.ReleaseCounter,
                version.Metadata,
                version.OriginalString)
        {
        }

        public DockerTag(string? prefix,
            int major,
            int minor,
            int patch,
            int revision,
            string? prerelease,
            string? prereleasePrefix,
            string? prereleaseCounter,
            string? metadata,
            string? originalVersion) : base(prefix,
            major,
            minor,
            patch,
            revision,
            prerelease,
            prereleasePrefix,
            prereleaseCounter,
            metadata,
            originalVersion)
        {
        }

        public override VersionFormat Format => VersionFormat.Docker;

        public override bool IsPrerelease => !string.IsNullOrEmpty(Release) && OriginalString != Latest;

        public override int GetHashCode()
        {
            if (OriginalString == Latest)
            {
                return Latest.GetHashCode();
            }

            var hashCode = Major;
            hashCode = (hashCode * 397) ^ Minor;
            hashCode = (hashCode * 397) ^ Patch;
            hashCode = (hashCode * 397) ^ Revision;
            hashCode = (hashCode * 397) ^ Release.GetHashCode();
            return hashCode;
        }
    }
}