using Newtonsoft.Json;

namespace Octopus.Core.Resources.Metadata
{
    /// <summary>
    /// Represents the metadata from a physical file
    /// </summary>
    public class PhysicalPackageMetadata : PackageMetadata
    {
        const string DEFAULT_VERSION_DELIMITER = ".";
        
        [JsonConstructor]
        public PhysicalPackageMetadata(string packageId, string version, long size, string hash, string fileExtension)
        {
            FileExtension = fileExtension;
            PackageId = packageId;
            Version = version;
            Size = size;
            Hash = hash;
            VersionDelimiter = DEFAULT_VERSION_DELIMITER;
        }
        
        /// <summary>
        /// Used when the file name representation of the pacakage does not use the Octopus/Nuget 
        /// default of a period
        /// </summary>
        /// <param name="packageId">The package id</param>
        /// <param name="version">The package version</param>
        /// <param name="size">The package size</param>
        /// <param name="hash">The package hash value</param>
        /// <param name="fileExtension">The package file extension</param>
        /// <param name="versionDelimiter">The custom delimiter used to separate packageId from version</param>
        public PhysicalPackageMetadata(string packageId, string version, long size, string hash, string fileExtension, string versionDelimiter)
        {
            FileExtension = fileExtension;
            PackageId = packageId;
            Version = version;
            Size = size;
            Hash = hash;
            VersionDelimiter = versionDelimiter;
        }

        public PhysicalPackageMetadata()
        {
            
        }
        
        public long Size { get; set; }
        public string Hash { get; set; }
        
        public bool Equals(PhysicalPackageMetadata other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(PackageId, other.PackageId) && 
                   string.Equals(Version, other.Version) && 
                   Size == other.Size && 
                   string.Equals(Hash, other.Hash);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PackageMetadata)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = PackageId?.GetHashCode() ?? 0;
                hashCode = (hashCode*397) ^ (Version?.GetHashCode() ?? 0);
                hashCode = (hashCode*397) ^ Size.GetHashCode();
                hashCode = (hashCode*397) ^ (Hash?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        public static bool operator ==(PhysicalPackageMetadata left, PhysicalPackageMetadata right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PhysicalPackageMetadata left, PhysicalPackageMetadata right)
        {
            return !Equals(left, right);
        }
    }
}