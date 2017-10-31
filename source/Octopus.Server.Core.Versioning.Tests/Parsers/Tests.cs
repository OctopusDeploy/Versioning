using NUnit.Framework;
using Octopus.Core.Resources;
using Octopus.Core.Resources.Metadata;

namespace Octopus.Server.Core.Versioning.Tests.Parsers
{
    [TestFixture]
    public class Tests
    {
        private readonly IPackageIDParser MavenParser = new MavenPackageIDParser();
        private readonly IPackageIDParser NuGetParser = new NuGetPackageIDParser();
        
        [Test]
        public void ParseMavenPackageId()
        {
            var metadata = MavenParser.GetMetadataFromPackageID("com.google.guava#guava");
            Assert.AreEqual(metadata.PackageId, "com.google.guava#guava");
            Assert.AreEqual(metadata.FeedType, FeedType.Maven);
        }
        
        [Test]
        public void ParseNuGetPackageId()
        {
            var metadata = NuGetParser.GetMetadataFromPackageID("NuGet.Package");
            Assert.AreEqual(metadata.PackageId, "NuGet.Package");
            Assert.AreEqual(metadata.FeedType, FeedType.NuGet);
        }
        
        [Test]
        public void ParseMavenServerPackagePhysicalMetadata()
        {
            var filePath = "C:\\Maven#com.google.guava#guava#23.3-jre_9822965F2883AD43AD79DA4E8795319F.jar";
            var metadata = MavenParser.GetMetadataFromServerPackageName(
                filePath, 
                new string[] {".jar"},
                123,
                "hash");
            Assert.AreEqual(metadata.FileExtension, ".jar");
            Assert.AreEqual(metadata.PackageId, "com.google.guava#guava");
            Assert.AreEqual(metadata.Version, "23.3-jre");
            Assert.AreEqual(metadata.FeedType, FeedType.Maven);
            Assert.AreEqual(metadata.Size, 123);
            Assert.AreEqual(metadata.Hash, "hash");
        }
        
        [Test]
        public void ParseNuGetServerPackagePhysicalMetadata()
        {
            var filePath = "C:\\package.suffix.1.0.0_9822965F2883AD43AD79DA4E8795319F.zip";
            var metadata = NuGetParser.GetMetadataFromServerPackageName(
                filePath, 
                new string[] {".zip"},
                123,
                "hash");
            Assert.AreEqual(metadata.FileExtension, ".zip");
            Assert.AreEqual(metadata.PackageId, "package.suffix");
            Assert.AreEqual(metadata.Version, "1.0.0");
            Assert.AreEqual(metadata.FeedType, FeedType.NuGet);
            Assert.AreEqual(metadata.Size, 123);
            Assert.AreEqual(metadata.Hash, "hash");
        }
        
        [Test]
        public void ParseMavenServerPackage()
        {
            var filePath = "C:\\Maven#com.google.guava#guava#23.3-jre_9822965F2883AD43AD79DA4E8795319F.jar";
            var metadata = MavenParser.GetMetadataFromServerPackageName(filePath, new string[] {".jar"});
            Assert.AreEqual(metadata.FileExtension, ".jar");
            Assert.AreEqual(metadata.PackageId, "com.google.guava#guava");
            Assert.AreEqual(metadata.Version, "23.3-jre");
            Assert.AreEqual(metadata.FeedType, FeedType.Maven);
        }
        
        [Test]
        public void ParseMavenTargetPackage()
        {
            var filePath = "C:\\Maven#com.google.guava#guava#22.0.jar-e55fcd51-6081-4300-91a3-117b7930c023";
            var metadata = MavenParser.GetMetadataFromPackageName(filePath, new string[] {".jar"});
            Assert.AreEqual(metadata.FileExtension, ".jar");
            Assert.AreEqual(metadata.PackageId, "com.google.guava#guava");
            Assert.AreEqual(metadata.Version, "22.0");
            Assert.AreEqual(metadata.FeedType, FeedType.Maven);
        }
        
        [Test]
        public void MavenFailToParseNuGetTargetPackage()
        {
            try
            {
                var filePath = "C:\\package.suffix.1.0.0.zip-e55fcd51-6081-4300-91a3-117b7930c023";
                var metadata = MavenParser.GetMetadataFromPackageName(filePath, new string[] {".jar"});
            }
            catch
            {
                /*
                 * This exception is expected
                 */
                return;
            }

            Assert.Fail();
        }
        
        [Test]
        public void ParseNugetServerPackage()
        {
            var filePath = "C:\\package.suffix.1.0.0_9822965F2883AD43AD79DA4E8795319F.zip";
            var metadata = NuGetParser.GetMetadataFromServerPackageName(filePath, new string[] {".zip"});
            Assert.AreEqual(metadata.FileExtension, ".zip");
            Assert.AreEqual(metadata.PackageId, "package.suffix");
            Assert.AreEqual(metadata.Version, "1.0.0");
            Assert.AreEqual(metadata.FeedType, FeedType.NuGet);
        }
        
        [Test]
        public void ParseNugetTargetPackage()
        {
            var filePath = "C:\\package.suffix.1.0.0.zip-e55fcd51-6081-4300-91a3-117b7930c023";
            var metadata = NuGetParser.GetMetadataFromPackageName(filePath, new string[] {".zip"});
            Assert.AreEqual(metadata.FileExtension, ".zip");
            Assert.AreEqual(metadata.PackageId, "package.suffix");
            Assert.AreEqual(metadata.Version, "1.0.0");
            Assert.AreEqual(metadata.FeedType, FeedType.NuGet);
        }
        
        [Test]
        public void NuGetFailToParseMavenTargetPackage()
        {
            try
            {
                var filePath = "C:\\Maven#com.google.guava#guava#22.0.jar-e55fcd51-6081-4300-91a3-117b7930c023";
                var metadata = NuGetParser.GetMetadataFromPackageName(filePath, new string[] {".jar"});
            }
            catch
            {
                /*
                 * This exception is expected
                 */
                return;
            }

            Assert.Fail();
        }
    }
}