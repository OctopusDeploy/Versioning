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
                Id = packageID,
                FeedType = FeedType.Maven
            };
        }

        public PackageMetadata GetMetadataFromPackageID(string packageID, string version, string extension)
        {
            var baseDetails = GetMetadataFromPackageID(packageID);
            return BuildMetadata(baseDetails.Id, version, extension);
        }

        public PackageMetadata GetMetadataFromPackageName(string packageFile, string[] extensions)
        {
            var metadataAndExtension =
                PackageIdentifier.ExtractPackageExtensionAndMetadata(packageFile, extensions);

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
            pkg.Id = id;
            pkg.Version = version;
            pkg.FileExtension = extension;
            pkg.FeedType = FeedType.Maven;            
            pkg.PackageSearchPattern = pkg.Id + JavaConstants.JAVA_FILENAME_DELIMITER + pkg.Version + "*";
            pkg.PackageFileName = pkg.Id + JavaConstants.JAVA_FILENAME_DELIMITER + pkg.Version + "_";
            return pkg;
        }
    }
}