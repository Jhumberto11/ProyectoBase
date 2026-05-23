using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoBDII.Application.Service.CassandraService;

namespace ProyectoBDII.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="admin")]
    public class HistorialController : ControllerBase
    {
        private readonly HistorialesServicio _historiales;
        public HistorialController(HistorialesServicio h)
        {
            _historiales = h;
        }

        [HttpPost("{userId}/{limit?}")]
        public async Task<IActionResult> HistorialLoginByUser(string userId, int? limit)
        {
            var result = await _historiales.GetHistorialLoginByUser(userId, limit);
            if (result.Count == 0) return BadRequest("Sin Registros");
            return Ok(result);
        }
    }
}
