using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgCodeFirst
{
    class Program
    {
        static void Main(string[] args)
        {
            using (dgContexto c = new dgContexto())
            {
                c.Clientes.Add(new dgCodeFirst.Cliente()
                {
                    Nome = "douglas",
                    Idade = 43
                });
                c.SaveChanges();
            };

            Console.WriteLine("Cliente adicionado");
            Console.ReadKey();
        }
    }
}
