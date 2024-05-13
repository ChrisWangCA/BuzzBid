using BuzzBid.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace BuzzBid.Pages
{

    public class RegisterModel : PageModel
    {
        private readonly BuzzBidContext _context;
        public RegisterModel(BuzzBidContext context)
        {
            _context = context;
        }

        [BindProperty]
        [Required(ErrorMessage = "Username is required")]
        public string InputUserName { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "First Name is required")]
        public string InputFirstName { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Last Name is required")]
        public string InputLastName { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Password is required")]
        public string InputPassword { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Password is required")]
        public string ConfirmedPassword { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Invalid input");
                return Page();
            }
            if (InputPassword != ConfirmedPassword)
            {
                ModelState.AddModelError(string.Empty, "Passwords do not match.");
                return Page();
            }
            DBService svc = new DBService();
            String HashedPassword = svc.ComputeSha256Hash(InputPassword);
            
            List<User> users = _context.Users.FromSqlRaw($"SELECT * FROM dbo.[User] WHERE UserName = '{InputUserName}'").ToList();
            if (users.Count != 0)
            {
                ModelState.AddModelError(string.Empty, "Username already exists.");
                return Page();
            }

            var sql = "INSERT INTO dbo.[User](UserName, Password, FirstName, LastName) VALUES(@UserName, @Password, @FirstName, @LastName)";
            var parameters = new Dictionary<string, object>
            {
                {"@UserName", InputUserName},
                {"@Password", HashedPassword},
                {"@FirstName", InputFirstName},
                {"@LastName", InputLastName}
            };
            svc.ExecuteNonQuerySql(sql, parameters);


            // Redirect to a different page after successful registration
            return RedirectToPage("/Login");
        }
    }
}
