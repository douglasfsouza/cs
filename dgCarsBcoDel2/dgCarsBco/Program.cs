using System;
using System.ComponentModel.DataAnnotations;

namespace dgCarsBco
{
    class Program
    {
        static void Main(string[] args)
        {
            Cars c = new Cars();
            CarsContext carsContext = new CarsContext();
            /*
             using (CarsContext d = new CarsContext())
             {
                 d.cars.Add(new Cars() { ID = 1, Model = "Palio", Manufacturer = "Fiat" });
                 d.SaveChanges();   
                 Console.WriteLine("Saved!");

             }
             */

            Console.WriteLine("Excluindo...");
            int id = 3;
            var f = carsContext.cars.Find(id);
            if (f != null)
            {
                carsContext.cars.Remove(f);
                carsContext.SaveChanges();
                Console.WriteLine($"Id:{id} Excluido");
            }
            else
            {
                Console.WriteLine($"Id:{id} não encontrado");
            }
            Console.ReadLine();
        }
    }
}
