namespace Octopus.Core.Resources
{
    /// <summary>
    /// Throughout the lifecycle of a package deployment we have access to various
    /// bits of information that we need to convert into consistent metadata to
    /// make decisions about what feed the package came from, how the filenames
    /// for the package should be formatted, the patterns to use when searching for
    /// similar packages etc.
    /// 
    /// This interface defines the various was we can get this consistent metadata
    /// from these snippets of information. 
    /// </summary>
    public interface IPackageIDParser
    {
        /// <summary>
        /// Extracts metadata from a package ID (i.e. no version information)
        /// </summary>
        /// <param name="packageID">The package id</param>
        /// <returns>The metadata assocaited with the package id</returns>
        BasePackageMetadata GetMetadataFromPackageID(string packageID);
        /// <summary>
        /// Extracts metadata from a package ID (i.e. no version information)
        /// </summary>
        /// <param name="packageID">The package id</param>
        /// <param name="version">The package version</param>
        /// <param name="extension">The package extension</param>
        /// <returns>The metadata assocaited with the package id</returns>
        PackageMetadata GetMetadataFromPackageID(string packageID, string version, string extension);
        /// <summary>
        /// The target Files cache has filenames like 
        /// "com.google.guava#guava#23.3-jre.jar-e55fcd51-6081-4300-91a3-117b7930c023" or
        /// "com.google.guava#guava#23.3-jre.jar" or
        /// "mypackage.1.0.0.0.nuget-f363ce3a-0657-401a-8831-f3634f6cca2b".
        /// This method will break down these filenames.
        /// </summary>
        /// <param name="packageFile">The package file name</param>
        /// <param name="extensions">The extensions that this parser should know about</param>
        /// <returns>The metadata assocaited with the package file</returns>
        PackageMetadata GetMetadataFromPackageName(string packageFile, string[] extensions);
        /// <summary>
        /// The server side cache has filenames like 
        /// "com.google.guava#guava#23.3-jre_9822965F2883AD43AD79DA4E8795319F.jar" or
        /// "mypackage.1.0.0.0_BA90F59B8C9EE04DADE2D3501181EFCD.nuget".
        /// This method will break down this filename.
        /// </summary>
        /// <param name="packageFile">The package file name</param>
        /// <param name="extensions">The extensions that this parser should know about</param>
        /// <returns>The metadata assocaited with the package file</returns>
        PackageMetadata GetMetadataFromServerPackageName(string packageFile, string[] extensions);
    }
}