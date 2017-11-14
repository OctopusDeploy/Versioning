using System.Xml;

namespace Octopus.Core.Resources.Parsing.Maven
{
    /// <summary>
    /// Defines a service for parsing metadata files
    /// </summary>
    public interface IMetadataParser
    {
        /// <summary>
        /// Given a maven-metadata.xml file parsed into a XML document, find the latest
        /// value of a snapshot artifact for a the given extension.
        /// </summary>
        /// <param name="snapshotMetadata">The parsed Snapshot maven-metadata.xml file</param>
        /// <param name="extension">The artifect extension</param>
        /// <returns></returns>
        string GetLatestSnapshotRelease(XmlDocument snapshotMetadata, string extension);
    }
}