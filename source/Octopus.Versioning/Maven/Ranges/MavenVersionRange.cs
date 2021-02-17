using System;
using System.Collections.Generic;

namespace Octopus.Versioning.Maven.Ranges
{
    /// <summary>
    /// A copy of https://github.com/apache/maven/blob/master/maven-artifact/src/main/java/org/apache/maven/artifact/versioning/VersionRange.java
    /// </summary>
    public class MavenVersionRange
    {
        MavenVersionRange(
            IVersion? recommendedVersion,
            List<Restriction> restrictions )
        {
            RecommendedVersion = recommendedVersion;
            Restrictions = restrictions;
        }

        public IVersion? RecommendedVersion { get; }

        public List<Restriction> Restrictions { get; }

        public MavenVersionRange CloneOf()
        {
            var copiedRestrictions = new List<Restriction>();

            if ( Restrictions != null )
                if ( Restrictions.Count != 0 )
                    copiedRestrictions.AddRange( Restrictions );

            return new MavenVersionRange( RecommendedVersion, copiedRestrictions );
        }

        /**
         * <p>
         * Create a version range from a string representation
         * </p>
         * Some spec examples are:
         * <ul>
         * <li><code>1.0</code> Version 1.0</li>
         * <li><code>[1.0,2.0)</code> Versions 1.0 (included) to 2.0 (not included)</li>
         * <li><code>[1.0,2.0]</code> Versions 1.0 to 2.0 (both included)</li>
         * <li><code>[1.5,)</code> Versions 1.5 and higher</li>
         * <li><code>(,1.0],[1.2,)</code> Versions up to 1.0 (included) and 1.2 or higher</li>
         * </ul>
         *
         * @param spec string representation of a version or version range
         * @return a new {@link MavenVersionRange} object that represents the spec
         * @throws InvalidVersionSpecificationException
         *
         */
        public static MavenVersionRange? CreateFromVersionSpec( string spec )
        {
            if ( spec == null )
                return null;

            var restrictions = new List<Restriction>();
            var process = spec;
            IVersion? version = null;
            IVersion? upperBound = null;
            IVersion? lowerBound = null;

            while ( process.StartsWith( "[" ) || process.StartsWith( "(" ) )
            {
                var index1 = process.IndexOf( ')' );
                var index2 = process.IndexOf( ']' );

                var index = index2;
                if ( index2 < 0 || index1 < index2 )
                    if ( index1 >= 0 )
                        index = index1;

                if ( index < 0 )
                    throw new InvalidVersionSpecificationException( "Unbounded range: " + spec );

                var restriction = ParseRestriction( process.Substring( 0, index + 1 ) );
                if ( lowerBound == null )
                    lowerBound = restriction.LowerBound;
                if ( upperBound != null )
                    if ( restriction.LowerBound == null || restriction.LowerBound.CompareTo( upperBound ) < 0 )
                        throw new InvalidVersionSpecificationException( "Ranges overlap: " + spec );
                restrictions.Add( restriction );
                upperBound = restriction.UpperBound;

                process = process.Substring( index + 1 ).Trim();

                if ( process.Length > 0 && process.StartsWith( "," ) )
                    process = process.Substring( 1 ).Trim();
            }

            if ( process.Length > 0 )
            {
                if ( restrictions.Count > 0 )
                    throw new InvalidVersionSpecificationException(
                        "Only fully-qualified sets allowed in multiple set scenario: " + spec );

                version = new MavenVersionParser().Parse( process );
                restrictions.Add( Restriction.EVERYTHING );
            }

            return new MavenVersionRange( version, restrictions );
        }

        static Restriction ParseRestriction( string spec )
        {
            var lowerBoundInclusive = spec.StartsWith( "[" );
            var upperBoundInclusive = spec.EndsWith( "]" );

            var process = spec.Substring( 1, spec.Length - 2 ).Trim();

            Restriction restriction;

            var index = process.IndexOf( ',' );

            if ( index < 0 )
            {
                if ( !lowerBoundInclusive || !upperBoundInclusive )
                    throw new InvalidVersionSpecificationException( "Single version must be surrounded by []: " + spec );

                IVersion version = new MavenVersionParser().Parse( process );

                restriction = new Restriction( version, lowerBoundInclusive, version, upperBoundInclusive );
            }
            else
            {
                var lowerBound = process.Substring( 0, index ).Trim();
                var upperBound = process.Substring( index + 1 ).Trim();
                if ( lowerBound.Equals( upperBound ) )
                    throw new InvalidVersionSpecificationException( "Range cannot have identical boundaries: " + spec );

                IVersion? lowerVersion = null;
                if ( lowerBound.Length > 0 )
                    lowerVersion = new MavenVersionParser().Parse( lowerBound );
                IVersion? upperVersion = null;
                if ( upperBound.Length > 0 )
                    upperVersion = new MavenVersionParser().Parse( upperBound );

                if ( upperVersion != null && lowerVersion != null && upperVersion.CompareTo( lowerVersion ) < 0 )
                    throw new InvalidVersionSpecificationException( "Range defies version ordering: " + spec );

                restriction = new Restriction( lowerVersion, lowerBoundInclusive, upperVersion, upperBoundInclusive );
            }

            return restriction;
        }

        public static MavenVersionRange CreateFromVersion( string version )
        {
            var restrictions = new List<Restriction>();
            return new MavenVersionRange( new MavenVersionParser().Parse( version ), restrictions );
        }

        /**
         * Creates and returns a new <code>MavenVersionRange</code> that is a restriction of this
         * version range and the specified version range.
         * <p>
         * Note: Precedence is given to the recommended version from this version range over the
         * recommended version from the specified version range.
         * </p>
         *
         * @param restriction the <code>MavenVersionRange</code> that will be used to restrict this version
         *                    range.
         * @return the <code>MavenVersionRange</code> that is a restriction of this version range and the
         *         specified version range.
         *         <p>
         *         The restrictions of the returned version range will be an intersection of the restrictions
         *         of this version range and the specified version range if both version ranges have
         *         restrictions. Otherwise, the restrictions on the returned range will be empty.
         *         </p>
         *         <p>
         *         The recommended version of the returned version range will be the recommended version of
         *         this version range, provided that ranges falls within the intersected restrictions. If
         *         the restrictions are empty, this version range's recommended version is used if it is not
         *         <code>null</code>. If it is <code>null</code>, the specified version range's recommended
         *         version is used (provided it is non-<code>null</code>). If no recommended version can be
         *         obtained, the returned version range's recommended version is set to <code>null</code>.
         *         </p>
         * @throws NullPointerException if the specified <code>MavenVersionRange</code> is
         *                              <code>null</code>.
         */
        public MavenVersionRange Restrict( MavenVersionRange restriction )
        {
            var r1 = Restrictions;
            var r2 = restriction.Restrictions;
            List<Restriction> restrictions;

            if ( r1.Count == 0 || r2.Count == 0 )
                restrictions = new List<Restriction>();
            else
                restrictions = Intersection( r1, r2 );

            IVersion? version = null;
            if ( restrictions.Count > 0 )
            {
                foreach (var r in restrictions)
                    if ( RecommendedVersion != null && r.ContainsVersion( RecommendedVersion ) )
                    {
                        // if we find the original, use that
                        version = RecommendedVersion;
                        break;
                    }
                    else if ( version == null && restriction.RecommendedVersion != null
                        && r.ContainsVersion( restriction.RecommendedVersion ) )
                    {
                        // use this if we can, but prefer the original if possible
                        version = restriction.RecommendedVersion;
                    }
            }
            // Either the original or the specified version ranges have no restrictions
            else if ( RecommendedVersion != null )
            {
                // Use the original recommended version since it exists
                version = RecommendedVersion;
            }
            else if ( restriction.RecommendedVersion != null )
            {
                // Use the recommended version from the specified MavenVersionRange since there is no
                // original recommended version
                version = restriction.RecommendedVersion;
            }
    /* TODO should throw this immediately, but need artifact
            else
            {
                throw new OverConstrainedVersionException( "Restricting incompatible version ranges" );
            }
    */

            return new MavenVersionRange( version, restrictions );
        }

        List<Restriction> Intersection( List<Restriction> r1, List<Restriction> r2 )
        {
            var restrictions = new List<Restriction>( r1.Count + r2.Count );
            var i1 = r1.GetEnumerator();
            var i2 = r2.GetEnumerator();

            i1.MoveNext();
            i2.MoveNext();

            var res1 = i1.Current;
            var res2 = i2.Current;

            var done = false;
            while ( !done )
                if ( res1.LowerBound == null || res2.UpperBound == null
                    || res1.LowerBound.CompareTo( res2.UpperBound ) <= 0 )
                {
                    if ( res1.UpperBound == null || res2.LowerBound == null
                        || res1.UpperBound.CompareTo( res2.LowerBound ) >= 0 )
                    {
                        IVersion? lower;
                        IVersion? upper;
                        bool lowerInclusive;
                        bool upperInclusive;

                        // overlaps
                        if ( res1.LowerBound == null )
                        {
                            lower = res2.LowerBound;
                            lowerInclusive = res2.IsLowerBoundInclusive;
                        }
                        else if ( res2.LowerBound == null )
                        {
                            lower = res1.LowerBound;
                            lowerInclusive = res1.IsLowerBoundInclusive;
                        }
                        else
                        {
                            var comparison = res1.LowerBound.CompareTo( res2.LowerBound );
                            if ( comparison < 0 )
                            {
                                lower = res2.LowerBound;
                                lowerInclusive = res2.IsLowerBoundInclusive;
                            }
                            else if ( comparison == 0 )
                            {
                                lower = res1.LowerBound;
                                lowerInclusive = res1.IsLowerBoundInclusive && res2.IsLowerBoundInclusive;
                            }
                            else
                            {
                                lower = res1.LowerBound;
                                lowerInclusive = res1.IsLowerBoundInclusive;
                            }
                        }

                        if ( res1.UpperBound == null )
                        {
                            upper = res2.UpperBound;
                            upperInclusive = res2.IsUpperBoundInclusive;
                        }
                        else if ( res2.UpperBound == null )
                        {
                            upper = res1.UpperBound;
                            upperInclusive = res1.IsUpperBoundInclusive;
                        }
                        else
                        {
                            var comparison = res1.UpperBound.CompareTo( res2.UpperBound );
                            if ( comparison < 0 )
                            {
                                upper = res1.UpperBound;
                                upperInclusive = res1.IsUpperBoundInclusive;
                            }
                            else if ( comparison == 0 )
                            {
                                upper = res1.UpperBound;
                                upperInclusive = res1.IsUpperBoundInclusive && res2.IsUpperBoundInclusive;
                            }
                            else
                            {
                                upper = res2.UpperBound;
                                upperInclusive = res2.IsUpperBoundInclusive;
                            }
                        }

                        // don't add if they are equal and one is not inclusive
                        if ( lower == null || upper == null || lower.CompareTo( upper ) != 0 )
                            restrictions.Add( new Restriction( lower, lowerInclusive, upper, upperInclusive ) );
                        else if ( lowerInclusive && upperInclusive )
                            restrictions.Add( new Restriction( lower, lowerInclusive, upper, upperInclusive ) );

                        //noinspection ObjectEquality
                        if ( upper == res2.UpperBound )
                        {
                            // advance res2
                            if ( i2.MoveNext() )
                                res2 = i2.Current;
                            else
                                done = true;
                        }
                        else
                        {
                            // advance res1
                            if ( i1.MoveNext() )
                                res1 = i1.Current;
                            else
                                done = true;
                        }
                    }
                    else
                    {
                        // move on to next in r1
                        if ( i1.MoveNext() )
                            res1 = i1.Current;
                        else
                            done = true;
                    }
                }
                else
                {
                    // move on to next in r2
                    if ( i2.MoveNext() )
                        res2 = i2.Current;
                    else
                        done = true;
                }

            return restrictions;
        }

        public IVersion? GetSelectedVersion(  )
        {
            IVersion? version = null;
            if ( RecommendedVersion != null )
            {
                version = RecommendedVersion;
            }
            else
            {
                if ( Restrictions.Count == 0 )
                    throw new OverConstrainedVersionException( "The artifact has no valid ranges" );
            }
            return version;
        }

        public bool IsSelectedVersionKnown( )
        {
            var value = false;
            if ( RecommendedVersion != null )
            {
                value = true;
            }
            else
            {
                if ( Restrictions.Count == 0 )
                    throw new OverConstrainedVersionException( "The artifact has no valid ranges" );
            }
            return value;
        }

        public override string ToString()
        {
            if ( RecommendedVersion != null )
                return RecommendedVersion.ToString();

            return string.Join(",", Restrictions);
        }

        public IVersion? MatchVersion( List<IVersion> versions )
        {
            // TODO could be more efficient by sorting the list and then moving along the restrictions in order?

            IVersion? matched = null;
            foreach (var version in versions)
                if ( ContainsVersion( version ) )
                    // valid - check if it is greater than the currently matched version
                    if ( matched == null || version.CompareTo( matched ) > 0 )
                        matched = version;
            return matched;
        }

        public bool ContainsVersion( IVersion version )
        {
            foreach (var restriction in Restrictions)
                if ( restriction.ContainsVersion( version ) )
                    return true;
            return false;
        }

        public bool HasRestrictions()
        {
            return Restrictions.Count != 0 && RecommendedVersion == null;
        }

        public override bool Equals( object obj )
        {
            if ( this == obj )
                return true;
            if ( !( obj is MavenVersionRange ) )
                return false;
            var other = (MavenVersionRange) obj;

            var equals =
                RecommendedVersion == other.RecommendedVersion
                    || RecommendedVersion != null && RecommendedVersion.Equals( other.RecommendedVersion );
            equals &=
                Restrictions == other.Restrictions
                    || Restrictions != null && Restrictions.Equals( other.Restrictions );
            return equals;
        }

        public override int GetHashCode()
        {
            var hash = 7;
            hash = 31 * hash + ( RecommendedVersion == null ? 0 : RecommendedVersion.GetHashCode() );
            hash = 31 * hash + ( Restrictions == null ? 0 : Restrictions.GetHashCode() );
            return hash;
        }
    }
}