using Octopus.Core.Resources.Versioning;
using Octopus.Core.Util;

namespace Octopus.Core.Resources.Metadata
{
    public class MetadataFactory : IMetadataFactory
    {
        static readonly IPackageIDParser MavenPackageIdParser = new MavenPackageIDParser();
        static readonly IPackageIDParser NugetPackageIdParser = new NuGetPackageIDParser();

        public BasePackageMetadata GetMetadataFromPackageID(string packageID, VersionFormat versionFormat)
        {
            return versionFormat == VersionFormat.Maven
                ? MavenPackageIdParser.GetMetadataFromPackageID(packageID)
                : NugetPackageIdParser.GetMetadataFromPackageID(packageID);
        }

        public bool TryGetMetadataFromPackageID(string packageID, out BasePackageMetadata metadata, VersionFormat format)
        {
            return format == VersionFormat.Maven
                ? MavenPackageIdParser.TryGetMetadataFromPackageID(packageID, out metadata)
                : NugetPackageIdParser.TryGetMetadataFromPackageID(packageID, out metadata);
        }

        public PackageMetadata GetMetadataFromPackageID(string packageID, string version, string extension,
            VersionFormat versionFormat)
        {
            return versionFormat == VersionFormat.Maven
                ? MavenPackageIdParser.GetMetadataFromPackageID(packageID, version, extension)
                : NugetPackageIdParser.GetMetadataFromPackageID(packageID, version, extension);
        }

        public PhysicalPackageMetadata GetMetadataFromPackageID(string packageID, string version, string extension,
            long size,
            string hash, VersionFormat versionFormat)
        {
            return versionFormat == VersionFormat.Maven
                ? MavenPackageIdParser.GetMetadataFromPackageID(packageID, version, extension, size, hash)
                : NugetPackageIdParser.GetMetadataFromPackageID(packageID, version, extension, size, hash);
        }

        public PackageMetadata GetMetadataFromPackageName(string packageFile, string[] extensions, VersionFormat versionFormat)
        {
            return versionFormat == VersionFormat.Maven
                ? MavenPackageIdParser.GetMetadataFromPackageName(packageFile, extensions)
                : NugetPackageIdParser.GetMetadataFromPackageName(packageFile, extensions);
        }

        public bool TryGetMetadataFromPackageName(string packageFile, string[] extensions,
            out PackageMetadata packageMetadata,
            VersionFormat versionFormat)
        {
            return versionFormat == VersionFormat.Maven
                ? MavenPackageIdParser.TryGetMetadataFromPackageName(packageFile, extensions, out packageMetadata)
                : NugetPackageIdParser.TryGetMetadataFromPackageName(packageFile, extensions, out packageMetadata);
        }

        public Maybe<PackageMetadata> TryGetMetadataFromPackageName(string packageFile, string[] extensions,
            VersionFormat format)
        {
            return format == VersionFormat.Maven
                ? MavenPackageIdParser.TryGetMetadataFromPackageName(packageFile, extensions)
                : NugetPackageIdParser.TryGetMetadataFromPackageName(packageFile, extensions);
        }

        public Maybe<PackageMetadata> TryGetMetadataFromPackageName(string packageFile, VersionFormat format)
        {
            return format == VersionFormat.Maven
                ? MavenPackageIdParser.TryGetMetadataFromPackageName(packageFile)
                : NugetPackageIdParser.TryGetMetadataFromPackageName(packageFile);
        }

        public PackageMetadata GetMetadataFromServerPackageName(string packageFile, string[] extensions,
            VersionFormat versionFormat)
        {
            return versionFormat == VersionFormat.Maven
                ? MavenPackageIdParser.GetMetadataFromServerPackageName(packageFile, extensions)
                : NugetPackageIdParser.GetMetadataFromServerPackageName(packageFile, extensions);
        }

        public PackageMetadata GetMetadataFromServerPackageName(string packageFile, VersionFormat versionFormat)
        {
            return versionFormat == VersionFormat.Maven
                ? MavenPackageIdParser.GetMetadataFromServerPackageName(packageFile)
                : NugetPackageIdParser.GetMetadataFromServerPackageName(packageFile);
        }

        public bool TryGetMetadataFromServerPackageName(string packageFile, string[] extensions,
            out PackageMetadata packageMetadata,
            VersionFormat versionFormat)
        {
            return versionFormat == VersionFormat.Maven
                ? MavenPackageIdParser.TryGetMetadataFromServerPackageName(packageFile, extensions, out packageMetadata)
                : NugetPackageIdParser.TryGetMetadataFromServerPackageName(packageFile, extensions,
                    out packageMetadata);
        }

        public Maybe<PackageMetadata> TryGetMetadataFromServerPackageName(string packageFile, string[] extensions,
            VersionFormat format)
        {
            return format == VersionFormat.Maven
                ? MavenPackageIdParser.TryGetMetadataFromServerPackageName(packageFile, extensions)
                : NugetPackageIdParser.TryGetMetadataFromServerPackageName(packageFile, extensions);
        }

        public Maybe<PackageMetadata> TryGetMetadataFromServerPackageName(string packageFile, VersionFormat format)
        {
            return format == VersionFormat.Maven
                ? MavenPackageIdParser.TryGetMetadataFromServerPackageName(packageFile)
                : NugetPackageIdParser.TryGetMetadataFromServerPackageName(packageFile);
        }

        public PhysicalPackageMetadata GetMetadataFromServerPackageName(string packageFile, string[] extensions,
            long size,
            string hash, VersionFormat versionFormat)
        {
            return versionFormat == VersionFormat.Maven
                ? MavenPackageIdParser.GetMetadataFromServerPackageName(packageFile, extensions, size, hash)
                : NugetPackageIdParser.GetMetadataFromServerPackageName(packageFile, extensions, size, hash);
        }

        public BasePackageMetadata GetMetadataFromPackageID(string packageID)
        {
            try
            {
                /*
                 * Try parsing with Maven first. The package ids for Maven feeds are designed to fail
                 * for NuGet package id.
                 */
                return MavenPackageIdParser.GetMetadataFromPackageID(packageID);
            }
            catch
            {
                /*
                 * If there was an exception, parse as a Nuget package id.
                 */
                return NugetPackageIdParser.GetMetadataFromPackageID(packageID);
            }
        }

        public bool TryGetMetadataFromPackageID(string packageID, out BasePackageMetadata metadata)
        {
            try
            {
                /*
                 * Try parsing with Maven first. The package ids for Maven feeds are designed to fail
                 * for NuGet package id.
                 */
                return MavenPackageIdParser.TryGetMetadataFromPackageID(packageID, out metadata);
            }
            catch
            {
                /*
                 * If there was an exception, parse as a Nuget package id.
                 */
                return NugetPackageIdParser.TryGetMetadataFromPackageID(packageID, out metadata);
            }
        }

        public PackageMetadata GetMetadataFromPackageID(string packageID, string version, string extension)
        {
            try
            {
                /*
                 * Try parsing with Maven first. The package ids for Maven feeds are designed to fail
                 * for NuGet package id.
                 */
                return MavenPackageIdParser.GetMetadataFromPackageID(packageID, version, extension);
            }
            catch
            {
                /*
                 * If there was an exception, parse as a Nuget package id.
                 */
                return NugetPackageIdParser.GetMetadataFromPackageID(packageID, version, extension);
            }
        }

        public PhysicalPackageMetadata GetMetadataFromPackageID(string packageID, string version, string extension,
            long size,
            string hash)
        {
            try
            {
                /*
                 * Try parsing with Maven first. The package ids for Maven feeds are designed to fail
                 * for NuGet package id.
                 */
                return MavenPackageIdParser.GetMetadataFromPackageID(packageID, version, extension, size,
                    hash);
            }
            catch
            {
                /*
                 * If there was an exception, parse as a Nuget package id.
                 */
                return NugetPackageIdParser.GetMetadataFromPackageID(packageID, version, extension, size,
                    hash);
            }
        }

        public PackageMetadata GetMetadataFromPackageName(string packageFile, string[] extensions)
        {
            try
            {
                /*
                 * Try parsing with Maven first. The package ids for Maven feeds are designed to fail
                 * for NuGet package id.
                 */
                return MavenPackageIdParser.GetMetadataFromPackageName(packageFile, extensions);
            }
            catch
            {
                /*
                 * If there was an exception, parse as a Nuget package id.
                 */
                return NugetPackageIdParser.GetMetadataFromPackageName(packageFile, extensions);
            }
        }

        public bool TryGetMetadataFromPackageName(string packageFile, string[] extensions,
            out PackageMetadata packageMetadata)
        {
            try
            {
                /*
                 * Try parsing with Maven first. The package ids for Maven feeds are designed to fail
                 * for NuGet package id.
                 */
                return MavenPackageIdParser.TryGetMetadataFromPackageName(packageFile, extensions,
                    out packageMetadata);
            }
            catch
            {
                /*
                 * If there was an exception, parse as a Nuget package id.
                 */
                return NugetPackageIdParser.TryGetMetadataFromPackageName(packageFile, extensions,
                    out packageMetadata);
            }
        }

        public Maybe<PackageMetadata> TryGetMetadataFromPackageName(string packageFile, string[] extensions)
        {
            try
            {
                /*
                 * Try parsing with Maven first. The package ids for Maven feeds are designed to fail
                 * for NuGet package id.
                 */
                return MavenPackageIdParser.TryGetMetadataFromPackageName(packageFile, extensions);
            }
            catch
            {
                /*
                 * If there was an exception, parse as a Nuget package id.
                 */
                return NugetPackageIdParser.TryGetMetadataFromPackageName(packageFile, extensions);
            }
        }

        public Maybe<PackageMetadata> TryGetMetadataFromPackageName(string packageFile)
        {
            try
            {
                /*
                 * Try parsing with Maven first. The package ids for Maven feeds are designed to fail
                 * for NuGet package id.
                 */
                return MavenPackageIdParser.TryGetMetadataFromPackageName(packageFile);
            }
            catch
            {
                /*
                 * If there was an exception, parse as a Nuget package id.
                 */
                return NugetPackageIdParser.TryGetMetadataFromPackageName(packageFile);
            }
        }

        public PackageMetadata GetMetadataFromServerPackageName(string packageFile, string[] extensions)
        {
            try
            {
                /*
                 * Try parsing with Maven first. The package ids for Maven feeds are designed to fail
                 * for NuGet package id.
                 */
                return MavenPackageIdParser.GetMetadataFromServerPackageName(packageFile, extensions);
            }
            catch
            {
                /*
                 * If there was an exception, parse as a Nuget package id.
                 */
                return NugetPackageIdParser.GetMetadataFromServerPackageName(packageFile, extensions);
            }
        }

        public PackageMetadata GetMetadataFromServerPackageName(string packageFile)
        {
            try
            {
                /*
                 * Try parsing with Maven first. The package ids for Maven feeds are designed to fail
                 * for NuGet package id.
                 */
                return MavenPackageIdParser.GetMetadataFromServerPackageName(packageFile);
            }
            catch
            {
                /*
                 * If there was an exception, parse as a Nuget package id.
                 */
                return NugetPackageIdParser.GetMetadataFromServerPackageName(packageFile);
            }
        }

        public bool TryGetMetadataFromServerPackageName(string packageFile, string[] extensions,
            out PackageMetadata packageMetadata)
        {
            try
            {
                /*
                 * Try parsing with Maven first. The package ids for Maven feeds are designed to fail
                 * for NuGet package id.
                 */
                return MavenPackageIdParser.TryGetMetadataFromServerPackageName(packageFile, extensions,
                    out packageMetadata);
            }
            catch
            {
                /*
                 * If there was an exception, parse as a Nuget package id.
                 */
                return NugetPackageIdParser.TryGetMetadataFromServerPackageName(packageFile, extensions,
                    out packageMetadata);
            }
        }

        public Maybe<PackageMetadata> TryGetMetadataFromServerPackageName(string packageFile, string[] extensions)
        {
            try
            {
                /*
                 * Try parsing with Maven first. The package ids for Maven feeds are designed to fail
                 * for NuGet package id.
                 */
                return MavenPackageIdParser.TryGetMetadataFromServerPackageName(packageFile, extensions);
            }
            catch
            {
                /*
                 * If there was an exception, parse as a Nuget package id.
                 */
                return NugetPackageIdParser.TryGetMetadataFromServerPackageName(packageFile, extensions);
            }
        }

        public Maybe<PackageMetadata> TryGetMetadataFromServerPackageName(string packageFile)
        {
            try
            {
                /*
                 * Try parsing with Maven first. The package ids for Maven feeds are designed to fail
                 * for NuGet package id.
                 */
                return MavenPackageIdParser.TryGetMetadataFromServerPackageName(packageFile);
            }
            catch
            {
                /*
                 * If there was an exception, parse as a Nuget package id.
                 */
                return NugetPackageIdParser.TryGetMetadataFromServerPackageName(packageFile);
            }
        }

        public PhysicalPackageMetadata GetMetadataFromServerPackageName(string packageFile, string[] extensions,
            long size,
            string hash)
        {
            try
            {
                /*
                 * Try parsing with Maven first. The package ids for Maven feeds are designed to fail
                 * for NuGet package id.
                 */
                return MavenPackageIdParser.GetMetadataFromServerPackageName(packageFile, extensions, size, hash);
            }
            catch
            {
                /*
                 * If there was an exception, parse as a Nuget package id.
                 */
                return NugetPackageIdParser.GetMetadataFromServerPackageName(packageFile, extensions, size, hash);
            }
        }
    }
}