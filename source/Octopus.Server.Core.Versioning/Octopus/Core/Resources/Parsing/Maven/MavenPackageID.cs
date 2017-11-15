using System;
using System.Linq;
using Octopus.Core.Constants;
using Octopus.Core.Resources.Versioning;
using Octopus.Core.Resources.Versioning.Maven;

namespace Octopus.Core.Resources.Parsing.Maven
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

        public string Group { get; private set; }
        public string[] Groups => Group?.Split('.');
        public string Artifact { get; private set; }
        public string Version { get; private set; }
        public string Packaging { get; private set; }
        public string Classifier { get; private set; }
        public string DisplayName => ToString(DISPLAY_DELIMITER);

        public string FileSystemName => JavaConstants.MavenFeedPrefix +
                                        JavaConstants.MavenFilenameDelimiter +
                                        ToString(JavaConstants.MavenFilenameDelimiter);

        public IVersion SemanticVersion => new MavenVersionParser().Parse(Version);

        /// <summary>
        /// The path to the metadata file for the artifact
        /// </summary>
        public string GroupMetadataPath {
            get
            {
                if (Groups == null || Groups.Length == 0)
                {
                    throw new ArgumentException("Groups can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");
                }

                if (Artifact == null || Artifact.Trim().Length == 0)
                {
                    throw new ArgumentException("Artifact can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");
                }
                
                return "/" + Groups?.Aggregate((result, item) => result + "/" + item) +
                       "/" +
                       Artifact +
                       "/maven-metadata.xml";
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
                {
                    throw new ArgumentException("Groups can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");
                }

                if (Artifact == null || Artifact.Trim().Length == 0)
                {
                    throw new ArgumentException("Artifact can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");
                }

                if (Version == null || Version.Trim().Length == 0)
                {
                    throw new ArgumentException("Version can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");
                }

                return "/" + Groups?.Aggregate((result, item) => result + "/" + item) +
                       "/" +
                       Artifact +
                       "/" + Version +
                       "/maven-metadata.xml";
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
                {
                    throw new ArgumentException("Groups can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");
                }

                if (Artifact == null || Artifact.Trim().Length == 0)
                {
                    throw new ArgumentException("Artifact can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");
                }

                if (Version == null || Version.Trim().Length == 0)
                {
                    throw new ArgumentException("Version can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");
                }

                return "/" + Groups?.Aggregate((result, item) => result + "/" + item) +
                       "/" + Artifact +
                       "/" + Version +
                       "/" + Artifact + "-" + Version + ".pom";
            }
        }

        /// <summary>
        /// The path to the metadata file for the artifact with a custome value
        /// </summary>
        public string SnapshotGroupVersionPomPath(string value)
        {
            if (Groups == null || Groups.Length == 0)
            {
                throw new ArgumentException("Groups can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");
            }

            if (Artifact == null || Artifact.Trim().Length == 0)
            {
                throw new ArgumentException("Artifact can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");
            }

            if (Version == null || Version.Trim().Length == 0)
            {
                throw new ArgumentException("Version can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");
            }

            return "/" + Groups?.Aggregate((result, item) => result + "/" + item) +
                   "/" + Artifact +
                   "/" + Version +
                   "/" + Artifact + "-" + value + ".pom";
        }

        /// <summary>
        /// The path to the archive file for the artifact
        /// </summary>
        public string DefaultArtifactPath {
            get
            {
                if (Groups == null || Groups.Length == 0)
                {
                    throw new ArgumentException("Groups can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");
                }

                if (Artifact == null || Artifact.Trim().Length == 0)
                {
                    throw new ArgumentException("Artifact can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");
                }

                if (Version == null || Version.Trim().Length == 0)
                {
                    throw new ArgumentException("Version can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");
                }
                
                return "/" + Groups?.Aggregate((result, item) => result + "/" + item) +
                       "/" + Artifact +
                       "/" + Version +
                       "/" + Artifact + "-" + Version + "." + Packaging;
            }
        }

        /// <summary>
        /// The path to the archive file for the artifact with a custom value
        /// </summary>
        public string SnapshotArtifactPath(string value)
        {
            if (Groups == null || Groups.Length == 0)
            {
                throw new ArgumentException("Groups can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");
            }

            if (Artifact == null || Artifact.Trim().Length == 0)
            {
                throw new ArgumentException("Artifact can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");
            }

            if (Version == null || Version.Trim().Length == 0)
            {
                throw new ArgumentException("Version can not be null or empty. Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");
            }
            
            return "/" + Groups?.Aggregate((result, item) => result + "/" + item) +
                "/" + Artifact +
                "/" + Version +
                "/" + Artifact + "-" + value + "." + Packaging;
        }

        public MavenPackageID(string group, string artifact)
        {
            if (group == null || group.Trim().Length == 0)
            {
                throw new ArgumentException("Group can not be empty");
            }

            if (artifact == null || artifact.Trim().Length == 0)
            {
                throw new ArgumentException("Artifact can not be empty.");
            }

            Group = group.Trim();
            Artifact = artifact.Trim();
        }

        public MavenPackageID(string group, string artifact, string version)
        {
            if (group == null || group.Trim().Length == 0)
            {
                throw new ArgumentException("Group can not be empty");
            }

            if (artifact == null || artifact.Trim().Length == 0)
            {
                throw new ArgumentException("Artifact can not be empty");
            }

            if (version == null || version.Trim().Length == 0)
            {
                throw new ArgumentException("Version can not be empty");
            }

            Group = group.Trim();
            Artifact = artifact.Trim();
            Version = version.Trim();
        }

        public MavenPackageID(string group, string artifact, string version, string packaging)
        {
            if (group == null || group.Trim().Length == 0)
            {
                throw new ArgumentException("Group can not be empty");
            }

            if (artifact == null || artifact.Trim().Length == 0)
            {
                throw new ArgumentException("Artifact can not be empty");
            }

            if (version == null || version.Trim().Length == 0)
            {
                throw new ArgumentException("Version can not be empty");
            }

            if (packaging == null || packaging.Trim().Length == 0)
            {
                throw new ArgumentException("Packaging can not be empty");
            }

            Group = group.Trim();
            Artifact = artifact.Trim();
            Version = version.Trim();
            Packaging = packaging.Trim();
        }

        public MavenPackageID(string id, IVersion version) : this(id)
        {
            Version = version.ToString();
        }

        /// <summary>
        /// Parses an octopus package id into the maven package details.
        /// </summary>
        /// <param name="id">
        /// The package id, either in a display format like "Group:Artifact"
        /// or in a filesystem format like "Maven#Group#Artifact".
        /// </param>
        public MavenPackageID(string id)
        {
            if (id == null || id.Trim().Length == 0)
            {
                throw new ArgumentException("id can not be empty");
            }

            var mavenDisplaySplit = id.Split(DISPLAY_DELIMITER);
            var mavenSplit = id.Split(JavaConstants.MavenFilenameDelimiter);

            /*
             * When downloading for the first time, we will use the G:A:V format
             * supplied by the end user.
             */
            if (mavenDisplaySplit.Length >= 2 && mavenDisplaySplit.All(x => x != null && x.Trim().Length != 0))
            {
                Group = mavenDisplaySplit[0].Trim();
                Artifact = mavenDisplaySplit[1].Trim();

                if (mavenDisplaySplit.Length == 3) // groupId:artifactId:version
                {
                    Version = mavenDisplaySplit[2].Trim();
                }
                else if (mavenDisplaySplit.Length == 4) // groupId:artifactId:packaging:version
                {
                    Packaging = mavenDisplaySplit[2].Trim();
                    Version = mavenDisplaySplit[3].Trim();
                }
                else if (mavenDisplaySplit.Length == 5) // groupId:artifactId:packaging:classifier:version
                {
                    Packaging = mavenDisplaySplit[2].Trim();
                    Classifier = mavenDisplaySplit[3].Trim();
                    Version = mavenDisplaySplit[4].Trim();
                }
            }
            /*
             * When pushing a delta we will be using the Maven#G#A#V format from
             * the name of the file saved in the local cache.
             */
            else if (mavenSplit.Length >= 3 && mavenSplit[0] == JavaConstants.MavenFeedPrefix)
            {
                Group = mavenSplit[1].Trim();
                Artifact = mavenSplit[2].Trim();
                if (mavenDisplaySplit.Length == 4)
                {
                    Version = mavenDisplaySplit[3].Trim();
                }
                else if (mavenDisplaySplit.Length == 5)
                {
                    Packaging = mavenDisplaySplit[3].Trim();
                    Version = mavenDisplaySplit[4].Trim();
                }
                else if (mavenDisplaySplit.Length == 6)
                {
                    Packaging = mavenDisplaySplit[3].Trim();
                    Classifier = mavenDisplaySplit[4].Trim();
                    Version = mavenDisplaySplit[5].Trim();
                }
            }
            else
            {
                throw new Exception(
                    "Package ID must be in the format Group:Artifact e.g. com.google.guava:guava or junit:junit.");
            }
        }

        public override string ToString()
        {
            return ToString(DISPLAY_DELIMITER);
        }

        public string ToString(char delimiter)
        {
            return string.Join(
                delimiter.ToString(),
                new[] {Group, Artifact, Packaging, Classifier}
                    .Where(item => item != null && item.Trim().Length != 0));
        }
    }
}