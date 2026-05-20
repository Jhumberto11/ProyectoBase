using MarketplaceApi.Models;
using ProyectoBDII.Domain.Interface;

namespace ProyectoBDII.Application.Service
{
    public class UserService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHash _passwordHasherService;

        public UserService(IUsuarioRepository _uR, IPasswordHash _pS)
        {
            _usuarioRepository = _uR;
            _passwordHasherService = _pS;
        }
        public async Task<Usuario> RegisterAsync(Usuario usuario, string plainPassword)
        {



            var email = usuario.Email.Trim().ToLower();

            var exists = await _usuarioRepository.ExistsByEmailAsync(email);
            if (exists)
                throw new InvalidOperationException("El correo ya está registrado.");

            usuario.Email = email;
            usuario.PasswordHash = _passwordHasherService.HashPassword(plainPassword);
            usuario.Role = string.IsNullOrWhiteSpace(usuario.Role) ? "user" : usuario.Role;
            usuario.Status = string.IsNullOrWhiteSpace(usuario.Status) ? "active" : usuario.Status;
            usuario.CreatedAt = DateTime.UtcNow;
            usuario.UpdatedAt = null;

            await _usuarioRepository.CreateAsync(usuario);
            return usuario;
        }
        public async Task<Usuario?> ValidateAccessAsync(string email, string plainPassword)
        {
            var normalizedEmail = email.Trim().ToLower();

            var usuario = await _usuarioRepository.GetByEmailAsync(normalizedEmail);
            if (usuario is null)
                return null;

            var isValid = _passwordHasherService.VerifyPassword(
                usuario.PasswordHash,
                plainPassword
            );

            return usuario;
        }
        public Task<bool> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Usuario>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Usuario?> GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<Usuario?> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }



        public Task<Usuario?> UpdateAsync(string id, Usuario usuario)
        {
            throw new NotImplementedException();
        }


    }
}
