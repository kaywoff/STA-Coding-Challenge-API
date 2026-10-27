using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DummyJson.ApiTests.Config;
using Reqnroll;

namespace DummyJson.ApiTests.Clients;

public class ApiResponse<T>
{
    public HttpStatusCode StatusCode { get; init; }
    public string RawBody { get; init; } = "";
    public T? Body { get; init; }
}

/// <summary>
/// The only place that knows how to talk HTTP. The resource clients (users, products...)
/// sit on top of this, and the step definitions never touch HttpClient directly.
/// </summary>
public class ApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    // One HttpClient for the whole run, so connections are reused.
    private static readonly HttpClient Http = new()
    {
        BaseAddress = new Uri(TestSettings.Current.BaseUrl),
        Timeout = TimeSpan.FromSeconds(TestSettings.Current.TimeoutSeconds)
    };

    private readonly IReqnrollOutputHelper _output;

    public ApiClient(IReqnrollOutputHelper output)
    {
        _output = output;
    }

    public Task<ApiResponse<T>> GetAsync<T>(string path, string? token = null)
        => SendAsync<T>(HttpMethod.Get, path, null, token);

    public Task<ApiResponse<T>> PostAsync<T>(string path, object body, string? token = null)
        => SendAsync<T>(HttpMethod.Post, path, body, token);

    public Task<ApiResponse<T>> PutAsync<T>(string path, object body, string? token = null)
        => SendAsync<T>(HttpMethod.Put, path, body, token);

    public Task<ApiResponse<T>> DeleteAsync<T>(string path, string? token = null)
        => SendAsync<T>(HttpMethod.Delete, path, null, token);

    private async Task<ApiResponse<T>> SendAsync<T>(HttpMethod method, string path, object? body, string? token)
    {
        using var request = new HttpRequestMessage(method, path);

        if (token != null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        if (body != null)
        {
            var json = JsonSerializer.Serialize(body, JsonOptions);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            _output.WriteLine($"{method} {path}  body: {json}");
        }
        else
        {
            _output.WriteLine($"{method} {path}");
        }

        using var response = await Http.SendAsync(request);
        var raw = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"  -> {(int)response.StatusCode} {response.StatusCode}");

        T? parsed = default;
        if (response.IsSuccessStatusCode)
        {
            try
            {
                parsed = JsonSerializer.Deserialize<T>(raw, JsonOptions);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException(
                    $"Could not read the response from {method} {path} as {typeof(T).Name}. Body was: {raw}", ex);
            }
        }

        return new ApiResponse<T> { StatusCode = response.StatusCode, RawBody = raw, Body = parsed };
    }
}
