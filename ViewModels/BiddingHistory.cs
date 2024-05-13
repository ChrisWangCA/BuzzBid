namespace BuzzBid.ViewModels;

public class BiddingHistory()
{

    public string? BidAmount { get; set; } = string.Empty;
    public string? BidTime { get; set; } = string.Empty;
    public string? BidBy { get; set; } = string.Empty;

    public BiddingHistory(string? amount, string? time, string? by) : this()
    {
        BidAmount = amount;
        BidTime = time;
        BidBy = by;
    }
}