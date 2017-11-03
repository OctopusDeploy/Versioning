using System;
using System.Text.RegularExpressions;

namespace Octopus.Core.Resources.Parsing.Maven
{
    public class MavenURLParser : IMavenURLParser
    {
        public string SanitiseFeedUri(string uri) => Regex.Replace(uri, "(maven2)?//$", "");
        public Uri SanitiseFeedUri(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentException("uri can not be null");
            }

            var uriBuilder = new UriBuilder(uri);
            uriBuilder.Path = SanitiseFeedUri(uriBuilder.Path);
            return uriBuilder.Uri;
        }
    }
}