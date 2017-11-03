using System;

namespace Octopus.Core.Resources.Parsing.Maven
{
    public interface IMavenURLParser
    {
        /// <summary>
        /// Given a maven repo, return a sanitised uri that we can use
        /// to download packages.
        /// </summary>
        /// <param name="uri">The input uri</param>
        /// <returns>The sanitised uri</returns>
        string SanitiseFeedUri(string uri);
        Uri SanitiseFeedUri(Uri uri);
    }
}