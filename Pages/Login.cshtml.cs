using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BuzzBid.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using BCrypt.Net;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;



namespace BuzzBid.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ILogger<LoginModel> _logger;
        private readonly BuzzBidContext _context;
        private readonly UserManager _userManager; // Inject the UserManage

        public LoginModel(ILogger<LoginModel> logger, BuzzBidContext context, UserManager userManager)
        {
            _logger = logger;
            _context = context;
            _userManager= userManager;
        }

        
        [BindProperty]
        [Required(ErrorMessage = "Username is required")]
        public string InputUserName { get; set; }


        [BindProperty]
        [Required(ErrorMessage = "Password is required")]
        public string InputPassword { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

             bool isAuthenticated = await _userManager.SignInAsync(HttpContext, InputUserName, InputPassword);
             if (isAuthenticated)
             {
                 return RedirectToPage("/MainMenu");
             }
             else
             {
                 ModelState.AddModelError(string.Empty, "Invalid username or password");
                 return Page();
             }

        }
    }
}
