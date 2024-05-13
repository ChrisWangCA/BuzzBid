using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;
using BuzzBid.Models;
using Microsoft.EntityFrameworkCore;

namespace BuzzBid
{
    public class UserManager
    {
        
        private readonly BuzzBidContext _context;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserManager(BuzzBidContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<bool> SignInAsync(HttpContext httpContext, string userName, string password, bool isPersistent = false)
        {
            DBService svc = new DBService();
            // Assume HashPassword is a method to hash the password before comparison
            string hashedPassword = svc.ComputeSha256Hash(password);

            // Using parameterized query to prevent SQL Injection
            var user = await _context.Users
                .FromSqlInterpolated($"SELECT * FROM dbo.[User] WHERE UserName = {userName} AND Password = {hashedPassword}")
                .FirstOrDefaultAsync();

            if (user != null)
            {
                var claims = GetUserClaims(user);
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await httpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties { IsPersistent = isPersistent }
                );

                return true;
            }
            return false;
        }

        private IEnumerable<Claim> GetUserClaims(User user)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserName),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName),
            new Claim("UserName", user.UserName)
        };

            return claims;
        }

        public async Task SignOutAsync(HttpContext httpContext)
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public Boolean IsAdminUser()
        {
            using (var context = new BuzzBidContext())
            {
                var userName = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userName == null)
                {
                    return false;
                }

                var admins = _context.Admins.FromSqlInterpolated($"SELECT * FROM dbo.[Admin] WHERE UserName = {userName}").ToList();
                if (admins.Count != 0)
                {
                    return true;
                }
                return false;
            }

        }

    }
}
