namespace HyteraAPI.Models;

public class Item
{
    public int Id { get; set; }
    public string? ItemCode { get; set; }
    public string? ItemName { get; set; }
    public string? ItemType { get; set; }
    public int Instock { get; set; }
    public int OnOrder { get; set; }
    public decimal MSRP { get; set; }
    public decimal DealerPrice { get; set; }
    public string? ItemImageUrl { get; set; }
    public string? ItemLinkUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}
