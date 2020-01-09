using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Octopus.Versioning
{
    public class OctoVersion : IComparable<OctoVersion>, IEquatable<OctoVersion>
    {
        readonly string version;
        readonly Lazy<SemVer> semVer;
        
        public OctoVersion(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
                throw new ArgumentException("Version can not be null or empty", nameof(version));
                
            this.version = version;
            semVer = new Lazy<SemVer>(() => SemVer.ParseSemVer(this.version));
        }

        private SemVer AsSemVer => semVer.Value; 

        public int CompareTo(OctoVersion other)
        {
            if (ReferenceEquals(this, other))
                return 0;

            if (ReferenceEquals(other, null))
                return 1;

            for (var i = 0; i < AsSemVer.CoreComponents.Count || i < other.AsSemVer.CoreComponents.Count; i++)
            {
                if (i >= AsSemVer.CoreComponents.Count)
                    return -1;
                    
                if (i >= other.AsSemVer.CoreComponents.Count)
                    return 1;

                var result = AsSemVer.CoreComponents[i].CompareTo(other.AsSemVer.CoreComponents[i]);
                
                if (result != 0)
                    return result;
            }

            for (var i = 0; i < AsSemVer.PreReleaseComponents.Count || i < other.AsSemVer.PreReleaseComponents.Count; i++)
            {
                if (i >= AsSemVer.PreReleaseComponents.Count)
                    return i == 0 ? 1 : -1;

                if (i >= other.AsSemVer.PreReleaseComponents.Count)
                    return i == 0 ? -1 : 1;

                var thisComponent = AsSemVer.PreReleaseComponents[i];
                var otherComponent = other.AsSemVer.PreReleaseComponents[i];

                var thisComponentIsNumeric = int.TryParse(thisComponent, out var thisNumericComponent);
                var otherComponentIsNumeric = int.TryParse(otherComponent, out var otherNumericComponent);

                if (thisComponentIsNumeric && otherComponentIsNumeric)
                {
                    var result = thisNumericComponent.CompareTo(otherNumericComponent);
                    if (result != 0)
                        return result;
                }

                if (thisComponentIsNumeric)
                    return -1;

                if (otherComponentIsNumeric)
                    return 1;

                var stringComparisonResult = StringComparer.OrdinalIgnoreCase.Compare(thisComponent, otherComponent);

                if (stringComparisonResult != 0)
                    return stringComparisonResult;
            }

            return 0;
        }

        public bool Equals(OctoVersion other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (ReferenceEquals(other, null))
                return false;

            return this.version.ToLowerInvariant().Equals(other.version.ToLowerInvariant());
        }

        public override int GetHashCode()
        {
            return version.ToLowerInvariant().GetHashCode();
        }

        public override string ToString()
        {
            return version;
        }

        class SemVer
        {
            const string VersionCorePattern = @"^(?<core>(\d+(\.\d+){0,3}))";
            const string PreReleasePattern = @"(-(?<preRelease>([0-9A-Za-z-]+(\.[0-9A-Za-z-]+)*)))?";
            const string BuildMetadataPattern = @"(\+[0-9A-Za-z-]+(\.[0-9A-Za-z-]+)*)?"; 
            
            static readonly Regex SemVerRegex = new Regex(
              VersionCorePattern + PreReleasePattern + BuildMetadataPattern,
              RegexOptions.Compiled);

            public IList<int> CoreComponents { get; }
            public IList<string> PreReleaseComponents { get; }

            SemVer(IList<int> coreComponents, IList<string> preReleaseComponents)
            {
                CoreComponents = coreComponents;
                PreReleaseComponents = preReleaseComponents;
            }
            
            public static SemVer ParseSemVer(string version)
            {
                var match = SemVerRegex.Match(version);

                if (match.Success)
                {
                    var coreMatch = match.Groups["core"];
                    var preReleaseMatch = match.Groups["preRelease"];
                    return new SemVer(
                        coreMatch.Value.Split('.').Select(int.Parse).ToList(),
                        preReleaseMatch.Success
                            ? preReleaseMatch.Value.Split('.').ToList()
                            : new List<string>());
                }

                return new SemVer(
                    new List<int> {0, 0, 0},
                    version.Split('.').ToList());
            }
            
        }
    }
}