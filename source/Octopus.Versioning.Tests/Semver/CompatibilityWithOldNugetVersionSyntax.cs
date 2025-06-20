using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Octopus.Versioning.Tests.Semver;

/// <summary>
/// Octopus used to maintain a custom fork of NuGet.Client with the following rationale:
///
///   NuGet 3 started removing leading zeros and the fourth digit if it is zero.
///   These are affectionately known as "NuGet zero quirks" and can be surprising when working with tooling outside the NuGet ecosystem.
///   We have made a choice to preserve the version as-is when working with Octopus tooling to create packages of any kind.
///
/// The `SemanticVersion` type in Octopus.Versioning has equivalent behavior to this Pre-NuGet 3 behaviour. This test
/// ensures that remains true over time.
/// </summary>
/// <remarks>
/// References:
/// - https://github.com/OctopusDeploy/NuGet.Client
/// - https://learn.microsoft.com/en-us/nuget/concepts/package-versioning?tabs=semver20sort#where-nugetversion-diverges-from-semantic-versioning
/// </remarks>
public class CompatibilityWithOldNugetVersionSyntax
{
    [Test]
    public void SemVerDoesNotStripLeadingZeroes()
    {
        var ver1 = VersionFactory.CreateSemanticVersion("1.001.002");
        Assert.AreEqual("1.001.002", ver1.ToString());
    }
    
    [Test]
    public void SemVerDoesNotStripFourthDigitZero()
    {
        var ver1 = VersionFactory.CreateSemanticVersion("1.2.3.0");
        Assert.AreEqual("1.2.3.0", ver1.ToString());
    }
    
    [Test]
    public void VersionsWithDifferingSegmentsAreEqual()
    {
        var majorOnly = VersionFactory.CreateSemanticVersion("1");
        var majorMinor = VersionFactory.CreateSemanticVersion("1.0");
        var majorMinorPatch = VersionFactory.CreateSemanticVersion("1.0.0");
        var majorMinorPatchRevision = VersionFactory.CreateSemanticVersion("1.0.0.0");
        
        // All things must equal eachother
        foreach (var (a, b) in GenerateUniquePairs([majorOnly, majorMinor, majorMinorPatch, majorMinorPatchRevision]))
        {
            Assert.AreEqual(a, b);
        }
    }
    
    [Test]
    public void VersionsWithDifferingLeadingZeroesAreEqual()
    {
        var basic = VersionFactory.CreateSemanticVersion("1.1");
        var zeroOne = VersionFactory.CreateSemanticVersion("1.01");
        var multipleZeroes = VersionFactory.CreateSemanticVersion("1.0001");
        var trailingZeroes = VersionFactory.CreateSemanticVersion("1.000001.00");
        var firstLeadingZero = VersionFactory.CreateSemanticVersion("001.1");
        
        // All things must equal eachother
        foreach (var (a, b) in GenerateUniquePairs([basic, zeroOne, multipleZeroes, trailingZeroes, firstLeadingZero]))
        {
            Assert.AreEqual(a, b);
        }
    }
    
    [Test]
    public void PreReleaseComponentsCompareEqual()
    {
        var lowerCase = VersionFactory.CreateSemanticVersion("1.0.0-alpha");
        var upperCase = VersionFactory.CreateSemanticVersion("1.0.0-Alpha");
        Assert.AreEqual(lowerCase, upperCase);
    }
    
    static IEnumerable<(T, T)> GenerateUniquePairs<T>(T[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            for (int j = i + 1; j < array.Length; j++)
            {
                yield return (array[i], array[j]);
            }
        }
    }
}