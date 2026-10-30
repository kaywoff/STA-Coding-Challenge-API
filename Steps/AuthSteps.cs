using System.Net;
using DummyJson.ApiTests.Clients;
using DummyJson.ApiTests.Config;
using DummyJson.ApiTests.Support;
using Reqnroll;
using Shouldly;

namespace DummyJson.ApiTests.Steps;

[Binding]
public class AuthSteps
{
    private readonly ScenarioState _state;
    private readonly AuthClient _auth;

    private string _username = "";
    private string _password = "";

    public AuthSteps(ScenarioState state, AuthClient auth)
    {
        _state = state;
        _auth = auth;
    }

    [Given("I have valid login credentials")]
    public void GivenIHaveValidLoginCredentials()
    {
        _username = TestSettings.Current.Username;
        _password = TestSettings.Current.Password;
    }

    [When("I log in")]
    public async Task WhenILogIn()
    {
        var login = _state.Record(await _auth.LoginAsync(_username, _password));
        _state.AccessToken = login?.AccessToken;
    }

    [Then("an access token is returned")]
    public void ThenAnAccessTokenIsReturned()
    {
        _state.AccessToken.ShouldNotBeNullOrWhiteSpace();
    }

    // Used a scenarios that needed a token but are not about logging in themselves
    [Given("I am logged in as a valid user")]
    public async Task GivenIAmLoggedIn()
    {
        var settings = TestSettings.Current;
        var response = await _auth.LoginAsync(settings.Username, settings.Password);

        response.StatusCode.ShouldBe(HttpStatusCode.OK, $"Login failed: {response.RawBody}");
        _state.AccessToken = response.Body?.AccessToken;
        _state.AccessToken.ShouldNotBeNullOrWhiteSpace();
    }

    [When("I ask for the current user's details")]
    public async Task WhenIAskForTheCurrentUser()
    {
        _state.CurrentUser = _state.Record(await _auth.GetCurrentUserAsync(_state.AccessToken!));
    }

    [Then("the logged-in user's id is {int}")]
    public void ThenTheIdIs(int expected)
    {
        _state.CurrentUser.ShouldNotBeNull().Id.ShouldBe(expected);
    }

    [Then("the logged-in user's first name is {string}")]
    public void ThenTheFirstNameIs(string expected)
    {
        _state.CurrentUser.ShouldNotBeNull().FirstName.ShouldBe(expected);
    }

    [Then("the logged-in user's last name is {string}")]
    public void ThenTheLastNameIs(string expected)
    {
        _state.CurrentUser.ShouldNotBeNull().LastName.ShouldBe(expected);
    }
}
