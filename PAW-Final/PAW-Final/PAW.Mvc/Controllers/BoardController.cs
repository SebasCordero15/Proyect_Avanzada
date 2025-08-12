using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAW.Data.MSSql;
using PAW.Models.Entities;
using PAW.Mvc.Models;
using System;
using System.Data.Common;
using System.Diagnostics;

namespace PAW.Mvc.Controllers
{
    public class BoardsController : Controller
    {
        private readonly ProyectDbContext _db;
        public BoardsController(ProyectDbContext db) => _db = db;

        // Listado de tableros (index de funciones)
        public async Task<IActionResult> Index()
        {
            var boards = await _db.Tableros
                .Include(t => t.Lista)
                .Select(t => new BoardsIndexVm
                {
                    Id = t.Id,
                    Titulo = t.Titulo,
                    CantListas = t.Lista.Count,
                    CantTarjetas = t.Lista.SelectMany(l => l.Tarjeta).Count()
                })
                .OrderBy(t => t.Titulo)
                .ToListAsync();

            return View(boards);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(string titulo)
        {
            if (!string.IsNullOrWhiteSpace(titulo))
            {
                _db.Tableros.Add(new Tablero { Titulo = titulo });
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var b = await _db.Tableros.FindAsync(id);
            if (b != null)
            {
                _db.Tableros.Remove(b);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }

    // ViewModel compacto para el índice
    public class BoardsIndexVm
    {
        public int Id { get; set; }
        public string? Titulo { get; set; }
        public int CantListas { get; set; }
        public int CantTarjetas { get; set; }
    }
}