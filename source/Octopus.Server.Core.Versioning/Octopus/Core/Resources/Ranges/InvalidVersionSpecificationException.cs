using System;

namespace Octopus.Core.Resources.Ranges
{
    /// <summary>
    /// Based on https://github.com/apache/maven/blob/master/maven-artifact/src/main/java/org/apache/maven/artifact/versioning/InvalidVersionSpecificationException.java
    /// </summary>
    public class InvalidVersionSpecificationException : Exception
    {
        public InvalidVersionSpecificationException()
        {
        }

        public InvalidVersionSpecificationException(string message)
            : base(message)
        {
        }

        public InvalidVersionSpecificationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}