using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using inmobiliaria_AT.Models;

namespace inmobiliaria_AT.Controllers
{
    public class InmuebleController : Controller
    {
        private readonly ILogger<InmuebleController> _logger;
        private readonly RepositorioInmueble _repo;
        private readonly RepositorioTipo _repositorioTipo;
        private readonly RepositorioPropietario _repoPropietario;
        public InmuebleController(ILogger<InmuebleController> logger, RepositorioInmueble repo, RepositorioTipo repositorioTipo, RepositorioPropietario repoPropietario)
        {
            _logger = logger;
            _repo = repo;
            _repositorioTipo = repositorioTipo;
            _repoPropietario = repoPropietario;
        }

        public IActionResult Crear()
        {
            //obtengo la lista de tipos
            var tipos = _repositorioTipo.ObtenerTodos();

            //uso viewBag para pasar los tipos a la vista
            ViewBag.Tipos = tipos;

            //creo una instancia de inmueble para la vista
            var inmueble = new Inmueble();

            return View(inmueble);
        }

        public IActionResult EditarTipo(int id)
        {
            // Obtiene el inmueble a editar
            var inmueble = _repo.ObtenerPorId(id);

            // Obtiene la lista de tipos de inmuebles
            var tipos = _repositorioTipo.ObtenerTodos();

            // Usa ViewBag para pasar los tipos a la vista
            ViewBag.Tipos = tipos;

            return View(inmueble);
        }


        public IActionResult Index()
        {
            var inmuebles = _repo.ObtenerTodos();
            return View(inmuebles);
        }

        public IActionResult Editar(int id)
        {
            // obtengo los tipos de inmueble
            var tipos = _repositorioTipo.ObtenerTodos();

            // creo un SelectList y selecciono el valor del tipo de inmueble a modificar
            ViewBag.Tipo = new SelectList(tipos, "Id", "Descripcion");


            // obtengo la lista de propietarios
            var propietarios = _repoPropietario.ObtenerTodos();

            // creo un SelectList y selecciono el propietario a modificar
            ViewBag.Propietario = new SelectList(propietarios, "Id", "NombreCompleto");

            if (id == 0)
            {
                return View(new Inmueble());
            }
            else
            {
                var inmueble = _repo.ObtenerPorId(id);
                if (inmueble == null)
                {
                    // Si el inmueble no se encuentra, devuelve una vista de error (página de "No Encontrado")
                    return NotFound();
                }
                // Crea un SelectList y establece el valor seleccionado
                ViewBag.Tipo = new SelectList(tipos, "Id", "Descripcion", inmueble.TipoId);
                ViewBag.Propietario = new SelectList(propietarios, "Id", "NombreCompleto", inmueble.IdPropietario);


                // Prepara la lista de SelectListItem y marca la opción seleccionada
                // var tipoItems = tipos.Select(t => new SelectListItem
                // {
                //     Value = t.Id.ToString(),
                //     Text = t.Descripcion,
                //     Selected = t.Id == inmueble.TipoId // Marca la opción como seleccionada
                // }).ToList();
                //
                // ViewBag.Tipo = tipoItems;
                return View(inmueble);
            }
        }
        public IActionResult Detalle(int id)
        {
            if (id == 0)
                return View();
            else
            {
                var inmueble = _repo.ObtenerPorId(id);
                return View(inmueble);
            }
        }
        [HttpPost]
        public IActionResult Guardar(Inmueble inmueble)
        {
            if (inmueble.Id == 0)
            {
                _repo.Alta(inmueble);
                TempData["SuccessMessage"] = "Inmueble generado correctamente.";
            }
            else
            {
                _repo.Modificar(inmueble);
                TempData["SuccessMessage"] = "Datos de inmueble modificados correctamente.";
            }
            return RedirectToAction("Index");
        }
        public IActionResult Eliminar(int id)
        {
            _repo.Baja(id);
            TempData["SuccessMessage"] = "Datos de inmueble eliminados correctamente.";
            return RedirectToAction("Index");
        }
    }
}