using MarketplaceApi.Models;
using Microsoft.AspNetCore.Identity;
using ProyectoBDII.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBDII.Infraestructure.Security
{
    public class PasswordHasherService : IPasswordHash
    {
        private readonly PasswordHasher<Usuario> _passwordHasher = new();

        public string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(null!, password);
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(
                null!,
                hashedPassword,
                providedPassword
            );

            return result == PasswordVerificationResult.Success
                || result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
