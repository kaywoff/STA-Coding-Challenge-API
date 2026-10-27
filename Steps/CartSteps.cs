using DummyJson.ApiTests.Clients;
using DummyJson.ApiTests.Models;
using DummyJson.ApiTests.Support;
using Reqnroll;
using Shouldly;

namespace DummyJson.ApiTests.Steps;

[Binding]
public class CartSteps
{
    // Prices are rounded to the penny by the API, so allow for a penny or two of drift.
    private const decimal MoneyTolerance = 0.02m;

    // The overall percentage is derived from rounded money values, so give it a bit more room.
    private const decimal PercentageTolerance = 0.1m;

    private readonly ScenarioState _state;
    private readonly CartsClient _carts;
    private readonly IReqnrollOutputHelper _output;

    public CartSteps(ScenarioState state, CartsClient carts, IReqnrollOutputHelper output)
    {
        _state = state;
        _carts = carts;
        _output = output;
    }

    // ---- Adding a cart ----

    [Given("I am shopping as user {int}")]
    public void GivenIAmShoppingAsUser(int userId)
    {
        _state.ShoppingUserId = userId;
    }

    [When("I add these products to a cart")]
    public async Task WhenIAddTheseProducts(DataTable table)
    {
        _state.RequestedItems = table.Rows
            .Select(row => new CartItem { Id = int.Parse(row["productId"]), Quantity = int.Parse(row["quantity"]) })
            .ToList();

        var request = new AddCartRequest { UserId = _state.ShoppingUserId, Products = _state.RequestedItems };

        _state.Cart = _state.Record(await _carts.AddAsync(request));
    }

    [Then("the cart contains {int} different products")]
    public void ThenTheCartContains(int count)
    {
        var cart = _state.Cart.ShouldNotBeNull();

        cart.Products.Count.ShouldBe(count);
        cart.TotalProducts.ShouldBe(count);
    }

    [Then("the quantities in the cart match what I asked for")]
    public void ThenTheQuantitiesMatch()
    {
        var cart = _state.Cart.ShouldNotBeNull();

        foreach (var wanted in _state.RequestedItems)
        {
            var line = cart.Products
                .SingleOrDefault(l => l.Id == wanted.Id)
                .ShouldNotBeNull($"product {wanted.Id} should be in the cart");
            line.Quantity.ShouldBe(wanted.Quantity);
        }

        cart.TotalQuantity.ShouldBe(_state.RequestedItems.Sum(i => i.Quantity));
    }

    [Then("the cart totals add up")]
    public void ThenTheTotalsAddUp()
    {
        var cart = _state.Cart.ShouldNotBeNull();

        foreach (var line in cart.Products)
        {
            IsClose(line.Total, line.Price * line.Quantity).ShouldBeTrue(
                $"{line.Title}: {line.Quantity} x {line.Price} should be {line.Total}");
        }

        IsClose(cart.Total, cart.Products.Sum(l => l.Total)).ShouldBeTrue(
            $"cart total {cart.Total} should be the sum of its lines");

        IsClose(cart.DiscountedTotal, cart.Products.Sum(l => l.DiscountedTotal)).ShouldBeTrue(
            $"cart discounted total {cart.DiscountedTotal} should be the sum of its lines");
    }

    [Then("each product's discounted total follows its discount percentage")]
    public void ThenEachLineFollowsItsDiscount()
    {
        foreach (var line in _state.Cart.ShouldNotBeNull().Products)
        {
            var expected = line.Total * (1 - line.DiscountPercentage / 100);

            IsClose(line.DiscountedTotal, expected).ShouldBeTrue(
                $"{line.Title}: {line.Total} with {line.DiscountPercentage}% off should be about {expected:0.00}, but was {line.DiscountedTotal}");
        }
    }

    // The task asks for the discount PERCENTAGE of the whole cart, which the API does not
    // hand us directly (it only gives money values). We work it out two different ways
    // and make sure they agree.
    [Then("the overall discount percentage is consistent with the discounts on each product")]
    public void ThenTheOverallPercentageIsConsistent()
    {
        var cart = _state.Cart.ShouldNotBeNull();

        // 1. From the cart's own totals: how much of the price was taken off.
        var fromTotals = (cart.Total - cart.DiscountedTotal) / cart.Total * 100;

        // 2. From the individual products: each percentage weighted by how much that line costs.
        var fromLines = cart.Products.Sum(l => l.Total * l.DiscountPercentage) / cart.Products.Sum(l => l.Total);

        _output.WriteLine($"Overall discount: {fromTotals:0.00}% (from totals), {fromLines:0.00}% (weighted from lines)");

        fromTotals.ShouldBeGreaterThan(0);
        (Math.Abs(fromTotals - fromLines) <= PercentageTolerance).ShouldBeTrue(
            $"overall discount was {fromTotals:0.000}% from the totals but {fromLines:0.000}% from the lines");
    }

    // ---- Deleting a cart ----

    [When("I delete cart {int}")]
    public async Task WhenIDeleteCart(int cartId)
    {
        _state.Cart = _state.Record(await _carts.DeleteAsync(cartId));
    }

    [Then("the deleted cart is cart {int}")]
    public void ThenTheDeletedCartIs(int cartId)
    {
        _state.Cart.ShouldNotBeNull().Id.ShouldBe(cartId);
    }

    [Then("the cart is marked as deleted")]
    public void ThenTheCartIsMarkedDeleted()
    {
        _state.Cart.ShouldNotBeNull().IsDeleted.ShouldBeTrue();
    }

    [Then("a deletion date is returned")]
    public void ThenADeletionDateIsReturned()
    {
        _state.Cart.ShouldNotBeNull().DeletedOn.ShouldNotBeNullOrWhiteSpace();
    }

    private static bool IsClose(decimal actual, decimal expected)
        => Math.Abs(actual - expected) <= MoneyTolerance;
}
