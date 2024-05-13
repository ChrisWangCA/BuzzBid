using BuzzBid.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNet.Identity;
using System.Security.Claims;

namespace BuzzBid.Pages
{
    [Authorize]
    public class ListItemModel : PageModel
    {
        private readonly BuzzBidContext _context;
        public ListItemModel(BuzzBidContext context)
        {
            _context = context;
        }

        [BindProperty]
        [Required(ErrorMessage = "Item name is required")]
        public string inputItemName { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Description is required")]
        public string inputDescription { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Minimum sale price is required")]
        public decimal miniSalePrice { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Start Auction Bidding is required")]
        public decimal startBidding { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Get it now price is required")]
        public decimal getitNow { get; set; }

        public bool isItemReturnable { get; set; }

        [BindProperty]
        public string selectCondition { get; set; }
        [BindProperty]
        public string selectCategoryId { get; set; }
        [BindProperty]
        public string selectAuctionLength { get; set; }

        public void OnGet()
        {
        }
        public List<Category> GetCategories()
        {

            using (var context = new BuzzBidContext())
            {
                return context.Categories.FromSqlRaw($"select * from dbo.[Category]").ToList();
            }

        }
        public async Task<IActionResult> OnPost()
        {
            var search = HttpContext.Request.Query;
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Invalid input");
                return Page();
            }

            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);

            DBService svc = new DBService();
            var sql = "INSERT INTO dbo.[Item](ItemName,ListBy, ListDate, Description, GetItNowPrice, AuctionLength, MinSalesPrice, StartBid, Returnable, Condition, CategoryId) " +
                "VALUES(@ItemName, @ListBy, @ListDate, @Description, @GetItNowPrice, @AuctionLength, @MinSalesPrice, @StartBid, @Returnable, @Condition, @CategoryId)";

            DateTime scheduledAuctionEnds = DateTime.Now.AddDays(Convert.ToDouble(selectAuctionLength));
            var parameters = new Dictionary<string, object>
            {
                {"@ItemName", inputItemName},
                {"@ListBy",userName},
                {"@ListDate", scheduledAuctionEnds},
                {"@Description", inputDescription},
                {"@GetItNowPrice",getitNow},
                {"@AuctionLength", selectAuctionLength},
                {"@MinSalesPrice",miniSalePrice},
                {"@StartBid",startBidding},
                {"@Returnable", isItemReturnable},
                {"@Condition",selectCondition},
                {"@CategoryId",selectCategoryId},
            };
            svc.ExecuteNonQuerySql(sql, parameters);
            return RedirectToPage("/MainMenu");
        }

    }
}
