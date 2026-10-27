namespace DummyJson.ApiTests.Models;

public class Product
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public decimal Price { get; set; }
}

public class ProductList
{
    public List<Product> Products { get; set; } = new();
    public int Total { get; set; }
    public int Skip { get; set; }
    public int Limit { get; set; }
}
