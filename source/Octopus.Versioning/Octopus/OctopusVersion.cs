using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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
            throw new System.NotImplementedException();
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