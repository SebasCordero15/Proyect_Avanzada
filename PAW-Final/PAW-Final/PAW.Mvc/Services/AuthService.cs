using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PAW.Data.MSSql;
using PAW.Models.Entities;
using PAW.Mvc.Services;
using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PAW.Mvc.Services
{
    public class AuthService : IAuthService
    {
        private readonly ProyectDbContext _db;
        private const string CookieName = "auth_token";
        private const int Iter = 100_000;

        public AuthService(ProyectDbContext db) { _db = db; }

        public async Task<Usuario> RegistrarAsync(string nombre, string correo, string password)
        {
            if (await _db.Usuarios.AnyAsync(u => u.Correo == correo))
                throw new InvalidOperationException("El correo ya está registrado.");
            var u = new Usuario { Nombre = nombre, Correo = correo, Clave = HashToString(password) };
            _db.Usuarios.Add(u);
            await _db.SaveChangesAsync();
            return u;
        }

        public async Task<Usuario?> ValidarCredencialesAsync(string correo, string password)
        {
            var u = await _db.Usuarios.FirstOrDefaultAsync(x => x.Correo == correo);
            if (u == null) return null;
            if (VerifyFromString(u.Clave, password) || u.Clave == password)
            {
                if (u.Clave == password) { u.Clave = HashToString(password); await _db.SaveChangesAsync(); }
                return u;
            }
            return null;
        }

        public async Task<string> CrearSesionAsync(Usuario usuario, HttpContext http, int dias = 7)
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
            _db.Sesiones.Add(new Sesion
            {
                UsuarioId = usuario.Id,
                Token = token,
                Creado = DateTime.UtcNow,
                Expira = DateTime.UtcNow.AddDays(dias),
                Ip = http.Connection.RemoteIpAddress?.ToString(),
                UserAgent = http.Request.Headers.UserAgent.ToString()
            });
            await _db.SaveChangesAsync();

            http.Response.Cookies.Append(CookieName, token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(dias)
            });
            return token;
        }

        public async Task<Usuario?> ObtenerUsuarioActualAsync(HttpContext http)
        {
            if (!http.Request.Cookies.TryGetValue(CookieName, out var token) || string.IsNullOrEmpty(token))
                return null;
            var ses = await _db.Sesiones.Include(s => s.Usuario)
                        .FirstOrDefaultAsync(s => s.Token == token && !s.Revocado && s.Expira > DateTime.UtcNow);
            return ses?.Usuario;
        }

        public async Task CerrarSesionActualAsync(HttpContext http)
        {
            if (http.Request.Cookies.TryGetValue(CookieName, out var token))
            {
                var ses = await _db.Sesiones.FirstOrDefaultAsync(s => s.Token == token);
                if (ses != null) { ses.Revocado = true; await _db.SaveChangesAsync(); }
            }
            http.Response.Cookies.Delete(CookieName);
        }

        // ===== helpers de password (PBKDF2-SHA256) =====
        private static string HashToString(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iter, HashAlgorithmName.SHA256);
            return $"PBKDF2${Iter}${Convert.ToBase64String(salt)}${Convert.ToBase64String(pbkdf2.GetBytes(32))}";
        }
        private static bool VerifyFromString(string stored, string password)
        {
            var m = Regex.Match(stored, @"^PBKDF2\$(\d+)\$([^$]+)\$([^$]+)$");
            if (!m.Success) return false;
            int it = int.Parse(m.Groups[1].Value);
            byte[] salt = Convert.FromBase64String(m.Groups[2].Value);
            byte[] hash = Convert.FromBase64String(m.Groups[3].Value);
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, it, HashAlgorithmName.SHA256);
            return CryptographicOperations.FixedTimeEquals(pbkdf2.GetBytes(32), hash);
        }
    }
}

