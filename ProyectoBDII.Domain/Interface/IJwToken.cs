using MarketplaceApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBDII.Domain.Interface
{
    public interface IJwToken
    {

        string GenerateToken(Usuario user);
    }
}
