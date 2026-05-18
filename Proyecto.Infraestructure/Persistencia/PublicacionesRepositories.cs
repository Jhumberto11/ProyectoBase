using MarketplaceApi.Models;
using MongoDB.Driver;
using ProyectoBDII.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBDII.Infraestructure.Persistencia
{
    public class PublicacionesRepositories : IPublicacionesRepository
    {

        private readonly IMongoCollection<Publicacion> _publicaciones;

        public PublicacionesRepositories(IMongoDatabase database)
        {
            _publicaciones = database.GetCollection<Publicacion>("Publicaciones");
        }



        public async Task CreateAsync(Publicacion publicacion)
        {
            
            await _publicaciones.InsertOneAsync(publicacion);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _publicaciones.DeleteOneAsync(p => p.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<List<Publicacion>> GetAllAsync()
        {
            return await _publicaciones.Find(_ => true).ToListAsync();
        }

        public async Task<List<Publicacion>> GetByCategoryIdAsync(string categoryId)
        {
            return await _publicaciones.Find(p => p.CategoryId == categoryId).ToListAsync();
        }

        public async Task<Publicacion?> GetByIdAsync(string id)
        {
           return  await _publicaciones.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Publicacion>> GetBySellerIdAsync(string sellerId)
        {
            return await _publicaciones.Find(p => p.SellerId == sellerId).ToListAsync();
        }

        public async Task<bool> UpdateAsync(Publicacion publicacion)
        {
            var result = await _publicaciones.ReplaceOneAsync(p => p.Id == publicacion.Id, publicacion);
            return result.MatchedCount > 0;
        }

        public async Task<List<Publicacion>> GetMyPublications(string id)
        {
            return await _publicaciones.Find(p=>p.SellerId==id).ToListAsync();
        }

        public async Task<List<Publicacion>> GetLowerThan(decimal price)
        {
            var result = await _publicaciones.Find(p => p.Price < price).ToListAsync();

            return result;
        }

        public async Task<(List<Publicacion> Items, long Total)> ObtenerPublicacionesPaginadas(int page, int pageSize)
        {
            var total = await _publicaciones.CountDocumentsAsync(_ => true);
            var items = await _publicaciones.Find(_ => true)
                                         .Skip((page - 1) * pageSize)
                                         .Limit(pageSize)
                                         .ToListAsync();
            return (items, total);
        }

    }
}
