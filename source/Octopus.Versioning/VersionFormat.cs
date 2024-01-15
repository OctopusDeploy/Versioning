using System;

namespace Octopus.Versioning
{
    public enum VersionFormat
    {
        Semver,
        Maven,
        Docker,
        Octopus,
        Lexicographic
    }
}