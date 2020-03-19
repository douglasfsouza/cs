using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgDict
{
    class Program
    {
        static void Main(string[] args)
        {
           Dictionary<int, string > sem = new Dictionary<int, string > ();
            sem.Add(1, "domingo");
            sem.Add(2, "segunda");
            sem.Add(3, "terça");
            sem.Add(4, "quarta");
            sem.Add(5,"quinta");
            sem.Add(6, "sexta");
            sem.Add(7, "sabado");

            foreach(var i in sem)
            {
                Console.WriteLine ( i.Value.ToString());
            }
            Console.WriteLine($"{sem.Count() } dias");

            Console.WriteLine("Informe um nro de 1 a 7");
            int n = int.Parse(Console.ReadLine());

            if (n < 1 || n > 7)
                Console.WriteLine("Valor invalido");
            else
                Console.WriteLine($"Voce informou {n} - {sem[n].ToString()}");

            Console.ReadKey();


        }
    }
}
