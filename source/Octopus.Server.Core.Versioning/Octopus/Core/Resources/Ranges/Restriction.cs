using System;
using System.Text;
using Octopus.Core.Resources.Versioning;

namespace Octopus.Core.Resources.Ranges
{
    /// <summary>
    /// Based on https://github.com/apache/maven/blob/master/maven-artifact/src/main/java/org/apache/maven/artifact/versioning/Restriction.java
    /// </summary>
    public class Restriction
    {
        public static readonly Restriction EVERYTHING = new Restriction(null, false, null, false);

        public Restriction(
            IVersion lowerBound, 
            bool lowerBoundInclusive, 
            IVersion upperBound,
            bool upperBoundInclusive)
        {
            LowerBound = lowerBound;
            IsLowerBoundInclusive = lowerBoundInclusive;
            UpperBound = upperBound;
            IsUpperBoundInclusive = upperBoundInclusive;
        }

        public IVersion LowerBound { get; }
        public bool IsLowerBoundInclusive { get; }
        public IVersion UpperBound { get; }
        public bool IsUpperBoundInclusive { get; }

        public bool ContainsVersion(IVersion version)
        {
            if (LowerBound != null)
            {
                int comparison = LowerBound.CompareTo(version);

                if ((comparison == 0) && !IsLowerBoundInclusive)
                {
                    return false;
                }
                if (comparison > 0)
                {
                    return false;
                }
            }
            if (UpperBound != null)
            {
                int comparison = UpperBound.CompareTo(version);

                if ((comparison == 0) && !IsUpperBoundInclusive)
                {
                    return false;
                }
                if (comparison < 0)
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            int result = 13;

            if (LowerBound == null)
            {
                result += 1;
            }
            else
            {
                result += LowerBound.GetHashCode();
            }

            result *= IsLowerBoundInclusive ? 1 : 2;

            if (UpperBound == null)
            {
                result -= 3;
            }
            else
            {
                result -= UpperBound.GetHashCode();
            }

            result *= IsUpperBoundInclusive ? 2 : 3;

            return result;
        }

        public override bool Equals(Object other)
        {
            if (this == other)
            {
                return true;
            }

            if (!(other is Restriction))
            {
                return false;
            }

            Restriction restriction = (Restriction) other;
            if (LowerBound != null)
            {
                if (!LowerBound.Equals(restriction.LowerBound))
                {
                    return false;
                }
            }
            else if (restriction.LowerBound != null)
            {
                return false;
            }

            if (IsLowerBoundInclusive != restriction.IsLowerBoundInclusive)
            {
                return false;
            }

            if (UpperBound != null)
            {
                if (!UpperBound.Equals(restriction.UpperBound))
                {
                    return false;
                }
            }
            else if (restriction.UpperBound != null)
            {
                return false;
            }

            return IsUpperBoundInclusive == restriction.IsUpperBoundInclusive;
        }

        public override String ToString()
        {
            StringBuilder buf = new StringBuilder();

            buf.Append(IsLowerBoundInclusive ? '[' : '(');
            if (LowerBound != null)
            {
                buf.Append(LowerBound.ToString());
            }
            buf.Append(',');
            if (UpperBound != null)
            {
                buf.Append(UpperBound.ToString());
            }
            buf.Append(IsUpperBoundInclusive ? ']' : ')');

            return buf.ToString();
        }
    }
}