using Octopus.Core.Util;

namespace Octopus.Core.Resources.Metadata
{
    /// <summary>
    /// Defines a factory that can be used to extract the metadata of a package from a variety of
    /// imports. The methods extended from IPackageIDParser will inspect the supplied paramaters
    /// to work out which feed type the package id represents. The methods from this interface 
    /// add a field called feedType that allows the feed type to be explictly defined.
    /// </summary>
    public interface IMetadataFactory : IPackageIDParser
    {
        /// <summary>
        /// Extracts metadata from a package ID (i.e. no version information)
        /// </summary>
        /// <param name="packageID">The package id</param>
        /// <param name="feedType">The type of feed that supplied the package</param>
        /// <returns>The metadata assocaited with the package id</returns>
        BasePackageMetadata GetMetadataFromPackageID(string packageID, FeedType feedType);
        /// <summary>
        /// Returns true or false based on whether or not we can parse the
        /// supplied package id.
        /// </summary>
        /// <param name="packageID">The package id</param>
        /// <param name="metadata">The parsed metadata if we returned true</param>
        /// <param name="feedType">The type of feed that supplied the package</param>
        /// <returns>True if this package id could be parsed</returns>
        bool TryGetMetadataFromPackageID(string packageID, out BasePackageMetadata metadata, FeedType feedType);
        /// <summary>
        /// Extracts metadata from a package ID and adds the supplied version and extension
        /// </summary>
        /// <param name="packageID">The package id</param>
        /// <param name="version">The package version</param>
        /// <param name="extension">The package extension</param>
        /// <param name="feedType">The type of feed that supplied the package</param>
        /// <returns>The metadata assocaited with the package id</returns>
        PackageMetadata GetMetadataFromPackageID(string packageID, string version, string extension, FeedType feedType);
        /// <summary>
        /// Extracts metadata from a package ID and adds the supplied version and extension
        /// </summary>
        /// <param name="packageID">The package id</param>
        /// <param name="version">The package version</param>
        /// <param name="extension">The package extension</param>
        /// <param name="size">The file size</param>
        /// <param name="hash">The file hash</param>
        /// <param name="feedType">The type of feed that supplied the package</param>
        /// <returns>The metadata assocaited with the package id</returns>
        PhysicalPackageMetadata GetMetadataFromPackageID(string packageID, string version, string extension, long size, string hash, FeedType feedType);
        /// <summary>
        /// The target Files cache has filenames like 
        /// "com.google.guava#guava#23.3-jre.jar-e55fcd51-6081-4300-91a3-117b7930c023" or
        /// "com.google.guava#guava#23.3-jre.jar" or
        /// "mypackage.1.0.0.0.nuget-f363ce3a-0657-401a-8831-f3634f6cca2b".
        /// This method will break down these filenames.
        /// </summary>
        /// <param name="packageFile">The package file name</param>
        /// <param name="extensions">The extensions that this parser should know about</param>
        /// <param name="feedType">The type of feed that supplied the package</param>
        /// <returns>The metadata assocaited with the package file</returns>
        PackageMetadata GetMetadataFromPackageName(string packageFile, string[] extensions, FeedType feedType);
        /// <summary>
        /// The target Files cache has filenames like 
        /// "com.google.guava#guava#23.3-jre.jar-e55fcd51-6081-4300-91a3-117b7930c023" or
        /// "com.google.guava#guava#23.3-jre.jar" or
        /// "mypackage.1.0.0.0.nuget-f363ce3a-0657-401a-8831-f3634f6cca2b".
        /// This method will break down these filenames.
        /// </summary>
        /// <param name="packageFile">The package file name</param>
        /// <param name="extensions">The extensions that this parser should know about</param>
        /// <param name="packageMetadata">The package metadata if the parsing was successful</param>
        /// <param name="feedType">The type of feed that supplied the package</param>
        /// <returns>True if the file could be parsed, and false otherwise</returns>
        bool TryGetMetadataFromPackageName(string packageFile, string[] extensions, out PackageMetadata packageMetadata, FeedType feedType);
        /// <summary>
        /// The target Files cache has filenames like 
        /// "com.google.guava#guava#23.3-jre.jar-e55fcd51-6081-4300-91a3-117b7930c023" or
        /// "com.google.guava#guava#23.3-jre.jar" or
        /// "mypackage.1.0.0.0.nuget-f363ce3a-0657-401a-8831-f3634f6cca2b".
        /// This method will break down these filenames.
        /// </summary>
        /// <param name="packageFile">The package file name</param>
        /// <param name="extensions">The extensions that this parser should know about</param>
        /// <param name="feedType">The type of feed that supplied the package</param>
        /// <returns>A tuple with a boolean indicating the success of the parsing, and the metadata if parsing was successful</returns>
        Maybe<PackageMetadata> TryGetMetadataFromPackageName(string packageFile, string[] extensions, FeedType feedType);
        /// <summary>
        /// The target Files cache has filenames like 
        /// "com.google.guava#guava#23.3-jre.jar-e55fcd51-6081-4300-91a3-117b7930c023" or
        /// "com.google.guava#guava#23.3-jre.jar" or
        /// "mypackage.1.0.0.0.nuget-f363ce3a-0657-401a-8831-f3634f6cca2b".
        /// This method will break down these filenames.
        /// </summary>
        /// <param name="packageFile">The package file name</param>
        /// <param name="feedType">The type of feed that supplied the package</param>
        /// <returns>A tuple with a boolean indicating the success of the parsing, and the metadata if parsing was successful</returns>
        Maybe<PackageMetadata> TryGetMetadataFromPackageName(string packageFile, FeedType feedType);
        /// <summary>
        /// The server side cache has filenames like 
        /// "com.google.guava#guava#23.3-jre_9822965F2883AD43AD79DA4E8795319F.jar" or
        /// "mypackage.1.0.0.0_BA90F59B8C9EE04DADE2D3501181EFCD.nuget".
        /// This method will break down this filename.
        /// </summary>
        /// <param name="packageFile">The package file name</param>
        /// <param name="extensions">The extensions that this parser should know about</param>
        /// <param name="feedType">The type of feed that supplied the package</param>
        /// <returns>The metadata assocaited with the package file</returns>
        PackageMetadata GetMetadataFromServerPackageName(string packageFile, string[] extensions, FeedType feedType);
        /// <summary>
        /// The server side cache has filenames like 
        /// "com.google.guava#guava#23.3-jre_9822965F2883AD43AD79DA4E8795319F.jar" or
        /// "mypackage.1.0.0.0_BA90F59B8C9EE04DADE2D3501181EFCD.nuget".
        /// This method will break down this filename.
        /// </summary>
        /// <param name="packageFile">The package file name</param>
        /// <param name="feedType">The type of feed that supplied the package</param>
        /// <returns>The metadata assocaited with the package file</returns>
        PackageMetadata GetMetadataFromServerPackageName(string packageFile, FeedType feedType);
        /// <summary>
        /// The server side cache has filenames like 
        /// "com.google.guava#guava#23.3-jre_9822965F2883AD43AD79DA4E8795319F.jar" or
        /// "mypackage.1.0.0.0_BA90F59B8C9EE04DADE2D3501181EFCD.nuget".
        /// This method will break down this filename.
        /// </summary>
        /// <param name="packageFile">The package file name</param>
        /// <param name="extensions">The extensions that this parser should know about</param>
        /// <param name="packageMetadata">The package metadata if the parsing was successful</param>
        /// <param name="feedType">The type of feed that supplied the package</param>
        /// <returns>True if the file could be parsed, and false otherwise</returns>
        bool TryGetMetadataFromServerPackageName(string packageFile, string[] extensions, out PackageMetadata packageMetadata, FeedType feedType);
        /// <summary>
        /// The server side cache has filenames like 
        /// "com.google.guava#guava#23.3-jre_9822965F2883AD43AD79DA4E8795319F.jar" or
        /// "mypackage.1.0.0.0_BA90F59B8C9EE04DADE2D3501181EFCD.nuget".
        /// This method will break down this filename.
        /// </summary>
        /// <param name="packageFile">The package file name</param>
        /// <param name="feedType">The type of feed that supplied the package</param>
        /// <returns>A tuple with a boolean indicating the success of the parsing, and the metadata if parsing was successful</returns>
        Maybe<PackageMetadata> TryGetMetadataFromServerPackageName(string packageFile, string[] extensions, FeedType feedType);
        /// <summary>
        /// The server side cache has filenames like 
        /// "com.google.guava#guava#23.3-jre_9822965F2883AD43AD79DA4E8795319F.jar" or
        /// "mypackage.1.0.0.0_BA90F59B8C9EE04DADE2D3501181EFCD.nuget".
        /// This method will break down this filename.
        /// </summary>
        /// <param name="packageFile">The package file name</param>
        /// <param name="feedType">The type of feed that supplied the package</param>
        /// <returns>A tuple with a boolean indicating the success of the parsing, and the metadata if parsing was successful</returns>
        Maybe<PackageMetadata> TryGetMetadataFromServerPackageName(string packageFile, FeedType feedType);
        /// <summary>
        /// The server side cache has filenames like 
        /// "com.google.guava#guava#23.3-jre_9822965F2883AD43AD79DA4E8795319F.jar" or
        /// "mypackage.1.0.0.0_BA90F59B8C9EE04DADE2D3501181EFCD.nuget".
        /// This method will break down this filename.
        /// </summary>
        /// <param name="packageFile">The package file name</param>
        /// <param name="extensions">The extensions that this parser should know about</param>
        /// <param name="size">The file size</param>
        /// <param name="hash">The file hash</param>
        /// <param name="feedType">The type of feed that supplied the package</param>
        /// <returns>The metadata assocaited with the package file</returns>
        PhysicalPackageMetadata GetMetadataFromServerPackageName(
            string packageFile, 
            string[] extensions,
            long size,
            string hash, 
            FeedType feedType);
    }
}