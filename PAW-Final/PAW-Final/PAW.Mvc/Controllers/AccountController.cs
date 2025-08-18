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

            var vm = new UsuarioEditViewModel
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo
            };

            return View(vm);
        }

        // POST: Account/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(UsuarioEditViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            // Id desde el claim (seguro)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim)) return RedirectToAction("Login");
            if (!int.TryParse(userIdClaim, out var userId)) return RedirectToAction("Login");

            var client = _http.CreateClient("api");

            // Cargar usuario actual desde el API para rescatar Clave
            var actual = await client.GetFromJsonAsync<UsuarioViewModel>($"api/usuario/{userId}");
            if (actual == null)
            {
                ModelState.AddModelError(string.Empty, "No se pudo cargar el usuario actual.");
                return View(vm);
            }

            // Construir DTO completo para el PUT
            var dto = new UsuarioViewModel
            {
                Id = userId, // Forzar coincidencia con la ruta
                Nombre = vm.Nombre?.Trim() ?? string.Empty,
                Correo = vm.Correo?.Trim() ?? string.Empty,
                Clave = actual.Clave // mantener la clave actual
            };

            // Log en consola de Visual Studio
            System.Diagnostics.Debug.WriteLine(
                $"[PUT Usuario] RutaId={userId}, Body={{ Id={dto.Id}, Nombre='{dto.Nombre}', Correo='{dto.Correo}', Clave={(string.IsNullOrEmpty(dto.Clave) ? "VACÍA" : "***")} }}");

            // Ejecutar PUT
            var res = await client.PutAsJsonAsync($"api/usuario/{userId}", dto);

            if (!res.IsSuccessStatusCode)
            {
                var detail = await res.Content.ReadAsStringAsync();

                ModelState.AddModelError(string.Empty,
                    $"No se pudo actualizar el usuario. " +
                    $"Status: {(int)res.StatusCode} - {res.ReasonPhrase}. " +
                    $"Detalle API: {detail} " +
                    $"(RutaId={userId}, BodyId={dto.Id})");

                return View(vm);
            }

            // Refrescar claims con los datos actualizados
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        new Claim(ClaimTypes.Name, dto.Nombre),
        new Claim(ClaimTypes.Email, dto.Correo),
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity)
            );

            TempData["Success"] = "Usuario actualizado correctamente.";
            return RedirectToAction("Index", "Board");
        }


    }
}


