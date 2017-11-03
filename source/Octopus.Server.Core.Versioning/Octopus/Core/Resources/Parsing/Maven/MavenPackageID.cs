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
        public string GroupMetadataPath => "/maven2/" +
                                           Groups?.Aggregate((result, item) => result + "/" + item) +
                                           "/" +
                                           Artifact +
                                           "/maven-metadata.xml";
        
        /// <summary>
        /// The path to the metadata file for the artifact
        /// </summary>
        public string GroupVersionPomPath => "/maven2/" +
                                             Groups?.Aggregate((result, item) => result + "/" + item) +
                                             "/" + Artifact +
                                             "/" + Version + 
                                             "/" + Artifact + "-" + Version + ".pom";
        
        /// <summary>
        /// The path to the archive file for the artifact
        /// </summary>
        public string ArtifactPath => "/maven2/" +
                                      Groups?.Aggregate((result, item) => result + "/" + item) +
                                      "/" + Artifact +
                                      "/" + Version + 
                                      "/" + Artifact + "-" + Version + "." + Packaging;  

        public MavenPackageID(string group, string artifact)
        {
            if (group == null || group.Trim().Length == 0)
            {
                throw new ArgumentException("group can not be empty");
            }
            
            if (artifact == null || artifact.Trim().Length == 0)
            {
                throw new ArgumentException("artifact can not be empty");
            }
            
            Group = group.Trim();
            Artifact = artifact.Trim();
        }
        
        public MavenPackageID(string group, string artifact, string version)
        {
            if (group == null || group.Trim().Length == 0)
            {
                throw new ArgumentException("group can not be empty");
            }
            
            if (artifact == null || artifact.Trim().Length == 0)
            {
                throw new ArgumentException("artifact can not be empty");
            }
            
            if (version == null || version.Trim().Length == 0)
            {
                throw new ArgumentException("version can not be empty");
            }
            
            Group = group.Trim();
            Artifact = artifact.Trim();
            Version = version.Trim();
        }
        
        public MavenPackageID(string group, string artifact, string version, string packaging)
        {
            if (group == null || group.Trim().Length == 0)
            {
                throw new ArgumentException("group can not be empty");
            }
            
            if (artifact == null || artifact.Trim().Length == 0)
            {
                throw new ArgumentException("artifact can not be empty");
            }
            
            if (version == null || version.Trim().Length == 0)
            {
                throw new ArgumentException("version can not be empty");
            }
            
            if (packaging == null || packaging.Trim().Length == 0)
            {
                throw new ArgumentException("packaging can not be empty");
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
            if (mavenDisplaySplit.Length >= 2)
            {
                Group = mavenDisplaySplit[0].Trim();
                Artifact = mavenDisplaySplit[1].Trim();
                Packaging = mavenDisplaySplit.Length >= 3 ? mavenDisplaySplit[2].Trim() : "";
                Classifier = mavenDisplaySplit.Length >= 4 ? mavenDisplaySplit[3].Trim() : "";
            }
            /*
             * When pushing a delta we will be using the Maven#G#A#V format from
             * the name of the file saved in the local cache.
             */
            else if (mavenSplit.Length >= 3 && mavenSplit[0] == JavaConstants.MavenFeedPrefix)
            {
                Group = mavenSplit[1].Trim();
                Artifact = mavenSplit[2].Trim();
                Packaging = mavenSplit.Length >= 4 ? mavenSplit[3].Trim() : "";
                Classifier = mavenSplit.Length >= 5 ? mavenSplit[4].Trim() : "";
            }
            else
            {
                throw new Exception("Package ID of " + id + " was neither a G:A:V or a Maven#G#A#V format");
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