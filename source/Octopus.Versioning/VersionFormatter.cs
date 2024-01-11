// Based on VersionFormatter from https://github.com/NuGet/NuGet.Client
// NuGet is licensed under the Apache license: https://github.com/NuGet/NuGet.Client/blob/dev/LICENSE.txt

using System;
using System.Globalization;
using System.Text;
using Octopus.Versioning.Semver;

namespace Octopus.Versioning
{
    public class VersionFormatter : IFormatProvider, ICustomFormatter
    {
        public string? Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg == null)
                throw new ArgumentException("arg can not be null");

            string? formatted = null;
            var argType = arg.GetType();

            if (argType == typeof(IFormattable))
                formatted = ((IFormattable)arg).ToString(format, formatProvider);
            else if (!string.IsNullOrEmpty(format))
                if (arg is ISortableVersion version)
                {
                    // single char identifiers
                    if (format.Length == 1)
                    {
                        formatted = Format(format[0], version);
                    }
                    else
                    {
                        var sb = new StringBuilder(format.Length);

                        for (var i = 0; i < format.Length; i++)
                        {
                            var s = Format(format[i], version);

                            if (s == null)
                                sb.Append(format[i]);
                            else
                                sb.Append(s);
                        }

                        formatted = sb.ToString();
                    }
                }

            return formatted;
        }

        public object? GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter) ||
                typeof(ISortableVersion).IsAssignableFrom(formatType))
                return this;

            return null;
        }

        static string GetNormalizedString(ISortableVersion sortableVersion)
        {
            var sb = new StringBuilder();

            sb.Append(Format('V', sortableVersion));

            if (sortableVersion.IsPrerelease)
            {
                sb.Append('-');
                sb.Append(sortableVersion.Release);
            }

            if (sortableVersion.HasMetadata)
            {
                sb.Append('+');
                sb.Append(sortableVersion.Metadata);
            }

            return sb.ToString();
        }

        static string? Format(char c, ISortableVersion sortableVersion)
        {
            string? s = null;

            switch (c)
            {
                case 'N':
                    s = GetNormalizedString(sortableVersion);
                    break;
                case 'R':
                    s = sortableVersion.Release;
                    break;
                case 'M':
                    s = sortableVersion.Metadata;
                    break;
                case 'V':
                    s = FormatVersion(sortableVersion);
                    break;
                case 'x':
                    s = string.Format(CultureInfo.InvariantCulture, "{0}", sortableVersion.Major);
                    break;
                case 'y':
                    s = string.Format(CultureInfo.InvariantCulture, "{0}", sortableVersion.Minor);
                    break;
                case 'z':
                    s = string.Format(CultureInfo.InvariantCulture, "{0}", sortableVersion.Patch);
                    break;
                case 'r':
                    var nuGetVersion = sortableVersion as SemanticVersion;
                    s = string.Format(CultureInfo.InvariantCulture, "{0}", nuGetVersion != null && nuGetVersion.IsLegacyVersion ? nuGetVersion.Version.Revision : 0);
                    break;
            }

            return s;
        }

        static string FormatVersion(ISortableVersion sortableVersion)
        {
            var nuGetVersion = sortableVersion as SemanticVersion;
            var legacy = nuGetVersion != null && nuGetVersion.IsLegacyVersion;

            return string.Format(CultureInfo.InvariantCulture,
                "{0}.{1}.{2}{3}",
                sortableVersion.Major,
                sortableVersion.Minor,
                sortableVersion.Patch,
                legacy ? string.Format(CultureInfo.InvariantCulture, ".{0}", nuGetVersion?.Version.Revision) : null);
        }
    }
}