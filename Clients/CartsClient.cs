using DummyJson.ApiTests.Models;

namespace DummyJson.ApiTests.Clients;

public class CartsClient
{
    private readonly ApiClient _api;

    public CartsClient(ApiClient api)
    {
        _api = api;
    }

    public Task<ApiResponse<Cart>> AddAsync(AddCartRequest request)
        => _api.PostAsync<Cart>("carts/add", request);

    public Task<ApiResponse<Cart>> DeleteAsync(int cartId)
        => _api.DeleteAsync<Cart>($"carts/{cartId}");
}
