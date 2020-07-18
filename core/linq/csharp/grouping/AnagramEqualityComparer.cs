using System;
using System.Collections.Generic;

namespace Grouping
{
    //Comparer that matches words that are anagrams of each other.
    public class AnagramEqualityComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return GetCanonicalString(x) == GetCanonicalString(y);
        }

        public int GetHashCode(string obj)
        {
            return GetCanonicalString(obj).GetHashCode();
        }

        private string GetCanonicalString(string word)
        {
            char[] wordChars = word.ToCharArray();
            Array.Sort<char>(wordChars);
            return new string(wordChars);
        }
    }
}
