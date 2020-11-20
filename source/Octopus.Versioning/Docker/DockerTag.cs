using System;
using Octopus.Versioning.Octopus;

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

        public override int Major => IsLatest ? int.MaxValue : base.Major;
        public override int Minor => IsLatest ? int.MaxValue : base.Minor;
        public override int Patch => IsLatest ? int.MaxValue : base.Patch;
        public override int Revision => IsLatest ? int.MaxValue : base.Revision;

        bool IsLatest => base.Major == 0 &&
            base.Minor == 0 &&
            base.Patch == 0 &&
            base.Revision == 0 &&
            base.ReleasePrefix == Latest &&
            string.IsNullOrEmpty(base.ReleaseCounter) &&
            string.IsNullOrEmpty(base.Metadata);

        public override VersionFormat Format => VersionFormat.Docker;

        public override bool IsPrerelease => !string.IsNullOrEmpty(Release) && OriginalString != Latest;

        public override int CompareTo(object obj)
        {
            if (obj is IVersion objVersion)
            {
                if (OriginalString == Latest && objVersion.OriginalString == Latest) return 0;
                if (OriginalString == Latest) return 1;
                if (objVersion.OriginalString == Latest) return -1;
            }

            return base.CompareTo(obj);
        }
    }
}