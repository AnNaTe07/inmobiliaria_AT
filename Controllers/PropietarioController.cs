using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Policy = "AdminEmpleado")]
        public IActionResult Index()
        {
            return View(_repo.ObtenerTodos());
        }

        [Authorize(Policy = "AdminEmpleado")]
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
        [Authorize(Policy = "AdminEmpleado")]
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

        [Authorize(Policy = "AdminEmpleado")]
        public IActionResult Guardar(Propietario propietario)
        {
            if (propietario.Id == 0)
            {
                _repo.Alta(propietario);
                TempData["SuccessMessage"] = "Propietario generado correctamente.";
            }
            else
            {
                _repo.Modificar(propietario);
                TempData["SuccessMessage"] = "Datos de propietario guardados correctamente.";
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policy = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            _repo.Baja(id);
            TempData["SuccessMessage"] = "Datos de propietario eliminados correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}