﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Octopus.Versioning.Maven
{
    public class MavenVersion : IVersion
    {
        public MavenVersion(int major,
            int minor,
            int patch,
            int revision,
            IEnumerable<string>? releaseLabels,
            string originalVersion)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Revision = revision;
            ReleaseLabels = releaseLabels ?? Enumerable.Empty<string>();
            OriginalString = originalVersion;
        }

        public long Major { get; }
        public long Minor { get; }
        public long Patch { get; }
        public long Revision { get; }

        public bool IsPrerelease => ReleaseLabels.Any(label =>
        {
            return label != null &&
                (label.Equals("SNAPSHOT", StringComparison.OrdinalIgnoreCase) ||
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
                    return string.Join(".", ReleaseLabels);

                return string.Empty;
            }
        }

        public string? Metadata => null;
        public bool HasMetadata => false;
        public string OriginalString { get; }

        public VersionFormat Format => VersionFormat.Maven;

        public int CompareTo(object obj)
        {
            return new ComparableVersion(OriginalString)
                .CompareTo(new ComparableVersion((obj as MavenVersion)?.OriginalString ?? ""));
        }

        public override string ToString()
        {
            return OriginalString ?? "";
        }

        public override bool Equals(object obj)
        {
            return CompareTo(obj) == 0;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)Major;
                hashCode = (hashCode * 397) ^ (int)Minor;
                hashCode = (hashCode * 397) ^ (int)Patch;
                hashCode = (hashCode * 397) ^ (int)Revision;
                hashCode = (hashCode * 397) ^ (ReleaseLabels != null ? ReleaseLabels.GetHashCode() : 0);
                return (int)hashCode;
            }
        }
    }
}