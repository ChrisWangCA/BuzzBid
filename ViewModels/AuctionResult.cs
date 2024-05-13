namespace BuzzBid.ViewModels;

public class AuctionResult()
{

    public string? ItemId { get; set; } = string.Empty;
    public string? ItemName { get; set; } = string.Empty;
    public string? SalePrice { get; set; } = string.Empty;
    public string? Winner { get; set; } = string.Empty;
    public string? AuctionEnds { get; set; } = string.Empty;

    public AuctionResult(string? id, string? name, string? price, string? winner, string? auctionEnd) : this()
    {
        ItemId = id;
        ItemName = name;
        SalePrice = !string.IsNullOrEmpty(price)?price:"-";
        Winner = !string.IsNullOrEmpty(winner)?winner:"-";
        AuctionEnds = auctionEnd;
    }

}