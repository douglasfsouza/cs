using IntegracaoMongoDsp;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Text.Json;

const string _baseURL = "http://localhost/api/Despesas";
List<Despesas> result = new List<Despesas>();

HttpClient apiClient = new HttpClient();
string sresult = await apiClient.GetStringAsync($"{_baseURL}");

var resultd = JsonConvert.DeserializeObject<List<DespesasDgs>>(sresult);
long cont = 0;

foreach (var item in resultd)
{ 
    Expense e = new Expense()
    {
        Date = Convert.ToDateTime(item.Data.ToString("yyyy-MM-dd")),
        Description = item.Descricao,
        Type = item.Tipo.ToUpper(),
        Value = (decimal)item.Valor
    };

    Insert(e);

    cont++;
}

Console.WriteLine($"{cont} lines added");
Console.ReadLine();


void Insert(Expense data)
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

   
}