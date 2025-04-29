using System;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Octopus.Versioning.Maven;

namespace Octopus.Versioning.Tests.Maven
{
    /// <summary>
    /// Tests copied from org.codehaus.mojo.buildhelper.ParseVersionTest
    /// </summary>
    [TestFixture]
    public class MavenTests
    {
        [Test]
        public void TestJunkVersion()
        {
            // Test a junk version string
            var version = new MavenVersionParser().Parse("junk");

            ClassicAssert.AreEqual(0, version.Major);
            ClassicAssert.AreEqual(0, version.Minor);
            ClassicAssert.AreEqual(0, version.Patch);
            ClassicAssert.AreEqual("junk", version.Release);
            ClassicAssert.AreEqual(0, version.Revision);
        }

        [Test]
        public void TestBasicMavenVersionstring()
        {
            // Test a basic maven version string
            var version = new MavenVersionParser().Parse("1.0.0");

            ClassicAssert.AreEqual(1, version.Major);
            ClassicAssert.AreEqual(0, version.Minor);
            ClassicAssert.AreEqual(0, version.Patch);
            ClassicAssert.AreEqual("", version.Release);
            ClassicAssert.AreEqual(0, version.Revision);
        }

        [Test]
        public void TestVersionstringWithQualifier()
        {
            // Test a version string with qualifier
            var version = new MavenVersionParser().Parse("2.3.4-beta-5");

            ClassicAssert.AreEqual(2, version.Major);
            ClassicAssert.AreEqual(3, version.Minor);
            ClassicAssert.AreEqual(4, version.Patch);
            ClassicAssert.AreEqual("beta-5", version.Release);
            ClassicAssert.AreEqual(0, version.Revision);
        }

        [Test]
        public void TestOsGiVersionstringWithQualifier()
        {
            // Test an osgi version string
            var version = new MavenVersionParser().Parse("2.3.4.beta_5");

            ClassicAssert.AreEqual(2, version.Major);
            ClassicAssert.AreEqual(3, version.Minor);
            ClassicAssert.AreEqual(4, version.Patch);
            ClassicAssert.AreEqual("beta_5", version.Release);
            ClassicAssert.AreEqual(0, version.Revision);
        }

        [Test]
        public void TestSnapshotVersion()
        {
            // Test a snapshot version string
            var version = new MavenVersionParser().Parse("1.2.3-SNAPSHOT");

            ClassicAssert.AreEqual(1, version.Major);
            ClassicAssert.AreEqual(2, version.Minor);
            ClassicAssert.AreEqual(3, version.Patch);
            ClassicAssert.AreEqual("SNAPSHOT", version.Release);
            ClassicAssert.AreEqual(0, version.Revision);
        }

        [Test]
        public void TestSnapshotVersion2()
        {
            // Test a snapshot version string
            var version = new MavenVersionParser().Parse("2.0.17-SNAPSHOT");

            ClassicAssert.AreEqual(2, version.Major);
            ClassicAssert.AreEqual(0, version.Minor);
            ClassicAssert.AreEqual(17, version.Patch);
            ClassicAssert.AreEqual("SNAPSHOT", version.Release);
            ClassicAssert.AreEqual(0, version.Revision);
        }

        [Test]
        public void TestVersionstringWithBuildNumber()
        {
            // Test a version string with a build number
            var version = new MavenVersionParser().Parse("1.2.3-4");

            ClassicAssert.AreEqual(1, version.Major);
            ClassicAssert.AreEqual(2, version.Minor);
            ClassicAssert.AreEqual(3, version.Patch);
            ClassicAssert.AreEqual("", version.Release);
            ClassicAssert.AreEqual(4, version.Revision);
        }

        [Test]
        public void TestSnapshotVersionstringWithBuildNumber()
        {
            // Test a version string with a build number
            var version = new MavenVersionParser().Parse("1.2.3-4-SNAPSHOT");

            ClassicAssert.AreEqual(1, version.Major);
            ClassicAssert.AreEqual(2, version.Minor);
            ClassicAssert.AreEqual(3, version.Patch);
            ClassicAssert.AreEqual("-SNAPSHOT", version.Release);
            ClassicAssert.AreEqual(4, version.Revision);
        }

        [Test]
        public void TestPrerelease()
        {
            ClassicAssert.IsTrue(new MavenVersionParser().Parse("1.0.0-alpha").IsPrerelease);
            ClassicAssert.IsTrue(new MavenVersionParser().Parse("1.0.0-alpha1").IsPrerelease);
            ClassicAssert.IsTrue(new MavenVersionParser().Parse("1.0.0-a1").IsPrerelease);
            ClassicAssert.IsTrue(new MavenVersionParser().Parse("1.0.0-alpha1-SNAPSHOT").IsPrerelease);
            ClassicAssert.IsTrue(new MavenVersionParser().Parse("1.0.0-ALPHA").IsPrerelease);
            ClassicAssert.IsTrue(new MavenVersionParser().Parse("1.0.0-ALPHA1").IsPrerelease);
            ClassicAssert.IsTrue(new MavenVersionParser().Parse("1.0.0-A1").IsPrerelease);
            ClassicAssert.IsTrue(new MavenVersionParser().Parse("1.0.0-ALPHA1-SNAPSHOT").IsPrerelease);
            ClassicAssert.IsTrue(new MavenVersionParser().Parse("1.0.0-beta").IsPrerelease);
            ClassicAssert.IsTrue(new MavenVersionParser().Parse("1.0.0-beta1").IsPrerelease);
            ClassicAssert.IsTrue(new MavenVersionParser().Parse("1.0.0-b1").IsPrerelease);
            ClassicAssert.IsTrue(new MavenVersionParser().Parse("1.0.0-BETA").IsPrerelease);
            ClassicAssert.IsTrue(new MavenVersionParser().Parse("1.0.0-BETA1").IsPrerelease);
            ClassicAssert.IsTrue(new MavenVersionParser().Parse("1.0.0-B1").IsPrerelease);
            ClassicAssert.IsTrue(new MavenVersionParser().Parse("1.0.0-milestone").IsPrerelease);
            ClassicAssert.IsTrue(new MavenVersionParser().Parse("1.0.0-milestone1").IsPrerelease);
            ClassicAssert.IsTrue(new MavenVersionParser().Parse("1.0.0-m1").IsPrerelease);
            ClassicAssert.IsTrue(new MavenVersionParser().Parse("1.0.0-MILESTONE").IsPrerelease);
            ClassicAssert.IsTrue(new MavenVersionParser().Parse("1.0.0-MILESTONE1").IsPrerelease);
            ClassicAssert.IsTrue(new MavenVersionParser().Parse("1.0.0-M1").IsPrerelease);
            ClassicAssert.IsTrue(new MavenVersionParser().Parse("1.0.0-CR").IsPrerelease);
            ClassicAssert.IsTrue(new MavenVersionParser().Parse("1.0.0-CR1").IsPrerelease);
            ClassicAssert.IsTrue(new MavenVersionParser().Parse("1.0.0-cr").IsPrerelease);
            ClassicAssert.IsTrue(new MavenVersionParser().Parse("1.0.0-cr1").IsPrerelease);
            ClassicAssert.IsTrue(new MavenVersionParser().Parse("1.0.0-RC").IsPrerelease);
            ClassicAssert.IsTrue(new MavenVersionParser().Parse("1.0.0-RC1").IsPrerelease);
            ClassicAssert.IsTrue(new MavenVersionParser().Parse("1.0.0-rc").IsPrerelease);
            ClassicAssert.IsTrue(new MavenVersionParser().Parse("1.0.0-rc1").IsPrerelease);
            ClassicAssert.IsTrue(new MavenVersionParser().Parse("22.0-rc1").IsPrerelease);
            ClassicAssert.IsTrue(new MavenVersionParser().Parse("1.0.0-SNAPSHOT").IsPrerelease);
            ClassicAssert.IsTrue(new MavenVersionParser().Parse("1.0.0-snapshot").IsPrerelease);
            ClassicAssert.IsFalse(new MavenVersionParser().Parse("1.0.0").IsPrerelease);
            ClassicAssert.IsFalse(new MavenVersionParser().Parse("1.0.0-a").IsPrerelease);
            ClassicAssert.IsFalse(new MavenVersionParser().Parse("1.0.0-release").IsPrerelease);
        }
    }
}