using System.ServiceModel;
using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace dgSerializeBin2
{
    class Program
    {
        static void Main(string[] args)
        {
            string file = @"c:\tst\toBin.txt";
            Movies m = new Movies() { ID = 1, Name = "Parasite" };            

            IFormatter formatter = new BinaryFormatter();
            FileStream buffer = File.Create(file);
            formatter.Serialize(buffer,m);
            
            
            if (buffer.Length > 0)
            {
                Console.WriteLine("File generated sucessfully");
            }
            buffer.Close();

            Console.WriteLine("Desserializar");

            FileStream bufferD = File.OpenRead(file);
            Movies md = formatter.Deserialize(bufferD) as Movies;
            Console.WriteLine("The movie is {0}", md.Name);

            Console.ReadKey();

        }
    }
    [Serializable]
    class Movies
    {        
        public int ID { get; set; }
        public string Name { get; set; }
    }    
}
