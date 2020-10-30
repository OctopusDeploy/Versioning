using System.Text.RegularExpressions;

namespace Octopus.Versioning
{
    public static class VersionSanitizeExtensions
    {
        // match any character that is no a letter or a number
        static readonly Regex NonAlphaNumeric = new Regex(@"[^\p{L}\p{N}]");

        public static string AlphaNumericOnly(this string self)
        {
            return NonAlphaNumeric.Replace(self, "-");
        }
    }
}