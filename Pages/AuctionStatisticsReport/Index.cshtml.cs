using BuzzBid.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
namespace BuzzBid.Pages.AuctionStatisticsReport
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly BuzzBidContext _context;
        private readonly UserManager _userManager;


        const String getTotalAuctionNum = "SELECT  COUNT(*) " +
                                          "FROM   ITEM " +
                                          "WHERE  DATEADD(day, AuctionLength, ListDate) > CURRENT_TIMESTAMP";

        const String getFinishedAuctionNum = "SELECT  COUNT(*) " +
                                            "FROM   ITEM    " +
                                            "WHERE CancelDate IS NULL AND DATEADD(day, AuctionLength, ListDate) <= CURRENT_TIMESTAMP ";

        const String getNumOfWinner = "SELECT Count(*) " +
                                      "FROM ITEM " +
                                      "WHERE WINNER IS NOT NULL";

        const String getNumOfCanclledAuction =  "SELECT Count(*) " +
                                                "FROM ITEM " +
                                                "WHERE cancelDate IS NOT NULL";

        const String getRatingSatitics = "SELECT Count(DISTINCT ITEMID) " +
                                          "FROM Rating";

        /// <summary>
        ///
        /// </summary>
        public AuctionStatisticsModel auctionStatisticsModel {  get; set; }
        public IndexModel(BuzzBidContext db, UserManager userManager)
        {
            this._context = db;
            auctionStatisticsModel = new AuctionStatisticsModel();
            _userManager = userManager;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            DBService svc = new DBService();

            var ds = svc.ExecuteSql(getTotalAuctionNum);
			auctionStatisticsModel.AuctionActive = ds.Tables[0].Rows[0][0].ToString();

			ds = svc.ExecuteSql(getFinishedAuctionNum);
			auctionStatisticsModel.AuctionFinished = ds.Tables[0].Rows[0][0].ToString();

			ds = svc.ExecuteSql(getNumOfWinner);
			auctionStatisticsModel.AuctionWon = ds.Tables[0].Rows[0][0].ToString();

			ds = svc.ExecuteSql(getNumOfCanclledAuction);
			auctionStatisticsModel.AuctionCancelled = ds.Tables[0].Rows[0][0].ToString();

			ds = svc.ExecuteSql(getRatingSatitics);
			auctionStatisticsModel.AuctionRated = ds.Tables[0].Rows[0][0].ToString();

            auctionStatisticsModel.AuctionNotRated = Int32.Parse(auctionStatisticsModel.AuctionWon) - Int32.Parse(auctionStatisticsModel.AuctionRated);

            if (!_userManager.IsAdminUser())
            {
                return RedirectToPage("/MainMenu");
            }
            return Page();
        }
    }
}
