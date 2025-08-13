using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PAW.Models.ViewModels;
using System.Net.Http.Json;
using System.Security.Claims;

namespace PAW.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _http;

        public AccountController(IHttpClientFactory http) => _http = http;

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginVm { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var client = _http.CreateClient("api");

            try
            {
                // Buscar usuario por correo en tu API
                var usuario = await client.GetFromJsonAsync<UsuarioViewModel>(
                    $"api/usuario/correo/{Uri.EscapeDataString(vm.Correo)}");

                // Validar contraseña (simple)
                if (usuario is null || usuario.Clave != vm.Clave)
                {
                    ModelState.AddModelError(string.Empty, "Credenciales inválidas.");
                    return View(vm);
                }

                // Crear identidad y claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.Nombre),
                    new Claim(ClaimTypes.Email, usuario.Correo)
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Firmar al usuario
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity)
                );

                // Redirigir
                var url = string.IsNullOrWhiteSpace(vm.ReturnUrl)
                    ? Url.Action("Index", "Board")!
                    : vm.ReturnUrl;

                return Redirect(url);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "No se pudo contactar el API. Verifica que esté en ejecución.");
                return View(vm);
            }
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View(new UsuarioViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UsuarioViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var client = _http.CreateClient("api");

            try
            {
                // POST al API para crear usuario
                var res = await client.PostAsJsonAsync("api/usuario", vm);

                if (!res.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "No se pudo registrar el usuario. Intente nuevamente.");
                    return View(vm);
                }

                // Después de registrar, redirigir a login
                return RedirectToAction("Login");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "No se pudo contactar el API. Verifica que esté en ejecución.");
                return View(vm);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
