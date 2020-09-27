using System;
using System.Collections.Generic;
using System.Linq;

namespace Octopus.Versioning.Octopus
{
    public class OctopusVersion : IVersion
    {
        public OctopusVersion(int major,
            int minor,
            int patch,
            int revision,
            string prerelease,
            string prereleasePrefix,
            string prereleaseCounter,
            string metadata,
            string originalVersion)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Revision = revision;
            Release = prerelease;
            ReleasePrefix = prereleasePrefix;
            ReleaseCounter = prereleaseCounter;
            Metadata = metadata;
            OriginalString = originalVersion;
        }

        public int CompareTo(object obj)
        {
            if (obj is IVersion objVersion)
            {
                if (Major.CompareTo(objVersion.Major) != 0) return Major.CompareTo(objVersion.Major);
                if (Minor.CompareTo(objVersion.Minor) != 0) return Minor.CompareTo(objVersion.Minor);
                if (Patch.CompareTo(objVersion.Patch) != 0) return Patch.CompareTo(objVersion.Patch);
                if (Revision.CompareTo(objVersion.Revision) != 0) return Revision.CompareTo(objVersion.Revision);
                if (string.Compare(Release ?? string.Empty, objVersion.Release ?? string.Empty, StringComparison.Ordinal) != 0)
                    return string.Compare(Release ?? string.Empty, objVersion.Release ?? string.Empty, StringComparison.Ordinal);
                if (string.Compare(Metadata ?? string.Empty, objVersion.Metadata ?? string.Empty, StringComparison.Ordinal) != 0)
                    return string.Compare(Metadata ?? string.Empty, objVersion.Metadata ?? string.Empty, StringComparison.Ordinal);

                return 0;
            }

            return -1;
        }

        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }
        public int Revision { get; }
        public bool IsPrerelease { get; }
        public IEnumerable<string> ReleaseLabels => Enumerable.Empty<string>();
        public string? Metadata { get; }
        public string Release { get; }
        public string ReleasePrefix { get; }
        public string ReleaseCounter { get; }
        public bool HasMetadata => string.IsNullOrWhiteSpace(Metadata);
        public string? OriginalString { get; }
        public VersionFormat Format => VersionFormat.Octopus;
    }
}