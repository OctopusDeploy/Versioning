using System;
using System.Text.RegularExpressions;
using Octopus.Versioning.Octopus;
using Octopus.Versioning.Semver;

namespace Octopus.Versioning.Docker
{
    public class DockerTag : OctopusVersion
    {
        const string Latest = "latest";
        bool IsLatest => string.Compare(OriginalString, Latest, StringComparison.Ordinal) == 0;

        // Matches the @sha256:<hex> suffix in a Docker tag string e.g. "6.5@sha256:abc123"
        static readonly Regex DigestSuffixRegex = new Regex(@"@sha256:[a-f0-9]+$", RegexOptions.Compiled);

        public DockerTag(OctopusVersion version)
            : base(version.Prefix,
                version.Major,
                version.Minor,
                version.Patch,
                version.Revision,
                version.Release,
                version.ReleasePrefix,
                version.ReleaseCounter,
                version.Metadata,
                version.OriginalString)
        {
        }

        public DockerTag(OctopusVersion version, string? digest)
            : base(version.Prefix,
                version.Major,
                version.Minor,
                version.Patch,
                version.Revision,
                version.Release,
                version.ReleasePrefix,
                version.ReleaseCounter,
                version.Metadata,
                version.OriginalString)
        {
            Digest = digest;
        }

        public DockerTag(string? prefix,
            int major,
            int minor,
            int patch,
            int revision,
            string? prerelease,
            string? prereleasePrefix,
            string? prereleaseCounter,
            string? metadata,
            string? originalVersion) : base(prefix,
            major,
            minor,
            patch,
            revision,
            prerelease,
            prereleasePrefix,
            prereleaseCounter,
            metadata,
            originalVersion)
        {
        }

        public string? Digest { get; }

        public override VersionFormat Format => VersionFormat.Docker;

        public override string ToString() => Digest is null ? base.ToString() : $"{base.ToString()}@{Digest}";

        public override bool IsPrerelease => !string.IsNullOrEmpty(Release) && OriginalString != Latest;

        /// <summary>
        /// Strips the digest suffix from a raw Docker tag string before version parsing.
        /// e.g. "6.5@sha256:abc123" → ("6.5", "sha256:abc123"), "6.5" → ("6.5", null)
        /// </summary>
        public static (string Tag, string? Digest) SplitDigest(string input)
        {
            var match = DigestSuffixRegex.Match(input);
            if (!match.Success)
                return (input, null);

            var tag = input.Substring(0, match.Index);
            var digest = match.Value.Substring(1); // strip the leading '@'
            return (tag, digest);
        }

        public override int CompareTo(object obj)
        {
            if (obj is DockerTag objDockerTag)
            {
                return IsLatest && objDockerTag.IsLatest ? 0 : base.CompareTo(obj);
            }

            return -1;
        }

        public override bool Equals(object obj)
        {
            if (obj is DockerTag objDockerTag)
                return CompareTo(objDockerTag) == 0;

            return false;
        }

        public override int GetHashCode()
        {
            return IsLatest ? Latest.GetHashCode() : base.GetHashCode();
        }
    }
}
