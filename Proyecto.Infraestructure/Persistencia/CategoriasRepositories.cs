using MarketplaceApi.Models;
using MongoDB.Driver;
using ProyectoBDII.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBDII.Infraestructure.Persistencia
{
    public class CategoriasRepositories : ICategoriasRepository
    {
        private readonly IMongoCollection<Categoria> _categorias;

        public CategoriasRepositories(IMongoDatabase database)
        {
            _categorias = database.GetCollection<Categoria>("Categorias");
        }


        public async Task<string> CreateAsync(Categoria categoria)
        {
            await _categorias.InsertOneAsync(categoria);
            return $"Categoria : {categoria.Name} creada con exito";
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _categorias.DeleteOneAsync(c => c.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
           return await _categorias.Find(c => c.Name == name).AnyAsync();
        }

        public async Task<bool> ExistsBySlugAsync(string slug)
        {
           return  await _categorias.Find(c => c.Slug == slug).AnyAsync();
        }

        public async Task<List<Categoria>> GetAllAsync()
        {
            return await _categorias.Find(_ => true).ToListAsync();
        }

        public async Task<Categoria?> GetByIdAsync(string id)
        {
            return await _categorias.Find(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Categoria?> GetByName(string name)
        {
            var result = await _categorias.Find(c => c.Name == name).FirstOrDefaultAsync();
            

            return result;
        }

        public async Task<Categoria?> GetBySlugAsync(string slug)
        {
            return await _categorias.Find(c => c.Slug == slug).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(Categoria categoria)
        {
            categoria.UpdatedAt = DateTime.UtcNow;
            var result = await _categorias.ReplaceOneAsync(c => c.Id == categoria.Id, categoria);
            return result.MatchedCount > 0;
        }


        

    }
}
