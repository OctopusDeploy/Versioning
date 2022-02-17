using System;
using Octopus.Versioning.Octopus;
using Octopus.Versioning.Semver;

namespace Octopus.Versioning.Docker
{
    public class DockerTag : OctopusVersion
    {
        const string Latest = "latest";
        bool IsLatest => string.Compare(OriginalString, Latest, StringComparison.Ordinal) == 0;

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

        public override int CompareTo(object obj)
        {
            if (obj is DockerTag objDockerTag)
            {
                return IsLatest && objDockerTag.IsLatest ? 0 : base.CompareTo(obj);
            }

            return -1;
        }

        public override bool Equals(object obj)
        {
            if (obj is DockerTag objDockerTag)
                return CompareTo(objDockerTag) == 0;

            return false;
        }

        public override int GetHashCode()
        {

            return IsLatest ? Latest.GetHashCode() : base.GetHashCode();
        }
    }
}