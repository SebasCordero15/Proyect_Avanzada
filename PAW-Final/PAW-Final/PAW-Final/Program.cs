using Microsoft.EntityFrameworkCore;
using PAW.Repository;
using PAW.Repository.Interfaces;
using PAW.Repository.Repositories;
using PAW.Business.Interfaces;
using PAW.Business;
using PAW.Models.Entities;
using PAW.Data.MSSql;

var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ProyectDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repos
builder.Services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

builder.Services.AddScoped<IListumRepository, ListumRepository>();
builder.Services.AddScoped<ITableroRepository, TableroRepository>();
builder.Services.AddScoped<ITarjetumRepository, TarjetumRepository>();

// Business
builder.Services.AddScoped<IUsuarioBusiness, UsuarioBusiness>();

builder.Services.AddScoped<IListumBusiness, ListumBusiness>();
builder.Services.AddScoped<ITableroBusiness, TableroBusiness>();
builder.Services.AddScoped<ITarjetumBusiness, TarjetumBusiness>();




const string CorsDev = "CorsDev";
builder.Services.AddCors(opt =>
{
    opt.AddPolicy(CorsDev, p =>
    {
        p.WithOrigins(
             "https://localhost:7218", 
             "http://localhost:7218",
             "https://localhost:7151", 
             "http://localhost:5000"
          )
         .AllowAnyHeader()
         .AllowAnyMethod();
        
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseCors(CorsDev);

app.UseAuthorization();

app.MapControllers();


app.MapGet("/ping", () => Results.Ok("pong"));

app.Run();
