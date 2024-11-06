using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OkController : ControllerBase
    {
        [HttpDelete]
        public IActionResult AnhEm(int id)
        {
            return Ok(id);
        }
    }
}
