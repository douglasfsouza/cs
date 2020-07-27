using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CarsBco2
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            using (CarsContext c = new CarsContext())
            {
                c.Cars.Add( new Cars() { ID = 1, Manufacturer = "Fiat", Model = "Prisma" });
                c.SaveChanges();
            }
            
            Console.WriteLine("Carro adicionado");
            */
            int id = 4;
            Console.WriteLine("Excluindo..");
            CarsContext cc = new CarsContext();
            var f = cc.Cars.Find(id);            

            if (f != null)
            {
                cc.Cars.Remove(cc.Cars.Single(a => a.ID == id));
                cc.SaveChanges();
                Console.WriteLine($"Id:{id} Excluido com sucesso");
            }
            else
            {
                Console.WriteLine($"Id:{id} não encontrado");
            }
            Console.Read();
        }
    }
}
