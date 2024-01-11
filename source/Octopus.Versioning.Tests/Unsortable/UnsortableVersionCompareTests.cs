using NUnit.Framework;
using Octopus.Versioning.Unsortable;

namespace Octopus.Versioning.Tests.Unsortable;

public class UnsortableVersionCompareTests
{
    static readonly UnsortableVersionParser UnsortableVersionParser = new();

    // [Test]
    // [TestCase("release", "release", 0)]
    // [TestCase("release", "qrelease", 1)]
    // [TestCase("release", "srelease", -1)]
    // [TestCase("123", "123", 0)]
    // [TestCase("123", "100", 1)]
    // [TestCase("123", "321", -1)]
    // [TestCase("123Release", "123Release", 0)]
    // [TestCase("123Release", "100Release", 1)]
    // [TestCase("123Release", "321Release", -1)]
    // [TestCase("release-1", "release-1", 0)]
    // [TestCase("release-1", "release-0", 1)]
    // [TestCase("release-1", "release-2", -1)]
    // [TestCase("release.1", "release.1", 0)]
    // [TestCase("release.1", "release.0", 1)]
    // [TestCase("release.1", "release.2", -1)]
    // [TestCase("release_1", "release_1", 0)]
    // [TestCase("release_1", "release_0", 1)]
    // [TestCase("release_1", "release_2", -1)]
    // [TestCase("release-1", "release_1", 0)]
    // [TestCase("release.1", "release_1", 0)]
    // [TestCase("release-1", "release_0", 1)]
    // [TestCase("release.1", "release_2", -1)]
    // [TestCase("release+123", "release+321", 0)]
    // [TestCase("release-1+123", "release-1+321", 0)]
    // [TestCase("release-1+123", "release-0+321", 1)]
    // [TestCase("release-1+123", "release-2+321", -1)]
    // public void TestComparisons(string version1, string version2, int result)
    // {
    //     var parsedVersion1 = UnsortableVersionParser.Parse(version1);
    //     var parsedVersion2 = UnsortableVersionParser.Parse(version2);
    //     Assert.AreEqual(result, parsedVersion1.CompareTo(parsedVersion2));
    // }

    [Test]
    [TestCase("release-1", "release-1", true)]
    [TestCase("release-1", "release-2", false)]
    public void TestEquality(string version1, string version2, bool result)
    {
        var parsedVersion1 = UnsortableVersionParser.Parse(version1);
        var parsedVersion2 = UnsortableVersionParser.Parse(version2);
        Assert.AreEqual(result, Equals(parsedVersion1, parsedVersion2));
    }

    [Test]
    public void TestGetHashCode()
    {
        var versionString = "release-1";
        var parsedVersion = UnsortableVersionParser.Parse(versionString);

        Assert.AreEqual(versionString.GetHashCode(), parsedVersion.GetHashCode());
    }
}