using System.Net;
using DummyJson.ApiTests.Clients;
using DummyJson.ApiTests.Models;

namespace DummyJson.ApiTests.Support;

/// <>
/// Reqnroll creates a fresh instance per scenario, scenarios never share data.
/// <remarks>
public class ScenarioState
{
    public HttpStatusCode LastStatusCode { get; set; }
    public string LastRawBody { get; set; } = "";

    // Auth
    public string? AccessToken { get; set; }
    public UserProfile? CurrentUser { get; set; }

    // Users
    public UserList? UserSearch { get; set; }
    public User? UserToAdd { get; set; }
    public User? AddedUser { get; set; }

    // Products
    public ProductList? ProductList { get; set; }
    public Product? Product { get; set; }

    // Carts
    public int ShoppingUserId { get; set; } = 1;
    public List<CartItem> RequestedItems { get; set; } = new();
    public Cart? Cart { get; set; }

    /// <summary>
    /// Remembers the status code and raw body of a response, and hands back the parsed body (i will verify all again ).
    /// </summary>
    public T? Record<T>(ApiResponse<T> response)
    {
        LastStatusCode = response.StatusCode;
        LastRawBody = response.RawBody;
        return response.Body;
    }
}
