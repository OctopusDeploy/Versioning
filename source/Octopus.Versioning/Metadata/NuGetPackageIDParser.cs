using System;
using System.IO;
using System.Text.RegularExpressions;
using Octopus.Core.Util;
using Octopus.Versioning.Constants;
using Octopus.Versioning.Factories;

namespace Octopus.Versioning.Metadata
{
    /// <summary>
    /// A service for extracting metadata from packages that are sourced from a NuGet compatible
    /// feed, including the built in feed.
    /// </summary>
    public class NuGetPackageIDParser : IPackageIDParser
    {
        static readonly IVersionFactory VersionFactory = new VersionFactory();
        
        /// <summary>
        /// NuGet is considered the fallback that will always match the supplied package id
        /// </summary>
        public BasePackageMetadata GetMetadataFromPackageID(string packageID)
        {
            return BuildMetadata(packageID);
        }

        public bool TryGetMetadataFromPackageID(string packageID, out BasePackageMetadata metadata)
        {
            try
            {
                metadata = GetMetadataFromPackageID(packageID);
                return true;
            }
            catch
            {
                metadata = null;
                return false;
            }
        }

        public PackageMetadata GetMetadataFromPackageID(string packageID, string version, string extension)
        {
            var baseDetails = GetMetadataFromPackageID(packageID);
            return BuildMetadata(baseDetails.PackageId, version, extension);
        }

        public PhysicalPackageMetadata GetMetadataFromPackageID(
            string packageID, 
            string version, 
            string extension, 
            long size,
            string hash)
        {
            return BuildMetadata(packageID, version, extension, size, hash);
        }

        public PackageMetadata GetMetadataFromPackageName(string packageFile, string[] extensions)
        {
            return GetMetadataFromPackageName(
                packageFile,
                PackageIdentifier.ExtractPackageExtensionAndMetadata(packageFile, extensions));
        }

        public bool TryGetMetadataFromPackageName(string packageFile, string[] extensions, out PackageMetadata packageMetadata)
        {
            try
            {
                packageMetadata = GetMetadataFromPackageName(packageFile, extensions);
                return true;
            }
            catch
            {
                packageMetadata = null;
                return false;
            }
        }
        
        public Maybe<PackageMetadata> TryGetMetadataFromPackageName(string packageFile, string[] extensions)
        {
            try
            {
                return Maybe<PackageMetadata>.Some(GetMetadataFromPackageName(packageFile, extensions));
            }
            catch
            {
                return Maybe<PackageMetadata>.None;
            }
        }

        public Maybe<PackageMetadata> TryGetMetadataFromPackageName(string packageFile)
        {
            try
            {
                return Maybe<PackageMetadata>.Some(GetMetadataFromPackageName(packageFile, new string[] {Path.GetExtension(packageFile)}));
            }
            catch
            {
                return Maybe<PackageMetadata>.None;
            }
        }

        public Maybe<PackageMetadata> TryGetMetadataFromServerPackageName(string packageFile, string[] extensions)
        {
            try
            {
                return Maybe<PackageMetadata>.Some(GetMetadataFromServerPackageName(packageFile, extensions));
            }
            catch
            {
                return Maybe<PackageMetadata>.None;
            }
        }

        public Maybe<PackageMetadata> TryGetMetadataFromServerPackageName(string packageFile)
        {
            try
            {
                return Maybe<PackageMetadata>.Some(GetMetadataFromServerPackageName(packageFile, new string[] {Path.GetExtension(packageFile)}));
            }
            catch
            {
                return Maybe<PackageMetadata>.None;
            }
        }

        public PackageMetadata GetMetadataFromServerPackageName(string packageFile)
        {
            return GetMetadataFromPackageName(
                packageFile,
                PackageIdentifier.ExtractPackageExtensionAndMetadataForServer(packageFile, new string[] {Path.GetExtension(packageFile)}));
        }

        public bool TryGetMetadataFromServerPackageName(string packageFile, string[] extensions,
            out PackageMetadata packageMetadata)
        {
            try
            {
                packageMetadata = GetMetadataFromServerPackageName(packageFile, extensions);
                return true;
            }
            catch
            {
                packageMetadata = null;
                return false;
            }
        }

        public PackageMetadata GetMetadataFromServerPackageName(string packageFile, string[] extensions)
        {
            return GetMetadataFromPackageName(
                packageFile,
                PackageIdentifier.ExtractPackageExtensionAndMetadataForServer(packageFile, extensions));
        }

        public PhysicalPackageMetadata GetMetadataFromServerPackageName(string packageFile, string[] extensions, long size, string hash)
        {
            var baseDetails = GetMetadataFromPackageName(
                packageFile,
                PackageIdentifier.ExtractPackageExtensionAndMetadataForServer(packageFile, extensions)); 
            return BuildMetadata(baseDetails.PackageId, baseDetails.Version, baseDetails.FileExtension, size, hash);
        }

        PackageMetadata GetMetadataFromPackageName(string packageFile, Tuple<string, string> metadataAndExtension)
        {            
            var idAndVersion = metadataAndExtension.Item1;
            var extension = metadataAndExtension.Item2;

            if (string.IsNullOrEmpty(extension))
            {
                throw new Exception($"Unable to determine filetype of file \"{packageFile}\"");
            }

            if (!TryParsePackageIdAndVersion(idAndVersion, out string packageId, out IVersion version))
            {
                throw new Exception($"Unable to extract the package ID and version from file \"{packageFile}\"");
            }

            return BuildMetadata(packageId, version.ToString(), extension);
        }
        
        BasePackageMetadata BuildMetadata(string packageID)
        {
            return new BasePackageMetadata()
            {
                PackageId = packageID,
                VersionFormat = VersionFormat.Semver,
                PackageSearchPattern = packageID + "*"
            };
        }
        
        PackageMetadata BuildMetadata(string id, string version, string extension)
        {
            var baseMetadata = BuildMetadata(id);
            
            var pkg = new PackageMetadata();
            pkg.VersionDelimiter = ".";
            pkg.PackageId = baseMetadata.PackageId;
            pkg.Version = version;
            pkg.FileExtension = extension;
            pkg.VersionFormat = baseMetadata.VersionFormat;
            pkg.PackageAndVersionSearchPattern = pkg.PackageId + "." + pkg.Version + "*";
            pkg.PackageSearchPattern = baseMetadata.PackageSearchPattern;
            pkg.ServerPackageFileName = pkg.PackageId + "." + pkg.Version + ServerConstants.SERVER_CACHE_DELIMITER;
            pkg.TargetPackageFileName = pkg.PackageId + "." + pkg.Version + extension;
            return pkg;
        }
        
        PhysicalPackageMetadata BuildMetadata(string id, string version, string extension, long size, string hash)
        {
            var basePackage = BuildMetadata(id, version, extension);
            var pkg = new PhysicalPackageMetadata();
            pkg.PackageId = basePackage.PackageId;
            pkg.Version = basePackage.Version;
            pkg.FileExtension = basePackage.FileExtension;
            pkg.VersionFormat = basePackage.VersionFormat;
            pkg.PackageAndVersionSearchPattern = basePackage.PackageAndVersionSearchPattern;
            pkg.PackageSearchPattern = basePackage.PackageSearchPattern;
            pkg.ServerPackageFileName = basePackage.ServerPackageFileName;
            pkg.TargetPackageFileName = basePackage.TargetPackageFileName;
            pkg.Size = size;
            pkg.Hash = hash;
            pkg.VersionDelimiter = basePackage.VersionDelimiter;
            return pkg;
        }
        
        /// <summary>
        /// Takes a string containing a concatenated package ID and version (e.g. a filename or database-key) and 
        /// attempts to parse a package ID and semantic version.  
        /// </summary>
        /// <param name="idAndVersion">The concatenated package ID and version.</param>
        /// <param name="packageId">The parsed package ID</param>
        /// <param name="version">The parsed semantic version</param>
        /// <returns>True if parsing was successful, else False</returns>
        public static bool TryParsePackageIdAndVersion(string idAndVersion, out string packageId, out IVersion version)
        {
            packageId = null;
            version = null;

            const string packageIdPattern = @"(?<packageId>(\w+([_.-]\w+)*?))";
            const string semanticVersionPattern = @"(?<semanticVersion>(\d+(\.\d+){0,3}" // Major Minor Patch
                                                  + @"(-[0-9A-Za-z-]+(\.[0-9A-Za-z-]+)*)?)" // Pre-release identifiers
                                                  + @"(\+[0-9A-Za-z-]+(\.[0-9A-Za-z-]+)*)?)"; // Build Metadata

            var match = Regex.Match(idAndVersion, $@"^{packageIdPattern}\.{semanticVersionPattern}$");
            var packageIdMatch = match.Groups["packageId"];
            var versionMatch = match.Groups["semanticVersion"];

            if (!packageIdMatch.Success || !versionMatch.Success)
                return false;

            packageId = packageIdMatch.Value;

            return VersionFactory.TryCreateSemanticVersion(versionMatch.Value, out version);
        }
    }
}