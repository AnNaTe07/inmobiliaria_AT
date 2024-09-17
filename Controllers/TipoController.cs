using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using inmobiliaria_AT.Models;

namespace inmobiliaria_AT.Controllers
{
    public class TipoController : Controller
    {
        private readonly ILogger<TipoController> _logger;
        private readonly RepositorioTipo _repoTipo;

        public TipoController(ILogger<TipoController> logger, RepositorioTipo repo)
        {
            _logger = logger;
            _repoTipo = repo;
        }

        public IActionResult Index()
        {
            var tipo = _repoTipo.ObtenerTodos();
            return View(tipo);
        }

        [HttpPost]
        [Authorize(Policy = "AdminEmpleado")]
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

                var resultado = _repoTipo.Alta(nuevoTipo);

                if (resultado > 0)
                {
                    TempData["SuccessMessage"] = "Tipo de inmueble agregado exitosamente.";
                    return Ok();
                }
                else
                {
                    //TempData["ErrorMessage"] = "El tipo de inmueble ya existe.";
                    //return RedirectToAction("Editar", "Inmueble");
                    return Conflict("El tipo de inmueble ya existe.");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error interno del servidor.";
                // Manejo de excepciones generales
                return StatusCode(500, "Error interno del servidor.");
            }
        }


        [HttpPost]
        [Authorize(Policy = "AdminEmpleado")]
        public IActionResult Modificar(Tipo tipo)
        {
            try


            {
                if (tipo == null || string.IsNullOrEmpty(tipo.Descripcion))
                {
                    TempData["ErrorMessage"] = "Datos inválidos.";
                    return RedirectToAction("Editar", "Inmueble");
                }

                // Verifica si el tipo existe
                Tipo tipoExistente = _repoTipo.ObtenerPorId(tipo.Id);
                if (tipoExistente == null)
                {
                    TempData["ErrorMessage"] = "Tipo de inmueble no encontrado.";
                    return RedirectToAction("Editar", "Inmueble");
                }





                // Verifica si ya existe otro tipo con la misma descripción
                bool existeTipoConDescripcion = _repoTipo.ExisteTipo(tipo.Descripcion);
                if (existeTipoConDescripcion && tipoExistente.Descripcion != tipo.Descripcion)
                {
                    TempData["ErrorMessage"] = "El tipo de inmueble ya existe.";
                    return RedirectToAction("Editar", "Inmueble");
                }

                // Intenta modificar el tipo existente
                int resultado = _repoTipo.Modificar(tipo);
                if (resultado > 0)
                {
                    TempData["SuccessMessage"] = "Tipo de inmueble modificado correctamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se pudo modificar el tipo de inmueble.";
                }
                return RedirectToAction("Editar", "Inmueble");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["ErrorMessage"] = "Error interno del servidor.";
                return RedirectToAction("Editar", "Inmueble");
            }

        }

        [HttpPost]
        [Authorize(Policy = "Administrador")]
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
                _repoTipo.Baja(Id);
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