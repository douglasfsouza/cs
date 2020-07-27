using System;
using System.Collections.Generic;

namespace dgLambda
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Cars> cars = new List<Cars>();
            Cars car1 = new Cars() { Manufacturer = "Fiat", Model = "Prisma" };
            Cars car2 = new Cars() { Manufacturer = "VW", Model = "Fusca" };
            cars.Add(car1);
            cars.Add(car2);
            
            var vw = cars.Find(c => c.Manufacturer == "VW");
            if (vw != null)
            {
                Console.WriteLine(vw.Model);
            }


        }
    }
}
