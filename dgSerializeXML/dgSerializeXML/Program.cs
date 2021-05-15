using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Soap;
using System.Security.Principal;

namespace dgSerializeXML
{
    class Program
    {
        static void Main(string[] args)
        {
            Movies m = new Movies() { ID = 1, MovieName = "Hell" };
            string file = @"c:\tst\file.xml";
            FileStream buffer = File.Create(file);
            IFormatter formatter = new SoapFormatter();
            formatter.Serialize(buffer, m);
            if (file.Length > 0)
            {
                Console.WriteLine("File.xml generated succefully");
            }
            buffer.Close();

            Console.WriteLine("Deserializar:");

            FileStream bufferD = File.OpenRead(file);
            Movies md = formatter.Deserialize(bufferD) as Movies;
            Console.WriteLine("Id: {0}, Moviename: {1}", md.ID, md.MovieName);

            Console.ReadLine();
            
        }

    }
    [Serializable]
    class Movies
    {
        public int ID { get; set; }
        public string MovieName  { get; set; }
    }
}

