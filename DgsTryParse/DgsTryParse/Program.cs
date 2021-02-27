using System;

namespace DgsTryParse
{
    class Program
    {
        static void Main(string[] args)
        {
            int a = 0;
            string b = "x1234";
            if (int.TryParse(b, out a))
            {
                Console.WriteLine($"Parse ok: {a} ");
            }
            else
            {
                Console.WriteLine($"Fail to Parse");
            }
            
        }
    }
}
