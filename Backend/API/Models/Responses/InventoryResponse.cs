namespace HyteraAPI.Models.Responses;

public class InventoryCheckResponse
{
    public int Status { get; set; }
    public string? Message { get; set; }
    public List<ItemInfo>? ItemsFound { get; set; }
}

public class ItemInfo
{
    public int ItemId { get; set; }
    public string? ItemCode { get; set; }
    public string? ItemName { get; set; }
    public string? ItemType { get; set; }
    public int Instock { get; set; }
    public int OnOrder { get; set; }
    public decimal MSRP { get; set; }
    public decimal DealerPrice { get; set; }
    public string? ItemImageUrl { get; set; }
    public string? ItemLinkUrl { get; set; }
}

public class NluResult
{
    public bool IsInventoryCheck { get; set; }
    public string? ItemCode { get; set; }
    public string? NormalizedQuery { get; set; }
}
