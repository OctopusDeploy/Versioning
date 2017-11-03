using System.Text.RegularExpressions;

namespace Octopus.Core.Resources.Parsing.Maven
{
    public class MavenURLParser : IMavenURLParser
    {
        public string SanitiseFeedUri(string uri) => Regex.Replace(uri, "(maven2)?//$", "");
    }
}