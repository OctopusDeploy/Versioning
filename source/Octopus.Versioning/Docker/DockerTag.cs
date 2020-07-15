using System;
using System.Collections.Generic;

namespace Octopus.Versioning.Docker
{
    public class DockerTag : IVersion
    {
        public DockerTag(string tag)
        {
            Tag = tag;
        }

        public string Tag { get; }
        public int Major => throw new NotSupportedException("Docker tags do not support major version parts");
        public int Minor => throw new NotSupportedException("Docker tags do not support minor version parts");
        public int Patch => throw new NotSupportedException("Docker tags do not support patch version parts");
        public int Revision => throw new NotSupportedException("Docker tags do not support revision version parts");
        public bool IsPrerelease => throw new NotSupportedException("Docker tags do not support pre-release versions");
        public IEnumerable<string> ReleaseLabels => throw new NotSupportedException("Docker tags do not support release labels");
        public string Metadata => throw new NotSupportedException("Docker tags do not support metadata version parts");
        public string Release => throw new NotSupportedException("Docker tags do not support release version parts");
        public bool HasMetadata => false;
        public string OriginalString => Tag;
        public VersionFormat Format => VersionFormat.Docker;

        public override string ToString()
        {
            return Tag ?? "latest";
        }

        bool Equals(DockerTag other)
        {
            return Tag == other.Tag;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((DockerTag)obj);
        }

        public override int GetHashCode()
        {
            return Tag != null ? Tag.GetHashCode() : 0;
        }

        public int CompareTo(object obj)
        {
            return string.Compare((obj as DockerTag)?.Tag ?? "", Tag, StringComparison.Ordinal);
        }
    }
}