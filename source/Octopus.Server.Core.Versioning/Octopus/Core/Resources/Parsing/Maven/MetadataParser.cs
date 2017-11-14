using System;
using System.Linq;
using System.Xml;
using Octopus.Core.Extensions;

namespace Octopus.Core.Resources.Parsing.Maven
{
    public class MetadataParser : IMetadataParser
    {
        public string GetLatestSnapshotRelease(XmlDocument snapshotMetadata, string extension)
        {
            return snapshotMetadata.ToEnumerable()
                       .Select(doc => doc.DocumentElement?.SelectSingleNode("./*[local-name()='versioning']"))
                       .Select(node => node?.SelectNodes("./*[local-name()='snapshotVersions']/*[local-name()='snapshotVersion']"))
                       .Where(nodes => nodes != null)
                       .SelectMany(nodes => nodes.Cast<XmlNode>())
                       .Where(node => (node.SelectSingleNode("./*[local-name()='extension']")?.InnerText.Trim() ?? "").Equals(extension.Trim(), StringComparison.OrdinalIgnoreCase))
                       .OrderByDescending(node => node.SelectSingleNode("./*[local-name()='updated']")?.InnerText)
                       .Select(node => node.SelectSingleNode("./*[local-name()='value']")?.InnerText)
                       .FirstOrDefault() ?? "";
        }
    }
}