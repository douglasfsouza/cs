using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace dgExtensions
{
    static class dgExtension
    {
        public static bool ContainNumbers(this string s)
        {
            return Regex.IsMatch(s,@"\d");
        }
    }
}
