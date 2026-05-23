using MarketplaceApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProyectoBDII.Domain.Interface;
using ProyectoBDII.Settings.JwSettings;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProyectoBDII.Infraestructure.Security
{
    public class JwTokens : IJwToken
    {


        IConfiguration _configuration;

        public JwTokens(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(Usuario user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email , user.Email),
                new Claim(ClaimTypes.Name, $"{user.Name} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.Role)


            };


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.Now.AddDays(7);

            var token = new JwtSecurityToken(
                 issuer: _configuration["Issuer"],
                audience: _configuration["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);



        }
    }
}
