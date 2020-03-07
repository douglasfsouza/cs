using System;
using System.Collections.Generic;
using System.Text;

namespace Lab5
{
    public static class Extensao
    {
        public static string PegaAString(this string recebe)
        {
            Console.WriteLine(recebe);
            return recebe.Replace("a", null).Replace("e", null).Replace("i", null).Replace("o", null).Replace("u", null);
        }
    }
}
