using System;
using System.Collections.Generic;
using System.Linq;

namespace Octopus.Versioning.Octopus
{
    public class OctopusVersion : ISortableVersion
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
        public virtual bool HasMetadata => !string.IsNullOrWhiteSpace(Metadata);
        public virtual string OriginalString { get; }
        public virtual VersionFormat Format => VersionFormat.Octopus;

        public virtual int CompareTo(object obj)
        {
            if (obj is ISortableVersion objVersion)
            {
                if (Major.CompareTo(objVersion.Major) != 0) return Major.CompareTo(objVersion.Major);
                if (Minor.CompareTo(objVersion.Minor) != 0) return Minor.CompareTo(objVersion.Minor);
                if (Patch.CompareTo(objVersion.Patch) != 0) return Patch.CompareTo(objVersion.Patch);
                if (Revision.CompareTo(objVersion.Revision) != 0) return Revision.CompareTo(objVersion.Revision);

                // anything with a release field is lower than anything without a release field
                if (!string.IsNullOrEmpty(Release) && string.IsNullOrEmpty(objVersion.Release)) return -1;
                if (!string.IsNullOrEmpty(objVersion.Release) && string.IsNullOrEmpty(Release)) return 1;

                /*
                 * We only consider alpha numeric characters when comparing two versions. This means characters used in
                 * in typical version ranges like [1.0,2.0] or 1.0->2.0 (i.e. the comma, square brackets and greater than)
                 * have no meaning for equality and comparison checks. This in turn means that if versions do have these
                 * special characters, they can be replaced with any placeholder character that has no special meaning in
                 * the context of a range check.
                 *
                 * For example, the version 1.0-prerelease[10] can be placed in a version range like [1.0-prerelease-10-],
                 * because the square brackets have the same value as a dash when comparing versions.
                 */
                if (string.Compare(Release.AlphaNumericOnly(), (objVersion.Release ?? string.Empty).AlphaNumericOnly(), StringComparison.Ordinal) != 0)
                    return CompareReleaseLabels(Release.AlphaNumericOnly().Split('.', '-', '_'), (objVersion.Release ?? string.Empty).AlphaNumericOnly().Split('.', '-', '_'));

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
            if (obj is ISortableVersion objVersion)
                return CompareTo(objVersion) == 0;

            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = Major;
            hashCode = (hashCode * 397) ^ Minor;
            hashCode = (hashCode * 397) ^ Patch;
            hashCode = (hashCode * 397) ^ Revision;
            hashCode = (hashCode * 397) ^ Release.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Compares sets of release labels.
        /// </summary>
        static int CompareReleaseLabels(IEnumerable<string> version1, IEnumerable<string> version2)
        {
            var result = 0;

            var a = version1.GetEnumerator();
            var b = version2.GetEnumerator();

            var aExists = a.MoveNext();
            var bExists = b.MoveNext();

            while (aExists || bExists)
            {
                if (!aExists && bExists)
                    return -1;

                if (aExists && !bExists)
                    return 1;

                // compare the labels
                result = CompareRelease(a.Current, b.Current);

                if (result != 0)
                    return result;

                aExists = a.MoveNext();
                bExists = b.MoveNext();
            }

            return result;
        }

        /// <summary>
        /// Release labels are compared as numbers if they are numeric, otherwise they will be compared
        /// as strings.
        /// </summary>
        static int CompareRelease(string version1, string version2)
        {
            var version1Num = 0;
            var version2Num = 0;
            var result = 0;

            // check if the identifiers are numeric
            var v1IsNumeric = int.TryParse(version1, out version1Num);
            var v2IsNumeric = int.TryParse(version2, out version2Num);

            // if both are numeric compare them as numbers
            if (v1IsNumeric && v2IsNumeric)
            {
                result = version1Num.CompareTo(version2Num);
            }
            else if (v1IsNumeric || v2IsNumeric)
            {
                // numeric labels come before alpha labels
                if (v1IsNumeric)
                    result = -1;
                else
                    result = 1;
            }
            else
            {
                // Ignoring 2.0.0 case sensitive compare. Everything will be compared case insensitively as 2.0.1 specifies.
                result = StringComparer.OrdinalIgnoreCase.Compare(version1, version2);
            }

            return result;
        }
    }
}