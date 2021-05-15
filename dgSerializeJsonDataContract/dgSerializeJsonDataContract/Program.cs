using System;
using System.IO;
using System.Runtime.Serialization.Json;

namespace dgSerializeJsonDataContract
{
    class Program
    {
        static void Main(string[] args)
        {
            string f = @"c:\tst\filejson.txt";
            Movies m = new Movies() { ID = 1, MovieName = "Matrix" };
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Movies));
            FileStream buffer = File.Create(f);
            jsonSerializer.WriteObject(buffer, m);
            if  (buffer.Length > 0)
                Console.WriteLine("{0} created",f);
            
            buffer.Close();
            buffer.Dispose();

            Console.WriteLine("Deserializar:");

            FileStream bufferD = File.OpenRead(f);
            Movies md = jsonSerializer.ReadObject(bufferD) as Movies;
            Console.WriteLine("MovieName:{0}", md.MovieName);
            bufferD.Close();

            Console.ReadKey();

        }
    }
    [Serializable]
    class Movies
    {
        public int ID { get; set; }
        public string MovieName { get; set; }
    }
}
