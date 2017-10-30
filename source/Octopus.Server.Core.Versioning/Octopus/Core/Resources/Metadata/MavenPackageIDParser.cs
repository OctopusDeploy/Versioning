﻿using System;
using Octopus.Core.Constants;
using Octopus.Core.Resources.Versioning;

namespace Octopus.Core.Resources.Metadata
{
    /// <summary>
    /// Maven package IDs come in the format: group#artifact
    /// </summary>
    public class MavenPackageIDParser : IPackageIDParser
    {
        /// <summary>
        /// The prefix added to all files to identify it as coming from a maven feed. This
        /// means that filenames will look like:
        /// Maven#org.example#artifact#1.0.jar
        /// 
        /// Appending a prefix like this is the recommended way to identify the origin of
        /// a package, as it means that a parser can quickly fail if it is looking at
        /// a file that it is not responsible for.
        /// </summary>
        private const string MavenFeedPrefix = "Maven";

        public BasePackageMetadata GetMetadataFromPackageID(string packageID)
        {
            var idAndVersionSplit = packageID.Split(JavaConstants.JAVA_FILENAME_DELIMITER);

            if (idAndVersionSplit.Length != 2)
            {
                throw new Exception(
                    $"Unable to extract the package ID from \"{packageID}\"");
            }

            return BuildMetadata(packageID);
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

        public PhysicalPackageMetadata GetMetadataFromServerPackageName(string packageFile, string[] extensions,
            long size, string hash)
        {
            var baseDetails = GetMetadataFromPackageName(
                packageFile,
                PackageIdentifier.ExtractPackageExtensionAndMetadataForServer(packageFile, extensions),
                extensions);
            return BuildMetadata(baseDetails.PackageId, baseDetails.Version, baseDetails.FileExtension, size, hash);
        }

        PackageMetadata GetMetadataFromPackageName(string packageFile, Tuple<string, string> metadataAndExtension,
            string[] extensions)
        {
            var idAndVersion = metadataAndExtension.Item1;
            var extension = metadataAndExtension.Item2;

            if (string.IsNullOrEmpty(extension))
            {
                throw new Exception($"Unable to determine filetype of file \"{packageFile}\"");
            }

            var idAndVersionSplit = idAndVersion.Split(JavaConstants.JAVA_FILENAME_DELIMITER);

            if (idAndVersionSplit.Length != 4 || idAndVersionSplit[0] != MavenFeedPrefix)
            {
                throw new Exception(
                    $"Unable to extract the package ID and version from file \"{packageFile}\"");
            }

            return BuildMetadata(
                idAndVersionSplit[1] + JavaConstants.JAVA_FILENAME_DELIMITER + idAndVersionSplit[2],
                idAndVersionSplit[3],
                extension);
        }

        BasePackageMetadata BuildMetadata(string packageID)
        {
            return new BasePackageMetadata()
            {
                PackageId = packageID,
                FeedType = FeedType.Maven,
                PackageSearchPattern = MavenFeedPrefix + JavaConstants.JAVA_FILENAME_DELIMITER +
                                       packageID + "*"
            };
        }

        PackageMetadata BuildMetadata(string id, string version, string extension)
        {
            var baseMetadata = BuildMetadata(id);
            
            var pkg = new PackageMetadata();
            pkg.PackageId = baseMetadata.PackageId;
            pkg.Version = version;
            pkg.FileExtension = extension;
            pkg.FeedType = FeedType.Maven;
            pkg.PackageSearchPattern = baseMetadata.PackageSearchPattern;
            pkg.PackageAndVersionSearchPattern = MavenFeedPrefix + JavaConstants.JAVA_FILENAME_DELIMITER +
                                                 pkg.PackageId + JavaConstants.JAVA_FILENAME_DELIMITER +
                                                 pkg.Version + "*";
            pkg.ServerPackageFileName = MavenFeedPrefix + JavaConstants.JAVA_FILENAME_DELIMITER +
                                        pkg.PackageId + JavaConstants.JAVA_FILENAME_DELIMITER +
                                        pkg.Version + ServerConstants.SERVER_CACHE_DELIMITER;
            pkg.TargetPackageFileName = MavenFeedPrefix + JavaConstants.JAVA_FILENAME_DELIMITER +
                                        pkg.PackageId + JavaConstants.JAVA_FILENAME_DELIMITER +
                                        pkg.Version + extension;
            pkg.VersionDelimiter = JavaConstants.JAVA_FILENAME_DELIMITER.ToString();
            return pkg;
        }

        PhysicalPackageMetadata BuildMetadata(string id, string version, string extension, long size, string hash)
        {
            var basePackage = BuildMetadata(id, version, extension);
            var pkg = new PhysicalPackageMetadata();
            pkg.PackageId = basePackage.PackageId;
            pkg.Version = basePackage.Version;
            pkg.FileExtension = basePackage.FileExtension;
            pkg.FeedType = basePackage.FeedType;
            pkg.PackageAndVersionSearchPattern = basePackage.PackageAndVersionSearchPattern;
            pkg.PackageSearchPattern = basePackage.PackageSearchPattern;
            pkg.ServerPackageFileName = basePackage.ServerPackageFileName;
            pkg.TargetPackageFileName = basePackage.TargetPackageFileName;
            pkg.Size = size;
            pkg.Hash = hash;
            pkg.VersionDelimiter = basePackage.VersionDelimiter;
            return pkg;
        }
    }
}