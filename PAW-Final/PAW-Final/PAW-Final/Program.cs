using Microsoft.EntityFrameworkCore;
using PAW.Repository;
using PAW.Repository.Interfaces;
using PAW.Repository.Repositories;
using PAW.Business.Interfaces;
using PAW.Business;
using PAW.Models.Entities;
using PAW.Data.MSSql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🧠 Configuración del DbContext
builder.Services.AddDbContext<ProyectDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositorios (RepositoryBase + específicos)
builder.Services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IComentarioRepository, ComentarioRepository>();
builder.Services.AddScoped<IListumRepository, ListumRepository>();
builder.Services.AddScoped<ITableroRepository, TableroRepository>();
builder.Services.AddScoped<ITarjetumRepository, TarjetumRepository>();

// Negocio (Business)
builder.Services.AddScoped<IUsuarioBusiness, UsuarioBusiness>();
builder.Services.AddScoped<IComentarioBusiness, ComentarioBusiness>();
builder.Services.AddScoped<IListumBusiness, ListumBusiness>();
builder.Services.AddScoped<ITableroBusiness, TableroBusiness>();
builder.Services.AddScoped<ITarjetumBusiness, TarjetumBusiness>();

var app = builder.Build();

// Middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
