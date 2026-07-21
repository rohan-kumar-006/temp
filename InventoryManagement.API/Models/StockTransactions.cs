namespace InventoryManagement.API.Models;
public class StockTransaction
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int UserId { get; set; }

    public string Type { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; }
}