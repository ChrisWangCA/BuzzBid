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
using Microsoft.Identity.Client;
using Microsoft.Net.Http.Headers;
using BuzzBid.ViewModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BuzzBid.Pages
{
    [Authorize]
    public class WriteCommentsModel : PageModel
    {
        private readonly BuzzBidContext _context;
        public WriteCommentsModel(BuzzBidContext context)
        {
            _context = context;
        }


        [BindProperty(SupportsGet = true)]
        public int ItemId { get; set; }

        public string inputItemName { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Comment is required")]
        public string comments { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Minimum sale price is required")]
        public decimal miniSalePrice { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Start Auction Bidding is required")]
        public decimal startBidding { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Get it now price is required")]

        public int getStars {get; set;}


        public void OnGet()
        {
    

        }



        public async Task<IActionResult> OnPost()
        {
            var search = HttpContext.Request.Query;
            
            

            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);

            DBService svc = new DBService();
            var sql = "INSERT INTO dbo.[Rating](ItemId, RateTime, Text, Stars)" + 
                "VALUES(@ItemId, @RateTime, @Text, @Stars)";
            var parameters = new Dictionary<string, object>
            {
                
                {"@ItemId", ItemId},
                {"@RateTime", DateTime.Now},
                {"@Text", comments},
                {"@Stars", getStars},
                

            };
            svc.ExecuteNonQuerySql(sql, parameters);
            return RedirectToPage("/MainMenu");
        }

    }
}
