using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConsumirTodoApi
{
    class Program
    {
        static HttpClient c = new HttpClient();
        static void ListItem( Item i)
        {
            Console.WriteLine($"Id:{i.id} - Name:{i.name} - Cadastro completo: {i.isComplete}");
        }
        static async Task<List<Item>> GetItem(string path)
        {            
            string x;
            HttpResponseMessage r = await c.GetAsync(path);
            if (r.IsSuccessStatusCode)
            {

                x = await r.Content.ReadAsStringAsync();
             
                List<Item> i = JsonConvert.DeserializeObject<List<Item>>(x);
               
                return i;
            }
            else
                return null;            
        }
        static async Task<string> PostURI(Uri u, HttpContent hc)
        {
            var response = string.Empty;
            using (var client = new HttpClient())
            {
                HttpResponseMessage result = await client.PostAsync(u, hc);
                if (result.IsSuccessStatusCode)
                {
                    response = result.StatusCode.ToString();
                }
            }
            return response;
        }
        static async Task<string> CreateItem(Uri u2, HttpContent hc)
        {
            /*var response = string.Empty;
            using (var cc = new HttpClient())
            {
                HttpResponseMessage r = await cc.PostAsync(u2, hc);
                if (r.IsSuccessStatusCode)
                {
                    return r.Headers.Location.ToString();                  
                }
                else
                {
                    return null;
                }
            }
            return response;
            */
            
            
            HttpResponseMessage r = await c.PostAsync(u2, hc);
            if (r.IsSuccessStatusCode)
            {
                return r.Headers.Location.ToString();
            }
            else
            {
                return null;
            }
            
            
        }
        static async Task<string> UpdateItem(Uri u2, HttpContent hc)
        {
            HttpResponseMessage r = await c.PutAsync(u2, hc);
            if (r.IsSuccessStatusCode)
            {
                return "Updated";
            }
            else
            {
                return null;
            }
        }
        static async Task<string> DeleteItem(string u2, HttpContent hc)
        {
            HttpResponseMessage r = await c.DeleteAsync(u2,System.Threading.CancellationToken.None);
            if (r.IsSuccessStatusCode)
            {
                return "Deleted";
            }
            else
            {
                return null;
            }
        }

        static async void Listar(string url)
        {
            List<Item> ii = new List<Item>();
            ii = await GetItem(url);
            foreach (var item in ii)
            {
                ListItem(item);
            }
        }
        static async Task RunAsync()
        {
            //Se der erro precisa reiniciar o servico da api
            string url = "https://localhost:44365/api/TodoItems";
            c.BaseAddress = new Uri(url);
            c.DefaultRequestHeaders.Accept.Clear();
            c.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                Item i = new Item
                {
                    id = 100,
                    name = "Consuming-ADD *************",
                    isComplete = true
                };
                
                ///
                string payload = JsonConvert.SerializeObject(i);

                Uri ux = new Uri(url);
                //var payload = "{\"id\": 100,\"name\": \"ConsumingADD\",\"isComplete\":true}";
                HttpContent hc = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");
                //Tambem funciona assim:
                //var t = Task.Run(() => PostURI(ux, hc));
                //Console.WriteLine($"Posturi retornou : {t.Result}");
                ///              


                //var uu = Task.Run(() => CreateItem(ux, hc));
                //Console.WriteLine($"CreateItem retornou : {uu.Result}");
                var p = await CreateItem(ux, hc);
                Console.WriteLine($"CreateItem retornou : {p}");

                
                List<Item> ii = new List<Item>();
                ii = await GetItem(url);
                foreach (var item in ii)
                {               
                    ListItem(item);
                }

                //update
                Uri uxupd = new Uri($"{url}/{100}");
                i.name = "Consuming-UPD *************";
                var payloadupd = JsonConvert.SerializeObject(i);
                //var payloadupd = "{\"id\": 100,\"name\": \"ConsumingUpd\",\"isComplete\":true}";
                HttpContent hcupd = new StringContent(payloadupd, System.Text.Encoding.UTF8, "application/json");

                var up = await UpdateItem(uxupd, hcupd);
                Console.WriteLine($"Update retornou : {up}");
                Listar(url);


                //delete               
                // var payloaddel = "{\"id\": 100,\"name\": \"ConsumingUpd\",\"isComplete\":true}";
                var payloaddel = payloadupd;
                HttpContent hcdel = new StringContent(payloaddel, System.Text.Encoding.UTF8, "application/json");
                string uxdel = $"{url}/{100}";
                var udel = await DeleteItem(uxdel, hcdel);
                Console.WriteLine($"Delete retornou : {udel}");
                Listar(url);
                Console.Read();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro:{ex.Message}");
            }
        }
        static void Main(string[] args)
        {
            RunAsync().GetAwaiter().GetResult();
        }
    }
    public class Item
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool isComplete { get; set; }
    }
}
