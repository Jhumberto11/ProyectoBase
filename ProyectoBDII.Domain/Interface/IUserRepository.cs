using MarketplaceApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBDII.Domain.Interface
{
    public interface IUsuarioRepository
    {

        /// <summary>
        /// Obtiene un usuario por id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Usuario?> GetByIdAsync(string id);

        /// <summary>
        /// Obtiene un usuario por email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<Usuario?> GetByEmailAsync(string email);

        /// <summary>
        /// Regresa todos los usurios
        /// </summary>
        /// <returns></returns>
        Task<List<Usuario>> GetAllAsync();
        /// <summary>
        /// Crea un nuevo Usuario
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        Task CreateAsync(Usuario usuario);

        /// <summary>
        /// Actualiza un usuario
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(Usuario usuario);

        /// <summary>
        /// Elimina por ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(string id);

        /// <summary>
        /// Confirma si existe un usuario por email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<bool> ExistsByEmailAsync(string email);
    }
}
