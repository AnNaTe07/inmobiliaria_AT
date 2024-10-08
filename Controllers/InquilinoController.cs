using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using inmobiliaria_AT.Models;

namespace inmobiliaria_AT.Controllers
{
    public class InquilinoController : Controller
    {
        private readonly ILogger<InquilinoController> _logger;
        private readonly RepositorioInquilino _repo;

        public InquilinoController(ILogger<InquilinoController> logger, RepositorioInquilino repo)
        {
            _logger = logger;
            _repo = repo;
        }


        [Authorize(Policy = "AdminEmpleado")]
        public IActionResult Index()
        {
            var inquilinos = _repo.ObtenerTodos();
            return View(inquilinos);
        }
        [Authorize(Policy = "AdminEmpleado")]
        public IActionResult Editar(int id)
        {
            if (id == 0)
                return View();
            else
            {
                var inquilino = _repo.ObtenerPorId(id);
                return View(inquilino);
            }
        }
        [Authorize(Policy = "AdminEmpleado")]
        public IActionResult Detalle(int id)
        {
            if (id == 0)
                return View();
            else
            {
                var inquilino = _repo.ObtenerPorId(id);
                return View(inquilino);
            }
        }
        [HttpPost]
        [Authorize(Policy = "AdminEmpleado")]
        public IActionResult Guardar(Inquilino inquilino)
        {
            if (inquilino.Id == 0)
            {
                _repo.Alta(inquilino);
                TempData["SuccessMessage"] = "Inquilino generado correctamente.";
            }
            else
            {
                _repo.Modificar(inquilino);
                TempData["SuccessMessage"] = "Datos de inquilino modificados correctamente.";
            }
            return RedirectToAction("Index");
        }

        [Authorize(Policy = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            _repo.Baja(id);
            TempData["SuccessMessage"] = "Datos de inquilino eliminados correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}