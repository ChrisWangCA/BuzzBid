using BuzzBid.Models;

namespace BuzzBid.Pages.Categories
{
    internal class BuzzBidDbContext
    {
        public IEnumerable<Category> Categories { get; internal set; }
    }
}