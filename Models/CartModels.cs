namespace DummyJson.ApiTests.Models;

public class CartItem
{
    public int Id { get; set; }
    public int Quantity { get; set; }
}

public class AddCartRequest
{
    public int UserId { get; set; }
    public List<CartItem> Products { get; set; } = new();
}

public class CartLine
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal Total { get; set; }
    public decimal DiscountPercentage { get; set; }
    // On each line the API calls this discountedPrice, and rounds it to a whole number.
    public decimal DiscountedPrice { get; set; }
}

public class Cart
{
    public int Id { get; set; }
    public List<CartLine> Products { get; set; } = new();
    public decimal Total { get; set; }
    // Rounded to a whole number by the API.
    public decimal DiscountedTotal { get; set; }
    public int UserId { get; set; }
    public int TotalProducts { get; set; }
    public int TotalQuantity { get; set; }

    // we Only present on the response todelete.
    public bool IsDeleted { get; set; }
    public string? DeletedOn { get; set; }
}
