using System;
using System.Collections.Generic;

namespace BuzzBid.Models;

public partial class Item
{
    public int ItemId { get; set; }

    public string ItemName { get; set; } = null!;

    public string ListBy { get; set; } = null!;

    public DateTime ListDate { get; set; }

    public string Description { get; set; } = null!;

    public decimal GetItNowPrice { get; set; }

    public int AuctionLength { get; set; }

    public decimal MinSalesPrice { get; set; }

    public decimal StartBid { get; set; }

    public bool Returnable { get; set; }

    public int Condition { get; set; }

    public DateTime? CancelDate { get; set; }

    public string? CancelReason { get; set; }

    public string? Winner { get; set; }

    public DateTime? WinDate { get; set; }

    public decimal? SalesPrice { get; set; }

    public int? CategoryId { get; set; }

    public virtual ICollection<Bidding> Biddings { get; set; } = new List<Bidding>();

    public virtual Category? Category { get; set; }

    public virtual User ListByNavigation { get; set; } = null!;

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual User? WinnerNavigation { get; set; }
}
