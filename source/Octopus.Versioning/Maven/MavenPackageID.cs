using System;
using System.Collections.Generic;
using System.Linq;

namespace Octopus.Versioning.Maven
{
    /// <summary>
    /// Back when Octopus only support NuGet (or NuGet like) feeds, it was possible to make
    /// a bunch of assumptions regarding the packages that were managed by Octopus:
    /// 1. All versions were valid NuGet versions.
    /// 2. Versions and package IDs could be embedded in the filename in the same way they were
    ///    shown to the user.
    ///
    /// Maven does not have this luxury:
    /// 1. In order to distinguish a Maven package from a NuGet package, package ID's have the
    ///    prefix of "Maven#".
    /// 2. Maven packages are referred to using the GAV format that looks like
    ///    "com.google.guava:guava:22.0". The colons in this format are not valid for filenames,
    ///    so the way a package ID is displayed to the user will be different to the way it
    ///    is saved locally.
    ///
    /// The purpose of this class is to serve as a mediator between how the end user sees a
    /// Maven package id (i.e. in the format "com.google.guava:guava") and the way that these
    /// files are saved on the disk (i.e. in the format "Maven#com.google.guava#guava").
    /// </summary>
    public class MavenPackageID
    {
        /// <summary>
        /// When we display the package ID to the user, this is the delimiter we use.
        /// The colon is the standard delimiter format for Maven packages, but it
        /// is not a valid file system character so we don't use it when saving
        /// packages to the disk.
        /// </summary>
        public const char DISPLAY_DELIMITER = ':';

        public MavenPackageID(string group, string artifact, string version)
        {
            if (string.IsNullOrWhiteSpace(group))
                throw new ArgumentException("Group can not be empty");
            if (string.IsNullOrWhiteSpace(artifact))
                throw new ArgumentException("Artifact can not be empty.");
            if (string.IsNullOrWhiteSpace(version))
                throw new ArgumentException("Version can not be empty");

            Group = group.Trim();
            Artifact = artifact.Trim();
            Version = version.Trim();
        }

        public MavenPackageID(string group, string artifact, string version, string packaging) :
            this(group, artifact, version)
        {
            if (string.IsNullOrWhiteSpace(packaging))
                throw new ArgumentException("Packaging can not be empty");

            Packaging = packaging.Trim();
        }

        public MavenPackageID(string group,
            string artifact,
            string version,
            string packaging,
            string? classifier) :
            this(group, artifact, version, packaging)
        {
            Classifier = string.IsNullOrWhiteSpace(classifier) ? null : classifier.Trim();
        }

        public MavenPackageID(string id, IVersion version) : this(id)
        {
            if (string.IsNullOrWhiteSpace(id) || id.Split(':').Length != 2)
                throw new ArgumentException("Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");

            if (version == null)
                throw new ArgumentException("version can not be null");

            Version = version.ToString();
        }

        /// <summary>
        /// Parses an octopus package id into the maven package details.
        /// The versioning syntax comes from http://maven.apache.org/plugins/maven-dependency-plugin/get-mojo.html being
        /// groupId:artifactId:version[:packaging[:classifier]]
        /// </summary>
        /// <param name="id">
        /// The package id is in the display format like "Group:Artifact".
        /// </param>
        public MavenPackageID(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("id can not be empty");

            var mavenDisplaySplit = id.Split(DISPLAY_DELIMITER);

            /*
             * When downloading for the first time, we will use the G:A:V format
             * supplied by the end user.
             */
            if (mavenDisplaySplit.Length >= 2)
            {
                Group = mavenDisplaySplit[0].Trim();
                Artifact = mavenDisplaySplit[1].Trim();

                if (mavenDisplaySplit.Length == 3) // groupId:artifactId:version
                {
                    Version = string.IsNullOrWhiteSpace(mavenDisplaySplit[2]) ? null : mavenDisplaySplit[2].Trim();
                }
                else if (mavenDisplaySplit.Length == 4) // groupId:artifactId:version:packaging
                {
                    Version = string.IsNullOrWhiteSpace(mavenDisplaySplit[2]) ? null : mavenDisplaySplit[2].Trim();
                    Packaging = string.IsNullOrWhiteSpace(mavenDisplaySplit[3]) ? null : mavenDisplaySplit[3].Trim();
                }
                else if (mavenDisplaySplit.Length == 5) // groupId:artifactId:version:packaging:classifier
                {
                    Version = string.IsNullOrWhiteSpace(mavenDisplaySplit[2]) ? null : mavenDisplaySplit[2].Trim();
                    Packaging = string.IsNullOrWhiteSpace(mavenDisplaySplit[3]) ? null : mavenDisplaySplit[3].Trim();
                    Classifier = string.IsNullOrWhiteSpace(mavenDisplaySplit[4]) ? null : mavenDisplaySplit[4].Trim();
                }
                else
                {
                    throw new Exception("Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");
                }
            }
            else
            {
                throw new Exception("Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");
            }
        }

        public string Group { get; }
        public string[]? Groups => Group?.Split('.');
        public string Artifact { get; }
        public string? Version { get; }
        public string? Packaging { get; }
        public string? Classifier { get; }
        public string DisplayName => ToString(DISPLAY_DELIMITER);

        public IVersion? SemanticVersion => Version == null ? null : new MavenVersionParser().Parse(Version);

        /// <summary>
        /// The path to the metadata file for the artifact
        /// </summary>
        public string GroupMetadataPath
        {
            get
            {
                if (Groups == null || Groups.Length == 0)
                    throw new ArgumentException("Groups can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");

                if (string.IsNullOrWhiteSpace(Artifact))
                    throw new ArgumentException("Artifact can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");

                return $"/{Groups?.Aggregate((result, item) => result + "/" + item)}/{Artifact}/maven-metadata.xml";
            }
        }

        /// <summary>
        /// The path to the metadata file for a particular artifact and version
        /// </summary>
        public string GroupVersionMetadataPath
        {
            get
            {
                if (Groups == null || Groups.Length == 0)
                    throw new ArgumentException("Groups can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");

                if (string.IsNullOrWhiteSpace(Artifact))
                    throw new ArgumentException("Artifact can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");

                if (string.IsNullOrWhiteSpace(Version))
                    throw new ArgumentException("Version can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");

                return $"/{Groups?.Aggregate((result, item) => result + "/" + item)}/{Artifact}/{Version}/maven-metadata.xml";
            }
        }

        /// <summary>
        /// The path to the metadata file for the artifact
        /// </summary>
        public string DefaultGroupVersionPomPath
        {
            get
            {
                if (Groups == null || Groups.Length == 0)
                    throw new ArgumentException("Groups can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");

                if (string.IsNullOrWhiteSpace(Artifact))
                    throw new ArgumentException("Artifact can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");

                if (string.IsNullOrWhiteSpace(Version))
                    throw new ArgumentException("Version can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");

                return $"/{Groups?.Aggregate((result, item) => result + "/" + item)}/{Artifact}/{Version}/{Artifact}-{Version}.pom";
            }
        }

        /// <summary>
        /// The path to the archive file for the artifact
        /// </summary>
        public string DefaultArtifactPath
        {
            get
            {
                if (Groups == null || Groups.Length == 0)
                    throw new ArgumentException("Groups can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");

                if (string.IsNullOrWhiteSpace(Artifact))
                    throw new ArgumentException("Artifact can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");

                if (string.IsNullOrWhiteSpace(Version))
                    throw new ArgumentException("Version can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");

                return $"/{Groups?.Aggregate((result, item) => result + "/" + item)}/{Artifact}/{Version}/{Artifact}-{Version}{(string.IsNullOrWhiteSpace(Classifier) ? "" : "-" + Classifier)}.{Packaging}";
            }
        }

        /// <summary>
        /// Standard GAV coordinates are group:artifact:version. This can also be extended to include the packaging in the
        /// format group:artifact:version:packaging or group:artifact:version:packaging:classifier. See
        /// http://maven.apache.org/plugins/maven-dependency-plugin/get-mojo.html for this format defined in the Maven
        /// documentation.
        ///
        /// However, we never pass the version in the package id from the UI. Instead we pass a string like group:artifact or
        /// group:artifact:packaging. This is because the version selection is a separate process from the package definition.
        ///
        /// It is here that we convert the package id sent from the UI into the standard Maven string.
        /// </summary>
        /// <param name="input">The input sent from the UI</param>
        /// <param name="version">The optional version</param>
        /// <returns>A MavenPackageID created to match the package id an optional packaging defined in the UI</returns>
        /// <exception cref="ArgumentException">thrown if the input is not in the correct format</exception>
        public static MavenPackageID CreatePackageIdFromOctopusInput(string input, IVersion? version = null)
        {
            var splitVersion = input.Split(':').ToList();
            if (!(splitVersion.Count >= 2 && splitVersion.Count <= 4))
                throw new ArgumentException("Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");

            var mavenStandardVersion = new List<string>
            {
                splitVersion[0],
                splitVersion[1],
                version != null ? version.ToString() : ""
            };
            if (splitVersion.Count >= 3)
                mavenStandardVersion.Add(splitVersion[2]);
            if (splitVersion.Count >= 4)
                mavenStandardVersion.Add(splitVersion[3]);

            return new MavenPackageID(string.Join(":", mavenStandardVersion));
        }

        /// <summary>
        /// The path to the metadata file for the artifact with a custome value
        /// </summary>
        public string SnapshotGroupVersionPomPath(string value)
        {
            if (Groups == null || Groups.Length == 0)
                throw new ArgumentException("Groups can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");

            if (string.IsNullOrWhiteSpace(Artifact))
                throw new ArgumentException("Artifact can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");

            if (string.IsNullOrWhiteSpace(Version))
                throw new ArgumentException("Version can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");

            return $"/{Groups?.Aggregate((result, item) => result + "/" + item)}/{Artifact}/{Version}/{Artifact}-{value}.pom";
        }

        /// <summary>
        /// The path to the archive file for the artifact with a custom value
        /// </summary>
        public string SnapshotArtifactPath(string value)
        {
            if (Groups == null || Groups.Length == 0)
                throw new ArgumentException("Groups can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");

            if (string.IsNullOrWhiteSpace(Artifact))
                throw new ArgumentException("Artifact can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");

            if (string.IsNullOrWhiteSpace(Version))
                throw new ArgumentException("Version can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");

            return $"/{Groups?.Aggregate((result, item) => result + "/" + item)}/{Artifact}/{Version}/{Artifact}-{value}{(string.IsNullOrWhiteSpace(Classifier) ? "" : "-" + Classifier)}.{Packaging}";
        }

        public override string ToString()
        {
            return ToString(DISPLAY_DELIMITER);
        }

        public string ToString(char delimiter)
        {
            return string.Join(
                delimiter.ToString(),
                new[] { Group, Artifact, Packaging, Classifier }
                    .Where(item => item != null && item.Trim().Length != 0));
        }
    }
}