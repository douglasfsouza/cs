using System;
using System.Diagnostics;

namespace DgDebugAssert
{
    class Program
    {
        static void Main(string[] args)
        {
            Debug.Assert(true, "Deu certo");
            //Debug.Assert(false, "Deu erro");

            string userInput;
            int number;
            Console.WriteLine("Type a number:");
            userInput = Console.ReadLine();
            Debug.Assert(int.TryParse(userInput, out number), string.Format("Não foi possivel converter {0} para Int", userInput));

            Console.WriteLine(string.Format("You typed {0}", number));
        }
    }
}
