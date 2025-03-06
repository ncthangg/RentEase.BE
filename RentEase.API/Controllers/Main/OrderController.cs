using Microsoft.AspNetCore.Mvc;

namespace RentEase.API.Controllers.Main
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
