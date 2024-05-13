using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuzzBid.Controllers
{
    public class BuzzBidController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        // This action can be accessed by anyone
        public IActionResult PublicPage()
        {
            return View();
        }

        // This action requires authentication
        [Authorize]
        public IActionResult SecurePage()
        {
            return View();
        }

    }
}
