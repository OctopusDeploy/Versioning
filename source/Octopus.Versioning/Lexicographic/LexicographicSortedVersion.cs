using System;
using System.Collections.Generic;
using System.Linq;

namespace Octopus.Versioning.Lexicographic
{
    public class LexicographicSortedVersion: IVersion
    {
        public LexicographicSortedVersion(string release, string? metadata, string? originalString)
        {
            Metadata = metadata;
            Release = release;
            OriginalString = originalString ?? string.Empty;
        }

        public int Major => 0;
        public int Minor => 0;
        public int Patch => 0;
        public int Revision => 0;
        public bool IsPrerelease => false;
        public IEnumerable<string> ReleaseLabels => Enumerable.Empty<string>();
        public string? Metadata { get; }
        public bool HasMetadata => !string.IsNullOrWhiteSpace(Metadata);
        public string Release { get; }
        public string OriginalString { get; }

        public VersionFormat Format => VersionFormat.Lexicographic;
        
        public int CompareTo(object obj)
        {
            if (!(obj is IVersion objVersion))
                return -1;
            
            if (string.Compare(Release.AlphaNumericOnly(), (objVersion.Release ?? string.Empty).AlphaNumericOnly(), StringComparison.Ordinal) != 0)
                return CompareReleaseLabels(Release.AlphaNumericOnly().Split('.', '-', '_'), (objVersion.Release ?? string.Empty).AlphaNumericOnly().Split('.', '-', '_'));
                
            return 0;
        }
        
        public override string ToString()
        {
            return OriginalString;
        }

        public override bool Equals(object obj)
        {
            if (obj is IVersion objVersion)
                return CompareTo(objVersion) == 0;

            return false;
        }
        
        public override int GetHashCode()
        {
            return Release.GetHashCode();
        }
        
        /// <summary>
        /// Compares sets of release labels.
        /// </summary>
        static int CompareReleaseLabels(IEnumerable<string> version1, IEnumerable<string> version2)
        {
            var result = 0;

            using var a = version1.GetEnumerator();
            using var b = version2.GetEnumerator();

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
                var stringCompareResult = StringComparer.OrdinalIgnoreCase.Compare(version1, version2);
                if (stringCompareResult < 0)
                {
                    result = -1;
                }
                else if (stringCompareResult > 0)
                {
                    result = 1;
                }
            }

            return result;
        }
    }
}