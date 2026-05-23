using MarketplaceApi.Models;
using MongoDB.Bson;
using ProyectoBDII.Domain.Interface;
using ProyectoBDII.Domain.Models;
using System.Text;

namespace ProyectoBDII.Application.Service
{
    public class PublicacionService
    {
        private readonly IPublicacionesRepository _publicacionRepository;
        private readonly ICategoriasRepository _categoriaRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public PublicacionService(
            IPublicacionesRepository publicacionRepository,
            ICategoriasRepository categoriaRepository,
            IUsuarioRepository usuarioRepository)
        {
            _publicacionRepository = publicacionRepository;
            _categoriaRepository = categoriaRepository;
            _usuarioRepository = usuarioRepository;
        }
        private static void ValidateAttributes(
           BsonDocument attributes,
           List<CategoriaAtributo> definitions)
        {
            foreach (var def in definitions)
            {
                var exists = attributes.Contains(def.Key);

                if (def.Required && !exists)
                    throw new InvalidOperationException($"El atributo '{def.Label}' es requerido.");

                if (!exists)
                    continue;

                var value = attributes[def.Key];

                switch (def.Type.ToLower())
                {
                    case "string":
                        if (!value.IsString)
                            throw new InvalidOperationException($"El atributo '{def.Label}' debe ser texto.");
                        break;

                    case "number":
                        if (!(value.IsInt32 || value.IsInt64 || value.IsDouble || value.IsDecimal128))
                            throw new InvalidOperationException($"El atributo '{def.Label}' debe ser numérico.");
                        break;

                    case "boolean":
                        if (!value.IsBoolean)
                            throw new InvalidOperationException($"El atributo '{def.Label}' debe ser booleano.");
                        break;

                    case "select":
                        if (!value.IsString)
                            throw new InvalidOperationException($"El atributo '{def.Label}' debe ser texto.");

                        if (def.Options is null || !def.Options.Contains(value.AsString))
                            throw new InvalidOperationException($"El atributo '{def.Label}' tiene un valor inválido.");
                        break;

                    default:
                        throw new InvalidOperationException($"Tipo de atributo no soportado: {def.Type}");
                }
            }
        }


        private static string Validador(BsonDocument attributes, List<CategoriaAtributo> definitions)
        {
            // Usaremos un StringBuilder para acumular los errores, si los hay
            var errorMessages = new StringBuilder();

            foreach (var def in definitions)
            {
                var exists = attributes.Contains(def.Key);

                if (def.Required && !exists)
                {
                    errorMessages.AppendLine($"El atributo '{def.Key}' es requerido.");
                }

                if (!exists)
                    continue;

                var value = attributes[def.Key];

                switch (def.Type.ToLower())
                {
                    case "string":
                        if (!value.IsString)
                            errorMessages.AppendLine($"El atributo '{def.Key}' debe ser texto.");
                        break;

                    case "number":
                        if (!(value.IsInt32 || value.IsInt64 || value.IsDouble || value.IsDecimal128))
                            errorMessages.AppendLine($"El atributo '{def.Key}' debe ser numérico.");
                        break;

                    case "boolean":
                        if (!value.IsBoolean)
                            errorMessages.AppendLine($"El atributo '{def.Key}' debe ser booleano.");
                        break;

                    case "select":
                        if (!value.IsString)
                            errorMessages.AppendLine($"El atributo '{def.Key}' debe ser texto.");

                        if (def.Options != null && !def.Options.Contains(value.AsString))

                            errorMessages.AppendLine($"El atributo '{def.Key}' tiene un valor inválido.[{string.Join(", ", def.Options)}]");
                        break;

                    default:
                        errorMessages.AppendLine($"Tipo de atributo no soportado: {def.Type}");
                        break;
                }
            }
            return errorMessages.Length > 0 ? errorMessages.ToString() : string.Empty;
        }

        /// Lado del creador de publicaciones
        public async Task<string> CrearPublicacion(Publicacion publicacion)
        {



            var categoria = await _categoriaRepository.GetByIdAsync(publicacion.CategoryId);
            if (categoria is null || !categoria.IsActive)
                return ("La categoría no existe o está inactiva.");



            var validationError = Validador(publicacion.Attributes, categoria.AtributesDefinition);

            // Si hay un error de validación, devolver el mensaje de error
            if (!string.IsNullOrEmpty(validationError))
                return validationError;

            publicacion.CreatedAt = DateTime.UtcNow;
            publicacion.UpdatedAt = null;
            publicacion.PausedAt = null;

            await _publicacionRepository.CreateAsync(publicacion);
            return "publicacion creada con exito";
        }



        public async Task<List<Publicacion>> MisPublicaciones(string id)
        {
            var publications = await _publicacionRepository.GetMyPublications(id);

            return publications.ToList();
        }



        public async Task<string> ActualizarPublicacion(string id, Publicacion publicacion)
        {
            var existing = await _publicacionRepository.GetByIdAsync(id);
            if (existing is null)
                return null;



            var categoria = await _categoriaRepository.GetByIdAsync(publicacion.CategoryId);
            if (categoria is null || !categoria.IsActive)
                throw new InvalidOperationException("La categoría no existe o está inactiva.");

            ValidateAttributes(publicacion.Attributes, categoria.AtributesDefinition);

            existing.Title = publicacion.Title;
            existing.Description = publicacion.Description;
            existing.Price = publicacion.Price;
            existing.Currency = publicacion.Currency;
            existing.Condition = publicacion.Condition;
            existing.Status = publicacion.Status;
            existing.CategoryId = publicacion.CategoryId;
            existing.LocationCity = publicacion.LocationCity;
            existing.LocationDepartment = publicacion.LocationDepartment;
            existing.Images = publicacion.Images;
            existing.Tags = publicacion.Tags;
            existing.Attributes = publicacion.Attributes;
            existing.UpdatedAt = DateTime.UtcNow;

            var updated = await _publicacionRepository.UpdateAsync(existing);
            return "Actualizado con exito";
        }

        public async Task<bool> ElimnarPublicacion(string id)
        {
            return await _publicacionRepository.DeleteAsync(id);
        }

        public async Task<Publicacion?> ObtenerPublicacionByID(string id)
        {
            return await _publicacionRepository.GetByIdAsync(id);

        }






        //Lado de Vista de publicaciones(Marketplace)
        public async Task<List<Publicacion>> TodasLasPublicaciones()
        {
            return await _publicacionRepository.GetAllAsync();
        }

        public async Task<List<Publicacion>> ObtenerPublicacionesPorCategoria(string name)
        {
            var c = await _categoriaRepository.GetByName(name);


            return await _publicacionRepository.GetByCategoryIdAsync(c.Id);
        }
        public async Task<List<Publicacion>> PrecioMenorA(decimal price)
        {
            return await _publicacionRepository.GetLowerThan(price);
        }

        //filtro por sellerId
        public async Task<List<Publicacion>> ObtenerPublicacionesPorVendedor(string Id)
        {
            return await _publicacionRepository.GetBySellerIdAsync(Id);
        }

        public async Task<(List<Publicacion> Items, long Total)> ObtenerPublicacionesPaginadas(int page, int pageSize)
        {
            return await _publicacionRepository.ObtenerPublicacionesPaginadas(page, pageSize);
        }

    }
}
