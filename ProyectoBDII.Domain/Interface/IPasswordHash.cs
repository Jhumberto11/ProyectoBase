using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBDII.Domain.Interface
{
    public interface IPasswordHash
    {
       
        string HashPassword(string password);
        bool VerifyPassword(string hashedPassword, string password);
    }
}

