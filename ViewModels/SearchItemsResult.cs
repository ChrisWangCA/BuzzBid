namespace BuzzBid.ViewModels;

public class SearchItemsResult()
{

    public string? ItemId { get; set; } = string.Empty;
    public string? ItemName { get; set; } = string.Empty;
    public string? CurrentBid { get; set; } = string.Empty;
    public string? HighBidder { get; set; } = string.Empty;
    public string? GetItNowPrice { get; set; } = string.Empty;
    public string? AuctionEnds { get; set; } = string.Empty;

    public SearchItemsResult(string? id, string? name, string? bid, string? bidder, string? price, string? auctionEnd) : this()
    {
        ItemId = id;
        ItemName = name;
        CurrentBid = !string.IsNullOrEmpty(bid)?bid:"-";
        HighBidder = !string.IsNullOrEmpty(bidder)?bidder:"-";
        GetItNowPrice = price;
        AuctionEnds = auctionEnd;
    }

}