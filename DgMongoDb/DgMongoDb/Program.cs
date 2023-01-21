using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Data.Common;

namespace DgMongoDb
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "mongodb://127.0.0.1:27017";
            MongoClient dbClient = new MongoClient(connectionString);
            var dbs = dbClient.ListDatabases().ToList();

            Console.WriteLine("Listar os bancos:");

            foreach (var item in dbs)
            {
                Console.WriteLine(item);
            }

            //incluir um registro
            Console.WriteLine("Incluir um registro na collection");
            var document = new BsonDocument { { "name", "brazil" }, { "capital", "brasilia" } };

            var database = dbClient.GetDatabase("db");
            var collection = database.GetCollection<BsonDocument>("countries");

            collection.InsertOne(document);

            //ler
            Console.WriteLine("Find na collection:");
            var find = collection.Find("{'name':'brazil'}").ToList();

            foreach (var item in find)
            {
                Console.WriteLine(item);                
            }

            //alterar
            Console.WriteLine("Alerar o registro:");
            var filter = Builders<BsonDocument>.Filter.Eq("name", "brazil");
            var update = Builders<BsonDocument>.Update.Set("name", "brasil");

            collection.UpdateOne(filter, update);

            //ler
            Console.WriteLine("Find na collection:");
            find = collection.Find("{'name':'brasil'}").ToList();

            foreach (var item in find)
            {
                Console.WriteLine(item);
            }

            //delete
            Console.WriteLine("Excluir o registro");
            collection.DeleteOne("{'name':'brasil'}");
            Console.WriteLine("Excluído!!");

            //ler
            Console.WriteLine("Find na collection:");
            find = collection.Find("{'name':'brasil'}").ToList();

        }


    }
}
