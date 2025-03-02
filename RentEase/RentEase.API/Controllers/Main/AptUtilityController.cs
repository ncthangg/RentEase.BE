using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RentEase.API.Controllers.Main
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "1,2,3,4")]
    public class AptUtilityController : Controller
    {
    }
}
