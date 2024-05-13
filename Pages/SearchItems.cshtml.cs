using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BuzzBid.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BuzzBid.Pages;

[Authorize]
public class SearchItemsModel : PageModel
{
    
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
}

