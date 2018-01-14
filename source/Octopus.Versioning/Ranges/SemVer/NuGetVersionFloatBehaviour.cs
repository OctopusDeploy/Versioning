// This class is based on NuGet's NuGetVersionFloatBehavior enum from https://github.com/NuGetArchive/NuGet.Versioning
// NuGet is licensed under the Apache license: https://github.com/NuGetArchive/NuGet.Versioning/blob/dev/LICENSE.txt

namespace Octopus.Versioning.Ranges.SemVer
{
    public enum NuGetVersionFloatBehavior
    {
        /// <summary>
        /// Lowest version, no float
        /// </summary>
        None,

        /// <summary>
        /// Highest matching pre-release label
        /// </summary>
        Prerelease,

        /// <summary>
        /// x.y.z.*
        /// </summary>
        Revision,

        /// <summary>
        /// x.y.*
        /// </summary>
        Patch,

        /// <summary>
        /// x.*
        /// </summary>
        Minor,

        /// <summary>
        /// *
        /// </summary>
        Major,

        /// <summary>
        /// Float major and pre-release
        /// </summary>
        AbsoluteLatest
    }
}
