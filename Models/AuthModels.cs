namespace DummyJson.ApiTests.Models;

public class LoginRequest
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public int ExpiresInMins { get; set; } = 30;
}

public class LoginResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string AccessToken { get; set; } = "";
    public string RefreshToken { get; set; } = "";
}

public class UserProfile
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
}
