using Octopus.Core.Util;

namespace Octopus.Core.Resources.Metadata
{
    public class MetadataFactory : IMetadataFactory
    {
        static readonly IPackageIDParser MavenPackageIdParser = new MavenPackageIDParser();
        static readonly IPackageIDParser NugetPackageIdParser = new NuGetPackageIDParser();
        
        public BasePackageMetadata GetMetadataFromPackageID(string packageID, FeedType feedType)
        {
            return feedType == FeedType.Maven ? 
                MavenPackageIdParser.GetMetadataFromPackageID(packageID) : 
                NugetPackageIdParser.GetMetadataFromPackageID(packageID);
        }

        public bool CanGetMetadataFromPackageID(string packageID, out BasePackageMetadata metadata, FeedType feedType)
        {
            return feedType == FeedType.Maven ? 
                MavenPackageIdParser.CanGetMetadataFromPackageID(packageID, out metadata) : 
                NugetPackageIdParser.CanGetMetadataFromPackageID(packageID, out metadata);
        }

        public PackageMetadata GetMetadataFromPackageID(string packageID, string version, string extension, FeedType feedType)
        {
            return feedType == FeedType.Maven ?
                MavenPackageIdParser.GetMetadataFromPackageID(packageID, version, extension) : 
                NugetPackageIdParser.GetMetadataFromPackageID(packageID, version, extension);
        }

        public PhysicalPackageMetadata GetMetadataFromPackageID(string packageID, string version, string extension, long size,
            string hash, FeedType feedType)
        {
            return feedType == FeedType.Maven ? 
                MavenPackageIdParser.GetMetadataFromPackageID(packageID, version, extension, size, hash) : 
                NugetPackageIdParser.GetMetadataFromPackageID(packageID, version, extension, size, hash);
        }

        public PackageMetadata GetMetadataFromPackageName(string packageFile, string[] extensions, FeedType feedType)
        {
            return feedType == FeedType.Maven ? 
                MavenPackageIdParser.GetMetadataFromPackageName(packageFile, extensions) : 
                NugetPackageIdParser.GetMetadataFromPackageName(packageFile, extensions);
        }

        public bool CanGetMetadataFromPackageName(string packageFile, string[] extensions, out PackageMetadata packageMetadata,
            FeedType feedType)
        {
            return feedType == FeedType.Maven ? 
                MavenPackageIdParser.CanGetMetadataFromPackageName(packageFile, extensions, out packageMetadata) : 
                NugetPackageIdParser.CanGetMetadataFromPackageName(packageFile, extensions, out packageMetadata);
        }

        public Maybe<PackageMetadata> CanGetMetadataFromPackageName(string packageFile, string[] extensions, FeedType feedType)
        {
            return feedType == FeedType.Maven ? 
                MavenPackageIdParser.CanGetMetadataFromPackageName(packageFile, extensions) : 
                NugetPackageIdParser.CanGetMetadataFromPackageName(packageFile, extensions);
        }

        public Maybe<PackageMetadata> CanGetMetadataFromPackageName(string packageFile, FeedType feedType)
        {
            return feedType == FeedType.Maven ? 
                MavenPackageIdParser.CanGetMetadataFromPackageName(packageFile) : 
                NugetPackageIdParser.CanGetMetadataFromPackageName(packageFile);
        }

        public PackageMetadata GetMetadataFromServerPackageName(string packageFile, string[] extensions, FeedType feedType)
        {
            return feedType == FeedType.Maven ? 
                MavenPackageIdParser.GetMetadataFromServerPackageName(packageFile, extensions) : 
                NugetPackageIdParser.GetMetadataFromServerPackageName(packageFile, extensions);
        }

        public PackageMetadata GetMetadataFromServerPackageName(string packageFile, FeedType feedType)
        {
            return feedType == FeedType.Maven ? 
                MavenPackageIdParser.GetMetadataFromServerPackageName(packageFile) : 
                NugetPackageIdParser.GetMetadataFromServerPackageName(packageFile);
        }

        public bool CanGetMetadataFromServerPackageName(string packageFile, string[] extensions, out PackageMetadata packageMetadata,
            FeedType feedType)
        {
            return feedType == FeedType.Maven ? 
                MavenPackageIdParser.CanGetMetadataFromServerPackageName(packageFile, extensions, out packageMetadata) : 
                NugetPackageIdParser.CanGetMetadataFromServerPackageName(packageFile, extensions, out packageMetadata);
        }

        public Maybe<PackageMetadata> CanGetMetadataFromServerPackageName(string packageFile, string[] extensions, FeedType feedType)
        {
            return feedType == FeedType.Maven ? 
                MavenPackageIdParser.CanGetMetadataFromServerPackageName(packageFile, extensions) : 
                NugetPackageIdParser.CanGetMetadataFromServerPackageName(packageFile, extensions);
        }

        public Maybe<PackageMetadata> CanGetMetadataFromServerPackageName(string packageFile, FeedType feedType)
        {
            return feedType == FeedType.Maven ? 
                MavenPackageIdParser.CanGetMetadataFromServerPackageName(packageFile) : 
                NugetPackageIdParser.CanGetMetadataFromServerPackageName(packageFile);
        }

        public PhysicalPackageMetadata GetMetadataFromServerPackageName(string packageFile, string[] extensions, long size,
            string hash, FeedType feedType)
        {
            return feedType == FeedType.Maven ? 
                MavenPackageIdParser.GetMetadataFromServerPackageName(packageFile, extensions, size, hash) : 
                NugetPackageIdParser.GetMetadataFromServerPackageName(packageFile, extensions, size, hash);
        }
    }
}