namespace Octopus.Versioning.Constants
{
    public static class JavaConstants
    {
        /// <summary>
        /// The delimiter here has to satisfy a few conditions:
        /// 1. It can not be a valid character in a maven group or artifact name.
        ///    The org.apache.maven.project.validation.DefaultModelValidator class
        ///    defines the valid group and artifact names as the regex
        ///    [A-Za-z0-9_\\-.]+
        /// 2. It has to be a valid file system character.
        /// 3. It has to be something that calamari can use to split a filename into a
        ///    pacakage id and version on the target. See the 
        ///    Calamari.Integration.Packages.Java.JarExtractor class to see how this char
        ///    is used to parse a filename.
        /// 4. It has to be something that the NuGet parsing routines 
        ///    (i.e. Calamari.Integration.Packages.PackageIdentifierUtils.TryParsePackageIdAndVersion)
        ///    will not parse successfully. We use the inability of the NuGet parsing routines
        ///    to fallback to a Maven parsing routine, which will handle all of the vagaries 
        ///    of Maven version strings.
        /// 
        /// Ideally the delimiter should also not be something used in
        /// version ranges (e.g. (,1.0]).
        /// </summary>
        public const char MavenFilenameDelimiter = '#';
        
        /// <summary>
        /// The prefix added to all files to identify it as coming from a maven feed. This
        /// means that filenames will look like:
        /// Maven#org.example#artifact#1.0.jar
        /// 
        /// Appending a prefix like this is the recommended way to identify the origin of
        /// a package, as it means that a parser can quickly fail if it is looking at
        /// a file that it is not responsible for.
        /// </summary>
        public const string MavenFeedPrefix = "Maven";
    }
}