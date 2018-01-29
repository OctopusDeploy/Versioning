using System;
using System.IO;
using Octopus.Core.Util;
using Octopus.Versioning.Constants;

namespace Octopus.Versioning.Metadata
{
    /// <summary>
    /// Maven package IDs come in the format: group#artifact
    /// </summary>
    public class MavenPackageIDParser : IPackageIDParser
    {
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

        public PackageMetadata GetMetadataFromPackageName(string packageFile, string[] extensions)
        {
            return GetMetadataFromPackageName(
                packageFile,
                PackageIdentifierUtils.ExtractPackageExtensionAndMetadata(packageFile, extensions),
                extensions);
        }

        public bool TryGetMetadataFromPackageName(string packageFile, string[] extensions,
            out PackageMetadata packageMetadata)
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

        public PackageMetadata GetMetadataFromServerPackageName(string packageFile, string[] extensions)
        {
            return GetMetadataFromPackageName(
                packageFile,
                PackageIdentifierUtils.ExtractPackageExtensionAndMetadataForServer(packageFile, extensions),
                extensions);
        }

        public PackageMetadata GetMetadataFromServerPackageName(string packageFile)
        {
            return GetMetadataFromPackageName(
                packageFile,
                PackageIdentifierUtils.ExtractPackageExtensionAndMetadataForServer(packageFile, new string[] {Path.GetExtension(packageFile)}),
                new string[] {Path.GetExtension(packageFile)});
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

        PackageMetadata GetMetadataFromPackageName(
            string packageFile, 
            Tuple<string, string> metadataAndExtension,
            string[] extensions)
        {
            var idAndVersion = metadataAndExtension.Item1;
            var extension = metadataAndExtension.Item2;

            if (string.IsNullOrEmpty(extension))
            {
                throw new Exception($"Unable to determine filetype of file \"{packageFile}\"");
            }

            var idAndVersionSplit = idAndVersion.Split(JavaConstants.MavenFilenameDelimiter);

            if (idAndVersionSplit.Length != 4 || idAndVersionSplit[0] != JavaConstants.MavenFeedPrefix)
            {
                throw new Exception(
                    $"Unable to extract the package ID and version from file \"{packageFile}\"");
            }

            return BuildMetadata(
                idAndVersionSplit[0] + JavaConstants.MavenFilenameDelimiter +
                idAndVersionSplit[1] + JavaConstants.MavenFilenameDelimiter +
                idAndVersionSplit[2],
                idAndVersionSplit[3],
                extension);
        }

        BasePackageMetadata BuildMetadata(string packageID)
        {
            var groupAndArtifact = packageID.Split(JavaConstants.MavenFilenameDelimiter);

            if (groupAndArtifact.Length != 3 || groupAndArtifact[0] != JavaConstants.MavenFeedPrefix)
            {
                throw new Exception(
                    $"Unable to extract the package ID and version from package ID \"{packageID}\"");
            }

            return new BasePackageMetadata()
            {
                PackageId = groupAndArtifact[0] + JavaConstants.MavenFilenameDelimiter +
                            groupAndArtifact[1] + JavaConstants.MavenFilenameDelimiter +
                            groupAndArtifact[2],
                VersionFormat = VersionFormat.Maven,
                PackageSearchPattern = packageID + "*"
            };
        }

        PackageMetadata BuildMetadata(string id, string version, string extension)
        {
            var baseMetadata = BuildMetadata(id);

            var pkg = new PackageMetadata();
            pkg.PackageId = baseMetadata.PackageId;
            pkg.Version = version;
            pkg.FileExtension = extension;
            pkg.VersionFormat = baseMetadata.VersionFormat;
            pkg.PackageSearchPattern = baseMetadata.PackageSearchPattern;
            pkg.PackageAndVersionSearchPattern = pkg.PackageId + JavaConstants.MavenFilenameDelimiter +
                                                 pkg.Version + "*";
            pkg.ServerPackageFileName = pkg.PackageId + JavaConstants.MavenFilenameDelimiter +
                                        pkg.Version + ServerConstants.SERVER_CACHE_DELIMITER;
            pkg.TargetPackageFileName = pkg.PackageId + JavaConstants.MavenFilenameDelimiter +
                                        pkg.Version + extension;
            pkg.VersionDelimiter = JavaConstants.MavenFilenameDelimiter.ToString();
            return pkg;
        }
    }
}