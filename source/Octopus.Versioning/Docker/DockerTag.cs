using System;
using Octopus.Versioning.Octopus;
using Octopus.Versioning.Semver;

namespace Octopus.Versioning.Docker
{
    public class DockerTag : OctopusSortableVersion
    {
        const string Latest = "latest";
        bool IsLatest => string.Compare(OriginalString, Latest, StringComparison.Ordinal) == 0;

        public DockerTag(OctopusSortableVersion sortableVersion)
            : base(sortableVersion.Prefix,
                sortableVersion.Major,
                sortableVersion.Minor,
                sortableVersion.Patch,
                sortableVersion.Revision,
                sortableVersion.Release,
                sortableVersion.ReleasePrefix,
                sortableVersion.ReleaseCounter,
                sortableVersion.Metadata,
                sortableVersion.OriginalString)
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