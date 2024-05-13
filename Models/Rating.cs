using System;
using System.Collections.Generic;

namespace BuzzBid.Models;

public partial class Rating
{
    public int RatingId { get; set; }

    public int ItemId { get; set; }

    public DateTime RateTime { get; set; }

    public string Text { get; set; } = null!;

    public int Stars { get; set; }

    public DateTime? DeleteDate { get; set; }

    public virtual Item Item { get; set; } = null!;
}
