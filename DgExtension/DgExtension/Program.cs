using System;

namespace DgExtension
{
    static class Program
    {
        static void Main(string[] args)
        {
            string t = "Sequenc1a com numer0s ?";
            string t2 = t.ContemNum() ? "R:Contem Numeros" : "R:Não Contem numeros";
            Console.WriteLine($"{t}\n {t2}" );
            Console.Read();
        }

        static bool ContemNum(this string s)
        {
            bool bn = false;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '0' || s[i] == '1' || s[i] == '2' || s[i] == '3' || s[i] == '4' || s[i] == '5' || s[i] == '6' || s[i] == '7' || s[i] == '8' || s[i] == '9')
                    bn = true;

            }
            return bn;
        }
    }
}
