using NUnit.Framework;
using Octopus.Versioning.Maven;

namespace Octopus.Versioning.Tests.Versions
{
    [TestFixture]
    public class MavenCoordinateTests
    {
        [Test]
        public void CoordinatesAreParsed()
        {
            var mavenId = new MavenPackageID("group", "artifact", "version", "packaging" ,"classifier");
            Assert.AreEqual("group", mavenId.Group);
            Assert.AreEqual("artifact", mavenId.Artifact);
            Assert.AreEqual("version", mavenId.Version);
            Assert.AreEqual("packaging", mavenId.Packaging);
            Assert.AreEqual("classifier", mavenId.Classifier);
        }
        
        [Test]
        public void GroupAndArtifactCoordinatesAreParsed()
        {
            var mavenId = new MavenPackageID("group:artifact");
            Assert.AreEqual("group", mavenId.Group);
            Assert.AreEqual("artifact", mavenId.Artifact);
        }
        
        [Test]
        public void GavCoordinatesAreParsed()
        {
            var mavenId = new MavenPackageID("group:artifact:version");
            Assert.AreEqual("group", mavenId.Group);
            Assert.AreEqual("artifact", mavenId.Artifact);
            Assert.AreEqual("version", mavenId.Version);
        }
        
        [Test]
        public void GavWithPackagingCoordinatesAreParsed()
        {
            var mavenId = new MavenPackageID("group:artifact:version:packaging");
            Assert.AreEqual("group", mavenId.Group);
            Assert.AreEqual("artifact", mavenId.Artifact);
            Assert.AreEqual("version", mavenId.Version);
            Assert.AreEqual("packaging", mavenId.Packaging);
        }
        
        [Test]
        public void GavWithPackagingAndClassifierCoordinatesAreParsed()
        {
            var mavenId = new MavenPackageID("group:artifact:version:packaging:classifier");
            Assert.AreEqual("group", mavenId.Group);
            Assert.AreEqual("artifact", mavenId.Artifact);
            Assert.AreEqual("version", mavenId.Version);
            Assert.AreEqual("packaging", mavenId.Packaging);
            Assert.AreEqual("classifier", mavenId.Classifier);
        }
        
        /// <summary>
        /// This test is an example of the non-standard group:artifact:packaging format used by
        /// Octopus because versions are not supplied when selecting a package.
        /// </summary>
        [Test]
        public void OctopusSpecificCoordinatesAreParsed()
        {
            var mavenId = MavenPackageID.CreatePackageIdFromOctopusInput("group:artifact:packaging");
            Assert.AreEqual("group", mavenId.Group);
            Assert.AreEqual("artifact", mavenId.Artifact);
            Assert.AreEqual("packaging", mavenId.Packaging);
        }
        
        [Test]
        public void OctopusSpecificCoordinatesAndClassifierAreParsed()
        {
            var mavenId = MavenPackageID.CreatePackageIdFromOctopusInput("group:artifact:packaging:classifier");
            Assert.AreEqual("group", mavenId.Group);
            Assert.AreEqual("artifact", mavenId.Artifact);
            Assert.AreEqual("packaging", mavenId.Packaging);
            Assert.AreEqual("classifier", mavenId.Classifier);
        }
        
        [Test]
        public void OctopusSpecificCoordinatesWithVersionAreParsed()
        {
            var mavenId = MavenPackageID.CreatePackageIdFromOctopusInput("group:artifact:packaging", new MavenVersionParser().Parse("1.0.0"));
            Assert.AreEqual("group", mavenId.Group);
            Assert.AreEqual("artifact", mavenId.Artifact);
            Assert.AreEqual("packaging", mavenId.Packaging);
            Assert.AreEqual("1.0.0", mavenId.Version);
        }
        
        [Test]
        public void OctopusSpecificCoordinatesWithVersionAndClassifierAreParsed()
        {
            var mavenId = MavenPackageID.CreatePackageIdFromOctopusInput("group:artifact:packaging:classifier", new MavenVersionParser().Parse("1.0.0"));
            Assert.AreEqual("group", mavenId.Group);
            Assert.AreEqual("artifact", mavenId.Artifact);
            Assert.AreEqual("packaging", mavenId.Packaging);
            Assert.AreEqual("classifier", mavenId.Classifier);
            Assert.AreEqual("1.0.0", mavenId.Version);
        }
    }
}