#if !HAS_NULLABLE_REF_TYPES
using System;

/// <summary>
/// These attributes replicate the ones from System.Diagnostics.CodeAnalysis, and are here so we can still compile against the older frameworks.
/// </summary>

namespace Octopus.Versioning
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = false)]
    public sealed class NotNullAttribute : Attribute
    {
    }
}
#endif