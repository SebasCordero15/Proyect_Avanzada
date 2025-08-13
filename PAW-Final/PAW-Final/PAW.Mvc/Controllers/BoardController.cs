using Microsoft.AspNetCore.Mvc;
using PAW.Models.ViewModels;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace PAW.Mvc.Controllers
{
    

    [Authorize]
    public class BoardController : Controller
    {
        // GET: /Board/Index/{id?}
        // id = Id del tablero
        [HttpGet]
        public IActionResult Index(int id = 1)
        {
            // Mock para ver la UI. Luego lo cambiamos por llamadas a tu API.
            var vm = new BoardPageViewModel
            {
                Tablero = new TableroViewModel
                {
                    Id = id,
                    Titulo = "Proyecto PAW - Board de ejemplo",
                    FechaCreacion = DateTime.Now,
                    UsuarioId = 1
                },
                Columnas = new List<ListaConTarjetas>
                {
                    new ListaConTarjetas
                    {
                        Lista = new ListumViewModel{ Id=101, Titulo="Por hacer", Orden=1, TableroId=id },
                        Tarjetas = new List<TarjetumViewModel>{
                            new TarjetumViewModel{ Id=1001, Titulo="Configurar API", Descripcion="Endpoints CRUD", ListaId=101 },
                            new TarjetumViewModel{ Id=1002, Titulo="Diseñar UI", Descripcion="Layout de columnas", ListaId=101 },
                        }
                    },
                    new ListaConTarjetas
                    {
                        Lista = new ListumViewModel{ Id=102, Titulo="En progreso", Orden=2, TableroId=id },
                        Tarjetas = new List<TarjetumViewModel>{
                            new TarjetumViewModel{ Id=1003, Titulo="Servicios MVC", Descripcion="Service layer hacia API", ListaId=102 },
                        }
                    },
                    new ListaConTarjetas
                    {
                        Lista = new ListumViewModel{ Id=103, Titulo="Hecho", Orden=3, TableroId=id },
                        Tarjetas = new List<TarjetumViewModel>{
                            new TarjetumViewModel{ Id=1004, Titulo="VM compuestos", Descripcion="BoardPageViewModel listo", ListaId=103 },
                        }
                    }
                }
            };

            return View("Index", vm);
        }
    }
}
