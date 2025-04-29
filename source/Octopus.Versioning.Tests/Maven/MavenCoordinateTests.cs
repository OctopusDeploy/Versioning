using System;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Octopus.Versioning.Maven;

namespace Octopus.Versioning.Tests.Versions
{
    [TestFixture]
    public class MavenCoordinateTests
    {
        [Test]
        public void CoordinatesAreParsed()
        {
            var mavenId = new MavenPackageID("group",
                "artifact",
                "version",
                "packaging",
                "classifier");
            ClassicAssert.AreEqual("group", mavenId.Group);
            ClassicAssert.AreEqual("artifact", mavenId.Artifact);
            ClassicAssert.AreEqual("version", mavenId.Version);
            ClassicAssert.AreEqual("packaging", mavenId.Packaging);
            ClassicAssert.AreEqual("classifier", mavenId.Classifier);
        }

        [Test]
        public void GavCoordinatesAreParsed()
        {
            var mavenId = new MavenPackageID("group:artifact:version");
            ClassicAssert.AreEqual("group", mavenId.Group);
            ClassicAssert.AreEqual("artifact", mavenId.Artifact);
            ClassicAssert.AreEqual("version", mavenId.Version);
        }

        [Test]
        public void GavWithPackagingCoordinatesAreParsed()
        {
            var mavenId = new MavenPackageID("group:artifact:version:packaging");
            ClassicAssert.AreEqual("group", mavenId.Group);
            ClassicAssert.AreEqual("artifact", mavenId.Artifact);
            ClassicAssert.AreEqual("version", mavenId.Version);
            ClassicAssert.AreEqual("packaging", mavenId.Packaging);
        }

        [Test]
        public void GavWithPackagingAndClassifierCoordinatesAreParsed()
        {
            var mavenId = new MavenPackageID("group:artifact:version:packaging:classifier");
            ClassicAssert.AreEqual("group", mavenId.Group);
            ClassicAssert.AreEqual("artifact", mavenId.Artifact);
            ClassicAssert.AreEqual("version", mavenId.Version);
            ClassicAssert.AreEqual("packaging", mavenId.Packaging);
            ClassicAssert.AreEqual("classifier", mavenId.Classifier);
        }

        /// <summary>
        /// This test is an example of the non-standard group:artifact:packaging format used by
        /// Octopus because versions are not supplied when selecting a package.
        /// </summary>
        [Test]
        public void OctopusSpecificCoordinatesAreParsed()
        {
            var mavenId = MavenPackageID.CreatePackageIdFromOctopusInput("group:artifact:packaging");
            ClassicAssert.AreEqual("group", mavenId.Group);
            ClassicAssert.AreEqual("artifact", mavenId.Artifact);
            ClassicAssert.AreEqual("packaging", mavenId.Packaging);
        }

        [Test]
        public void OctopusSpecificCoordinatesAndClassifierAreParsed()
        {
            var mavenId = MavenPackageID.CreatePackageIdFromOctopusInput("group:artifact:packaging:classifier");
            ClassicAssert.AreEqual("group", mavenId.Group);
            ClassicAssert.AreEqual("artifact", mavenId.Artifact);
            ClassicAssert.AreEqual("packaging", mavenId.Packaging);
            ClassicAssert.AreEqual("classifier", mavenId.Classifier);
        }

        [Test]
        public void OctopusSpecificCoordinatesWithVersionAreParsed()
        {
            var mavenId = MavenPackageID.CreatePackageIdFromOctopusInput("group:artifact:packaging", new MavenVersionParser().Parse("1.0.0"));
            ClassicAssert.AreEqual("group", mavenId.Group);
            ClassicAssert.AreEqual("artifact", mavenId.Artifact);
            ClassicAssert.AreEqual("packaging", mavenId.Packaging);
            ClassicAssert.AreEqual("1.0.0", mavenId.Version);
        }

        [Test]
        public void OctopusSpecificCoordinatesWithVersionAndClassifierAreParsed()
        {
            var mavenId = MavenPackageID.CreatePackageIdFromOctopusInput("group:artifact:packaging:classifier", new MavenVersionParser().Parse("1.0.0"));
            ClassicAssert.AreEqual("group", mavenId.Group);
            ClassicAssert.AreEqual("artifact", mavenId.Artifact);
            ClassicAssert.AreEqual("packaging", mavenId.Packaging);
            ClassicAssert.AreEqual("classifier", mavenId.Classifier);
            ClassicAssert.AreEqual("1.0.0", mavenId.Version);
        }
    }
}