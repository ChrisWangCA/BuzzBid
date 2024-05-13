using BuzzBid.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
namespace BuzzBid.Pages.RatedItemReport
{
	[Authorize]
    public class IndexModel : PageModel
    {

		private readonly BuzzBidContext _context;

        private readonly UserManager _userManager;

        public List<RateItemModel> RateItems { get; set; }

		public IndexModel(BuzzBidContext db, UserManager userManager)
		{
			this._context = db;
			RateItems = new List<RateItemModel>();
			_userManager = userManager;

		}
        public async Task<IActionResult> OnGetAsync()
        {
			var sql = @"SELECT TOP(10) ItemName , ROUND(AVG(CAST(Stars AS FLOAT)), 1) as avgSatr, Count(Stars)
			FROM Rating INNER JOIN Item ON Rating.ItemId = Item.ItemId
			GROUP BY ItemName
			ORDER BY avgSatr desc";

			DBService svc = new DBService();
			var ds = svc.ExecuteSql(sql);
			DataRowCollection categoryTable = ds.Tables[0].Rows;

			foreach (DataRow row in categoryTable)
			{
				RateItemModel rateItemModel = new RateItemModel();
				rateItemModel.ItemName = row[0].ToString();
				double d = Convert.ToDouble(row[1]);
				rateItemModel.AveStars = Math.Round(d, 1).ToString("0.0");
				rateItemModel.totalStars = row[2].ToString();


				RateItems.Add(rateItemModel);
			}

            if (!_userManager.IsAdminUser())
            {
                return RedirectToPage("/MainMenu");
            }
            return Page();
        }
    }
}
