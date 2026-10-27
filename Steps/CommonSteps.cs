using System.Net;
using DummyJson.ApiTests.Support;
using Reqnroll;
using Shouldly;

namespace DummyJson.ApiTests.Steps;

[Binding]
public class CommonSteps
{
    private readonly ScenarioState _state;

    public CommonSteps(ScenarioState state)
    {
        _state = state;
    }

    [Then("the response status code is {int}")]
    public void ThenTheStatusCodeIs(int expected)
    {
        _state.LastStatusCode.ShouldBe((HttpStatusCode)expected, $"Response body was: {_state.LastRawBody}");
    }
}
