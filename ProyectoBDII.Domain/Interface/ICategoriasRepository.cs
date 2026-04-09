using MarketplaceApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBDII.Domain.Interface
{
    public interface ICategoriasRepository
    {

        Task<List<Categoria>> GetAllAsync();
        Task<Categoria?> GetByIdAsync(string id);
        Task<Categoria?> GetBySlugAsync(string slug);
        Task<string> CreateAsync(Categoria categoria);
        Task<bool> UpdateAsync(Categoria categoria);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsByNameAsync(string name);
        Task<bool> ExistsBySlugAsync(string slug);
        Task<Categoria?> GetByName(string name);

    }
}
