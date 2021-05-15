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

            DateTime dt = DateTime.Today;

            dt = Convert.ToDateTime("02-01-2020 00:00");

            Console.WriteLine(dt.AddDays(1));

            int y = 5;
            string s = y.ToString().PadLeft(7, '0');
            Console.WriteLine(s);


            Dictionary<int, string > sem = new Dictionary<int, string > ();
            sem.Add(1, "domingo");
            sem.Add(2, "segunda");
            sem.Add(3, "terça");
            //dia não cadastrado
            //sem.Add(4, "quarta");
            sem.Add(5,"quinta");
            sem.Add(6, "sexta");
            sem.Add(7, "sabado");

            List<string> seml = new List<string>();
            seml.Add("Dom");
            seml.Add("Seg");
            if (seml.Contains("Dom"))
            {
                Console.WriteLine("Tem domingo na lista");
            }


          

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
            {
                if (sem.TryGetValue(n, out string dia))
                {
                    //Console.WriteLine($"Voce informou {n} - {sem[n].ToString()}");
                    Console.WriteLine($"Voce informou {n} - {dia}");
                }
                else
                    Console.WriteLine("Dia {0} não cadastrado",n);
            }

            Console.ReadKey();


        }
    }
}
