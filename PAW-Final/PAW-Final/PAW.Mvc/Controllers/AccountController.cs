using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using PAW.Mvc.Services;

namespace PAW.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _auth;
        public AccountController(IAuthService auth) { _auth = auth; }

        public IActionResult Login(string? returnUrl = null) => View(new LoginVm { ReturnUrl = returnUrl });

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = await _auth.ValidarCredencialesAsync(vm.Correo, vm.Clave);
            if (user == null)
            {
                ModelState.AddModelError("", "Credenciales inválidas.");
                return View(vm);
            }
            await _auth.CrearSesionAsync(user, HttpContext);
            if (!string.IsNullOrWhiteSpace(vm.ReturnUrl) && Url.IsLocalUrl(vm.ReturnUrl))
                return Redirect(vm.ReturnUrl);

            return RedirectToAction("Index", "Boards");
        }

        public IActionResult Register(string? returnUrl = null) => View(new RegisterVm { ReturnUrl = returnUrl });

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVm vm)
        {
            if (!ModelState.IsValid) return View(vm);
            if (vm.Clave != vm.ConfirmarClave)
            {
                ModelState.AddModelError(nameof(RegisterVm.ConfirmarClave), "Las contraseñas no coinciden.");
                return View(vm);
            }

            try
            {
                var u = await _auth.RegistrarAsync(vm.Nombre, vm.Correo, vm.Clave);
                await _auth.CrearSesionAsync(u, HttpContext);
                if (!string.IsNullOrWhiteSpace(vm.ReturnUrl) && Url.IsLocalUrl(vm.ReturnUrl))
                    return Redirect(vm.ReturnUrl);
                return RedirectToAction("Index", "Boards");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _auth.CerrarSesionActualAsync(HttpContext);
            return RedirectToAction("Login");
        }
    }

    public class LoginVm
    {
        [Required, EmailAddress] public string Correo { get; set; } = "";
        [Required, DataType(DataType.Password)] public string Clave { get; set; } = "";
        public string? ReturnUrl { get; set; }
    }

    public class RegisterVm
    {
        [Required, MaxLength(120)] public string Nombre { get; set; } = "";
        [Required, EmailAddress] public string Correo { get; set; } = "";
        [Required, MinLength(6), DataType(DataType.Password)] public string Clave { get; set; } = "";
        [Required, DataType(DataType.Password)] public string ConfirmarClave { get; set; } = "";
        public string? ReturnUrl { get; set; }
    }
}

