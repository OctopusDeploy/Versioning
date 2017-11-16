using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Octopus.Core.Resources.Metadata;
using Octopus.Core.Resources.Versioning.Maven;
using Octopus.Core.Util;
using SemanticVersion = Octopus.Core.Resources.Versioning.Semver.SemanticVersion;

namespace Octopus.Core.Resources.Versioning.Factories
{
    public class VersionFactory : IVersionFactory
    {
        static readonly SemVerFactory SemVerFactory = new SemVerFactory();
        static readonly IPackageIDParser MavenPackageIdParser = new MavenPackageIDParser();
        static readonly IPackageIDParser NugetPackageIdParser = new NuGetPackageIDParser();

        public IVersion CreateVersion(string input, FeedType type)
        {
            switch (type)
            {
                case FeedType.Maven:
                    return CreateMavenVersion(input);
                default:
                    return CreateSemanticVersion(input);
            }
        }

        public IVersion CreateVersion(string input, string packageId)
        {
            if (MavenPackageIdParser.CanGetMetadataFromPackageID(packageId, out var metadata))
            {
                return CreateMavenVersion(input);
            }
            
            if (NugetPackageIdParser.CanGetMetadataFromPackageID(packageId, out var nugetMetdata))
            {
                return CreateSemanticVersion(input);
            }

            throw new ArgumentException($"Package id {packageId} is not recognised");
        }

        public Maybe<IVersion> CreateOptionalVersion(string input, FeedType type)
        {
            try
            {
                switch (type)
                {
                    case FeedType.Maven:
                        return Maybe<IVersion>.Some(CreateMavenVersion(input));
                    default:
                        return CreateSemanticVersionOrNone(input);
                }
            }
            catch
            {
                return Maybe<IVersion>.None;
            }
        }

        public IVersion CreateMavenVersion(string input)
        {
            return new MavenVersionParser().Parse(input);
        }

        public IVersion CreateSemanticVersion(string input, bool preserveMissingComponents = false)
        {
            return SemVerFactory.CreateVersion(input, preserveMissingComponents);
        }

        public IVersion CreateSemanticVersion(int major, int minor, int patch, string releaseLabel)
        {
            return new SemanticVersion(major, minor, patch, releaseLabel);
        }

        public IVersion CreateSemanticVersion(int major, int minor, int patch)
        {
            return new SemanticVersion(major, minor, patch);
        }

        public IVersion CreateSemanticVersion(int major, int minor, int patch, int revision)
        {
            return new SemanticVersion(major, minor, patch, revision);
        }

        public IVersion CreateSemanticVersion(Version version, string releaseLabel = null, string metadata = null)
        {
            return new SemanticVersion(version, releaseLabel, metadata);
        }

        public IVersion CreateSemanticVersion(int major, int minor, int patch, int revision,
            IEnumerable<string> releaseLabels,
            string metadata, string originalVersion)
        {
            return new SemanticVersion(major, minor, patch, revision, releaseLabels, metadata);
        }

        public bool CanCreateVersion(string input, out IVersion version, FeedType type)
        {
            switch (type)
            {
                case FeedType.Maven:
                    return CanCreateMavenVersion(input, out version);
                default:
                    return CanCreateSemanticVersion(input, out version);
            }
        }

        public bool CanCreateVersion(string input, out IVersion version, string packageId)
        {
            if (MavenPackageIdParser.CanGetMetadataFromPackageID(packageId, out var metadata))
            {
                return CanCreateSemanticVersion(input, out version);
            }
            
            if (NugetPackageIdParser.CanGetMetadataFromPackageID(packageId, out var nugetMetdata))
            {
                return CanCreateMavenVersion(input,  out version);
            }

            throw new ArgumentException($"Package id {packageId} is not recognised");
        }

        public bool CanCreateMavenVersion(string input, out IVersion version)
        {
            /*
             * Any version is valid for Maven
             */
            version = new MavenVersionParser().Parse(input);
            return true;
        }

        public bool CanCreateSemanticVersion(string input, out IVersion version, bool preserveMissingComponents = false)
        {
            var retValue = SemVerFactory.CanCreateVersion(input, out var semVersion, preserveMissingComponents);
            version = semVersion;
            return retValue;
        }

        public Maybe<IVersion> CreateSemanticVersionOrNone(string input, bool preserveMissingComponents = false)
        {
            return SemVerFactory.CreateVersionOrNone(input, preserveMissingComponents);
        }

        public IVersion CreateSemanticVersion(Version version, IEnumerable<string> releaseLabels, string metadata,
            string originalVersion)
        {
            return new SemanticVersion(
                version,
                releaseLabels,
                metadata,
                originalVersion);
        }
    }
}