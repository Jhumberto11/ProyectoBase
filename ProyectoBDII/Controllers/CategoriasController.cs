using MarketplaceApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoBDII.Application.Service;
using ProyectoBDII.Domain.Models;
using ProyectoBDII.DTOS.Categorias;

namespace ProyectoBDII.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class CategoriasController : ControllerBase
    {

        readonly CategoriasService _categoriasService;

        public CategoriasController(CategoriasService categoriasService)
        {
            _categoriasService = categoriasService;
        }


        [HttpGet("allCategorias")]
        public async Task<ActionResult<List<RespuestaCategorias>>>ObtenerTodas()
        {
            var categorias = await _categoriasService.ObtenerCategorias();

            return Ok(categorias);
        }

        [HttpPost("crearCategoria")]
        public async Task<IActionResult> CrearCategoria(CrearCategoriasDto dto)
        {
            try
            {
                var categoria = new Categoria
                {
                    Name = dto.Name,
                    Slug = dto.Slug,
                    Description = dto.Description,
                    AtributesDefinition = dto.AttributesDefinition.Select(a => new CategoriaAtributo
                    {
                        Key = a.Key,
                        Label = a.Label,
                        Type = a.Type,
                        Required = a.Required,
                        Options = a.Options
                    }).ToList()
                };

                var creacion = await _categoriasService.CrearCategoria(categoria);

                return Ok($"Categoria creada con exito : nombre {categoria.Name}");

            }
            catch (InvalidOperationException e)
            {
                return BadRequest(new { message = e.Message });

            }
        }



        [HttpPut("{id}")]
        public async Task<ActionResult> ActualizarCategoriaById(string id, ActualizarCategoriasDto dto)
        {
            try
            {
                var categoria = new Categoria
                {
                    Name = dto.Name,
                    Slug = dto.Slug,
                    Description = dto.Description,
                    IsActive = true,
                    AtributesDefinition = dto.AttributesDefinition.Select(a => new CategoriaAtributo
                     {
                         Key = a.Key,
                         Label = a.Label,
                         Type = a.Type,
                         Required = a.Required,
                         Options = a.Options
                     }).ToList()
                };

                var updated = await _categoriasService.ActualizarCategoriaById(id, categoria);

                if (updated is null)
                    return NotFound(new { message = "Categoría no encontrada." });

                return Ok("Actualizado con exito");
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }


        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeStatus(string id, [FromBody]bool isActive)
        {
            var changed = await _categoriasService.BorrarCategoria(id, isActive);

            if (!changed)
                return NotFound(new { message = "Categoría no encontrada." });

            return NoContent();
        }



    }
}
