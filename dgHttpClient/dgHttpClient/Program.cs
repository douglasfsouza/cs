string url = "https://pje.jfce.jus.br/pje/ConsultaPublica/listView.seam";

HttpClient httpClient = new HttpClient
{
    BaseAddress = new Uri(url)
};

//ou

HttpClient httpClient2 = new HttpClient();

GetAsync(httpClient2,url).Wait();    
static async Task GetAsync(HttpClient httpClient, string url)
{
    using HttpResponseMessage response = await httpClient.GetAsync(url);

    response.EnsureSuccessStatusCode();
    
    var jsonResponse = await response.Content.ReadAsStringAsync();

    Console.WriteLine(jsonResponse);

}
