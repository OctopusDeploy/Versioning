using System;
using NUnit.Framework;
using Octopus.Versioning.Metadata;

namespace Octopus.Versioning.Tests.Parsers
{
    [TestFixture]
    public class Tests
    {
        private readonly IPackageIDParser MavenParser = new MavenPackageIDParser();
        
        [Test]
        public void ParseMavenPackageId()
        {
            var metadata = MavenParser.GetMetadataFromPackageID("Maven#com.google.guava#guava");
            Assert.AreEqual(metadata.PackageId, "Maven#com.google.guava#guava");
            Assert.AreEqual(metadata.VersionFormat, VersionFormat.Maven);
        }
        
        [Test]
        public void ParseMavenServerPackage()
        {
            var filePath = "C:\\Maven#com.google.guava#guava#23.3-jre_9822965F2883AD43AD79DA4E8795319F.jar";
            var metadata = MavenParser.GetMetadataFromServerPackageName(filePath, new string[] {".jar"});
            Assert.AreEqual(metadata.FileExtension, ".jar");
            Assert.AreEqual(metadata.PackageId, "Maven#com.google.guava#guava");
            Assert.AreEqual(metadata.Version, "23.3-jre");
            Assert.AreEqual(metadata.VersionFormat, VersionFormat.Maven);
        }
        
        [Test]
        public void ParseMavenTargetPackage()
        {
            var filePath = "C:\\Maven#com.google.guava#guava#22.0.jar-e55fcd51-6081-4300-91a3-117b7930c023";
            var metadata = MavenParser.GetMetadataFromPackageName(filePath, new string[] {".jar"});
            Assert.AreEqual(metadata.FileExtension, ".jar");
            Assert.AreEqual(metadata.PackageId, "Maven#com.google.guava#guava");
            Assert.AreEqual(metadata.Version, "22.0");
            Assert.AreEqual(metadata.VersionFormat, VersionFormat.Maven);
        }
        
        [Test]
        public void MavenFailToParseNuGetTargetPackage()
        {
            Assert.Throws<Exception>(() =>
            {
                var filePath = "C:\\package.suffix.1.0.0.zip-e55fcd51-6081-4300-91a3-117b7930c023";
                var metadata = MavenParser.GetMetadataFromPackageName(filePath, new string[] {".jar"});
            });
        }
    }
}