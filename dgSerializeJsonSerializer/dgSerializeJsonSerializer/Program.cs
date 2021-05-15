using System;
using System.IO;
using System.Text.Json;

namespace dgSerializeJsonSerializer
{
    class Program
    {
        static void Main(string[] args)
        {
            Movies m = new Movies() { ID = 1, MovieName = "Doom" };

            Console.WriteLine("Serializar:");

            string json = JsonSerializer.Serialize(m);
            Console.WriteLine(json);

            Console.WriteLine("Deserializar:");

            Movies md = JsonSerializer.Deserialize<Movies>(json);
            Console.WriteLine("id:{0}, name:{1}", md.ID, md.MovieName);

            Console.ReadLine();       

           
        }
    }
    [Serializable]
    class Movies
    {
        public int ID { get; set; }
        public string MovieName { get; set; }
    }
}
