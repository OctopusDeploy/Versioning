using System;
using Octopus.Versioning.Octopus;

namespace Octopus.Versioning.Docker
{
    public class DockerTag : OctopusVersion
    {
        const string Latest = "latest";

        public DockerTag(int major,
            int minor,
            int patch,
            int revision,
            string prerelease,
            string prereleasePrefix,
            string prereleaseCounter,
            string metadata,
            string originalVersion) : base(major, minor, patch, revision, prerelease, prereleasePrefix, prereleaseCounter, metadata, originalVersion)
        {

        }

        public VersionFormat Format => VersionFormat.Docker;

        public int CompareTo(object obj)
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