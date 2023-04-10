using DspODataFramework.Models;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.OData;
using System.Web.Mvc;

namespace DspODataFramework.Service
{
    public class ExpenseService
    {
        public async Task<List<Expense>> GetList(string Args)
        {
            Dictionary<long, string> months = new Dictionary<long, string>()
            {
                {1, "January" }, {2,"February"}, {3,"March"}, {4,"April" }, {5,"May"}, {6, "June"},
                {7, "July" }, {8,"August"}, {9,"September"}, {10,"October" }, {11,"November"}, {12, "December"}
            };

            MongoClient cliente = new MongoClient("mongodb://127.0.0.1:27017");
            MongoServer server = cliente.GetServer();

            MongoDatabase database = server.GetDatabase("db");
            var listEx = database.GetCollection<Expense>("dsp");

            var query = from e in listEx.AsQueryable<Expense>()
                        select e;

            var result = new List<Expense>();
            foreach (var item in query)
            {                
                result.Add(new Expense
                {
                    Code = item.Id.ToString(),
                    Id = item.Id,
                    Date = item.Date,
                    Type = item.Type,
                    Value = item.Value,
                    Description = item.Description,

                    Month = item.Date.Month,
                    Year = item.Date.Year,
                    MonthDescription = months[item.Date.Month],
                    TypeDescription = item.Type == "C" ? "Crédito" : "Débito"
                });
            }

            return result;
           

            //var find = collection.Find("{'type':'C'}").ToList();
            

        }

        public async Task<Expense> Insert(Expense data)
        {
            try
            {
                MongoClient cliente = new MongoClient("mongodb://127.0.0.1:27017");
                MongoServer server = cliente.GetServer();

                MongoDatabase database = server.GetDatabase("db");
                var col = database.GetCollection<Expense>("dsp");

                data.Code = Guid.NewGuid().ToString();
                col.Insert(data);

                //Deixar o Code igual ao Id gerado

                var databaseClient = cliente.GetDatabase("db");

                string query = "{'Code' : '" + data.Code + "'}";

                var collection = databaseClient.GetCollection<BsonDocument>("dsp");

                var find = collection.Find(query).ToList();

                foreach (var item in find)
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("Code", data.Code);
                    var update = Builders<BsonDocument>.Update.Set("Code", item.ToString().Substring(20, 24));
                    collection.UpdateOne(filter, update);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao incluir dados. " + ex.Message);
            }

            return data;           
        }

        public async Task<bool> Delete(string key)
        {
            try
            {
                string connectionString = "mongodb://127.0.0.1:27017";
                MongoClient dbClient = new MongoClient(connectionString);
                var dbs = dbClient.ListDatabases().ToList();

                var database = dbClient.GetDatabase("db");
                var collection = database.GetCollection<BsonDocument>("dsp");

                string query = "{'_id' : ObjectId('" + key + "')}";

                var find = collection.Find(query).ToList();

                collection.DeleteOne(query);

                find = collection.Find(query).ToList();

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao excluir dados. " + ex.Message);
            }

            return true;

        }

        public async Task<Expense> Update(string key, Delta<Expense> delta)
        {
            Expense data = delta.GetEntity();
            try
            {
                string connectionString = "mongodb://127.0.0.1:27017";
                MongoClient dbClient = new MongoClient(connectionString);
                var dbs = dbClient.ListDatabases().ToList();

                var database = dbClient.GetDatabase("db");
                var collection = database.GetCollection<BsonDocument>("dsp");

                var filter = Builders<BsonDocument>.Filter.Eq("Code", data.Code);
                var update = Builders<BsonDocument>.Update.Set("Type", data.Type);
                collection.UpdateOne(filter, update);

                update = Builders<BsonDocument>.Update.Set("Value", data.Value);
                collection.UpdateOne(filter, update);

                update = Builders<BsonDocument>.Update.Set("Description", data.Description);
                collection.UpdateOne(filter, update);

                update = Builders<BsonDocument>.Update.Set("Date", data.Date);
                collection.UpdateOne(filter, update);

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao excluir dados. " + ex.Message);
            }

            return data;

        }

         
    }
}
