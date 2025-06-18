using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Octopus.Versioning.Octopus
{
    /// <summary>
    /// Masks are used to indicate how a version should be incremented to create a next higher version.
    /// </summary>
    public class OctopusVersionMask
    {
        public OctopusVersionMask(
            bool didParse,
            string? prefix,
            Component major,
            Component minor,
            Component patch,
            Component revision,
            TagComponent prerelease,
            MetadataComponent metadata)
        {
            DidParse = didParse;
            Prefix = prefix ?? string.Empty;
            Major = major;
            Minor = minor;
            Patch = patch;
            Revision = revision;
            Release = prerelease;
            Metadata = metadata;
        }

        public string Prefix { get; }
        public Component Major { get; }
        public Component Minor { get; }
        public Component Patch { get; }
        public Component Revision { get; }
        public MetadataComponent Metadata { get; }
        public TagComponent Release { get; }

        /// <summary>
        /// This is the property that *actually* determines if a string is a mask and will be used
        /// for masking operations.
        /// See OctopusVersionMaskParserTests.IsMask for cases where this differs from IsMask.
        /// </summary>
        public bool DidParse { get; }

        /// <summary>
        /// This is the original logic that appears to determine if the supplied version was a mask.
        /// It is not consistent though. See OctopusVersionMaskParserTests.IsMask for examples where
        /// IsMask is false, but used as a mask anyway.
        /// </summary>
        public bool IsMask =>
            DidParse && (Major.IsSubstitute || Minor.IsSubstitute || Patch.IsSubstitute || Release.IsSubstitute || Revision.IsSubstitute || Metadata.IsSubstitute);

        /// <summary>
        /// Gets the latest masked version from a list of versions.
        /// </summary>
        /// <remarks>
        /// When determining version precedence, metadata is ignored (e.g., pre-release tags 
        /// such as "branch1.a" or build metadata). If two versions are considered equal, 
        /// the method will return the first occurrence in the list.
        /// </remarks>
        /// <param name="versions">
        /// Example: If passed a list of versions [3.2.1-branch1.a, 3.2.2-branch1.a, 3.2.2-branch1.b],
        /// the method will determine the latest version and return 3.2.2-branch1.a. because it's the first occurrence of the two 'highest version'
        /// </param>
        /// <returns>The latest version based on precedence rules.</returns>
        [Obsolete("GetLatestMaskedVersion ignores metadata when determining version precedence, it can give unexpected 'latest' version depending list of versions order.")]
        public IVersion? GetLatestMaskedVersion(List<IVersion> versions)
        {
            var maskMajor = Major.IsPresent && !Major.IsSubstitute ? int.Parse(Major.Value) : 0;
            var maskMinor = Minor.IsPresent && !Minor.IsSubstitute ? int.Parse(Minor.Value) : 0;
            var maskBuild = Patch.IsPresent && !Patch.IsSubstitute ? int.Parse(Patch.Value) : 0;
            var maskRevision = Revision.IsPresent && !Revision.IsSubstitute ? int.Parse(Revision.Value) : 0;

            return versions
                .Where(v => v != null)
                .Where(v =>
                {
                    if (Major.IsSubstitute)
                        return true;

                    if (v.Major != maskMajor)
                        return false;

                    if (Minor.IsSubstitute)
                        return true;

                    if (v.Minor != maskMinor)
                        return false;

                    if (Patch.IsSubstitute)
                        return true;

                    if (v.Patch != maskBuild)
                        return false;

                    if (Revision.IsSubstitute)
                        return true;

                    if (v.Revision != maskRevision)
                        return false;

                    return true;
                })
                .OrderByDescending(o => o)
                .FirstOrDefault();
        }

        public IVersion GenerateVersionFromMask()
        {
            var result = new StringBuilder();
            result.Append(Prefix);
            result.Append(Major.EvaluateFromMask());
            result.Append(Minor.EvaluateFromMask("."));
            result.Append(Patch.EvaluateFromMask("."));
            result.Append(Revision.EvaluateFromMask("."));
            result.Append(Release.EvaluateFromMask("-"));
            result.Append(Metadata.EvaluateFromMask("+"));
            return VersionFactory.CreateOctopusVersion(result.ToString());
        }

        public IVersion GenerateVersionFromCurrent(OctopusVersionMask current)
        {
            var result = new StringBuilder();
            result.Append(Prefix);
            result.Append(Major.Substitute(current.Major));
            result.Append(Minor.EvaluateFromCurrent(current.Minor, Major));
            result.Append(Patch.EvaluateFromCurrent(current.Patch, Minor));
            result.Append(Revision.EvaluateFromCurrent(current.Revision, Patch));
            result.Append(Release.EvaluateFromCurrent(current.Release, Revision));
            result.Append(Metadata.EvaluateFromCurrent(current.Metadata, Revision));
            return VersionFactory.CreateOctopusVersion(result.ToString());
        }

        public class Component
        {
            protected readonly Group matchGroup;

            public Component(Group matchGroup)
            {
                this.matchGroup = matchGroup;
            }

            public bool IsPresent => matchGroup.Success;

            public virtual bool IsSubstitute
            {
                get
                {
                    if (!IsPresent)
                        return false;

                    return matchGroup.Value == OctopusVersionMaskParser.PatternIncrement || matchGroup.Value == OctopusVersionMaskParser.PatternCurrent;
                }
            }

            public string Value => matchGroup.Value;

            public virtual string EvaluateFromMask(string separator = "")
            {
                return IsPresent ? string.Format("{0}{1}", separator, IsSubstitute ? "0" : Value) : string.Empty;
            }

            public virtual string EvaluateFromCurrent(Component current, Component prevMaskComponent)
            {
                if (IsPresent)
                {
                    if (prevMaskComponent.Value != OctopusVersionMaskParser.PatternIncrement || !IsSubstitute)
                        return $".{Substitute(current)}";

                    return ".0";
                }

                if (current.IsPresent && prevMaskComponent.IsPresent)
                    return ".0";

                return string.Empty;
            }

            public virtual string Substitute(Component current)
            {
                var currentValue = current.IsPresent ? long.Parse(current.Value) : 0;

                if (Value == OctopusVersionMaskParser.PatternIncrement)
                    return (currentValue + 1).ToString(CultureInfo.InvariantCulture);
                if (Value == OctopusVersionMaskParser.PatternCurrent)
                    return currentValue.ToString(CultureInfo.InvariantCulture);
                return Value;
            }
        }

        public class TagComponent : Component
        {
            public TagComponent(Group matchGroup) : base(matchGroup)
            {
            }

            public override bool IsSubstitute
            {
                get
                {
                    if (!IsPresent)
                        return false;

                    return Regex.IsMatch(matchGroup.Value, @$"\.(?:{OctopusVersionMaskParser.PatternIncrement}|{OctopusVersionMaskParser.PatternCurrent})$");
                }
            }

            public override string EvaluateFromMask(string separator = "")
            {
                if (!IsPresent || string.IsNullOrEmpty(Value))
                    return string.Empty;

                var identifiers = Value.Split('.');
                var substitutedIdentifiers = new List<string>();

                for (var i = 0; i < identifiers.Length; i++)
                    switch (identifiers[i])
                    {
                        case OctopusVersionMaskParser.PatternIncrement:
                        case OctopusVersionMaskParser.PatternCurrent:
                            substitutedIdentifiers.Add("0");
                            break;
                        default:
                            substitutedIdentifiers.Add(identifiers[i]);
                            break;
                    }

                return separator + string.Join(".", substitutedIdentifiers);
            }

            public override string EvaluateFromCurrent(Component current, Component prevMaskComponent)
            {
                if (!IsPresent || string.IsNullOrEmpty(Value))
                    return string.Empty;

                return "-" + Substitute(current);
            }

            public override string Substitute(Component current)
            {
                var identifiers = Value.Split('.');
                var currentIdentifiers = current.IsPresent ? current.Value.Split('.') : new string[0];
                var substitutedIdentifiers = new List<string>();

                for (var i = 0; i < identifiers.Length; i++)
                {
                    if (i > 0 && identifiers[i - 1] == OctopusVersionMaskParser.PatternIncrement && IsSubstitute)
                    {
                        substitutedIdentifiers.Add("0");
                        continue;
                    }

                    var currentIdentifierValue = 0;
                    if (currentIdentifiers.Length > i)
                        int.TryParse(currentIdentifiers[i], out currentIdentifierValue);

                    switch (identifiers[i])
                    {
                        case OctopusVersionMaskParser.PatternIncrement:
                            substitutedIdentifiers.Add((currentIdentifierValue + 1).ToString(CultureInfo.InvariantCulture));
                            break;
                        case OctopusVersionMaskParser.PatternCurrent:
                            substitutedIdentifiers.Add(currentIdentifierValue.ToString(CultureInfo.InvariantCulture));
                            break;
                        default:
                            substitutedIdentifiers.Add(identifiers[i]);
                            break;
                    }
                }

                return string.Join(".", substitutedIdentifiers);
            }
        }

        public class MetadataComponent : TagComponent
        {
            public MetadataComponent(Group matchGroup) : base(matchGroup)
            {
            }

            public override string EvaluateFromCurrent(Component current, Component prevMaskComponent)
            {
                if (!IsPresent || string.IsNullOrEmpty(Value))
                    return string.Empty;

                return "+" + Substitute(current);
            }
        }
    }
}