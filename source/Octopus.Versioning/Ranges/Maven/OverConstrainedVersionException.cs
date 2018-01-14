using System;

namespace Octopus.Versioning.Ranges.Maven
{
    /// <summary>
    /// Based on https://github.com/apache/maven/blob/master/maven-artifact/src/main/java/org/apache/maven/artifact/versioning/OverConstrainedVersionException.java
    /// </summary>
    public class OverConstrainedVersionException: Exception
    {
        public OverConstrainedVersionException()
        {
        }

        public OverConstrainedVersionException(string message)
            : base(message)
        {
        }

        public OverConstrainedVersionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}