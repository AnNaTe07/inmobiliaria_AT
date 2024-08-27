using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using inmobiliaria_AT.Models;

namespace inmobiliaria_AT.Controllers
{
    public class InmuebleController : Controller
    {
        private readonly ILogger<InmuebleController> _logger;
        private readonly RepositorioInmueble _repo;
        public InmuebleController(ILogger<InmuebleController> logger, RepositorioInmueble repo)
        {
            _logger = logger;
            _repo = repo;
        }
        public IActionResult Index()
        {
            var inmuebles=_repo.ObtenerTodos();
            return View(inmuebles);
        }

        public IActionResult Editar(int id)
        {
            if(id==0)
            return View(new Inmueble());
            else
            {
                var inmueble=_repo.ObtenerPorId(id);
                 if (inmueble == null)
        {
            // Si el inmueble no se encuentra, devuelve una vista de error o una página de "No Encontrado".
            return NotFound();
        }
                return View(inmueble);
            }
        }
        public IActionResult Detalle(int id)
        {
            if(id==0)
            return View();
            else
            {
                var inmueble=_repo.ObtenerPorId(id);
                return View(inmueble);
            }
        }
        [HttpPost]
        public IActionResult Guardar(Inmueble inmueble)
        {            
            if(inmueble.Id==0)
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
        public IActionResult Baja(int id)
        {
            _repo.Baja(id);
            TempData["SuccessMessage"] = "Datos de inmueble eliminados correctamente.";
            return RedirectToAction("Index");
        }
    }
}