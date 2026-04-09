using MarketplaceApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBDII.Domain.Interface
{
    public interface IUsuarioService
    {

        /// <summary>
        /// Registra un usuario
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="plainPassword"></param>
        /// <returns></returns>
        Task<Usuario> RegisterAsync(Usuario usuario, string plainPassword);

        /// <summary>
        /// Obtiene un usuario por ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Usuario?> GetByIdAsync(string id);
        Task<Usuario?> GetByEmailAsync(string email);
        Task<List<Usuario>> GetAllAsync();
        Task<Usuario?> UpdateAsync(string id, Usuario usuario);
        Task<bool> DeleteAsync(string id);
        Task<Usuario?> ValidateAccessAsync(string email, string plainPassword);
    }
}
