using MarketplaceApi.Models;
using ProyectoBDII.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBDII.Application.Service
{
    public class CategoriasService

    {
        readonly ICategoriasRepository _repository;
        public CategoriasService(ICategoriasRepository r) 
        {
            _repository = r;
        }

        public async Task<List<Categoria>> ObtenerCategorias()
        {
            return await _repository.GetAllAsync();
        }


        public async Task<Categoria?> CheckCategoriaExist(string name)
        {
            return await _repository.GetByName(name);
        }

       

        public async Task<Categoria> CrearCategoria(Categoria categoria)
        {
            categoria.Name = categoria.Name.Trim();
            categoria.Slug = categoria.Slug.Trim().ToLower();

            var existsByName = await _repository.ExistsByNameAsync(categoria.Name);
            if (existsByName)
                throw new InvalidOperationException("Ya existe una categoría con ese nombre.");

            var existsBySlug = await _repository.ExistsBySlugAsync(categoria.Slug);
            if (existsBySlug)
                throw new InvalidOperationException("Ya existe una categoría con ese slug.");

            categoria.CreatedAt = DateTime.UtcNow;
            categoria.UpdatedAt = null;
            categoria.IsActive = true;

            await _repository.CreateAsync(categoria);
            return categoria;
        }

        public async Task<Categoria?> ActualizarCategoriaById(string id, Categoria categoria)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing is null)
                return null;

            var newName = categoria.Name.Trim();
            var newSlug = categoria.Slug.Trim().ToLower();


            //Comparacion con los nuevos datos
            if (!string.Equals(existing.Name, newName, StringComparison.OrdinalIgnoreCase))
            {
                var existsByName = await _repository.ExistsByNameAsync(newName);
                if (existsByName)
                    throw new InvalidOperationException("Ya existe una categoría con ese nombre.");
            }

            if (!string.Equals(existing.Slug, newSlug, StringComparison.OrdinalIgnoreCase))
            {
                var existsBySlug = await _repository.ExistsBySlugAsync(newSlug);
                if (existsBySlug)
                    throw new InvalidOperationException("Ya existe una categoría con ese slug.");
            }

            existing.Name = newName;
            existing.Slug = newSlug;
            existing.Description = categoria.Description;
            existing.IsActive = categoria.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;

            var updated = await _repository.UpdateAsync(existing);
            return updated ? existing : null;
        }

        

        public async Task<bool> BorrarCategoria(string id, bool isActive)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing is null)
                return false;

            existing.IsActive = isActive;
            existing.UpdatedAt = DateTime.UtcNow;

            return await _repository.UpdateAsync(existing);
        }




    }
}
