using System;
using Newtonsoft.Json;

namespace Octopus.Core.Resources
{
    public class PackageMetadata : BasePackageMetadata
    {
        private string _packageAndVersionSearchPattern;
        private string _serverPackageFileName;
        private string _targetPackageFileName;
        private string _versionDelimiter;

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
        public string PackageAndVersionSearchPattern
        {
            get
            {
                if (_packageAndVersionSearchPattern == null)
                {
                    throw new NullReferenceException("packageAndVersionSearchPattern can not be null");
                }
                return _packageAndVersionSearchPattern;
            }
            set => _packageAndVersionSearchPattern = value;
        }

        /// <summary>
        /// Defines the server side cache file name
        /// </summary>
        [JsonIgnore]
        public string ServerPackageFileName
        {
            get
            {
                if (_serverPackageFileName == null)
                {
                    throw new NullReferenceException("serverPackageFileName can not be null");
                }
                return _serverPackageFileName;
            }
            set => _serverPackageFileName = value;
        }

        /// <summary>
        /// Defines the target file name
        /// </summary>
        [JsonIgnore]
        public string TargetPackageFileName
        {
            get
            {
                if (_targetPackageFileName == null)
                {
                    throw new NullReferenceException("targetPackageFileName can not be null");
                }
                return _targetPackageFileName;
            }
            set => _targetPackageFileName = value;
        }

        /// <summary>
        /// The delimiter to use between the packageid and the version
        /// </summary>
        [JsonIgnore]
        public string VersionDelimiter       
        {
            get
            {
                if (_versionDelimiter == null)
                {
                    throw new NullReferenceException("versionDelimiter can not be null");
                }
                return _versionDelimiter;
            }
            set => _versionDelimiter = value;
        }

        public override string ToString()
        {
            return $"{PackageId}{VersionDelimiter}{Version}";
        }
    }
}