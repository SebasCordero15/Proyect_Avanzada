using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// 1?? Agregar soporte para controladores con vistas
builder.Services.AddControllersWithViews();


// 2?? Configurar HttpClient para consumir tu API
builder.Services.AddHttpClient("api", c =>
{
    c.BaseAddress = new Uri("https://localhost:7218/"); // Ajusta puerto/base a tu PAW.API
})
.ConfigurePrimaryHttpMessageHandler(() =>
    new HttpClientHandler
    {
        // Para certificados locales de desarrollo
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    });

// 3?? Configurar autenticación por cookies
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/Account/Login";
        o.LogoutPath = "/Account/Logout";
        o.AccessDeniedPath = "/Account/Login";
        o.SlidingExpiration = true;
        // o.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

var app = builder.Build();

// 4?? Middleware para manejo de errores
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// 5?? Middleware general
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// 6?? Autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// 7?? Mapear rutas por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// 8?? Ejecutar la aplicación
app.Run();
