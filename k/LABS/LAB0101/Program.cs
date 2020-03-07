using System;
using System.Text.RegularExpressions;

namespace LAB0101
{
    class Program
    {
        static void Main(string[] args)
        {
                       
            /*
            String resultado = "A" + "B";
            string r2 = string.Concat("A", "B");
            string r3 = string.Format("{0} {1}", "A", "B");
            Console.WriteLine(resultado);
            Console.WriteLine(r2);
            Console.WriteLine(r3);

            DateTime datax = DateTime.Now;

            string stringInterpolation = $" data= {datax.ToShortDateString()} ";

            Console.WriteLine(stringInterpolation);

            string string2 = $"data={datax.ToString("dd/MM/yy hh:mm:ss")}";
            Console.WriteLine(string2);

            //operators
            int soma = 10 + 10;

            Console.WriteLine("Type a number:");
            int n1 = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Type another number:");
            int n2 = int.Parse(Console.ReadLine());

            Console.WriteLine($"The result is: {n1 + n2}");

            Console.Clear();

            Console.WriteLine("Type a number:");
            int n3 = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Type another number:");
            int n4 = int.Parse(Console.ReadLine());

            Console.WriteLine($"The multiplication is: {n3 * n4}");
            Console.WriteLine($"The division is: {n3 / n4}");

            Console.WriteLine($"Type another:");

            int n5 = Convert.ToInt32(Console.ReadLine())
              , n6 = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("{0} and {1}", n5, n6);
            
            string ax = $@"texto tesxt fd fa fsd asdf sdfasdf 
a s afsdfafas
as fas
fasdf";
Console.WriteLine(ax);

 */
            
            string result = "fadf {um} afasf {dois} ";

            MatchCollection regex = Regex.Matches(result, @"\{(\w)*\}");
            Console.WriteLine($"{regex[0]} - {regex[1]} ");

            for (int i = 0; i < 30; i++)
            {
                Console.WriteLine(i);

            }



            Console.ReadKey();
{}
        }
    }
}
