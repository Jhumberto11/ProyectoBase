using MarketplaceApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBDII.Domain.Interface
{
    public interface IPublicacionesRepository
    {
        /// <summary>
        /// busca la categoria por id
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        Task<List<Publicacion>> GetByCategoryIdAsync(string categoryId);

        /// <summary>
        /// Obtiene Toda las publiaciones
        /// </summary>
        /// <returns></returns>
        Task<List<Publicacion>> GetAllAsync();

        /// <summary>
        /// Obtiene Publicacion por ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Publicacion?> GetByIdAsync(string id);

        /// <summary>
        /// obtiene publicaciones por vendedor
        /// </summary>
        /// <param name="sellerId"></param>
        /// <returns></returns>
        Task<List<Publicacion>> GetBySellerIdAsync(string sellerId);

        /// <summary>
        /// Crea Publicacion
        /// </summary>
        /// <param name="listing"></param>
        /// <returns></returns>
        Task CreateAsync(Publicacion listing);

        /// <summary>
        /// AActuaaliza una publicacion
        /// </summary>
        /// <param name="listing"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(Publicacion listing);
        /// <summary>
        /// Elimina una publicaion
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(string id);

    }
}
