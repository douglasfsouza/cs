using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

namespace dgSerializeJsonConvert
{
    class Program
    {
        static void Main(string[] args)
        {
            Movies m = new Movies() { ID = 1, MovieName = "Titanic" };
            string json = JsonConvert.SerializeObject(m);

            Console.WriteLine("Json:");
            Console.WriteLine(json);
            Console.ReadLine();
            Console.WriteLine("Deserializar:");

            Movies md = JsonConvert.DeserializeObject<Movies>(json);
            Console.WriteLine("Id:{0}, Name:{1}", md.ID, md.MovieName);

            Console.ReadLine();

        }
    }
    [Serializable]
    class Movies
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }
        [JsonPropertyName("movieName")]
        [JsonProperty("movieName")]
        public string MovieName { get; set; }
    }
}
