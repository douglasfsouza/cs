using System;
using System.Threading.Tasks;

namespace dgTask13_lock
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Coffee coffee = new Coffee(1000);
                //coffee.MakeCoffees(2);
                //coffee.MakeCoffees(1);
                Random r = new Random();
                Parallel.For(0, 100, index =>
                 {
                     coffee.MakeCoffees(r.Next(1, 100));
                 });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error do make coffees. Error: {ex.Message}");

                //throw;
            }
            
        }
    }
}
