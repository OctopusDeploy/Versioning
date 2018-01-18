using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Octopus.Versioning.Semver
{
    public class SemanticVersionUtils : ISemanticVersionUtils
    {
        public IEnumerable<string> ParseReleaseLabels(string releaseLabels)
        {
            if (!string.IsNullOrEmpty(releaseLabels))
            {
                return releaseLabels.Split('.');
            }

            return null;
        }

        public string GetLegacyString(Version version, IEnumerable<string> releaseLabels, string metadata)
        {
            var sb = new StringBuilder(version.ToString());

            if (releaseLabels != null)
            {
                sb.AppendFormat(CultureInfo.InvariantCulture, "-{0}", String.Join(".", releaseLabels));
            }

            if (!string.IsNullOrEmpty(metadata))
            {
                sb.AppendFormat(CultureInfo.InvariantCulture, "+{0}", metadata);
            }

            return sb.ToString();
        }

        public Version NormalizeVersionValue(Version version)
        {
            var normalized = version;

            if (version.Build < 0
                || version.Revision < 0)
            {
                normalized = new Version(
                    version.Major,
                    version.Minor,
                    Math.Max(version.Build, 0),
                    Math.Max(version.Revision, 0));
            }

            return normalized;
        }

        public Tuple<string, string[], string> ParseSections(string value)
        {
            string versionString = null;
            string[] releaseLabels = null;
            string buildMetadata = null;

            var dashPos = -1;
            var plusPos = -1;

            var chars = value.ToCharArray();

            var end = false;
            for (var i = 0; i < chars.Length; i++)
            {
                end = (i == chars.Length - 1);

                if (dashPos < 0)
                {
                    if (end
                        || chars[i] == '-'
                        || chars[i] == '+')
                    {
                        var endPos = i + (end ? 1 : 0);
                        versionString = value.Substring(0, endPos);

                        dashPos = i;

                        if (chars[i] == '+')
                        {
                            plusPos = i;
                        }
                    }
                }
                else if (plusPos < 0)
                {
                    if (end || chars[i] == '+')
                    {
                        var start = dashPos + 1;
                        var endPos = i + (end ? 1 : 0);
                        var releaseLabel = value.Substring(start, endPos - start);

                        releaseLabels = releaseLabel.Split('.');

                        plusPos = i;
                    }
                }
                else if (end)
                {
                    var start = plusPos + 1;
                    var endPos = i + (end ? 1 : 0);
                    buildMetadata = value.Substring(start, endPos - start);
                }
            }

            return new Tuple<string, string[], string>(versionString, releaseLabels, buildMetadata);
        }

        public bool IsValid(string s, bool allowLeadingZeros)
        {
            return s.Split('.').All(p => IsValidPart(p, allowLeadingZeros));
        }

        public bool IsValidPart(string s, bool allowLeadingZeros)
        {
            return IsValidPart(s.ToCharArray(), allowLeadingZeros);
        }

        public bool IsValidPart(char[] chars, bool allowLeadingZeros)
        {
            var result = true;

            if (chars.Length == 0)
            {
                // empty labels are not allowed
                result = false;
            }

            // 0 is fine, but 00 is not. 
            // 0A counts as an alpha numeric string where zeros are not counted
            if (!allowLeadingZeros
                && chars.Length > 1
                && chars[0] == '0'
                && chars.All(c => Char.IsDigit(c)))
            {
                // no leading zeros in labels allowed
                result = false;
            }
            else
            {
                result &= chars.All(c => IsLetterOrDigitOrDash(c));
            }

            return result;
        }

        public bool IsLetterOrDigitOrDash(char c)
        {
            var x = (int)c;

            // "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-"
            return (x >= 48 && x <= 57) || (x >= 65 && x <= 90) || (x >= 97 && x <= 122) || x == 45;
        }

        public string IncrementRelease(string release)
        {
            if (release == null)
                throw new ArgumentNullException(nameof(release));

            var rev = release.Reverse().ToArray();
            var digits = new string(rev.TakeWhile(char.IsDigit).Reverse().ToArray());
            var alpha = new string(rev.SkipWhile(char.IsDigit).Reverse().ToArray());

            if (digits != "")
                return alpha + (BigInteger.Parse(digits) + 1);

            if (alpha != "")
                return alpha + ".2";

            return "2";
        }
    }
}