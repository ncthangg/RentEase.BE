using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentEase.Service.Service.Main;

namespace RentEase.API.Controllers.Main
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "1,2,3")]
    public class PostRequireController : Controller
    {
        private readonly IPostRequireService _postRequireService;
        public PostRequireController(IPostRequireService postRequireService)
        {
            _postRequireService = postRequireService;
        }
    }
}
