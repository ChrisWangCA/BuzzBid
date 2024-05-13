using BuzzBid.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using static BuzzBid.UserManager;
using System.Diagnostics;

namespace BuzzBid.Pages.CancelledAuctionsReport
{
	[Authorize]
    public class IndexModel : PageModel
    {
		private readonly BuzzBidContext _context;
        private readonly UserManager _userManager;
        const String na = "N/A";
		public List<cancelledAuctionModel> cancelledAuctionModel { get; set; }

		public IndexModel(BuzzBidContext db, UserManager userManager)
		{
			this._context = db;
			this._userManager = userManager;
			


		}
        public async Task<IActionResult> OnGetAsync()
        {

			var getCancelledAuctionInfo = @"
            SELECT ItemId, ListBy, CancelDate, CancelReason
            FROM Item
            WHERE cancelDate IS NOT NULL
            ORDER BY ItemId DESC";


            cancelledAuctionModel = new List<cancelledAuctionModel>();

            
            DBService svc = new DBService();
			var ds = svc.ExecuteSql(getCancelledAuctionInfo);

			DataRowCollection categoryTable = ds.Tables[0].Rows;
	


			foreach (DataRow row in categoryTable)
			{
                cancelledAuctionModel cancelled = new cancelledAuctionModel();
                cancelled.ItemId = row[0].ToString();
                cancelled.ListBy = row[1].ToString();
                cancelled.cancelledDate = row[2].ToString();
                cancelled.cancelledReason = row[3].ToString().Equals("") ? na : row[3].ToString();

                cancelledAuctionModel.Add(cancelled);

			}
            if (!_userManager.IsAdminUser())
            {
                return RedirectToPage("/MainMenu");
            }
            return Page();
        }
	}
}
