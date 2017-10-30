using System;
using System.Text.RegularExpressions;
using Octopus.Core.Constants;
using Octopus.Core.Resources.Versioning;
using Octopus.Core.Resources.Versioning.Factories;

namespace Octopus.Core.Resources.Metadata
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
            return new BasePackageMetadata()
            {
                PackageId = packageID,
                FeedType = FeedType.NuGet
            };
        }

        public PackageMetadata GetMetadataFromPackageID(string packageID, string version, string extension)
        {
            throw new NotImplementedException();
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
        
        PackageMetadata BuildMetadata(string id, string version, string extension)
        {
            var pkg = new PackageMetadata();
            pkg.PackageId = id;
            pkg.Version = version;
            pkg.FileExtension = extension;
            pkg.FeedType = FeedType.NuGet;             
            pkg.PackageSearchPattern = pkg.PackageId + "." + pkg.Version + "*";
            pkg.ServerPackageFileName = pkg.PackageId + "." + pkg.Version + ServerConstants.SERVER_CACHE_DELIMITER;
            pkg.TargetPackageFileName = pkg.PackageId + "." + pkg.Version + extension;
            return pkg;
        }
        
        PhysicalPackageMetadata BuildMetadata(string id, string version, string extension, long size, string hash)
        {
            var pkg = new PhysicalPackageMetadata();
            pkg.PackageId = id;
            pkg.Version = version;
            pkg.FileExtension = extension;
            pkg.FeedType = FeedType.NuGet;            
            pkg.PackageSearchPattern = pkg.PackageId + JavaConstants.JAVA_FILENAME_DELIMITER + pkg.Version + "*";
            pkg.ServerPackageFileName = pkg.PackageId + JavaConstants.JAVA_FILENAME_DELIMITER + pkg.Version + ServerConstants.SERVER_CACHE_DELIMITER;
            pkg.TargetPackageFileName = pkg.PackageId + "." + pkg.Version + extension;
            pkg.Size = size;
            pkg.Hash = hash;
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
        bool TryParsePackageIdAndVersion(string idAndVersion, out string packageId, out IVersion version)
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

            return VersionFactory.CanCreateSemanticVersion(versionMatch.Value, out version);
        }
    }
}