using MarketplaceApi.Models;
using MongoDB.Bson;
using ProyectoBDII.Domain.Interface;
using ProyectoBDII.Domain.Models;
using System;
using System.Collections.Generic;
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


        public async Task<Publicacion> CrearPublicacion(Publicacion publicacion)
        {

            var c = await _categoriaRepository.GetByName(publicacion.CategoryId);

            var categoria = await _categoriaRepository.GetByIdAsync(c.Id);
            if (categoria is null || !categoria.IsActive)
                throw new InvalidOperationException("La categoría no existe o está inactiva.");

          

            ValidateAttributes(publicacion.Attributes, categoria.AtributesDefinition);

            publicacion.CreatedAt = DateTime.UtcNow;
            publicacion.UpdatedAt = null;
            publicacion.PausedAt = null;

            await _publicacionRepository.CreateAsync(publicacion);
            return publicacion;
        }


        public async Task<List<Publicacion>> TodasLasPublicaciones()
        {
            return await _publicacionRepository.GetAllAsync();
        }

        public async Task<List<Publicacion>> ObtenerPorCategoria(string name)
        {
            var c = await _categoriaRepository.GetByName(name);


            return await _publicacionRepository.GetByCategoryIdAsync(c.Id);
        }





    }
}
