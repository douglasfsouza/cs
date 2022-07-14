using GTIAspMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GTIAspMVC.Services
{
    public class GTIClienteService
    {
        const string _baseURL = "http://localhost:21473/api";
        public async Task<List<GTICliente>> GetClients()
        {
            HttpClient apiClient = new HttpClient();
            string result = await apiClient.GetStringAsync($"{_baseURL}/Clientes");
            List<GTICliente> clients = JsonConvert.DeserializeObject<List<GTICliente>>(result);

            return clients;
        }

        public async Task<GTICliente> GetClientById(int id)
        {
            HttpClient apiClient = new HttpClient();
            string result = await apiClient.GetStringAsync($"{_baseURL}/Clientes/{id}");
            GTICliente client = JsonConvert.DeserializeObject<GTICliente>(result);

            return client;
        }

        private static async Task<List<GTICliente>> GetClientsRest()
        {
            var apiClient = new RestClient($"{_baseURL}/Clientes");
            var request = new RestRequest($"{_baseURL}/Clientes", Method.Get);
            var response = apiClient.Execute(request);

            List<GTICliente> clients = JsonConvert.DeserializeObject<List<GTICliente>>(response.Content);

            return clients;
        }

        public async Task<RestResponse> Adicionar(GTICliente cliente)
        {
            var apiClient = new RestClient($"{_baseURL}/Clientes");
            var request = new RestRequest($"{_baseURL}/Clientes", Method.Post);

            string json = JsonConvert.SerializeObject(cliente);

            request.AddJsonBody(json);

            var response = await apiClient.ExecuteAsync(request);

            return response;           
        }

        public async Task<RestResponse> Atualizar(GTICliente cliente)
        {
            var apiClient = new RestClient($"{_baseURL}/Clientes/{cliente.Id}");
            var request = new RestRequest($"{_baseURL}/Clientes/{cliente.Id}", Method.Put);

            string json = JsonConvert.SerializeObject(cliente);

            request.AddJsonBody(json);

            var response = await apiClient.ExecuteAsync(request);

            return response;            
        }

        public async Task Excluir(int id)
        {
            var apiClient = new RestClient($"{_baseURL}/Clientes/{id}");
            var request = new RestRequest($"{_baseURL}/Clientes/{id}", Method.Delete);

            var response = apiClient.Execute(request);

            return;             
        }

    }
}
