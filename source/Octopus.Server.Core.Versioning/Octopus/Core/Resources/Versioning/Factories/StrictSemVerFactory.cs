using System;
using System.Linq;
using Octopus.Core.Resources.Versioning.Semver;

namespace Octopus.Core.Resources.Versioning.Factories
{
    public class StrictSemVerFactory
    {
        static readonly ISemanticVersionUtils utils = new SemanticVersionUtils();

        public Semver.StrictSemanticVersion CreateVersion(string input)
        {
            Semver.StrictSemanticVersion ver = null;
            if (!TryParse(input, out ver))
            {
                throw new ArgumentException($"'{input}' is not a valid version string", nameof(input));
            }

            return ver;
        }
        
        /// <summary>
        /// Parses a SemVer string using strict SemVer rules.
        /// </summary>
        public Semver.StrictSemanticVersion Parse(string value)
        {
            Semver.StrictSemanticVersion ver = null;
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
        public bool TryParse(string value, out Semver.StrictSemanticVersion version)
        {
            version = null;

            if (value != null)
            {
                Version systemVersion = null;

                var sections = utils.ParseSections(value);

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
                        if (!utils.IsValidPart(part, false))
                        {
                            // leading zeros are not allowed
                            return false;
                        }
                    }

                    // labels
                    if (sections.Item2 != null
                        && !sections.Item2.All(s => utils.IsValidPart(s, false)))
                    {
                        return false;
                    }

                    // build metadata
                    if (sections.Item3 != null
                        && !utils.IsValid(sections.Item3, true))
                    {
                        return false;
                    }

                    var ver = utils.NormalizeVersionValue(systemVersion);

                    version = new Semver.StrictSemanticVersion(version: ver,
                        releaseLabels: sections.Item2,
                        metadata: sections.Item3 ?? string.Empty);

                    return true;
                }
            }

            return false;
        }
    }
}