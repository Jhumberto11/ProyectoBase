using MarketplaceApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoBDII.Application.Service;
using ProyectoBDII.Domain.Interface;
using ProyectoBDII.DTOS;

namespace ProyectoBDII.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        readonly UserService _userService;
        readonly IJwToken _jwToken;
        public AuthController (UserService _u, IJwToken jwToken)
        {
            _userService = _u;
            _jwToken = jwToken;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                var usuario = new Usuario
                {
                    Name = dto.Name,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    ProfileImageUrl = dto.ProfileImageUrl,
                    City = dto.City,
                    Department = dto.Department,
                    Country = dto.Country,
                    Role = "user",
                    Status = "active"
                };

                var creado = await _userService.RegisterAsync(usuario, dto.Password);

                return Ok("Usuario Registrado con Exito");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var usuario = await _userService.ValidateAccessAsync(dto.Email, dto.Password);

            if (usuario is null)
                return Unauthorized(new { message = "Credenciales inválidas." });

            var token = _jwToken.GenerateToken(usuario);

            return Ok($"Acceso Concedido {new { Token = token}}");
        }


    }
}
