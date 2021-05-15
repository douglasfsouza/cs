using System;
using System.Collections;

namespace dgArrayList
{
    class Program
    {
        static void Main(string[] args)
        {
            Fruits f = new Fruits() { Id = 1, Name = "Apple" };
            ArrayList fruits = new ArrayList();
            fruits.Add(f);

            f = new Fruits();
            f.Id = 2;
            f.Name = "Banana";
            fruits.Add(f);

            f = new Fruits()
            {
                Id = 3,
                Name = "Strowberry"
            };
            fruits.Add(f);

            foreach (Fruits fruit in fruits)
            {
                Console.WriteLine(fruit.Name);
            }

            Fruits x = (Fruits)fruits[0];
            Console.WriteLine($"Eat an {x.Name} a day is the doctor away !!! ");
                        
        }
    }
    class Fruits
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
