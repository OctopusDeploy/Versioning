using System;

namespace Octopus.Core.Resources
{
    public enum FeedType
    {
        None = 0,
        NuGet,
        Docker,
        Maven
    }
    
    public static class FeedTypeExtensions
    {
        /// <summary>
        /// Returns the precidence of a feed type. Feeds with higher precidence will be 
        /// prefered when multiple feeds can support the same extension type.
        /// 
        /// Typically the only precidence that matters is between NuGet (which is also considered
        /// to be the built in library) and a more specific feed like Maven. For example, both
        /// these feeds support .zip, .jar, .war, .ear etc files. But because the Maven feed has
        /// a higher priority, if the package can be parsed as a maven feed package, it will be
        /// preferred over any parsing that shows that it could be a nuget package.
        /// </summary>
        /// <param name="self">The FeedType enum</param>
        /// <returns>The precidence</returns>
        /// <exception cref="Exception"></exception>
        public static int Precidence(this FeedType self)
        {            
            switch(self)
            {
                case FeedType.None:
                    return 0;
                case FeedType.NuGet:
                    return 1;
                case FeedType.Docker:
                    return 2;
                case FeedType.Maven:
                    return 3;
                default:
                    throw new Exception("Invalid Feed Type");
            }
        }
    }
}