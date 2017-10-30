using System;
using Octopus.Core.Constants;
using Octopus.Core.Resources.Versioning;

namespace Octopus.Core.Resources.Metadata
{
    /// <summary>
    /// Maven package IDs come in the format: group#artifact
    /// </summary>
    public class MavenPackageIDParser : IPackageIDParser
    {       
        public BasePackageMetadata GetMetadataFromPackageID(string packageID)
        {
            var idAndVersionSplit = packageID.Split(JavaConstants.JAVA_FILENAME_DELIMITER);

            if (idAndVersionSplit.Length != 2)
            {
                throw new Exception(
                    $"Unable to extract the package ID from \"{packageID}\"");
            }

            return new BasePackageMetadata()
            {
                PackageId = packageID,
                FeedType = FeedType.Maven
            };
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
            var baseDetails = GetMetadataFromPackageID(packageID);
            return BuildMetadata(baseDetails.PackageId, version, extension, size, hash);
        }

        public PackageMetadata GetMetadataFromPackageName(string packageFile, string[] extensions)
        {
            return GetMetadataFromPackageName(
                packageFile,
                PackageIdentifier.ExtractPackageExtensionAndMetadata(packageFile, extensions),
                extensions);         
        }

        public PackageMetadata GetMetadataFromServerPackageName(string packageFile, string[] extensions)
        {
            return GetMetadataFromPackageName(
                packageFile,
                PackageIdentifier.ExtractPackageExtensionAndMetadataForServer(packageFile, extensions),
                extensions);   
        }

        PackageMetadata GetMetadataFromPackageName(string packageFile, Tuple<string, string> metadataAndExtension, string[] extensions)
        {
            var idAndVersion = metadataAndExtension.Item1;
            var extension = metadataAndExtension.Item2;

            if (string.IsNullOrEmpty(extension))
            {
                throw new Exception($"Unable to determine filetype of file \"{packageFile}\"");
            }

            var idAndVersionSplit = idAndVersion.Split(JavaConstants.JAVA_FILENAME_DELIMITER);

            if (idAndVersionSplit.Length != 3)
            {
                throw new Exception(
                    $"Unable to extract the package ID and version from file \"{packageFile}\"");
            }

            return BuildMetadata(
                idAndVersionSplit[0] + JavaConstants.JAVA_FILENAME_DELIMITER + idAndVersionSplit[1],
                idAndVersionSplit[2],
                extension);            
        }

        PackageMetadata BuildMetadata(string id, string version, string extension)
        {
            var pkg = new PackageMetadata();
            pkg.PackageId = id;
            pkg.Version = version;
            pkg.FileExtension = extension;
            pkg.FeedType = FeedType.Maven;            
            pkg.PackageSearchPattern = pkg.PackageId + JavaConstants.JAVA_FILENAME_DELIMITER + pkg.Version + "*";
            pkg.PackageFileName = pkg.PackageId + JavaConstants.JAVA_FILENAME_DELIMITER + pkg.Version + ServerConstants.SERVER_CACHE_DELIMITER;
            pkg.VersionDelimiter = JavaConstants.JAVA_FILENAME_DELIMITER.ToString();
            return pkg;
        }
        
        PhysicalPackageMetadata BuildMetadata(string id, string version, string extension, long size, string hash)
        {
            var pkg = new PhysicalPackageMetadata();
            pkg.PackageId = id;
            pkg.Version = version;
            pkg.FileExtension = extension;
            pkg.FeedType = FeedType.Maven;            
            pkg.PackageSearchPattern = pkg.PackageId + JavaConstants.JAVA_FILENAME_DELIMITER + pkg.Version + "*";
            pkg.PackageFileName = pkg.PackageId + JavaConstants.JAVA_FILENAME_DELIMITER + pkg.Version + ServerConstants.SERVER_CACHE_DELIMITER;
            pkg.Size = size;
            pkg.Hash = hash;
            pkg.VersionDelimiter = JavaConstants.JAVA_FILENAME_DELIMITER.ToString();
            return pkg;
        }
    }
}