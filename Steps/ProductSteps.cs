using System.Text.Json;
using DummyJson.ApiTests.Clients;
using DummyJson.ApiTests.Support;
using Reqnroll;
using Shouldly;

namespace DummyJson.ApiTests.Steps;

[Binding]
public class ProductSteps
{
    private readonly ScenarioState _state;
    private readonly ProductsClient _products;

    public ProductSteps(ScenarioState state, ProductsClient products)
    {
        _state = state;
        _products = products;
    }

    // ---- Search ----

    [When("I search for the product {string}")]
    public async Task WhenISearchForTheProduct(string term)
    {
        _state.ProductList = _state.Record(await _products.SearchAsync(term));
    }

    [Then("the search results include a product titled {string}")]
    public void ThenTheResultsIncludeAProduct(string title)
    {
        var matches = _state.ProductList.ShouldNotBeNull().Products
            .Where(p => string.Equals(p.Title, title, StringComparison.OrdinalIgnoreCase))
            .ToList();

        // Searching is fuzzy, so make sure we are looking at exactly the product we meant.
        matches.Count.ShouldBe(1, $"expected exactly one product titled '{title}'");
        _state.Product = matches[0];
    }

    [Then(@"the product costs (\d+\.\d+)")]
    public void ThenTheProductCosts(decimal expectedPrice)
    {
        _state.Product.ShouldNotBeNull().Price.ShouldBe(expectedPrice);
    }

    // ---- Pagination ----

    [When("I request product number {int} with a page size of 1")]
    public async Task WhenIRequestProductNumber(int position)
    {
        const int pageSize = 1;
        var skip = position - 1;   // the 15th product means skipping the first 14

        var page = _state.Record(await _products.GetPageAsync(pageSize, skip, "id,title,price"));
        _state.ProductList = page;
        _state.Product = page?.Products.FirstOrDefault();
    }

    [Then("the response contains exactly {int} product")]
    public void ThenTheResponseContainsExactly(int count)
    {
        _state.ProductList.ShouldNotBeNull().Products.Count.ShouldBe(count);
    }

    [Then("the product contains only the id, title and price")]
    public void ThenTheProductHasOnlyThreeFields()
    {
        using var json = JsonDocument.Parse(_state.LastRawBody);
        var firstProduct = json.RootElement.GetProperty("products")[0];

        var fieldNames = firstProduct.EnumerateObject().Select(f => f.Name).OrderBy(n => n).ToArray();

        fieldNames.ShouldBe(new[] { "id", "price", "title" });
    }

    // ---- Update ----

    [When("I change the title of product {int} to {string}")]
    public async Task WhenIChangeTheTitle(int id, string newTitle)
    {
        _state.Product = _state.Record(await _products.UpdateTitleAsync(id, newTitle));
    }

    // ---- Shared checks ----

    [Then("the product id is {int}")]
    public void ThenTheProductIdIs(int expected)
    {
        _state.Product.ShouldNotBeNull().Id.ShouldBe(expected);
    }

    [Then("the product title is {string}")]
    public void ThenTheProductTitleIs(string expected)
    {
        _state.Product.ShouldNotBeNull().Title.ShouldBe(expected);
    }

    [Then("the product's own page shows the same title and price")]
    public async Task ThenTheProductPageAgrees()
    {
        var product = _state.Product.ShouldNotBeNull();

        var ownPage = (await _products.GetByIdAsync(product.Id)).Body.ShouldNotBeNull();

        ownPage.Title.ShouldBe(product.Title);
        ownPage.Price.ShouldBe(product.Price);
    }
}
