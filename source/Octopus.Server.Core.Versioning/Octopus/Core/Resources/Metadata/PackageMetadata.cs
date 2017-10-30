namespace Octopus.Core.Resources
{
    public class PackageMetadata : BasePackageMetadata
    {
        /// <summary>
        /// The package version
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// The package file extension
        /// </summary>
        public string FileExtension { get; set; }
        /// <summary>
        /// Used to define the search pattern for files on the target
        /// </summary>
        public string PackageSearchPattern {get; set; }
        /// <summary>
        /// Defines the server side cache file name
        /// </summary>
        public string PackageFileName {get; set; }
    }
}