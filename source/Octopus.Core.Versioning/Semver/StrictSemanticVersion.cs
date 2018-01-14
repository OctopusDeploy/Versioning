// This class is based on NuGet's SemanticVersion class from https://github.com/NuGet/NuGet.Client
// NuGet is licensed under the Apache license: https://github.com/NuGet/NuGet.Client/blob/dev/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;

namespace Octopus.Core.Versioning.Semver
{

    /// <summary>
    /// A strict SemVer implementation
    /// </summary>
    public partial class StrictSemanticVersion : IVersion
    {
        readonly IEnumerable<string> _releaseLabels;
        readonly string _metadata;
        protected readonly Version _version;

        /// <summary>
        /// Creates a SemanticVersion X.Y.Z
        /// </summary>
        /// <param name="major">X.y.z</param>
        /// <param name="minor">x.Y.z</param>
        /// <param name="patch">x.y.Z</param>
        public StrictSemanticVersion(int major, int minor, int patch)
            : this(major, minor, patch, Enumerable.Empty<string>(), null)
        {
        }

        /// <summary>
        /// Creates a NuGetVersion X.Y.Z-alpha.1.2#build01
        /// </summary>
        /// <param name="major">X.y.z</param>
        /// <param name="minor">x.Y.z</param>
        /// <param name="patch">x.y.Z</param>
        /// <param name="releaseLabels">Release labels that have been split by the dot separator</param>
        /// <param name="metadata">Build metadata</param>
        StrictSemanticVersion(int major, int minor, int patch, IEnumerable<string> releaseLabels, string metadata)
            : this(new Version(major, minor, patch, 0), releaseLabels, metadata)
        {
        }

        public StrictSemanticVersion(Version version, IEnumerable<string> releaseLabels, string metadata, bool preserveMissingComponents = false)
        {
            if (version == null)
            {
               throw new ArgumentException("version can not be null");
            }         

            _version = preserveMissingComponents
                ? version
                : SemanticVersionUtils.NormalizeVersionValue(version);

            _metadata = metadata;

            if (releaseLabels != null)
            {
                // enumerate the list
                _releaseLabels = releaseLabels.ToArray();
            }
        }

        /// <summary>
        /// Major version X (X.y.z)
        /// </summary>
        public int Major => _version.Major;

        /// <summary>
        /// Minor version Y (x.Y.z)
        /// </summary>
        public int Minor => _version.Minor;

        /// <summary>
        /// Patch version Z (x.y.Z)
        /// </summary>
        public int Patch => _version.Build;

        /// <summary>
        /// Revision version R (x.y.z.R)
        /// </summary>
        public int Revision => _version.Revision;

        /// <summary>
        /// A collection of pre-release labels attached to the version.
        /// </summary>
        public IEnumerable<string> ReleaseLabels
        {
            get { return _releaseLabels ?? Enumerable.Empty<string>(); }
        }

        /// <summary>
        /// The full pre-release label for the version.
        /// </summary>
        public string Release
        {
            get
            {
                if (_releaseLabels != null)
                {
                    return String.Join(".", _releaseLabels);
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// True if pre-release labels exist for the version.
        /// </summary>
        public virtual bool IsPrerelease
        {
            get
            {
                if (ReleaseLabels != null)
                {
                    var enumerator = ReleaseLabels.GetEnumerator();
                    return (enumerator.MoveNext() && !string.IsNullOrEmpty(enumerator.Current));
                }

                return false;
            }
        }

        /// <summary>
        /// True if metadata exists for the version.
        /// </summary>
        public virtual bool HasMetadata => !string.IsNullOrEmpty(Metadata);

        public virtual string OriginalString => null;

        /// <summary>
        /// Build metadata attached to the version.
        /// </summary>
        public virtual string Metadata => _metadata;
    }
}