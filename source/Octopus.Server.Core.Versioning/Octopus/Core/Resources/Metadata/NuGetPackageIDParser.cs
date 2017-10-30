using System;
using Octopus.Core.Resources.Versioning;

namespace Octopus.Core.Resources.Metadata
{
    /// <summary>
    /// A service for extracting metadata from packages that are sourced from a NuGet compatible
    /// feed, including the built in feed.
    /// </summary>
    public class NuGetPackageIDParser : IPackageIDParser
    {
        /// <summary>
        /// NuGet is considered the fallback that will always match the supplied package id
        /// </summary>
        public BasePackageMetadata GetMetadataFromPackageID(string packageID)
        {
            return new BasePackageMetadata()
            {
                Id = packageID,
                FeedType = FeedType.NuGet
            };
        }

        public PackageMetadata GetMetadataFromPackageID(string packageID, string version, string extension)
        {
            throw new NotImplementedException();
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

        PackageMetadata GetMetadataFromPackageName(string packageFile, Tuple<string, string> metadataAndExtension)
        {            
            var idAndVersion = metadataAndExtension.Item1;
            var extension = metadataAndExtension.Item2;

            if (string.IsNullOrEmpty(extension))
            {
                throw new Exception($"Unable to determine filetype of file \"{packageFile}\"");
            }

            if (!PackageIdentifier.TryParsePackageIdAndVersion(idAndVersion, out string packageId, out IVersion version))
            {
                throw new Exception($"Unable to extract the package ID and version from file \"{packageFile}\"");
            }

            return BuildMetadata(packageId, version.ToString(), extension);
        }
        
        PackageMetadata BuildMetadata(string id, string version, string extension)
        {
            var pkg = new PackageMetadata();
            pkg.Id = id;
            pkg.Version = version;
            pkg.FileExtension = extension;
            pkg.FeedType = FeedType.NuGet;             
            pkg.PackageSearchPattern = pkg.Id + "." + pkg.Version + "*";
            pkg.PackageFileName = pkg.Id + "." + pkg.Version + "_";
            return pkg;
        }
    }
}