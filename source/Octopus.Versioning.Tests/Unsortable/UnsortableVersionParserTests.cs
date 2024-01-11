using System;
using NUnit.Framework;
using Octopus.Versioning.Unsortable;

namespace Octopus.Versioning.Tests.Unsortable;

[TestFixture]
public class UnsortableVersionParserTests
{
    [Test]
    // Release
    [TestCase("foobar", "foobar", "")]
    [TestCase("2db4a87840113c", "2db4a87840113c", "")]
    [TestCase("123456", "123456", "")]
    [TestCase("foobar-qwerty", "foobar-qwerty", "")]
    [TestCase("foobar.qwerty", "foobar.qwerty", "")]
    [TestCase("foobar_qwerty", "foobar_qwerty", "")]
    [TestCase("foobar-12345", "foobar-12345", "")]
    // Metadata 
    [TestCase("foobar+12345", "foobar", "12345")]
    [TestCase("foobar+123.456", "foobar", "123.456")]
    [TestCase("foobar+123_456", "foobar", "123_456")]
    [TestCase("foobar+123-456", "foobar", "123-456")]
    [TestCase("foobar+123+456", "foobar", "123+456")]
    [TestCase("foobar+1.2_3-4+5", "foobar", "1.2_3-4+5")]
    [TestCase("foobar+qwerty", "foobar", "qwerty")]
    [TestCase("foobar-qwerty+12345", "foobar-qwerty", "12345")]
    // Fail Cases
    [TestCase("!@#$%^", "", "")]
    [TestCase("foobar-!@#$%", "", "")]
    [TestCase("foobar-qwerty+!@#$%", "", "")]
    [TestCase("foo bar", "", "")]
    [TestCase("foobar-qwe ty", "", "")]
    [TestCase("foobar+123 456", "", "")]
    [TestCase("foo bar-qwe ty+123 456", "", "")]
    [TestCase("!foobar", "", "")]
    [TestCase("foo!bar", "", "")]
    [TestCase("foobar!", "", "")]
    public void ShouldParseSuccessfully(string input, string expectedRelease, string expectedMetadata)
    {
        _ = new UnsortableVersionParser().TryParse(input, out var parsedVersion);
        AssertVersionNumbersAreZero(parsedVersion);
        Assert.AreEqual(expectedRelease, parsedVersion.Release);
        Assert.AreEqual(expectedMetadata, parsedVersion.Metadata);
    }

    [Test]
    public void ShouldThrowExceptionOnEmptyInput()
    {
        var input = "";
        Assert.Catch<ArgumentException>(() => new UnsortableVersionParser().Parse(input));
    }
    
    [Test]
    public void ShouldThrowExceptionOnWhiteSpaceInput()
    {
        var input = " ";
        Assert.Catch<ArgumentException>(() => new UnsortableVersionParser().Parse(input));
    }
    
    [Test]
    public void ShouldThrowExceptionOnNullInput()
    {
        Assert.Catch<ArgumentException>(() => new UnsortableVersionParser().Parse(null));
    }
    
    [Test]
    public void ShouldThrowExceptionOnFailureToParse()
    {
        var input = "bad versions string";
        Assert.Catch<ArgumentException>(() => new UnsortableVersionParser().Parse(input));
    }

    void AssertVersionNumbersAreZero(UnsortableSortableVersion sortableVersion)
    {
        Assert.AreEqual(0, sortableVersion.Major);
        Assert.AreEqual(0, sortableVersion.Minor);
        Assert.AreEqual(0, sortableVersion.Patch);
        Assert.AreEqual(0, sortableVersion.Revision);
    }
}