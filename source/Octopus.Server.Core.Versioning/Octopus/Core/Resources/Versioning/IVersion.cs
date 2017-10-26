using System;
using System.Collections.Generic;

namespace Octopus.Core.Resources.Versioning
{
    /// <summary>
    /// Represents the base components of a version recognised by Octopus.
    /// The terminolgy here comes from SemVer, but can be mapped to other
    /// versioning schemes like Maven.
    /// 
    /// All classes should reference this interface, but there are some 
    /// exceptions.
    /// 
    /// Some modelling classes need to reference the SemanticVersionConverter
    /// to ensure that constructor paramaters and properties of the type
    /// IVersion are converted to and from a SemanticVersion object.
    /// 
    /// JSON converters like SemanticVersionConverter assume that
    /// any version object being stored in the database is a SemanticVersion
    /// object. This is because Octopus assumes the use of SemanticVersion
    /// in all processes other than external feeds, which do not have
    /// version information directly saved in the database.
    /// 
    /// The version factory classes also need to know about the conrete
    /// classes.
    /// 
    /// Outside of those two use cases, all other references to a version
    /// should be through this interface.
    /// </summary>
    public interface IVersion : IComparable
    {
        int Major { get; }
        int Minor { get; }
        int Patch { get; }
        int Revision { get; }
        bool IsPrerelease { get; }
        IEnumerable<string> ReleaseLabels { get; }
        string Metadata { get; }
        string Release { get; }
        bool HasMetadata { get; }
        object ToType(Type type);
    }
}