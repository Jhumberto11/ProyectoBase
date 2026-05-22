using MarketplaceApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ProyectoBDII.Application.Service;
using ProyectoBDII.Domain.Interface;
using ProyectoBDII.DTOS.Publicaciones;
using System.Security.Claims;
using System.Text.Json;

namespace ProyectoBDII.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PublicacionController : ControllerBase
    {
        private readonly PublicacionService _publicacionService;
        private readonly CategoriasService _categoriaService;
        private readonly IJwToken _jwtService;

        public PublicacionController(
            PublicacionService publicacionService,
            CategoriasService categoriaService,
            IJwToken jwtService)
        {
            _publicacionService = publicacionService;
            _categoriaService = categoriaService;
            _jwtService = jwtService;
        }


        [HttpPost("crear")]
        public async Task<ActionResult<string>> CrearPublicacion([FromBody] CrearPublicacionDto dto)
        {


            try
            {
                // Obtener el ID del vendedor desde el token
                var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(sellerId))
                    return Unauthorized(new { message = "Token inválido o expirado." });

                // Validar que la categoría existe



                var categoria = await _categoriaService.CheckCategoriaExist(dto.CategoryId);
                if (categoria == null || !categoria.IsActive)
                    return BadRequest(new { message = "Categoría inválida o inactiva." });

                // Crear la nueva publicación
                var publicacion = new Publicacion
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    Price = dto.Price,
                    Currency = dto.Currency,
                    Condition = dto.Condition,
                    Status = "active", // Por defecto "active"
                    CategoryId = categoria.Id,
                    SellerId = sellerId,
                    LocationCity = dto.LocationCity,
                    LocationDepartment = dto.LocationDepartment,
                    Tags = dto.Tags,
                    Attributes = ConvertAttributesBson(dto.Attributes), // Convertir el diccionario a BsonDocument
                    CreatedAt = DateTime.UtcNow
                };

                var createdPublicacion = await _publicacionService.CrearPublicacion(publicacion);

                return createdPublicacion;
            }
            catch (MongoDB.Driver.MongoWriteException ex) when (ex.WriteError.Code == 121)
            {
                // Error de validación de esquema (JSON Schema)
                return BadRequest(new
                {
                    message = "Error de integridad de datos.",
                    details = "La publicación no cumple con las reglas de validación (ej: precio mínimo, moneda inválida o título muy corto)."
                });
            }
            catch (MongoDB.Driver.MongoWriteException ex) when (ex.WriteError.Code == 11000)
            {
                // Por si tienes un índice único en el título por ejemplo
                return Conflict(new { message = "Ya existe una publicación con datos duplicados." });
            }
            catch (Exception ex)
            {
                // Captura cualquier otro error inesperado (como fallos de conexión a Mongo)
                return StatusCode(500, new
                {
                    message = "Ocurrió un error inesperado al procesar la publicación.",
                    details = ex.Message
                });
            }
        }

        [HttpPut("actualizar/{id}")]
        public async Task<ActionResult<Publicacion>> Update(string id, [FromBody] ActualizarPublicacionDto dto)
        {


            
            // Obtener el ID del vendedor desde el token
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(sellerId))
                return Unauthorized(new { message = "Token inválido o expirado." });

            // Buscar la publicación existente
            var existingPublicacion = await _publicacionService.ObtenerPublicacionByID(id);
            if (existingPublicacion == null)
                return NotFound(new { message = "Publicación no encontrada." });

            // Verificar que el vendedor sea el mismo
            if (existingPublicacion.SellerId != sellerId)
                return Unauthorized(new { message = "No tienes permiso para editar esta publicación." });

            // Validar que la categoría exista
           
            // Actualizar la publicación
            existingPublicacion.Title = dto.Title;
            existingPublicacion.Description = dto.Description;
            existingPublicacion.Price = dto.Price;
            existingPublicacion.Tags = dto.Tags;
            existingPublicacion.Condition = dto.Condition;
            
            existingPublicacion.UpdatedAt = DateTime.UtcNow;

            var updated = await _publicacionService.ActualizarPublicacion(id, existingPublicacion);
            if (updated is null)
                return BadRequest(new { message = "No se pudo actualizar la publicación." });

            return Ok(updated);
        }



        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> EliminarPublicacion(string id)
        {
            // Obtener el ID del vendedor desde el token
            var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(sellerId))
                return Unauthorized(new { message = "Token inválido o expirado." });

            // Buscar la publicación existente
            var existingPublicacion = await _publicacionService.ObtenerPublicacionByID(id);
            if (existingPublicacion == null)
                return NotFound(new { message = "Publicación no encontrada." });

            // Verificar que el vendedor sea el mismo
            if (existingPublicacion.SellerId != sellerId)
                return Unauthorized(new { message = "No tienes permiso para eliminar esta publicación." });

            var deleted = await _publicacionService.ElimnarPublicacion(id);
            if (!deleted)
                return BadRequest(new { message = "No se pudo eliminar la publicación." });

            return Ok("Publicacion Eliminada");
        }



        [HttpGet("publicaciones")]
        public  async Task<ActionResult<List<Publicacion>>> MisPublicaciones()
        {
            var myId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(myId == null)
            {
                Unauthorized("Sin Acceso");
            }
            var publicaciones = await _publicacionService.MisPublicaciones(myId);
            if(publicaciones.Count == 0)
            {
                return NotFound("No Tienes Publicaciones");
            }

            var publicacionesDto = publicaciones.Select(p => new PublicacionDto
            {
                Id = p.Id,
                Title = p.Title,
                Descripcion = p.Description,
                Condition = p.Condition,
                Price = p.Price,
                Status = p.Status,
                CreatedAt = p.CreatedAt,

            }).ToList();



            return Ok(publicacionesDto);


        }


        private BsonDocument ConvertAttributesBson(Dictionary<string, object> attributes)
        {
            var bsonDoc = new BsonDocument();

            foreach (var attribute in attributes)
            {
                try
                {
                    Console.WriteLine($"Procesando atributo: {attribute.Key} -> {attribute.Value}");

                    if (attribute.Value is string strValue)
                    {
                        bsonDoc.Add(attribute.Key, strValue);
                    }
                    else if (attribute.Value is bool boolValue)
                    {
                        bsonDoc.Add(attribute.Key, boolValue);
                    }
                    else if (attribute.Value is int intValue)
                    {
                        bsonDoc.Add(attribute.Key, intValue);
                    }
                    else if (attribute.Value is decimal decimalValue)
                    {
                        bsonDoc.Add(attribute.Key, decimalValue);
                    }
                    else if (attribute.Value is double doubleValue)
                    {
                        bsonDoc.Add(attribute.Key, doubleValue);
                    }
                    else if (attribute.Value is JsonElement jsonElement)
                    {
                        // Si el JsonElement es un valor simple (string, number, boolean)
                        if (jsonElement.ValueKind == JsonValueKind.String)
                        {
                            bsonDoc.Add(attribute.Key, jsonElement.GetString());
                        }
                        else if (jsonElement.ValueKind == JsonValueKind.Number)
                        {
                            // Si el valor es numérico, obtenemos el número y lo agregamos
                            if (jsonElement.TryGetInt32(out var intVal))
                            {
                                bsonDoc.Add(attribute.Key, intVal);
                            }
                            else if (jsonElement.TryGetDouble(out var doubleVal))
                            {
                                bsonDoc.Add(attribute.Key, doubleVal);
                            }
                            else if (jsonElement.TryGetDecimal(out var decimalVal))
                            {
                                bsonDoc.Add(attribute.Key, decimalVal);
                            }
                        }
                        else if (jsonElement.ValueKind == JsonValueKind.True)
                        {
                            bsonDoc.Add(attribute.Key, jsonElement.GetBoolean());
                        }
                        else if (jsonElement.ValueKind == JsonValueKind.Object)
                        {
                            // Si es un objeto JSON, lo convertimos a BsonDocument
                            bsonDoc.Add(attribute.Key, BsonDocument.Parse(jsonElement.ToString()));
                        }
                        else
                        {
                            bsonDoc.Add(attribute.Key, BsonNull.Value); // Si es otro tipo de valor
                        }
                    }
                    else if (attribute.Value is Dictionary<string, object> nestedDict)
                    {
                        // Si el valor es un diccionario anidado, convertirlo recursivamente
                        bsonDoc.Add(attribute.Key, ConvertAttributesBson(nestedDict));
                    }
                    else
                    {
                        // Si el valor no es compatible, lo añadimos como null
                        bsonDoc.Add(attribute.Key, BsonNull.Value);
                        Console.WriteLine($"Tipo desconocido para el atributo {attribute.Key}: {attribute.Value}");
                    }
                }
                catch (Exception ex)
                {
                    // Manejar cualquier error durante la conversión
                    bsonDoc.Add(attribute.Key, BsonNull.Value); // Añadir como null si hay un error
                    Console.WriteLine($"Error al convertir el atributo {attribute.Key}: {ex.Message}");
                }
            }

            return bsonDoc;
        }

        [HttpGet("publicaciones_test")]
        public async Task<ActionResult> Alltest([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {

                if (page <= 0) page = 1;
                if (pageSize <= 0 || pageSize > 100) pageSize = 10;

                var (items, total) = await _publicacionService.ObtenerPublicacionesPaginadas(page, pageSize);

                var totalPages = (int)Math.Ceiling((double)total / pageSize);

                return Ok(new
                {
                    Message = "publicaciones recuperadas con éxito",
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
    }
}
