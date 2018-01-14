using System;
using System.Collections.Generic;
using NUnit.Framework;
using Octopus.Versioning;
using Octopus.Versioning.Maven;
using Octopus.Versioning.Ranges.Maven;

namespace Octopus.Server.Core.Versioning.Tests.Versions
{
    /// <summary>
    /// Based on https://github.com/apache/maven/blob/master/maven-artifact/src/test/java/org/apache/maven/artifact/versioning/VersionRangeTest.java
    /// </summary>
    [TestFixture]
    public class RangeTests
    {
        private const string CHECK_NUM_RESTRICTIONS = "check number of restrictions";

        private const string CHECK_UPPER_BOUND = "check upper bound";

        private const string CHECK_UPPER_BOUND_INCLUSIVE = "check upper bound is inclusive";

        private const string CHECK_LOWER_BOUND = "check lower bound";

        private const string CHECK_LOWER_BOUND_INCLUSIVE = "check lower bound is inclusive";

        private const string CHECK_VERSION_RECOMMENDATION = "check version recommended";

        private const string CHECK_SELECTED_VERSION_KNOWN = "check selected version known";

        private const string CHECK_SELECTED_VERSION = "check selected version";

        [Test]
        public void testCustom()
        {
            var parser = new MavenVersionParser();
            MavenVersionRange range1 = MavenVersionRange.CreateFromVersionSpec( "(,9.0.1)" );
            Assert.IsTrue(range1.ContainsVersion(parser.Parse("8.0.1")));
            Assert.IsFalse(range1.ContainsVersion(parser.Parse( "9.0.10.M27" )) );
            Assert.IsFalse(range1.ContainsVersion(parser.Parse("10.0.0.M22" )));
            Assert.IsTrue(range1.ContainsVersion(parser.Parse( "9.0.0.M25" )));
            Assert.IsTrue(range1.ContainsVersion(parser.Parse( "9.0.0.M22" )));
        }

        [Test]
        public void testRange()
        {
            MavenVersionRange range = MavenVersionRange.CreateFromVersionSpec("(,1.0]");
            List<Restriction> restrictions = range.Restrictions;
            Assert.AreEqual(1, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            Restriction restriction = restrictions[0];
            Assert.IsNull(restriction.LowerBound, CHECK_LOWER_BOUND);
            Assert.IsFalse(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.0", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsTrue(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);
            Assert.IsNull(range.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            Assert.IsFalse(range.IsSelectedVersionKnown(), CHECK_SELECTED_VERSION_KNOWN);
            Assert.IsNull(range.GetSelectedVersion(), CHECK_SELECTED_VERSION);

            range = MavenVersionRange.CreateFromVersionSpec("1.0");
            Assert.AreEqual("1.0", range.RecommendedVersion.ToString(), CHECK_VERSION_RECOMMENDATION);
            restrictions = range.Restrictions;
            Assert.AreEqual(1, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.IsNull(restriction.LowerBound, CHECK_LOWER_BOUND);
            Assert.IsFalse(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.IsNull(restriction.UpperBound, CHECK_UPPER_BOUND);
            Assert.IsFalse(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);
            Assert.IsTrue(range.IsSelectedVersionKnown(), CHECK_SELECTED_VERSION_KNOWN);
            Assert.AreEqual("1.0", range.GetSelectedVersion().ToString(), CHECK_SELECTED_VERSION);

            range = MavenVersionRange.CreateFromVersionSpec("[1.0]");
            restrictions = range.Restrictions;
            Assert.AreEqual(1, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.AreEqual("1.0", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsTrue(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.0", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsTrue(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);
            Assert.IsNull(range.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            Assert.IsFalse(range.IsSelectedVersionKnown(), CHECK_SELECTED_VERSION_KNOWN);
            Assert.IsNull(range.GetSelectedVersion(), CHECK_SELECTED_VERSION);

            range = MavenVersionRange.CreateFromVersionSpec("[1.2,1.3]");
            restrictions = range.Restrictions;
            Assert.AreEqual(1, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.AreEqual("1.2", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsTrue(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.3", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsTrue(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);
            Assert.IsNull(range.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            Assert.IsFalse(range.IsSelectedVersionKnown(), CHECK_SELECTED_VERSION_KNOWN);
            Assert.IsNull(range.GetSelectedVersion(), CHECK_SELECTED_VERSION);

            range = MavenVersionRange.CreateFromVersionSpec("[1.0,2.0)");
            restrictions = range.Restrictions;
            Assert.AreEqual(1, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.AreEqual("1.0", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsTrue(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("2.0", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsFalse(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);
            Assert.IsNull(range.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            Assert.IsFalse(range.IsSelectedVersionKnown(), CHECK_SELECTED_VERSION_KNOWN);
            Assert.IsNull(range.GetSelectedVersion(), CHECK_SELECTED_VERSION);

            range = MavenVersionRange.CreateFromVersionSpec("[1.5,)");
            restrictions = range.Restrictions;
            Assert.AreEqual(1, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.AreEqual("1.5", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsTrue(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.IsNull(restriction.UpperBound, CHECK_UPPER_BOUND);
            Assert.IsFalse(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);
            Assert.IsNull(range.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            Assert.IsFalse(range.IsSelectedVersionKnown(), CHECK_SELECTED_VERSION_KNOWN);
            Assert.IsNull(range.GetSelectedVersion(), CHECK_SELECTED_VERSION);

            range = MavenVersionRange.CreateFromVersionSpec("(,1.0],[1.2,)");
            restrictions = range.Restrictions;
            Assert.AreEqual(2, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.IsNull(restriction.LowerBound, CHECK_LOWER_BOUND);
            Assert.IsFalse(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.0", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsTrue(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);
            Assert.IsNull(range.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            restriction = restrictions[1];
            Assert.AreEqual("1.2", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsTrue(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.IsNull(restriction.UpperBound, CHECK_UPPER_BOUND);
            Assert.IsFalse(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);
            Assert.IsNull(range.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            Assert.IsFalse(range.IsSelectedVersionKnown(), CHECK_SELECTED_VERSION_KNOWN);
            Assert.IsNull(range.GetSelectedVersion(), CHECK_SELECTED_VERSION);

            range = MavenVersionRange.CreateFromVersionSpec("[1.0,)");
            Assert.IsFalse(range.ContainsVersion(new MavenVersionParser().Parse("1.0-SNAPSHOT")));

            range = MavenVersionRange.CreateFromVersionSpec("[1.0,1.1-SNAPSHOT]");
            Assert.IsTrue(range.ContainsVersion(new MavenVersionParser().Parse("1.1-SNAPSHOT")));

            range = MavenVersionRange.CreateFromVersionSpec("[5.0.9.0,5.0.10.0)");
            Assert.IsTrue(range.ContainsVersion(new MavenVersionParser().Parse("5.0.9.0")));
        }

        [Test]
        public void testInvalidRanges()
        {
            checkInvalidRange("(1.0)");
            checkInvalidRange("[1.0)");
            checkInvalidRange("(1.0]");
            checkInvalidRange("(1.0,1.0]");
            checkInvalidRange("[1.0,1.0)");
            checkInvalidRange("(1.0,1.0)");
            checkInvalidRange("[1.1,1.0]");
            checkInvalidRange("[1.0,1.2),1.3");
            // overlap
            checkInvalidRange("[1.0,1.2),(1.1,1.3]");
            // overlap
            checkInvalidRange("[1.1,1.3),(1.0,1.2]");
            // ordering
            checkInvalidRange("(1.1,1.2],[1.0,1.1)");
        }

        [Test]
        public void testIntersections()
        {
            MavenVersionRange range1 = MavenVersionRange.CreateFromVersionSpec("1.0");
            MavenVersionRange range2 = MavenVersionRange.CreateFromVersionSpec("1.1");
            MavenVersionRange mergedRange = range1.Restrict(range2);
            // TODO current policy is to retain the original version - is this correct, do we need strategies or is that handled elsewhere?
//        Assert.AreEqual( "1.1", mergedRange.RecommendedVersion.ToString(), CHECK_VERSION_RECOMMENDATION );
            Assert.AreEqual("1.0", mergedRange.RecommendedVersion.ToString(), CHECK_VERSION_RECOMMENDATION);
            List<Restriction> restrictions = mergedRange.Restrictions;
            Assert.AreEqual(1, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            Restriction restriction = restrictions[0];
            Assert.IsNull(restriction.LowerBound, CHECK_LOWER_BOUND);
            Assert.IsFalse(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.IsNull(restriction.UpperBound, CHECK_UPPER_BOUND);
            Assert.IsFalse(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);

            mergedRange = range2.Restrict(range1);
            Assert.AreEqual("1.1", mergedRange.RecommendedVersion.ToString(), CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(1, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.IsNull(restriction.LowerBound, CHECK_LOWER_BOUND);
            Assert.IsFalse(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.IsNull(restriction.UpperBound, CHECK_UPPER_BOUND);
            Assert.IsFalse(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);

            // TODO test reversed restrictions on all below
            range1 = MavenVersionRange.CreateFromVersionSpec("[1.0,)");
            range2 = MavenVersionRange.CreateFromVersionSpec("1.1");
            mergedRange = range1.Restrict(range2);
            Assert.AreEqual("1.1", mergedRange.RecommendedVersion.ToString(), CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(1, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.AreEqual("1.0", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsTrue(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.IsNull(restriction.UpperBound, CHECK_UPPER_BOUND);
            Assert.IsFalse(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);

            range1 = MavenVersionRange.CreateFromVersionSpec("[1.1,)");
            range2 = MavenVersionRange.CreateFromVersionSpec("1.1");
            mergedRange = range1.Restrict(range2);
            Assert.AreEqual("1.1", mergedRange.RecommendedVersion.ToString(), CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(1, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.AreEqual("1.1", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsTrue(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.IsNull(restriction.UpperBound, CHECK_UPPER_BOUND);
            Assert.IsFalse(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);

            range1 = MavenVersionRange.CreateFromVersionSpec("[1.1]");
            range2 = MavenVersionRange.CreateFromVersionSpec("1.1");
            mergedRange = range1.Restrict(range2);
            Assert.AreEqual("1.1", mergedRange.RecommendedVersion.ToString(), CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(1, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.AreEqual("1.1", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsTrue(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.1", restriction.LowerBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsTrue(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);

            range1 = MavenVersionRange.CreateFromVersionSpec("(1.1,)");
            range2 = MavenVersionRange.CreateFromVersionSpec("1.1");
            mergedRange = range1.Restrict(range2);
            Assert.IsNull(mergedRange.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(1, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.AreEqual("1.1", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsFalse(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.IsNull(restriction.UpperBound, CHECK_UPPER_BOUND);
            Assert.IsFalse(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);

            range1 = MavenVersionRange.CreateFromVersionSpec("[1.2,)");
            range2 = MavenVersionRange.CreateFromVersionSpec("1.1");
            mergedRange = range1.Restrict(range2);
            Assert.IsNull(mergedRange.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(1, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.AreEqual("1.2", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsTrue(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.IsNull(restriction.UpperBound, CHECK_UPPER_BOUND);
            Assert.IsFalse(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);

            range1 = MavenVersionRange.CreateFromVersionSpec("(,1.2]");
            range2 = MavenVersionRange.CreateFromVersionSpec("1.1");
            mergedRange = range1.Restrict(range2);
            Assert.AreEqual("1.1", mergedRange.RecommendedVersion.ToString(), CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(1, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.IsNull(restriction.LowerBound, CHECK_LOWER_BOUND);
            Assert.IsFalse(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.2", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsTrue(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);

            range1 = MavenVersionRange.CreateFromVersionSpec("(,1.1]");
            range2 = MavenVersionRange.CreateFromVersionSpec("1.1");
            mergedRange = range1.Restrict(range2);
            Assert.AreEqual("1.1", mergedRange.RecommendedVersion.ToString(), CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(1, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.IsNull(restriction.LowerBound, CHECK_LOWER_BOUND);
            Assert.IsFalse(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.1", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsTrue(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);

            range1 = MavenVersionRange.CreateFromVersionSpec("(,1.1)");
            range2 = MavenVersionRange.CreateFromVersionSpec("1.1");
            mergedRange = range1.Restrict(range2);
            Assert.IsNull(mergedRange.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(1, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.IsNull(restriction.LowerBound, CHECK_LOWER_BOUND);
            Assert.IsFalse(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.1", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsFalse(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);

            range1 = MavenVersionRange.CreateFromVersionSpec("(,1.0]");
            range2 = MavenVersionRange.CreateFromVersionSpec("1.1");
            mergedRange = range1.Restrict(range2);
            Assert.IsNull(mergedRange.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(1, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.IsNull(restriction.LowerBound, CHECK_LOWER_BOUND);
            Assert.IsFalse(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.0", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsTrue(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);

            range1 = MavenVersionRange.CreateFromVersionSpec("(,1.0], [1.1,)");
            range2 = MavenVersionRange.CreateFromVersionSpec("1.2");
            mergedRange = range1.Restrict(range2);
            Assert.AreEqual("1.2", mergedRange.RecommendedVersion.ToString(), CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(2, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.IsNull(restriction.LowerBound, CHECK_LOWER_BOUND);
            Assert.IsFalse(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.0", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsTrue(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);
            restriction = restrictions[1];
            Assert.AreEqual("1.1", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsTrue(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.IsNull(restriction.UpperBound, CHECK_UPPER_BOUND);
            Assert.IsFalse(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);

            range1 = MavenVersionRange.CreateFromVersionSpec("(,1.0], [1.1,)");
            range2 = MavenVersionRange.CreateFromVersionSpec("1.0.5");
            mergedRange = range1.Restrict(range2);
            Assert.IsNull(mergedRange.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(2, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.IsNull(restriction.LowerBound, CHECK_LOWER_BOUND);
            Assert.IsFalse(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.0", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsTrue(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);
            restriction = restrictions[1];
            Assert.AreEqual("1.1", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsTrue(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.IsNull(restriction.UpperBound, CHECK_UPPER_BOUND);
            Assert.IsFalse(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);

            range1 = MavenVersionRange.CreateFromVersionSpec("(,1.1), (1.1,)");
            range2 = MavenVersionRange.CreateFromVersionSpec("1.1");
            mergedRange = range1.Restrict(range2);
            Assert.IsNull(mergedRange.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(2, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.IsNull(restriction.LowerBound, CHECK_LOWER_BOUND);
            Assert.IsFalse(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.1", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsFalse(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);
            restriction = restrictions[1];
            Assert.AreEqual("1.1", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsFalse(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.IsNull(restriction.UpperBound, CHECK_UPPER_BOUND);
            Assert.IsFalse(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);

            range1 = MavenVersionRange.CreateFromVersionSpec("[1.1,1.3]");
            range2 = MavenVersionRange.CreateFromVersionSpec("(1.1,)");
            mergedRange = range1.Restrict(range2);
            Assert.IsNull(mergedRange.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(1, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.AreEqual("1.1", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsFalse(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.3", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsTrue(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);

            range1 = MavenVersionRange.CreateFromVersionSpec("(,1.3)");
            range2 = MavenVersionRange.CreateFromVersionSpec("[1.2,1.3]");
            mergedRange = range1.Restrict(range2);
            Assert.IsNull(mergedRange.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(1, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.AreEqual("1.2", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsTrue(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.3", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsFalse(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);

            range1 = MavenVersionRange.CreateFromVersionSpec("[1.1,1.3]");
            range2 = MavenVersionRange.CreateFromVersionSpec("[1.2,)");
            mergedRange = range1.Restrict(range2);
            Assert.IsNull(mergedRange.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(1, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.AreEqual("1.2", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsTrue(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.3", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsTrue(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);

            range1 = MavenVersionRange.CreateFromVersionSpec("(,1.3]");
            range2 = MavenVersionRange.CreateFromVersionSpec("[1.2,1.4]");
            mergedRange = range1.Restrict(range2);
            Assert.IsNull(mergedRange.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(1, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.AreEqual("1.2", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsTrue(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.3", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsTrue(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);

            range1 = MavenVersionRange.CreateFromVersionSpec("(1.2,1.3]");
            range2 = MavenVersionRange.CreateFromVersionSpec("[1.1,1.4]");
            mergedRange = range1.Restrict(range2);
            Assert.IsNull(mergedRange.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(1, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.AreEqual("1.2", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsFalse(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.3", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsTrue(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);

            range1 = MavenVersionRange.CreateFromVersionSpec("(1.2,1.3)");
            range2 = MavenVersionRange.CreateFromVersionSpec("[1.1,1.4]");
            mergedRange = range1.Restrict(range2);
            Assert.IsNull(mergedRange.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(1, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.AreEqual("1.2", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsFalse(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.3", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsFalse(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);

            range1 = MavenVersionRange.CreateFromVersionSpec("[1.2,1.3)");
            range2 = MavenVersionRange.CreateFromVersionSpec("[1.1,1.4]");
            mergedRange = range1.Restrict(range2);
            Assert.IsNull(mergedRange.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(1, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.AreEqual("1.2", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsTrue(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.3", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsFalse(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);

            range1 = MavenVersionRange.CreateFromVersionSpec("[1.0,1.1]");
            range2 = MavenVersionRange.CreateFromVersionSpec("[1.1,1.4]");
            mergedRange = range1.Restrict(range2);
            Assert.IsNull(mergedRange.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(1, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.AreEqual("1.1", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsTrue(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.1", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsTrue(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);

            range1 = MavenVersionRange.CreateFromVersionSpec("[1.0,1.1)");
            range2 = MavenVersionRange.CreateFromVersionSpec("[1.1,1.4]");
            mergedRange = range1.Restrict(range2);
            Assert.IsNull(mergedRange.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(0, restrictions.Count, CHECK_NUM_RESTRICTIONS);

            range1 = MavenVersionRange.CreateFromVersionSpec("[1.0,1.2],[1.3,1.5]");
            range2 = MavenVersionRange.CreateFromVersionSpec("[1.1]");
            mergedRange = range1.Restrict(range2);
            Assert.IsNull(mergedRange.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(1, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.AreEqual("1.1", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsTrue(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.1", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsTrue(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);

            range1 = MavenVersionRange.CreateFromVersionSpec("[1.0,1.2],[1.3,1.5]");
            range2 = MavenVersionRange.CreateFromVersionSpec("[1.4]");
            mergedRange = range1.Restrict(range2);
            Assert.IsNull(mergedRange.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(1, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.AreEqual("1.4", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsTrue(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.4", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsTrue(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);

            range1 = MavenVersionRange.CreateFromVersionSpec("[1.0,1.2],[1.3,1.5]");
            range2 = MavenVersionRange.CreateFromVersionSpec("[1.1,1.4]");
            mergedRange = range1.Restrict(range2);
            Assert.IsNull(mergedRange.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(2, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.AreEqual("1.1", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsTrue(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.2", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsTrue(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);
            restriction = restrictions[1];
            Assert.AreEqual("1.3", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsTrue(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.4", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsTrue(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);

            range1 = MavenVersionRange.CreateFromVersionSpec("[1.0,1.2),(1.3,1.5]");
            range2 = MavenVersionRange.CreateFromVersionSpec("[1.1,1.4]");
            mergedRange = range1.Restrict(range2);
            Assert.IsNull(mergedRange.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(2, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.AreEqual("1.1", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsTrue(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.2", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsFalse(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);
            restriction = restrictions[1];
            Assert.AreEqual("1.3", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsFalse(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.4", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsTrue(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);

            range1 = MavenVersionRange.CreateFromVersionSpec("[1.0,1.2],[1.3,1.5]");
            range2 = MavenVersionRange.CreateFromVersionSpec("(1.1,1.4)");
            mergedRange = range1.Restrict(range2);
            Assert.IsNull(mergedRange.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(2, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.AreEqual("1.1", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsFalse(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.2", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsTrue(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);
            restriction = restrictions[1];
            Assert.AreEqual("1.3", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsTrue(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.4", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsFalse(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);

            range1 = MavenVersionRange.CreateFromVersionSpec("[1.0,1.2),(1.3,1.5]");
            range2 = MavenVersionRange.CreateFromVersionSpec("(1.1,1.4)");
            mergedRange = range1.Restrict(range2);
            Assert.IsNull(mergedRange.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(2, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.AreEqual("1.1", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsFalse(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.2", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsFalse(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);
            restriction = restrictions[1];
            Assert.AreEqual("1.3", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsFalse(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.4", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsFalse(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);

            range1 = MavenVersionRange.CreateFromVersionSpec("(,1.1),(1.4,)");
            range2 = MavenVersionRange.CreateFromVersionSpec("[1.1,1.4]");
            mergedRange = range1.Restrict(range2);
            Assert.IsNull(mergedRange.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(0, restrictions.Count, CHECK_NUM_RESTRICTIONS);

            range1 = MavenVersionRange.CreateFromVersionSpec("(,1.1],[1.4,)");
            range2 = MavenVersionRange.CreateFromVersionSpec("(1.1,1.4)");
            mergedRange = range1.Restrict(range2);
            Assert.IsNull(mergedRange.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(0, restrictions.Count, CHECK_NUM_RESTRICTIONS);

            range1 = MavenVersionRange.CreateFromVersionSpec("[,1.1],[1.4,]");
            range2 = MavenVersionRange.CreateFromVersionSpec("[1.2,1.3]");
            mergedRange = range1.Restrict(range2);
            Assert.IsNull(mergedRange.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(0, restrictions.Count, CHECK_NUM_RESTRICTIONS);

            range1 = MavenVersionRange.CreateFromVersionSpec("[1.0,1.2],[1.3,1.5]");
            range2 = MavenVersionRange.CreateFromVersionSpec("[1.1,1.4],[1.6,]");
            mergedRange = range1.Restrict(range2);
            Assert.IsNull(mergedRange.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(2, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.AreEqual("1.1", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsTrue(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.2", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsTrue(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);
            restriction = restrictions[1];
            Assert.AreEqual("1.3", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsTrue(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.4", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsTrue(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);

            range1 = MavenVersionRange.CreateFromVersionSpec("[1.0,1.2],[1.3,1.5]");
            range2 = MavenVersionRange.CreateFromVersionSpec("[1.1,1.4],[1.5,]");
            mergedRange = range1.Restrict(range2);
            Assert.IsNull(mergedRange.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(3, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.AreEqual("1.1", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsTrue(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.2", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsTrue(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);
            restriction = restrictions[1];
            Assert.AreEqual("1.3", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsTrue(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.4", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsTrue(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);
            restriction = restrictions[2];
            Assert.AreEqual("1.5", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsTrue(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.5", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsTrue(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);

            range1 = MavenVersionRange.CreateFromVersionSpec("[1.0,1.2],[1.3,1.7]");
            range2 = MavenVersionRange.CreateFromVersionSpec("[1.1,1.4],[1.5,1.6]");
            mergedRange = range1.Restrict(range2);
            Assert.IsNull(mergedRange.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(3, restrictions.Count, CHECK_NUM_RESTRICTIONS);
            restriction = restrictions[0];
            Assert.AreEqual("1.1", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsTrue(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.2", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsTrue(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);
            restriction = restrictions[1];
            Assert.AreEqual("1.3", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsTrue(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.4", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsTrue(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);
            restriction = restrictions[2];
            Assert.AreEqual("1.5", restriction.LowerBound.ToString(), CHECK_LOWER_BOUND);
            Assert.IsTrue(restriction.IsLowerBoundInclusive, CHECK_LOWER_BOUND_INCLUSIVE);
            Assert.AreEqual("1.6", restriction.UpperBound.ToString(), CHECK_UPPER_BOUND);
            Assert.IsTrue(restriction.IsUpperBoundInclusive, CHECK_UPPER_BOUND_INCLUSIVE);

            // test restricting empty sets
            range1 = MavenVersionRange.CreateFromVersionSpec("[,1.1],[1.4,]");
            range2 = MavenVersionRange.CreateFromVersionSpec("[1.2,1.3]");
            range1 = range1.Restrict(range2);
            mergedRange = range1.Restrict(range2);
            Assert.IsNull(mergedRange.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(0, restrictions.Count, CHECK_NUM_RESTRICTIONS);

            range1 = MavenVersionRange.CreateFromVersionSpec("[,1.1],[1.4,]");
            range2 = MavenVersionRange.CreateFromVersionSpec("[1.2,1.3]");
            range2 = range1.Restrict(range2);
            mergedRange = range1.Restrict(range2);
            Assert.IsNull(mergedRange.RecommendedVersion, CHECK_VERSION_RECOMMENDATION);
            restrictions = mergedRange.Restrictions;
            Assert.AreEqual(0, restrictions.Count, CHECK_NUM_RESTRICTIONS);
        }

        [Test]
        public void testReleaseRangeBoundsContainsSnapshots()
        {
            MavenVersionRange range = MavenVersionRange.CreateFromVersionSpec("[1.0,1.2]");

            Assert.IsTrue(range.ContainsVersion(new MavenVersionParser().Parse("1.1-SNAPSHOT")));
            Assert.IsTrue(range.ContainsVersion(new MavenVersionParser().Parse("1.2-SNAPSHOT")));
            Assert.IsFalse(range.ContainsVersion(new MavenVersionParser().Parse("1.0-SNAPSHOT")));
        }

        [Test]
        public void testSnapshotRangeBoundsCanContainSnapshots()
        {
            MavenVersionRange range = MavenVersionRange.CreateFromVersionSpec("[1.0,1.2-SNAPSHOT]");

            Assert.IsTrue(range.ContainsVersion(new MavenVersionParser().Parse("1.1-SNAPSHOT")));
            Assert.IsTrue(range.ContainsVersion(new MavenVersionParser().Parse("1.2-SNAPSHOT")));

            range = MavenVersionRange.CreateFromVersionSpec("[1.0-SNAPSHOT,1.2]");

            Assert.IsTrue(range.ContainsVersion(new MavenVersionParser().Parse("1.0-SNAPSHOT")));
            Assert.IsTrue(range.ContainsVersion(new MavenVersionParser().Parse("1.1-SNAPSHOT")));
        }

        [Test]
        public void testSnapshotSoftVersionCanContainSnapshot()
        {
            MavenVersionRange range = MavenVersionRange.CreateFromVersionSpec("1.0-SNAPSHOT");

            Assert.IsTrue(range.ContainsVersion(new MavenVersionParser().Parse("1.0-SNAPSHOT")));
        }

        private void checkInvalidRange(string version)
        {
            try
            {
                MavenVersionRange.CreateFromVersionSpec(version);
                Assert.Fail("Version " + version + " should have failed to construct");
            }
            catch (InvalidVersionSpecificationException)
            {
                // expected
            }
        }

        [Test]
        public void testContains()
        {
            IVersion actualVersion = new MavenVersionParser().Parse("2.0.5");
            Assert.IsTrue(enforceVersion("2.0.5", actualVersion));
            Assert.IsTrue(enforceVersion("2.0.4", actualVersion));
            Assert.IsTrue(enforceVersion("[2.0.5]", actualVersion));
            Assert.IsFalse(enforceVersion("[2.0.6,)", actualVersion));
            Assert.IsFalse(enforceVersion("[2.0.6]", actualVersion));
            Assert.IsTrue(enforceVersion("[2.0,2.1]", actualVersion));
            Assert.IsFalse(enforceVersion("[2.0,2.0.3]", actualVersion));
            Assert.IsTrue(enforceVersion("[2.0,2.0.5]", actualVersion));
            Assert.IsFalse(enforceVersion("[2.0,2.0.5)", actualVersion));
        }

        public bool enforceVersion(String requiredMavenVersionRange, IVersion actualVersion)
        {
            MavenVersionRange vr = null;

            vr = MavenVersionRange.CreateFromVersionSpec(requiredMavenVersionRange);

            return vr.ContainsVersion(actualVersion);
        }

        [Test]
        public void testOrder0()
        {
            // Assert.IsTrue( new DefaultArtifactVersion( "1.0-alpha10" ).compareTo( new DefaultArtifactVersion( "1.0-alpha1" ) ) > 0 );
        }
    }
}