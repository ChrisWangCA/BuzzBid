using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BuzzBid.Models;

namespace BuzzBid.Pages
{
    public class LogoutModel : PageModel
    {

        private readonly ILogger<LogoutModel> _logger;
        private readonly UserManager _userManager;

        public LogoutModel(ILogger<LogoutModel> logger, UserManager userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            await _userManager.SignOutAsync(HttpContext);
            return RedirectToPage("/Index");
        }
    }
}
