using System.Collections.Generic;

namespace Octopus.Versioning
{
    public interface IVersion
    {
        int Major { get; }
        int Minor { get; }
        int Patch { get; }
        int Revision { get; }
        bool IsPrerelease { get; }
        IEnumerable<string> ReleaseLabels { get; }
        string? Metadata { get; }
        string Release { get; }
        bool HasMetadata { get; }
        string? OriginalString { get; }
        VersionFormat Format { get; }
    }
}