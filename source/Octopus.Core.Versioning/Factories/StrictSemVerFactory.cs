using System;
using System.Linq;
using Octopus.Core.Versioning.Semver;
using StrictSemanticVersion = Octopus.Core.Versioning.Semver.StrictSemanticVersion;

namespace Octopus.Core.Versioning.Factories
{
    public class StrictSemVerFactory
    {
        public StrictSemanticVersion CreateVersion(string input)
        {
            StrictSemanticVersion ver = null;
            if (!TryParse(input, out ver))
            {
                throw new ArgumentException($"'{input}' is not a valid version string", nameof(input));
            }

            return ver;
        }
        
        /// <summary>
        /// Parses a SemVer string using strict SemVer rules.
        /// </summary>
        public StrictSemanticVersion Parse(string value)
        {
            StrictSemanticVersion ver = null;
            if (!TryParse(value, out ver))
            {
                throw new ArgumentException($"'{value}' is not a valid version string", nameof(value));
            }

            return ver;
        }

        /// <summary>
        /// Parse a version string
        /// </summary>
        /// <returns>false if the version is not a strict semver</returns>
        public bool TryParse(string value, out StrictSemanticVersion version)
        {
            version = null;

            if (value != null)
            {
                Version systemVersion = null;

                var sections = SemanticVersionUtils.ParseSections(value);

                // null indicates the string did not meet the rules
                if (sections != null
                    && Version.TryParse(sections.Item1, out systemVersion))
                {
                    // validate the version string
                    var parts = sections.Item1.Split('.');

                    if (parts.Length != 3)
                    {
                        // versions must be 3 parts
                        return false;
                    }

                    foreach (var part in parts)
                    {
                        if (!SemanticVersionUtils.IsValidPart(part, false))
                        {
                            // leading zeros are not allowed
                            return false;
                        }
                    }

                    // labels
                    if (sections.Item2 != null
                        && !sections.Item2.All(s => SemanticVersionUtils.IsValidPart(s, false)))
                    {
                        return false;
                    }

                    // build metadata
                    if (sections.Item3 != null
                        && !SemanticVersionUtils.IsValid(sections.Item3, true))
                    {
                        return false;
                    }

                    var ver = SemanticVersionUtils.NormalizeVersionValue(systemVersion);

                    version = new StrictSemanticVersion(version: ver,
                        releaseLabels: sections.Item2,
                        metadata: sections.Item3 ?? string.Empty);

                    return true;
                }
            }

            return false;
        }
    }
}