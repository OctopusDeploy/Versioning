using NUnit.Framework;
using Octopus.Core.Resources.Versioning.Maven;

namespace Octopus.Server.Core.Versioning.Tests.Versions
{
    /// <summary>
    /// Tests copied from org.codehaus.mojo.buildhelper.ParseVersionTest
    /// </summary>
    [TestFixture]
    public class MavenTests
    {       
        [Test]
        public void testJunkVersion()           
        {
            // Test a junk version string
            var version = new MavenVersionParser().Parse( "junk" );
                       
            Assert.AreEqual( 0, version.Major );
            Assert.AreEqual( 0, version.Minor );
            Assert.AreEqual( 0, version.Patch );
            Assert.AreEqual( "junk", version.Release );
            Assert.AreEqual( 0, version.Revision );
        }

        [Test]
        public void testBasicMavenVersionString()
            
        {
            // Test a basic maven version string
            var version = new MavenVersionParser().Parse( "1.0.0" );                       

            Assert.AreEqual( 1, version.Major );
            Assert.AreEqual( 0, version.Minor );
            Assert.AreEqual( 0, version.Patch );
            Assert.AreEqual( "", version.Release );
            Assert.AreEqual( 0, version.Revision );
        }

        [Test]
        public void testVersionStringWithQualifier()
        {
            // Test a version string with qualifier
            var version = new MavenVersionParser().Parse( "2.3.4-beta-5" );

            Assert.AreEqual( 2, version.Major );
            Assert.AreEqual( 3, version.Minor );
            Assert.AreEqual( 4, version.Patch );
            Assert.AreEqual( "beta-5", version.Release );
            Assert.AreEqual( 0, version.Revision );
        }

        [Test]
        public void testOSGiVersionStringWithQualifier()
        {
            // Test an osgi version string
            var version = new MavenVersionParser().Parse( "2.3.4.beta_5" );

            Assert.AreEqual( 2, version.Major );
            Assert.AreEqual( 3, version.Minor );
            Assert.AreEqual( 4, version.Patch );
            Assert.AreEqual( "beta_5", version.Release );
            Assert.AreEqual( 0, version.Revision );
        }

        [Test]
        public void testSnapshotVersion()
        {
            // Test a snapshot version string
            var version = new MavenVersionParser().Parse( "1.2.3-SNAPSHOT" );

            Assert.AreEqual( 1, version.Major );
            Assert.AreEqual( 2, version.Minor );
            Assert.AreEqual( 3, version.Patch );
            Assert.AreEqual( "SNAPSHOT", version.Release );
            Assert.AreEqual( 0, version.Revision );
        }

        [Test]
        public void testSnapshotVersion2()
        {
            // Test a snapshot version string
            var version = new MavenVersionParser().Parse( "2.0.17-SNAPSHOT" );

            Assert.AreEqual( 2, version.Major );
            Assert.AreEqual( 0, version.Minor );
            Assert.AreEqual( 17, version.Patch );
            Assert.AreEqual( "SNAPSHOT", version.Release );
            Assert.AreEqual( 0, version.Revision );
        }

        [Test]
        public void testVersionStringWithBuildNumber()
        {
            // Test a version string with a build number
            var version = new MavenVersionParser().Parse( "1.2.3-4" );

            Assert.AreEqual( 1, version.Major );
            Assert.AreEqual( 2, version.Minor );
            Assert.AreEqual( 3, version.Patch );
            Assert.AreEqual( "", version.Release );
            Assert.AreEqual( 4, version.Revision );

        }

        [Test]
        public void testSnapshotVersionStringWithBuildNumber()
        {
            // Test a version string with a build number
            var version = new MavenVersionParser().Parse( "1.2.3-4-SNAPSHOT" );

            Assert.AreEqual( 1, version.Major );
            Assert.AreEqual( 2, version.Minor );
            Assert.AreEqual( 3, version.Patch );
            Assert.AreEqual( "-SNAPSHOT", version.Release );
            Assert.AreEqual( 4, version.Revision );
        }
        
        [Test]
        public void testPrerelease()            
        {
            Assert.IsTrue(new MavenVersionParser().Parse( "1.0.0-alpha" ).IsPrerelease);                       
            Assert.IsTrue(new MavenVersionParser().Parse( "1.0.0-alpha1" ).IsPrerelease);                       
            Assert.IsTrue(new MavenVersionParser().Parse( "1.0.0-a1" ).IsPrerelease);                       
            Assert.IsTrue(new MavenVersionParser().Parse( "1.0.0-alpha1-SNAPSHOT" ).IsPrerelease);                       
            Assert.IsTrue(new MavenVersionParser().Parse( "1.0.0-beta" ).IsPrerelease);                       
            Assert.IsTrue(new MavenVersionParser().Parse( "1.0.0-beta1" ).IsPrerelease);                       
            Assert.IsTrue(new MavenVersionParser().Parse( "1.0.0-b1" ).IsPrerelease);                       
            Assert.IsTrue(new MavenVersionParser().Parse( "1.0.0-MILESTONE" ).IsPrerelease);                       
            Assert.IsTrue(new MavenVersionParser().Parse( "1.0.0-MILESTONE1" ).IsPrerelease);                       
            Assert.IsTrue(new MavenVersionParser().Parse( "1.0.0-M1" ).IsPrerelease);                       
            Assert.IsTrue(new MavenVersionParser().Parse( "1.0.0-CR" ).IsPrerelease);                       
            Assert.IsTrue(new MavenVersionParser().Parse( "1.0.0-CR1" ).IsPrerelease);                       
            Assert.IsTrue(new MavenVersionParser().Parse( "1.0.0-RC" ).IsPrerelease);                       
            Assert.IsTrue(new MavenVersionParser().Parse( "1.0.0-RC1" ).IsPrerelease);                       
            Assert.IsTrue(new MavenVersionParser().Parse( "1.0.0-SNAPSHOT" ).IsPrerelease);                  
            Assert.IsFalse(new MavenVersionParser().Parse( "1.0.0" ).IsPrerelease);                  
            Assert.IsFalse(new MavenVersionParser().Parse( "1.0.0-a" ).IsPrerelease);                  
            Assert.IsFalse(new MavenVersionParser().Parse( "1.0.0-release" ).IsPrerelease);                  

        }
    }
}