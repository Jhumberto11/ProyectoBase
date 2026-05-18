using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoBDII.Application.Service;
using ProyectoBDII.Domain.Interface;
using ProyectoBDII.DTOS.Publicaciones;
using ProyectoBDII.DTOS.PublicacionMarketplace;

namespace ProyectoBDII.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MarketplaceController : ControllerBase
    {
        private readonly PublicacionService _publicacionService;
        private readonly CategoriasService _categoriaService;
        private readonly IJwToken _jwtService;

        public MarketplaceController(
            PublicacionService publicacionService,
            
            IJwToken jwtService)
        {

            _publicacionService = publicacionService;
            
            _jwtService = jwtService;
        }

        [HttpGet("publicaciones")]
        public async Task<List<PublicacionMarketDto>> All()
        {
            var publicaciones = await _publicacionService.TodasLasPublicaciones();
            List<PublicacionMarketDto> publiDto = new List<PublicacionMarketDto>();

            foreach(var p in publicaciones)
            {
                var publicacionDto = new PublicacionMarketDto()
                {
                    Id = p.Id,
                    Title = p.Title,
                    Condition = p.Condition,
                    Status = p.Status,
                    Descripcion = p.Description,
                    Price = p.Price,
                    CreatedAt = p.CreatedAt,
                    SelleId = p.SellerId,
                    CategoryId = p.CategoryId,
                    Tags = p.Tags

                };
                publiDto.Add(publicacionDto);
            }




             return  publiDto;
        }



        [HttpGet("publicaciones/{categoryName}")]
        public async Task<List<PublicacionMarketDto>> AllByCategory(string categoryName)
        {
            var publicaciones = await _publicacionService.ObtenerPublicacionesPorCategoria(categoryName);
            List<PublicacionMarketDto> publiDto = new List<PublicacionMarketDto>();

            foreach (var p in publicaciones)
            {
                var publicacionDto = new PublicacionMarketDto()
                {
                    Id = p.Id,
                    Title = p.Title,
                    Condition = p.Condition,
                    Status = p.Status,
                    Descripcion = p.Description,
                    Price = p.Price,
                    CreatedAt = p.CreatedAt,
                    SelleId = p.SellerId,
                    CategoryId = p.CategoryId,
                    Tags = p.Tags


                };
                publiDto.Add(publicacionDto);
            }


            return publiDto;
        }




        [HttpGet("publicaciones/lt-{price:decimal}")]
        public async Task<List<PublicacionMarketDto>> PriceLowerThan(decimal price)
        {

            var publicaciones = await _publicacionService.PrecioMenorA(price);
            List<PublicacionMarketDto> publiDto = new List<PublicacionMarketDto>();

            foreach (var p in publicaciones)
            {
                var publicacionDto = new PublicacionMarketDto()
                {
                    Id = p.Id,
                    Title = p.Title,
                    Condition = p.Condition,
                    Status = p.Status,
                    Descripcion = p.Description,
                    Price = p.Price,
                    CreatedAt = p.CreatedAt,
                    SelleId = p.SellerId,
                    CategoryId = p.CategoryId,
                    Tags = p.Tags


                };
                publiDto.Add(publicacionDto);
            }


            return publiDto;

        }

        [HttpGet("publicaciones/sellerId-{sellerId}")]
        public async Task<List<PublicacionMarketDto>> AllbySellerId(string sellerId)
        {

            var publicaciones = await _publicacionService.ObtenerPublicacionesPorVendedor(sellerId);
            List<PublicacionMarketDto> publiDto = new List<PublicacionMarketDto>();

            foreach (var p in publicaciones)
            {
                var publicacionDto = new PublicacionMarketDto()
                {
                    Id = p.Id,
                    Title = p.Title,
                    Condition = p.Condition,
                    Status = p.Status,
                    Descripcion = p.Description,
                    Price = p.Price,
                    CreatedAt = p.CreatedAt,
                    SelleId = p.SellerId,
                    CategoryId = p.CategoryId,
                    Tags = p.Tags


                };
                publiDto.Add(publicacionDto);
            }


            return publiDto;

        }














    }
}
