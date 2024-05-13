using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BuzzBid.Models;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace BuzzBid.Pages;

    public class UserPageModel : PageModel
    {
        private readonly BuzzBidContext _context;

        public UserPageModel(BuzzBidContext context)
        {
            _context = context;
        }


        public IActionResult OnGet()
        {
            // Example: Retrieve data from the database
            var users = _context.Users.ToList();

            // Do something with the data (e.g., pass it to the Razor Page)
            ViewData["Users"] = users;

            return Page();
        }

        public List<User> GetUsers()
        {
            //FormattableString sql = $"select * from dbo.[USER] where LastName like 'J%'";
            //return _context.Users.FromSqlRaw(FormattableString).ToList();
            
            using (var context = new BuzzBidContext())
            {
                return context.Users.FromSqlRaw($"select * from dbo.[USER] where LastName like 'J%'").ToList();
            }

        }
    }

