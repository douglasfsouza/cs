using System;
using System.Collections;

namespace dgHashtable
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hashtable is a Dictionary collection, use a pair: key and value!");

            Hashtable ingredients = new Hashtable();
            ingredients.Add("Café au lait", "coffee and milk");
            ingredients.Add("Mocha", "coffee, milk and chocolate");
            ingredients.Add("Capuccino", "coffee,milk and foam");
            ingredients.Add("Macchiato", "coffee,milk and foam");
            ingredients.Add("Irish Coffee", "coffee, whiskey, cream and foam");

            ingredients.Add(1, "teste"); //aceita qquer tipo de dados diferentemente de dictionary

            foreach(var i in ingredients.Keys)
            {
                Console.WriteLine($"{i} is: {ingredients[i]}");
            }

            Console.WriteLine($"Capuccino is: {ingredients["Capuccino"]} AND NOT coffee, milk and chocolate ");

            Console.WriteLine("Há {0} ingredientes na lista", ingredients.Count);
            ingredients.Remove(1);
            Console.WriteLine("Removido item 1:");
            foreach (var i in ingredients.Values)
            {
                Console.WriteLine(i);
            }         

        }
    }
}
