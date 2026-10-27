using DummyJson.ApiTests.Models;

namespace DummyJson.ApiTests.Clients;

public class AuthClient
{
    private readonly ApiClient _api;

    public AuthClient(ApiClient api)
    {
        _api = api;
    }

    public Task<ApiResponse<LoginResponse>> LoginAsync(string username, string password)
        => _api.PostAsync<LoginResponse>("auth/login", new LoginRequest { Username = username, Password = password });

    public Task<ApiResponse<UserProfile>> GetCurrentUserAsync(string token)
        => _api.GetAsync<UserProfile>("auth/me", token);
}
