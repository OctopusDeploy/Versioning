using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace Octopus.Versioning.Tests.Versions
{
    [TestFixture]
    public class ComparisonFixture
    {
        [Test]
        public void CompareVersions()
        {
            var input = new[]
            {
                "1.2.0",
                "2.0.0",
                "1.3.0+foo",
                "1.0.1",
                "1.0.0",
                "alpha.beta",
                "alpha.10",
                "alpha.3",
                "alpha",
                "1.0.0-alpha.11",
                "1.0.0-alpha.beta",
                "1.0.0-alpha.2",
                "1.0.0-alpha"
            };

            var versions = input
                .Select(x => new OctoVersion(x))
                .OrderBy(v => v)
                .Select(v => v.ToString())
                .ToList();

            var expected = new List<string>
            {
                "alpha",
                "alpha.3",
                "alpha.10",
                "alpha.beta",
                "1.0.0-alpha",
                "1.0.0-alpha.2",
                "1.0.0-alpha.11",
                "1.0.0-alpha.beta",
                "1.0.0",
                "1.0.1",
                "1.2.0",
                "1.3.0+foo",
                "2.0.0",
            };

            versions.ShouldBe(expected);
        }
    }
}