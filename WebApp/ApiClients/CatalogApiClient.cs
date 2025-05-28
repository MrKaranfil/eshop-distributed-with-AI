using Catalog.Models;

namespace WebApp.ApiClients;

public class CatalogApiClient(HttpClient httpClient)
{
    public async Task<List<Product>> GetProducts()
    {
        var res = await httpClient.GetFromJsonAsync<List<Product>>($"/products");
        return res;
    }

    public async Task<Product> GetProductById(int id)
    {
        var res = await httpClient.GetFromJsonAsync<Product>($"/products/{id}");
        return res!;
    }

    public async Task<string> SupportProducts(string query)
    {
        var res = await httpClient.GetFromJsonAsync<string>($"/products/support/{query}");
        return res!;
    }

    public async Task<List<Product>> SearchProduct(string query,bool aiSearch)
    {
        if (aiSearch)
        {
            return await httpClient.GetFromJsonAsync<List<Product>>($"/products/aisearch/{query}");
        }
        else
        {
            return await httpClient.GetFromJsonAsync<List<Product>>($"/products/search/{query}");
        }
    }
}
