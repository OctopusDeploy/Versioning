namespace Octopus.Core.Resources
{
    public class PackageMetadata : BasePackageMetadata
    {
        public string Version { get; set; }
        public string FileExtension { get; set; }
        public string PackageSearchPattern {get; set; }
    }
}