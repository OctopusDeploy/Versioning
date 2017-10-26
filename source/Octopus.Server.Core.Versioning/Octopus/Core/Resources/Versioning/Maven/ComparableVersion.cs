using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Octopus.Core.Resources.Versioning.Maven
{
    /// <summary>
    /// This class is based on the org.apache.maven.artifact.versioning.ComparableVersion
    /// class from the Java org.apachce.maven:maven-artifact maven package.
    /// 
    /// It is, as much as possible, a one-to-one port of the Java class.
    /// </summary>
    public class ComparableVersion : IComparable<ComparableVersion>
    {
        const int INTEGER_ITEM = 0;
        const int STRING_ITEM = 1;
        const int LIST_ITEM = 2;
        
        string value;
        string canonical;
        ListItem items;

        public ComparableVersion(string version)
        {
            ParseVersion(version);
        }

        interface Item
        {
            int CompareTo(Item item);

            int GetItemType();

            bool IsNull();
        }

        /**
         * Represents a numeric item in the version item list.
         */
        class IntegerItem : Item
        {
            static readonly BigInteger BigInteger_ZERO = new BigInteger(0);

            readonly BigInteger value;

            public static readonly IntegerItem ZERO = new IntegerItem();

            IntegerItem()
            {
                value = BigInteger_ZERO;
            }

            public IntegerItem(string str)
            {
                value = BigInteger.Parse(str);
            }

            public int GetItemType()
            {
                return INTEGER_ITEM;
            }

            public bool IsNull()
            {
                return BigInteger_ZERO.Equals(value);
            }

            public int CompareTo(Item item)
            {
                if (item == null)
                {
                    return BigInteger_ZERO.Equals(value) ? 0 : 1; // 1.0 == 1, 1.1 > 1
                }

                switch (item.GetItemType())
                {
                    case INTEGER_ITEM:
                        return value.CompareTo(((IntegerItem) item).value);

                    case STRING_ITEM:
                        return 1; // 1.1 > 1-sp

                    case LIST_ITEM:
                        return 1; // 1.1 > 1-1

                    default:
                        throw new Exception("invalid item: " + item.GetType());
                }
            }

            public override string ToString()
            {
                return value.ToString();
            }
        }

        /**
         * Represents a string in the version item list, usually a qualifier.
         */
        class StringItem : Item
        {
            static readonly String[] QUALIFIERS = {"alpha", "beta", "milestone", "rc", "snapshot", "", "sp"};

            static readonly IList<String> _QUALIFIERS = QUALIFIERS.ToList();

            static readonly Dictionary<string, string> ALIASES = new Dictionary<string, string>()
            {
                {"ga", ""},
                {"final", ""},
                {"cr", "rc"}
            };

            /**
             * A comparable value for the empty-string qualifier. This one is used to determine if a given qualifier makes
             * the version older than one without a qualifier, or more recent.
             */
            static readonly String RELEASE_VERSION_INDEX = _QUALIFIERS.IndexOf("").ToString();

            string value;

            public StringItem(string value, bool followedByDigit)
            {
                if (followedByDigit && value.Length == 1)
                {
                    // a1 = alpha-1, b1 = beta-1, m1 = milestone-1
                    switch (value[0])
                    {
                        case 'a':
                            value = "alpha";
                            break;
                        case 'b':
                            value = "beta";
                            break;
                        case 'm':
                            value = "milestone";
                            break;
                    }
                }
                this.value = ALIASES.ContainsKey(value) ? ALIASES[value] : value;
            }

            public int GetItemType()
            {
                return STRING_ITEM;
            }

            public bool IsNull()
            {
                return (ComparableQualifier(value).CompareTo(RELEASE_VERSION_INDEX) == 0);
            }

            /**
             * Returns a comparable value for a qualifier.
             *
             * This method both takes into account the ordering of known qualifiers as well as lexical ordering for unknown
             * qualifiers.
             *
             * just returning an Integer with the index here is faster, but requires a lot of if/then/else to check for -1
             * or QUALIFIERS.size and then resort to lexical ordering. Most comparisons are decided by the first character,
             * so this is still fast. If more characters are needed then it requires a lexical sort anyway.
             *
             * @param qualifier
             * @return an equivalent value that can be used with lexical comparison
             */
            public static string ComparableQualifier(string qualifier)
            {
                int i = _QUALIFIERS.IndexOf(qualifier);

                return i == -1 ? _QUALIFIERS.Count + "-" + qualifier : i.ToString();
            }

            public int CompareTo(Item item)
            {
                if (item == null)
                {
                    // 1-rc < 1, 1-ga > 1
                    return ComparableQualifier(value).CompareTo(RELEASE_VERSION_INDEX);
                }
                switch (item.GetItemType())
                {
                    case INTEGER_ITEM:
                        return -1; // 1.any < 1.1 ?

                    case STRING_ITEM:
                        return ComparableQualifier(value).CompareTo(ComparableQualifier(((StringItem) item).value));

                    case LIST_ITEM:
                        return -1; // 1.any < 1-1

                    default:
                        throw new Exception("invalid item: " + item.GetType());
                }
            }

            public override string ToString()
            {
                return value;
            }
        }

        /**
         * Represents a version list item. This class is used both for the global item list and for sub-lists (which start
         * with '-(number)' in the version specification).
         */
        class ListItem : List<Item>, Item
        {
            public int GetItemType()
            {
                return LIST_ITEM;
            }

            public bool IsNull()
            {
                return (Count == 0);
            }

            public void Normalize()
            {
                while (Count > 0 && this[Count - 1].IsNull())
                {
                    RemoveAt(Count - 1); // remove null trailing items: 0, "", empty list
                }               
            }

            public int CompareTo(Item item)
            {
                if (item == null)
                {
                    if (Count == 0)
                    {
                        return 0; // 1-0 = 1- (normalize) = 1
                    }
                    Item first = this[0];
                    return first.CompareTo(null);
                }
                switch (item.GetItemType())
                {
                    case INTEGER_ITEM:
                        return -1; // 1-1 < 1.0.x

                    case STRING_ITEM:
                        return 1; // 1-1 > 1-sp

                    case LIST_ITEM:
                        ListItem listItem = (ListItem) item;
                        for (int index = 0; index < Math.Max(Count, listItem.Count); ++index)
                        {
                            Item l = index < Count ? this[index] : null;
                            Item r = index < listItem.Count ? listItem[index] : null;

                            // if this is shorter, then invert the compare and mul with -1
                            int result = l == null ? -1 * r.CompareTo(l) : l.CompareTo(r);

                            if (result != 0)
                            {
                                return result;
                            }
                        }                        

                        return 0;

                    default:
                        throw new Exception("invalid item: " + item.GetType());
                }
            }

            public override string ToString()
            {
                return "(" + string.Join(",", this) + ")";
            }
        }
      
        void ParseVersion(string version)
        {
            value = version;

            items = new ListItem();

            version = version.ToLower();

            ListItem list = items;

            Stack<Item> stack = new Stack<Item>();
            stack.Push(list);

            bool isDigit = false;

            int startIndex = 0;

            for (int i = 0; i < version.Length; i++)
            {
                char c = version[i];

                if (c == '.')
                {
                    if (i == startIndex)
                    {
                        list.Add(IntegerItem.ZERO);
                    }
                    else
                    {
                        list.Add(ParseItem(isDigit, version.Substring(startIndex, i - startIndex)));
                    }
                    startIndex = i + 1;
                }
                else if (c == '-')
                {
                    if (i == startIndex)
                    {
                        list.Add(IntegerItem.ZERO);
                    }
                    else
                    {
                        list.Add(ParseItem(isDigit, version.Substring(startIndex, i - startIndex)));
                    }
                    startIndex = i + 1;

                    if (isDigit)
                    {
                        list.Normalize(); // 1.0-* = 1-*

                        if ((i + 1 < version.Length) && char.IsDigit(version[i + 1]))
                        {
                            // new ListItem only if previous were digits and new char is a digit,
                            // ie need to differentiate only 1.1 from 1-1
                            list.Add(list = new ListItem());

                            stack.Push(list);
                        }
                    }
                }
                else if (char.IsDigit(c))
                {
                    if (!isDigit && i > startIndex)
                    {
                        list.Add(new StringItem(version.Substring(startIndex, i - startIndex), true));
                        startIndex = i;
                    }

                    isDigit = true;
                }
                else
                {
                    if (isDigit && i > startIndex)
                    {
                        list.Add(ParseItem(true, version.Substring(startIndex, i - startIndex)));
                        startIndex = i;
                    }

                    isDigit = false;
                }
            }

            if (version.Length > startIndex)
            {
                list.Add(ParseItem(isDigit, version.Substring(startIndex)));
            }

            while (stack.Count != 0)
            {
                list = (ListItem) stack.Pop();
                list.Normalize();
            }

            canonical = items.ToString();
        }

        static Item ParseItem(bool isDigit, string buf)
        {
            if(isDigit) return new IntegerItem(buf);
            return new StringItem(buf, false);
        }

        public int CompareTo(ComparableVersion o)
        {
            return items.CompareTo(o.items);
        }

        public override string ToString()
        {
            return value;
        }

        public override bool Equals(Object o)
        {
            return (o is ComparableVersion comparer ) && canonical.Equals(comparer.canonical);
        }

        public override int GetHashCode()
        {
            return canonical.GetHashCode();
        }
    }
}