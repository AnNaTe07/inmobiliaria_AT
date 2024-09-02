using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using inmobiliaria_AT.Models;

namespace inmobiliaria_AT.Controllers
{
    public class TipoController : Controller
    {
        private readonly ILogger<TipoController> _logger;
        private readonly RepositorioTipo _repo;

        public TipoController(ILogger<TipoController> logger, RepositorioTipo repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public IActionResult Index()
        {
            var tipo = _repo.ObtenerTodos();
            return View(tipo);
        }

        [HttpPost]
        public IActionResult AgregarTipo(string descripcion)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
            {
                return BadRequest("La descripción no puede estar vacía.");
            }

            try
            {
                var nuevoTipo = new Tipo
                {
                    Descripcion = descripcion
                };

                var resultado = _repo.Alta(nuevoTipo);

                if (resultado > 0)
                {
                    return Ok(); // Tipo agregado exitosamente
                }
                else
                {
                    return Conflict("El tipo de inmueble ya existe."); // Conflicto: el tipo ya existe
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones generales
                return StatusCode(500, "Error interno del servidor.");
            }
        }


        public IActionResult Detalle(int id)
        {
            if (id == 0)
                return View();
            else
            {
                var tipo = _repo.ObtenerPorId(id);
                return View(tipo);
            }
        }

        public IActionResult Editar(int id)
        {
            if (id == 0)
                return View();
            else
            {
                var tipo = _repo.ObtenerPorId(id);
                TempData["CurrentId"] = id; // Guarda el ID actual en TempData
                return View(tipo);
            }
        }

        [HttpPost]
        public IActionResult Guardar(Tipo tipo)
        {
            if (tipo.Id == 0)
            {
                _repo.Alta(tipo);
                TempData["SuccessMessage"] = "Tipo de inmueble generado correctamente.";
            }
            else
            {
                _repo.Modificar(tipo);
                TempData["SuccessMessage"] = "Datos de tipo de inmueble modificados correctamente.";
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Eliminar(int Id, int InmuebleId)
        {
            if (Id <= 0)
            {
                TempData["ErrorMessage"] = "ID de tipo de inmueble no válido.";
                // Redirige de vuelta a la vista de edición del inmueble
                return RedirectToAction("Editar", "Inmueble", new { id = InmuebleId });
            }

            try
            {
                _repo.Baja(Id);
                TempData["SuccessMessage"] = "Tipo de inmueble eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al eliminar el tipo de inmueble: {ex.Message}";
            }

            // Redirige de vuelta a la vista de edición del inmueble
            return RedirectToAction("Editar", "Inmueble", new { id = InmuebleId });
        }

    }
}