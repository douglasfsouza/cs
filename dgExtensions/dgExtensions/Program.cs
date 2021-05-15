using System;

namespace dgExtensions
{
    class Program
    {
        static void Main(string[] args)
        {
            string t = "numb3r5";
            //t = "numbers";
            
            Console.WriteLine("Text: {0} Contain numbers: {1}",t,t.ContainNumbers() ? "Sim" : "Não");
        }
    }
}
