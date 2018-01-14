using NUnit.Framework;
using Octopus.Versioning.Semver;

namespace Octopus.Server.Core.Versioning.Tests.Properties
{
    [TestFixture]
    public class SemanticVersionUtilsTest
    {
        [Test]
        public void SpecialVersionPartsCanBeIncrementedEvenWhenVeryLarge()
        {
            var inc = SemanticVersionUtils.IncrementRelease("H191779432091");
            Assert.AreEqual("H191779432092", inc);
        }
    }
}
