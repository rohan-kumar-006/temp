namespace InventoryManagement.API.DTOs.Dashboard
{
    public class LowStockProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string SKU { get; set; } = String.Empty;

        public int Quantity { get; set; }

        public int ReorderLevel { get; set; }

    }
}
