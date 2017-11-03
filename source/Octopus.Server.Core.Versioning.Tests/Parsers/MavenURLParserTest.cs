using NUnit.Framework;
using Octopus.Core.Resources.Parsing.Maven;

namespace Octopus.Server.Core.Versioning.Tests.Parsers
{
    [TestFixture]
    public class MavenURLParserTest
    {
        private static readonly IMavenURLParser MavenUrlParser = new MavenURLParser();
        
        [Test]
        public void TestURLParsing()
        {
            Assert.AreEqual(
                "http://repo.org",
                MavenUrlParser.SanitiseFeedUri("http://repo.org/maven2"));
            
            Assert.AreEqual(
                "http://repo.org",
                MavenUrlParser.SanitiseFeedUri("http://repo.org/maven2/"));
            
            Assert.AreEqual(
                "http://repo.org",
                MavenUrlParser.SanitiseFeedUri("http://repo.org/"));
            
            Assert.AreEqual(
                "http://repo.org",
                MavenUrlParser.SanitiseFeedUri("http://repo.org"));
        }
    }
}