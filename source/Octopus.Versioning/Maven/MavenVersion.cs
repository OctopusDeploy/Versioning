using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Octopus.Versioning.Maven
{
    public class MavenVersion : IVersion
    {
        readonly string originalVersion;
        
        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }
        public int Revision { get; }

        public bool IsPrerelease => ReleaseLabels.Any(label =>
        {
            return label != null && (label.Equals("SNAPSHOT", StringComparison.OrdinalIgnoreCase) ||
                   label.StartsWith("ALPHA", StringComparison.OrdinalIgnoreCase) ||
                   Regex.Match(label, "^[Aa]\\d+").Success ||
                   label.StartsWith("BETA", StringComparison.OrdinalIgnoreCase) ||
                   Regex.Match(label, "^[Bb]\\d+").Success ||
                   label.StartsWith("MILESTONE", StringComparison.OrdinalIgnoreCase) ||
                   Regex.Match(label, "^[Mm]\\d+").Success ||
                   label.StartsWith("CR", StringComparison.OrdinalIgnoreCase) ||
                   label.StartsWith("RC", StringComparison.OrdinalIgnoreCase));
        });

        public IEnumerable<string> ReleaseLabels { get; }

        public string Release
        {
            get
            {
                if (ReleaseLabels != null)
                {
                    return String.Join(".", ReleaseLabels);
                }

                return string.Empty;
            }
        }

        public string? Metadata => null;
        public bool HasMetadata => false;
        public string OriginalString => originalVersion;
        public VersionFormat Format => VersionFormat.Maven;

        public MavenVersion(int major, int minor, int patch, int revision, IEnumerable<string>? releaseLabels, string originalVersion)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Revision = revision;
            ReleaseLabels = releaseLabels ?? Enumerable.Empty<string>();
            this.originalVersion = originalVersion;
        }

        public int CompareTo(object obj)
        {
            return new ComparableVersion(originalVersion)
                .CompareTo(new ComparableVersion((obj as MavenVersion)?.originalVersion ?? ""));
        }

        public override string ToString()
        {
            return originalVersion ?? "";
        } 
        
        public override bool Equals(object obj)
        {
            return this.CompareTo(obj) == 0;
        }
        
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Major;
                hashCode = (hashCode * 397) ^ Minor;
                hashCode = (hashCode * 397) ^ Patch;
                hashCode = (hashCode * 397) ^ Revision;
                hashCode = (hashCode * 397) ^ (ReleaseLabels != null ? ReleaseLabels.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}