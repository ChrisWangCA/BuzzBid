using BuzzBid.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BuzzBid.Pages
{
    [Authorize]
    public class Main_MenuModel : PageModel
    {
        private readonly UserManager _userManager;
        public bool IsAdmin { get; set; }

        public Main_MenuModel(UserManager userManager)
        {
            _userManager = userManager;
        }

        public void OnGet()
        {
            IsAdmin = _userManager.IsAdminUser();
        }
       
    }
}
