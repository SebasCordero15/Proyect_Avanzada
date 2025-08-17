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
                var usuario = await client.GetFromJsonAsync<UsuarioViewModel>(
                    $"api/usuario/correo/{Uri.EscapeDataString(vm.Correo)}");

                if (usuario is null || usuario.Clave != vm.Clave)
                {
                    ModelState.AddModelError(string.Empty, "Credenciales inválidas.");
                    return View(vm);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.Nombre),
                    new Claim(ClaimTypes.Email, usuario.Correo)
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity)
                );

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
                var res = await client.PostAsJsonAsync("api/usuario", vm);

                if (!res.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "No se pudo registrar el usuario. Intente nuevamente.");
                    return View(vm);
                }

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

        // GET: Account/Edit
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return RedirectToAction("Login");

            var client = _http.CreateClient("api");
            var usuario = await client.GetFromJsonAsync<UsuarioViewModel>($"api/usuario/{userIdClaim}");
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // POST: Account/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UsuarioViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var client = _http.CreateClient("api");

            // Traer usuario actual de la API para asegurarnos que exista
            var usuarioActual = await client.GetFromJsonAsync<UsuarioViewModel>($"api/usuario/{vm.Id}");
            if (usuarioActual == null)
                return NotFound();

            // Crear objeto para actualizar solo nombre y correo, manteniendo clave actual
            var usuarioParaActualizar = new UsuarioViewModel
            {
                Id = usuarioActual.Id,
                Nombre = vm.Nombre,
                Correo = vm.Correo,
                Clave = usuarioActual.Clave // <--- mantenemos la clave existente
            };

            // Enviar PUT a la API
            var res = await client.PutAsJsonAsync($"api/usuario/{vm.Id}", usuarioParaActualizar);
            if (!res.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "No se pudo actualizar el usuario.");
                return View(vm);
            }

            // Actualizar claim de Name si cambió el nombre
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var nameClaim = claimsIdentity.FindFirst(ClaimTypes.Name);
            if (nameClaim != null && nameClaim.Value != vm.Nombre)
            {
                claimsIdentity.RemoveClaim(nameClaim);
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, vm.Nombre));
            }

            // Redirigir al Board
            return RedirectToAction("Index", "Board");
        }
    }
}


