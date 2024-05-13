namespace BuzzBid.Models
{
    public class AuctionStatisticsModel
    {
        public String AuctionActive { get; set; }

        public String AuctionFinished { get; set; }

        public String AuctionWon { get; set; }

        public String AuctionCancelled { get; set; }

        public String AuctionRated { get; set; }

        public int AuctionNotRated { get; set; }


    }
}
