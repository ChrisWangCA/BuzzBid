using System;
using System.Collections.Generic;

namespace BuzzBid.Models;

public partial class Bidding
{
    public int BidId { get; set; }

    public int BidItem { get; set; }

    public string BidBy { get; set; } = null!;

    public DateTime BidTime { get; set; }

    public decimal BidAmount { get; set; }

    public virtual User BidByNavigation { get; set; } = null!;

    public virtual Item BidItemNavigation { get; set; } = null!;
}
