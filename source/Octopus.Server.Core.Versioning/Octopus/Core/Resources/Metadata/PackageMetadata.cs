using Newtonsoft.Json;

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
        /// Used to define the search pattern for files on the target based
        /// on the package id and the version.
        /// </summary>
        [JsonIgnore]
        public string PackageAndVersionSearchPattern {get; set; }
        /// <summary>
        /// Defines the server side cache file name
        /// </summary>
        [JsonIgnore]
        public string ServerPackageFileName {get; set; }
        /// <summary>
        /// Defines the target file name
        /// </summary>
        [JsonIgnore]
        public string TargetPackageFileName {get; set; }
        
        /// <summary>
        /// The delimiter to use between the packageid and the version
        /// </summary>
        [JsonIgnore]
        public string VersionDelimiter { get; set; }
        
        public override string ToString()
        {
            return $"{PackageId}{VersionDelimiter}{Version}";
        }        
    }
}