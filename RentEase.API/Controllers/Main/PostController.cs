using Microsoft.AspNetCore.Mvc;

namespace RentEase.API.Controllers.Main
{
    public class PostController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
