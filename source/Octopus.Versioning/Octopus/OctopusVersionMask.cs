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
        public Component Major { get; }
        public Component Minor { get; }
        public Component Patch { get; }
        public Component Revision { get; }
        public MetadataComponent Metadata { get; }
        public TagComponent Release { get; }

        public bool IsMask =>
            Major.IsSubstitute || Minor.IsSubstitute || Patch.IsSubstitute || Release.IsSubstitute || Revision.IsSubstitute || Metadata.IsSubstitute;

        public OctopusVersionMask(Component major,
            Component minor,
            Component patch,
            Component revision,
            TagComponent prerelease,
            MetadataComponent metadata)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            Revision = revision;
            Release = prerelease;
            Metadata = metadata;
        }

        public IVersion? GetLatestMaskedVersion(List<IVersion> versions)
        {
            var maskMajor = Major.IsPresent && !Major.IsSubstitute ? int.Parse(Major.Value) : 0;
            var maskMinor = Minor.IsPresent && !Minor.IsSubstitute ? int.Parse(Minor.Value) : 0;
            var maskBuild = Patch.IsPresent && !Patch.IsSubstitute ? int.Parse(Patch.Value) : 0;
            var maskRevision = Revision.IsPresent && !Revision.IsSubstitute ? int.Parse(Revision.Value) : 0;

            return versions
                .Where (v => v != null)
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
                }).OrderByDescending(o => o).FirstOrDefault();

        }

        public IVersion GenerateVersionFromMask()
        {
            var result = new StringBuilder();
            result.Append(Major.EvaluateFromMask());
            result.Append(Minor.EvaluateFromMask("."));
            result.Append(Patch.EvaluateFromMask("."));
            result.Append(Revision.EvaluateFromMask("."));
            result.Append(Release.EvaluateFromMask("-"));
            result.Append(Metadata.EvaluateFromMask("+"));
            return VersionFactory.CreateSemanticVersion(result.ToString());
        }

        public IVersion GenerateVersionFromCurrent(OctopusVersionMask current)
        {
            var result = new StringBuilder();
            result.Append(Major.Substitute(current.Major));
            result.Append(Minor.EvaluateFromCurrent(current.Minor, Major));
            result.Append(Patch.EvaluateFromCurrent(current.Patch, Minor));
            result.Append(Revision.EvaluateFromCurrent(current.Revision, Patch));
            result.Append(Release.EvaluateFromCurrent(current.Release, Revision));
            result.Append(Metadata.EvaluateFromCurrent(current.Metadata, Revision));
            return new OctopusVersionParser().Parse(result.ToString());
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
                    return IsPresent ?
                        string.Format("{0}{1}", separator, IsSubstitute ? "0" : Value) :
                        string.Empty;
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
                    {
                        return ".0";
                    }

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

                        return Regex.IsMatch(matchGroup.Value, @$"[.\-_]{OctopusVersionMaskParser.PatternIncrement}|{OctopusVersionMaskParser.PatternCurrent}$");
                    }
                }

                public override string EvaluateFromMask(string separator = "")
                {
                    if (!IsPresent || string.IsNullOrEmpty(Value))
                        return string.Empty;

                    var identifiers = Value.Split('.', '-', '_');
                    var substitutedIdentifiers = new List<string>();

                    for (var i = 0; i < identifiers.Length; i++)
                    {
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
                    var identifiers = Value.Split('.', '-', '_');
                    var currentIdentifiers = current.IsPresent ? current.Value.Split('.') : new string[0];
                    var substitutedIdentifiers = new List<string>();

                    for (var i = 0; i < identifiers.Length; i++)
                    {
                        if (i > 0 && identifiers[i-1] == OctopusVersionMaskParser.PatternIncrement && IsSubstitute)
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
                                substitutedIdentifiers.Add((currentIdentifierValue+1).ToString(CultureInfo.InvariantCulture));
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