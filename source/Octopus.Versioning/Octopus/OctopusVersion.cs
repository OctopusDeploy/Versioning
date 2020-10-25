using System;
using System.Collections.Generic;
using System.Linq;

namespace Octopus.Versioning.Octopus
{
    public class OctopusVersion : IVersion
    {
        public OctopusVersion(string? prefix,
            int major,
            int minor,
            int patch,
            int revision,
            string? prerelease,
            string? prereleasePrefix,
            string? prereleaseCounter,
            string? metadata,
            string? originalVersion)
        {
            Prefix = prefix ?? string.Empty;
            Major = major;
            Minor = minor;
            Patch = patch;
            Revision = revision;
            Release = prerelease ?? string.Empty;
            ReleasePrefix = prereleasePrefix ?? string.Empty;
            ReleaseCounter = prereleaseCounter ?? string.Empty;
            Metadata = metadata ?? string.Empty;
            OriginalString = originalVersion ?? string.Empty;
        }

        public virtual int CompareTo(object obj)
        {
            if (obj is IVersion objVersion)
            {
                if (Major.CompareTo(objVersion.Major) != 0) return Major.CompareTo(objVersion.Major);
                if (Minor.CompareTo(objVersion.Minor) != 0) return Minor.CompareTo(objVersion.Minor);
                if (Patch.CompareTo(objVersion.Patch) != 0) return Patch.CompareTo(objVersion.Patch);
                if (Revision.CompareTo(objVersion.Revision) != 0) return Revision.CompareTo(objVersion.Revision);

                // anything with a release field is lower than anything without a release field
                if (!string.IsNullOrEmpty(Release) && string.IsNullOrEmpty(objVersion.Release)) return -1;
                if (!string.IsNullOrEmpty(objVersion.Release) && string.IsNullOrEmpty(Release)) return 1;


                if (obj is OctopusVersion objOctoVersion)
                {
                    // octopus versions break down the release into a prefix and counter that can be compared

                    if (string.Compare(ReleasePrefix ?? string.Empty, objOctoVersion.ReleasePrefix ?? string.Empty, StringComparison.Ordinal) != 0)
                        return string.Compare(ReleasePrefix ?? string.Empty, objOctoVersion.ReleasePrefix ?? string.Empty, StringComparison.Ordinal);

                    if (string.Compare(ReleaseCounter ?? string.Empty, objOctoVersion.ReleaseCounter ?? string.Empty, StringComparison.Ordinal) != 0)
                        return string.Compare(ReleaseCounter ?? string.Empty, objOctoVersion.ReleaseCounter ?? string.Empty, StringComparison.Ordinal);
                }
                else
                {
                    // otherwise string compare the release field

                    if (string.Compare(Release ?? string.Empty, objVersion.Release ?? string.Empty, StringComparison.Ordinal) != 0)
                        return string.Compare(Release ?? string.Empty, objVersion.Release ?? string.Empty, StringComparison.Ordinal);
                }

                if (string.Compare(Metadata ?? string.Empty, objVersion.Metadata ?? string.Empty, StringComparison.Ordinal) != 0)
                    return string.Compare(Metadata ?? string.Empty, objVersion.Metadata ?? string.Empty, StringComparison.Ordinal);

                return 0;
            }

            return -1;
        }

        public override string ToString()
        {
            return OriginalString;
        }

        public override bool Equals(object obj)
        {
            if (obj is IVersion objVersion)
            {
                if (Major != objVersion.Major ||
                    Minor != objVersion.Minor ||
                    Patch != objVersion.Patch ||
                    Revision != objVersion.Revision ||
                    Release != objVersion.Release) return false;

                return true;
            }

            return false;
        }

        public virtual string Prefix { get; }
        public virtual int Major { get; }
        public virtual int Minor { get; }
        public virtual int Patch { get; }
        public virtual int Revision { get; }
        public virtual bool IsPrerelease => !string.IsNullOrEmpty(Release);
        public virtual IEnumerable<string> ReleaseLabels => Enumerable.Empty<string>();
        public virtual string Metadata { get; }
        public virtual string Release { get; }
        public virtual string ReleasePrefix { get; }
        public virtual string ReleaseCounter { get; }
        public virtual bool HasMetadata => string.IsNullOrWhiteSpace(Metadata);
        public virtual string OriginalString { get; }
        public virtual VersionFormat Format => VersionFormat.Octopus;
    }
}