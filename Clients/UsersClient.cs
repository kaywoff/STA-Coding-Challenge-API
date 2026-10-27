using DummyJson.ApiTests.Models;

namespace DummyJson.ApiTests.Clients;

public class UsersClient
{
    private readonly ApiClient _api;

    public UsersClient(ApiClient api)
    {
        _api = api;
    }

    // limit=0 asks for everything, otherwise the API only returns the first 30 matches.
    public Task<ApiResponse<UserList>> FilterByHairColourAsync(string colour)
        => _api.GetAsync<UserList>($"users/filter?key=hair.color&value={Uri.EscapeDataString(colour)}&limit=0");

    public Task<ApiResponse<UserList>> GetAllAsync()
        => _api.GetAsync<UserList>("users?limit=0");

    public Task<ApiResponse<User>> AddAsync(User user)
        => _api.PostAsync<User>("users/add", user);
}
