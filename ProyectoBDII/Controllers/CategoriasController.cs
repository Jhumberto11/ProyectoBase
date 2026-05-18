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
        public async Task<ActionResult> ObtenerTodas([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {

                if (page <= 0) page = 1;
                if (pageSize <= 0 || pageSize > 100) pageSize = 10;

                var (items, total) = await _categoriasService.ObtenerCategoriasPaginadas(page, pageSize);

                var totalPages = (int)Math.Ceiling((double)total / pageSize);

                return Ok(new
                {
                    Message = "Categorías recuperadas con éxito",
                    TotalRecords = total,
                    TotalPages = totalPages,
                    CurrentPage = page,
                    PageSize = pageSize,
                    Data = items
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error inesperado al recuperar categorías.",
                    details = ex.Message
                });
            }
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
            catch (MongoDB.Driver.MongoWriteException ex) when (ex.WriteError.Code == 11000)
            {
                // Este bloque captura errores de duplicidad (ej: el Slug ya existe)
                return Conflict(new
                {
                    message = "Error de duplicidad.",
                    details = "Ya existe una categoría con el mismo nombre o slug. Intente con uno diferente."
                });
            }
            catch (MongoDB.Driver.MongoWriteException ex) when (ex.WriteError.Code == 121)
            {
                // Captura el error de validación de esquema (JSON Schema en Mongo)
                return BadRequest(new
                {
                    message = "Error de validación en la base de datos.",
                    details = "Los datos de la categoría no cumplen con las reglas definidas en el servidor."
                });
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(new { message = e.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Ocurrió un error inesperado al crear la categoría.",
                    details = ex.Message
                });
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
