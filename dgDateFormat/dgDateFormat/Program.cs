using System;

namespace dgDateFormat
{
    class Program
    {
        static void Main(string[] args)
        {
            string strdt1 = DateTime.Now.ToString("dd-MM-yyyy");
            DateTime dt2 = DateTime.Now;
            string strdt2 = $"{dt2:dd-MM-yyyy}";

            DateTime dt3 = DateTime.ParseExact("05/01/1977", "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            string strdt4 = string.Format("{0:dd-MM-yyyy}", DateTime.Now);
            
            Console.WriteLine(strdt1);
            Console.WriteLine(strdt2);
            Console.WriteLine($"{dt3:dd-MM-yyyy}");
            Console.WriteLine(strdt4);

        }
    }
}
