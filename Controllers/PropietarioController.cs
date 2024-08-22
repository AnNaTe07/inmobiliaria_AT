using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using inmobiliaria_AT.Models;

namespace inmobiliaria_AT.Controllers
{
    public class PropietarioController : Controller
    {
        private readonly ILogger<PropietarioController> _logger;
        private readonly RepositorioPropietario _repo;

        // Actualiza el constructor para usar la inyección de dependencias
        public PropietarioController(ILogger<PropietarioController> logger, RepositorioPropietario repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public IActionResult Index()
        {
            return View(_repo.ObtenerTodos());
        }

        public IActionResult Editar(int id)
        {
            if (id == 0)
                return View();
            else
            {
                var propietario = _repo.ObtenerPorId(id);
                return View(propietario);
            }
        }
        public IActionResult Detalle(int id)
        {
            if (id == 0)
                return View();
            else
            {
                var propietario = _repo.ObtenerPorId(id);
                return View(propietario);
            }
        }
        [HttpPost]
        public IActionResult Guardar(Propietario propietario)
        {
            if (propietario.Id == 0)
            {
                _repo.Alta(propietario);
                TempData["SuccessMessage"] = "Propietario guardado correctamente.";
            }
            else
            {
                _repo.Modificar(propietario);
                TempData["SuccessMessage"] = "Propietario guardado correctamente.";
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Eliminar(int id)
        {
            _repo.Baja(id);
            TempData["SuccessMessage"] = "Propietario eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}