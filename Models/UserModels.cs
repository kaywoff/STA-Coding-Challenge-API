using System.Text.Json.Serialization;

namespace DummyJson.ApiTests.Models;

public class User
{
    // Null when we are the ones sending the user, so it is left out of the request.
    public int? Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public int Age { get; set; }
    public string Gender { get; set; } = "";
    public Hair Hair { get; set; } = new();
    public Address Address { get; set; } = new();
}

public class Hair
{
    public string Color { get; set; } = "";
    public string Type { get; set; } = "";
}

public class Address
{
    // The API willcalls the street line "address", which cannot be a property name inside a class called Address.
    [JsonPropertyName("address")]
    public string Street { get; set; } = "";
    public string City { get; set; } = "";
    public string State { get; set; } = "";
    public string StateCode { get; set; } = "";
    public string PostalCode { get; set; } = "";
    public Coordinates Coordinates { get; set; } = new();
    public string Country { get; set; } = "";
}

public class Coordinates
{
    public double Lat { get; set; }
    public double Lng { get; set; }
}

public class UserList
{
    public List<User> Users { get; set; } = new();
    public int Total { get; set; }
    public int Skip { get; set; }
    public int Limit { get; set; }
}
