using NUnit.Framework;
using Octopus.Core.Resources;
using Octopus.Core.Resources.Metadata;

namespace Octopus.Server.Core.Versioning.Tests.Parsers
{
    [TestFixture]
    public class Tests
    {
        private readonly IPackageIDParser MavenParser = new MavenPackageIDParser();
        
        [Test]
        public void Test1()
        {
            var filePath = "C:\\com.google.guava#guava#23.3-jre_9822965F2883AD43AD79DA4E8795319F.jar";
            var metadata = MavenParser.GetMetadataFromServerPackageName(filePath, new string[] {".jar"});
            Assert.AreEqual(metadata.FileExtension, ".jar");
        }
    }
}