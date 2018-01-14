using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Octopus.Core.Versioning.Constants;

namespace Octopus.Core.Versioning.Metadata
{
    public class PackageIdentifier
    {
        /// <summary>
        /// Given a package-file path and a list of valid extensions extracts the package-metadata component and the extension.  
        /// E.g: Given `C:\HelloWorld.1.0.0.zip-0d0010a3-d421-4e3d-9b28-49dad989281c` would return `{ "HelloWorld.1.0.0", ".zip" }` 
        /// (assuming `.zip` was in the list of valid extensions).
        /// </summary>
        /// <param name="packageFilePath">A package file path</param>
        /// <param name="validExtensions">A list of valid extensions</param>
        /// <returns>a Tuple where Item1 is the package metadata component and Item2 is the extension</returns>
        public static Tuple<string,string> ExtractPackageExtensionAndMetadata(string packageFilePath, ICollection<string> validExtensions)
        {
            var fileName = Path.GetFileName(packageFilePath);
            var matchingExtension = validExtensions.FirstOrDefault(fileName.EndsWith);
            var metaDataSection = string.Empty;
            if (matchingExtension != null)
            {
                metaDataSection = fileName.Substring(0, fileName.Length - matchingExtension.Length);
            }
            else
            {
                foreach (var ext in validExtensions)
                {
                    var match = new Regex("(?<extension>" + Regex.Escape(ext) + ")-[a-z0-9\\-]*$").Match(fileName);
                    if (match.Success)
                    {
                        matchingExtension = match.Groups["extension"].Value;
                        metaDataSection = fileName.Substring(0, match.Index);
                        break;
                    }
                }
            }

            return new Tuple<string, string>(metaDataSection, matchingExtension);
        }
        
        /// <summary>
        /// Given a package-file path and a list of valid extensions extracts the package-metadata component and the extension.  
        /// E.g: Given `C:\HelloWorld.1.0.0_9822965F2883AD43AD79DA4E8795319F.zip` would return `{ "HelloWorld.1.0.0", ".zip" }` 
        /// (assuming `.zip` was in the list of valid extensions).
        /// </summary>
        /// <param name="packageFilePath">A package file path</param>
        /// <param name="validExtensions">A list of valid extensions</param>
        /// <returns>a Tuple where Item1 is the package metadata component and Item2 is the extension</returns>
        public static Tuple<string,string> ExtractPackageExtensionAndMetadataForServer(string packageFilePath, ICollection<string> validExtensions)
        {
            var fileName = Path.GetFileName(packageFilePath);
            foreach (var ext in validExtensions)
            {
                var match = new Regex(ServerConstants.SERVER_CACHE_DELIMITER + "[0-9A-F]{32}(?<extension>" + Regex.Escape(ext) + ")$").Match(fileName);
                if (match.Success)
                {
                    return new Tuple<string, string>(fileName.Substring(0, match.Index), ext);
                }
            }

            return new Tuple<string, string>(string.Empty, null);
        }
    }
}