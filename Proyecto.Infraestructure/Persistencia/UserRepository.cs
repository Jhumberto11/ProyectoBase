using MarketplaceApi.Models;
using MongoDB.Driver;
using ProyectoBDII.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBDII.Infraestructure.Persistencia
{
    public class UserRepository : IUsuarioRepository
    {
        private readonly IMongoCollection<Usuario> _usuarios;

        public UserRepository(IMongoDatabase db)
        {
            _usuarios = db.GetCollection<Usuario>("Usuarios");
        }


        public async Task CreateAsync(Usuario usuario)
        {
            await _usuarios.InsertOneAsync(usuario);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _usuarios.DeleteOneAsync(u => u.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _usuarios.Find(u => u.Email == email).AnyAsync();
        }

        public Task<List<Usuario>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _usuarios.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<Usuario?> GetByIdAsync(string id)
        {
            return await _usuarios.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(Usuario usuario)
        {
            usuario.UpdatedAt = DateTime.UtcNow;

            var result = await _usuarios.ReplaceOneAsync(
                u => u.Id == usuario.Id,
                usuario
            );

            return result.MatchedCount > 0;
        }
    }
}
