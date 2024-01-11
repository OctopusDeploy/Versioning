// This class is based on NuGet's SemanticVersion class from https://github.com/NuGet/NuGet.Client
// NuGet is licensed under the Apache license: https://github.com/NuGet/NuGet.Client/blob/dev/LICENSE.txt

using System;

namespace Octopus.Versioning.Semver
{
    /// <summary>
    /// A base version operations
    /// </summary>
    public partial class StrictSemanticSortableVersion : IFormattable, IComparable<StrictSemanticSortableVersion>, IEquatable<StrictSemanticSortableVersion>
    {
        /// <summary>
        /// Gives a normalized representation of the version.
        /// </summary>
        public virtual string ToNormalizedString()
        {
            return ToString("N", new VersionFormatter());
        }

        public override string ToString()
        {
            return ToNormalizedString();
        }

        public virtual string ToString(string format, IFormatProvider formatProvider)
        {
            return TryFormatter(format, formatProvider) ?? ToString();
        }

        protected string? TryFormatter(string format, IFormatProvider formatProvider)
        {
            if (formatProvider?.GetFormat(GetType()) is ICustomFormatter formatter)
                return formatter.Format(format, this, formatProvider);
            return null;
        }

        public override int GetHashCode()
        {
            return VersionComparer.Default.GetHashCode(this);
        }

        public virtual int CompareTo(object obj)
        {
            return CompareTo(obj as StrictSemanticSortableVersion);
        }

        public virtual int CompareTo(StrictSemanticSortableVersion? other)
        {
            return CompareTo(other, VersionComparison.Default);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as StrictSemanticSortableVersion);
        }

        public virtual bool Equals(StrictSemanticSortableVersion? other)
        {
            return Equals(other, VersionComparison.Default);
        }

        /// <summary>
        /// True if the VersionBase objects are equal based on the given comparison mode.
        /// </summary>
        public virtual bool Equals(StrictSemanticSortableVersion? other, VersionComparison versionComparison)
        {
            return CompareTo(other, versionComparison) == 0;
        }

        /// <summary>
        /// Compares NuGetVersion objects using the given comparison mode.
        /// </summary>
        public virtual int CompareTo(StrictSemanticSortableVersion? other, VersionComparison versionComparison)
        {
            var comparer = new VersionComparer(versionComparison);
            return comparer.Compare(this, other);
        }

        /// <summary>
        /// ==
        /// </summary>
        public static bool operator ==(StrictSemanticSortableVersion? version1, StrictSemanticSortableVersion? version2)
        {
            return Compare(version1, version2) == 0;
        }

        /// <summary>
        /// !=
        /// </summary>
        public static bool operator !=(StrictSemanticSortableVersion? version1, StrictSemanticSortableVersion? version2)
        {
            return Compare(version1, version2) != 0;
        }

        public static bool operator <(StrictSemanticSortableVersion? version1, StrictSemanticSortableVersion? version2)
        {
            return Compare(version1, version2) < 0;
        }

        public static bool operator <=(StrictSemanticSortableVersion? version1, StrictSemanticSortableVersion? version2)
        {
            return Compare(version1, version2) <= 0;
        }

        /// <summary>
        /// >
        /// </summary>
        public static bool operator >(StrictSemanticSortableVersion? version1, StrictSemanticSortableVersion? version2)
        {
            return Compare(version1, version2) > 0;
        }

        /// <summary>
        /// >=
        /// </summary>
        public static bool operator >=(StrictSemanticSortableVersion? version1, StrictSemanticSortableVersion? version2)
        {
            return Compare(version1, version2) >= 0;
        }

        static int Compare(StrictSemanticSortableVersion? version1, StrictSemanticSortableVersion? version2)
        {
            IVersionComparer comparer = new VersionComparer();
            return comparer.Compare(version1, version2);
        }
    }
}