using System;
using System.Linq;

namespace LibOpenNFS.Utils
{
    public static class StringExtensions
    {
        public static string Repeat(this string s, int n)
        {
            return new string(Enumerable.Range(0, n).SelectMany(x => s).ToArray());
        }

        public static string Repeat(this char c, int n)
        {
            return new string(c, n);
        }
    }
}