﻿using Newtonsoft.Json;

namespace Octopus.Core.Resources
{
    /// <summary>
    /// Represents the metadata that can be extracted from the package id alone
    /// </summary>
    public class BasePackageMetadata
    {
        public string PackageId { get; set; }
        
        [JsonIgnore]
        public FeedType FeedType { get; set; }
        
        /// <summary>
        /// Used to define the search pattern for files on the target base only
        /// on the package id.
        /// </summary>
        [JsonIgnore]
        public string PackageSearchPattern {get; set; }
    }
}