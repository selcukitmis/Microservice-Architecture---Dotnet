using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Play.Inventory.Service.Dtos;

namespace Play.Inventory.Service.Clients
{
    public class CatalogClient
    {
        public CatalogClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        private readonly HttpClient httpClient;

        public async Task<IReadOnlyCollection<CatalogItemDto>> GetCatalogItemAsync()
        {
            var response = await httpClient.GetAsync($"/items/");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IReadOnlyCollection<CatalogItemDto>>();
        }
    }
}