using DummyJson.ApiTests.Models;

namespace DummyJson.ApiTests.Clients;

public class ProductsClient
{
    private readonly ApiClient _api;

    public ProductsClient(ApiClient api)
    {
        _api = api;
    }

    public Task<ApiResponse<ProductList>> SearchAsync(string term)
        => _api.GetAsync<ProductList>($"products/search?q={Uri.EscapeDataString(term)}");

    public Task<ApiResponse<Product>> GetByIdAsync(int id)
        => _api.GetAsync<Product>($"products/{id}");

    public Task<ApiResponse<ProductList>> GetPageAsync(int limit, int skip, string fields)
        => _api.GetAsync<ProductList>($"products?limit={limit}&skip={skip}&select={fields}");

    public Task<ApiResponse<Product>> UpdateTitleAsync(int id, string newTitle)
        => _api.PutAsync<Product>($"products/{id}", new { title = newTitle });
}
