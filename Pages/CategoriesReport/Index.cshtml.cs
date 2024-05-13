using BuzzBid.Models;


using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuzzBid.Pages.Categories
{
	[Authorize]
	public class IndexModel : PageModel
    {

        private readonly BuzzBidContext _context;

        private readonly UserManager _userManager;

        public List<Category> categories { get; set; }

		public List<String> totals;
		public List<String> minMums ;
		public List<String> maxMums;
		public List<String> averages;

		public  String NA = "N/A";

		public IndexModel(BuzzBidContext db, UserManager userManager)
        {
            this._context = db;
			totals = new List<String>();
			minMums = new List<String>();
			maxMums = new List<String>();
			averages = new List<String>();
            _userManager = userManager;



        }

        int CategoryId;
        public async Task<IActionResult> OnGetAsync()
        {
            categories = new List<Category>();


            var LoadCategoryMapper = @"
             SELECT  Category.CategoryId, Category.description, Count(ItemId), MIN(GetItNowPrice), MAX(GetItNowPrice), AVG(GetItNowPrice)
             FROM Category, Item
             WHERE Item.CategoryId = Category.CategoryId AND CancelDate IS NULL
             group by Category.CategoryId,Category.description
             ORDER BY Category.description";



 

            DBService svc = new DBService();
            var ds = svc.ExecuteSql(LoadCategoryMapper);

            DataRowCollection categoryTable = ds.Tables[0].Rows;
       


            foreach (DataRow row in categoryTable)
            {

                categories.Add(new Category(
                    (int)row[0],
                    row[1].ToString()
                    )
                );

                totals.Add(row[2].ToString());
             
                
                



                minMums.Add(row[3].ToString().Equals("") ? NA : "$" + Math.Round(Convert.ToDouble(row[3]), 1).ToString("0.00")) ;
				maxMums.Add(row[4].ToString().Equals("") ? NA : "$" + Math.Round(Convert.ToDouble(row[4]), 1).ToString("0.00"));
				averages.Add(row[5].ToString().Equals("") ? NA : "$" + Math.Round(Convert.ToDouble(row[5]), 1).ToString("0.00"));

			}

            if (!_userManager.IsAdminUser())
            {
                return RedirectToPage("/MainMenu");
            }
            return Page();




        }
    }
}
