using System.Globalization;
using DummyJson.ApiTests.Clients;
using DummyJson.ApiTests.Models;
using DummyJson.ApiTests.Support;
using Reqnroll;
using Shouldly;

namespace DummyJson.ApiTests.Steps;

[Binding]
public class UserSteps
{
    private readonly ScenarioState _state;
    private readonly UsersClient _users;
    private readonly IReqnrollOutputHelper _output;

    public UserSteps(ScenarioState state, UsersClient users, IReqnrollOutputHelper output)
    {
        _state = state;
        _users = users;
        _output = output;
    }

    // ---- Red hair ----

    [When("I search for users with {string} hair colour")]
    public async Task WhenISearchForUsersByHairColour(string colour)
    {
        _state.UserSearch = _state.Record(await _users.FilterByHairColourAsync(colour));
    }

    [Then("every user returned has {string} hair")]
    public void ThenEveryUserHasHair(string colour)
    {
        var search = _state.UserSearch.ShouldNotBeNull();

        search.Users.ShouldNotBeEmpty();
        search.Users.Count.ShouldBe(search.Total, "the list should hold every match, not just the first page");

        foreach (var user in search.Users)
        {
            user.Hair.Color.ShouldBe(colour, $"{user.FirstName} {user.LastName} should have {colour} hair");
        }

        _output.WriteLine($"{search.Total} users have {colour} hair");
    }

    [Then("the total agrees with a manual count of {string} hair in the full user list")]
    public async Task ThenTheTotalMatchesAManualCount(string colour)
    {
        var search = _state.UserSearch.ShouldNotBeNull();

        // A second opinion: ignore the filter endpoint and count the whole list ourselves.
        var everyone = (await _users.GetAllAsync()).Body.ShouldNotBeNull();
        var counted = everyone.Users.Count(u => string.Equals(u.Hair.Color, colour, StringComparison.OrdinalIgnoreCase));

        search.Total.ShouldBe(counted);
    }

    //  Adding a user 

    [Given("I have a new user with these details")]
    public void GivenIHaveANewUser(DataTable table)
    {
        var d = table.Rows.ToDictionary(row => row["field"], row => row["value"]);

        _state.UserToAdd = new User
        {
            FirstName = d["firstName"],
            LastName = d["lastName"],
            Age = int.Parse(d["age"]),
            Gender = d["gender"],
            Hair = new Hair { Color = d["hairColour"], Type = d["hairType"] },
            Address = new Address
            {
                Street = d["street"],
                City = d["city"],
                State = d["state"],
                StateCode = d["stateCode"],
                PostalCode = d["postalCode"],
                Coordinates = new Coordinates
                {
                    Lat = double.Parse(d["latitude"], CultureInfo.InvariantCulture),
                    Lng = double.Parse(d["longitude"], CultureInfo.InvariantCulture)
                },
                Country = d["country"]
            }
        };
    }

    [When("I add the new user")]
    public async Task WhenIAddTheNewUser()
    {
        _state.AddedUser = _state.Record(await _users.AddAsync(_state.UserToAdd.ShouldNotBeNull()));
    }

    [Then("the new user has been given an id")]
    public void ThenTheUserHasAnId()
    {
        var id = _state.AddedUser.ShouldNotBeNull().Id;

        id.HasValue.ShouldBeTrue("the response should contain an id for the new user");
        id!.Value.ShouldBeGreaterThan(0);
    }

    // The API does not save anything, so we only check what comes back in this one response.
    [Then("the response repeats every detail I sent")]
    public void ThenTheResponseRepeatsWhatWasSent()
    {
        var sent = _state.UserToAdd.ShouldNotBeNull();
        var got = _state.AddedUser.ShouldNotBeNull();

        got.FirstName.ShouldBe(sent.FirstName);
        got.LastName.ShouldBe(sent.LastName);
        got.Age.ShouldBe(sent.Age);
        got.Gender.ShouldBe(sent.Gender);

        got.Hair.Color.ShouldBe(sent.Hair.Color);
        got.Hair.Type.ShouldBe(sent.Hair.Type);

        got.Address.Street.ShouldBe(sent.Address.Street);
        got.Address.City.ShouldBe(sent.Address.City);
        got.Address.State.ShouldBe(sent.Address.State);
        got.Address.StateCode.ShouldBe(sent.Address.StateCode);
        got.Address.PostalCode.ShouldBe(sent.Address.PostalCode);
        got.Address.Country.ShouldBe(sent.Address.Country);
        got.Address.Coordinates.Lat.ShouldBe(sent.Address.Coordinates.Lat);
        got.Address.Coordinates.Lng.ShouldBe(sent.Address.Coordinates.Lng);
    }
}
