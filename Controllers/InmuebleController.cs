using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
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
        private readonly RepositorioContrato _repoContrato;
        public InmuebleController(ILogger<InmuebleController> logger, RepositorioInmueble repo, RepositorioTipo repositorioTipo, RepositorioPropietario repoPropietario, RepositorioContrato repoContrato)
        {
            _logger = logger;
            _repo = repo;
            _repositorioTipo = repositorioTipo;
            _repoPropietario = repoPropietario;
            _repoContrato = repoContrato;
        }

        [Authorize(Policy = "AdminEmpleado")]
        public IActionResult Index()
        {
            var inmuebles = _repo.ObtenerTodos();
            return View(inmuebles);
        }

        [Authorize(Policy = "AdminEmpleado")]
        public IActionResult Editar(int id)
        {
            // Configuración de opciones para el uso del inmueble
            var usos = Enum.GetValues(typeof(UsoInmueble))
                    .Cast<UsoInmueble>()
                    .Select(u => new SelectListItem
                    {
                        Value = ((int)u).ToString(), // entero que representa el enum
                        Text = u.ToString() // texto del enum
                    }).ToList();

            // Agregar la opción "Seleccione uso de inmueble" al principio
            usos.Insert(0, new SelectListItem
            {
                Value = "", // Valor vacío para la opción por defecto
                Text = "Seleccione uso de inmueble"
            });

            // obtengo los tipos de inmueble
            var tipos = _repositorioTipo.ObtenerTodos();

            // creo un SelectList y selecciono el valor del tipo de inmueble a modificar
            //ViewBag.Tipo = new SelectList(tipos, "Id", "Descripcion");
            // Crear una lista con la opción por defecto para Tipo
            var listaTipos = new List<SelectListItem>
    {
        new SelectListItem { Value = "", Text = "Seleccione tipo de inmueble" }
    };

            listaTipos.AddRange(tipos.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Descripcion
            }));


            // obtengo la lista de propietarios
            var propietarios = _repoPropietario.ObtenerTodos();


            // Crear una lista con la opción por defecto
            var listaPropietarios = new List<SelectListItem>
    {
        new SelectListItem { Value = "", Text = "Seleccione propietario" }
    };

            listaPropietarios.AddRange(propietarios.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.NombreCompleto
            }));

            if (id == 0)
            {
                ViewBag.Usos = new SelectList(usos, "Value", "Text");

                // Para un nuevo inmueble, la opción por defecto es la inicial
                ViewBag.Tipo = new SelectList(listaTipos, "Value", "Text");
                ViewBag.Propietario = new SelectList(listaPropietarios, "Value", "Text");
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
                ViewBag.Usos = new SelectList(usos, "Value", "Text", inmueble.Uso.ToString());
                // Crea un SelectList y establece el valor seleccionado
                ViewBag.Tipo = new SelectList(listaTipos, "Value", "Text", inmueble.TipoId.ToString());
                // creo un SelectList y selecciono el propietario a modificar
                ViewBag.Propietario = new SelectList(listaPropietarios, "Value", "Text", inmueble.IdPropietario.ToString());

                return View(inmueble);
            }
        }
        [Authorize(Policy = "AdminEmpleado")]
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
        [Authorize(Policy = "AdminEmpleado")]
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

        [Authorize(Policy = "Administrador")]

        public IActionResult Eliminar(int id)

        {
            _repo.Baja(id);
            TempData["SuccessMessage"] = "Datos de inmueble eliminados correctamente.";
            return RedirectToAction("Index");
        }
        [Authorize(Policy = "AdminEmpleado")]
        // Método para obtener los inmuebles disponibles por propietario

        [Authorize(Policy = "AdminEmpleado")]
        public IActionResult Disponibles(int IdPropietario)
        {
            if (IdPropietario <= 0)
            {
                return BadRequest("El ID del propietario es inválido.");
            }
            var inmuebles = _repo.ObtenerDisponibles(IdPropietario);
            return PartialView("_InmueblesPartial", inmuebles);  // Uso PartialView para actualizar una sección
        }
        // Método para obtener los inmuebles disponibles

        [Authorize(Policy = "AdminEmpleado")]
        public IActionResult DisponiblesTotales()
        {
            var inmuebles = _repo.ObtenerDisponiblesTotales();
            return PartialView("_InmueblesIndexPartial", inmuebles);
        }

        // Método para obtener los inmuebles no disponibles por propietario
        [Authorize(Policy = "AdminEmpleado")]
        public IActionResult NoDisponibles(int IdPropietario)
        {
            _logger.LogInformation("NoDisponibles llamado con IdPropietario: {IdPropietario}", IdPropietario);

            if (IdPropietario <= 0)
            {
                return BadRequest("El ID del propietario es inválido.");
            }
            try
            {
                var inmuebles = _repo.ObtenerNoDisponibles(IdPropietario);
                return PartialView("_InmueblesPartial", inmuebles); // Uso PartialView para actualizar una sección
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener inmuebles no disponibles.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [Authorize(Policy = "AdminEmpleado")]
        public IActionResult NoDisponiblesTotales()
        {
            var inmuebles = _repo.ObtenerNoDisponiblesTotales();
            return PartialView("_InmueblesIndexPartial", inmuebles);
        }

        [Authorize(Policy = "AdminEmpleado")]
        [HttpGet]
        public IActionResult ListaPorPropietario()
        {
            // Obtener la lista de propietarios
            var propietarios = _repoPropietario.ObtenerTodos();
            var listaPropietarios = new List<SelectListItem>
    {
        new SelectListItem { Value = "", Text = "Seleccione propietario" }
    };

            listaPropietarios.AddRange(propietarios.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Nombre} {p.Apellido}"
            }));

            // Pasar la lista de propietarios a la vista
            ViewBag.Propietarios = listaPropietarios;
            ViewBag.SelectedPropietarioId = null;
            ViewBag.SelectedPropietarioNombre = null;
            // Pasar una lista vacía de inmuebles
            var inmuebles = new List<Inmueble>();

            return View(inmuebles);
        }

        [HttpPost]
        [Authorize(Policy = "AdminEmpleado")]
        public IActionResult ListaPorPropietario(int? IdPropietario)
        {
            _logger.LogInformation("FiltrarPorPropietario llamado con IdPropietario: {IdPropietario}", IdPropietario);

            var propietarios = _repoPropietario.ObtenerTodos();
            var listaPropietarios = new List<SelectListItem>
    {
        new SelectListItem { Value = "", Text = "Seleccione propietario" }
    };

            listaPropietarios.AddRange(propietarios.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Nombre} {p.Apellido}"
            }));

            var inmuebles = IdPropietario.HasValue ? _repo.BuscarPorPropietario(IdPropietario.Value) : new List<Inmueble>();
            var propietarioSeleccionado = propietarios.FirstOrDefault(p => p.Id == IdPropietario);

            ViewBag.Propietarios = listaPropietarios;
            ViewBag.SelectedPropietarioId = IdPropietario?.ToString();
            ViewBag.SelectedPropietarioNombre = propietarioSeleccionado != null ? $"{propietarioSeleccionado.Nombre} {propietarioSeleccionado.Apellido}" : null;

            _logger.LogInformation("Retornando vista ListaPorPropietario con {InmueblesCount} inmuebles", inmuebles.Count);

            return View(inmuebles);
        }


        // Método para suspender inmueble
        [HttpPost]
        [Authorize(Policy = "AdminEmpleado")]
        public IActionResult SuspenderInmueble(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "Debe iniciar sesión para realizar esta acción." });
            }

            if (!User.IsInRole("Administrador") && !User.IsInRole("Empleado"))
            {
                return Forbid();
            }

            _logger.LogInformation("SuspenderInmueble llamado con Id: {Id}", id);

            // Llama al repositorio para suspender el inmueble
            var resultado = _repo.SuspenderInmueble(id);

            if (resultado)
            {
                _logger.LogInformation("Inmueble con Id: {Id} suspendido con éxito.", id);
                return Json(new { success = true });
            }
            else
            {
                _logger.LogWarning("No se pudo suspender el inmueble con Id: {Id}.", id);
                return Json(new { success = false });
            }
        }

        // Método para reactivar inmueble
        [HttpPost]
        [Authorize(Policy = "AdminEmpleado")]
        public IActionResult ReactivarInmueble(int id)
        {
            _logger.LogInformation("ReactivarInmueble llamado con Id: {Id}", id);

            // Llama al repositorio para reactivar el inmueble
            var resultado = _repo.ReactivarInmueble(id);

            if (resultado)
            {
                _logger.LogInformation("Inmueble con Id: {Id} reactivado con éxito.", id);
                return Json(new { success = true });
            }
            else
            {
                _logger.LogWarning("No se pudo reactivar el inmueble con Id: {Id}.", id);
                return Json(new { success = false });
            }
        }
        [Authorize(Policy = "AdminEmpleado")]

        public IActionResult ObtenerInmueblesTodos(string filtro)
        {
            List<Inmueble> inmuebles;

            switch (filtro)
            {
                case "disponible":
                    inmuebles = _repo.ObtenerDisponiblesTotales();
                    break;
                case "noDisponible":
                    inmuebles = _repo.ObtenerNoDisponiblesTotales();
                    break;
                default:
                    inmuebles = _repo.ObtenerTodos();
                    break;
            }

            return PartialView("_InmueblesIndexPartial", inmuebles);
        }
        [Authorize(Policy = "AdminEmpleado")]
        public IActionResult ObtenerInmuebles(string filtro, int idPropietario)
        {
            List<Inmueble> inmuebles;

            switch (filtro)
            {
                case "inmueblesDisponibles":
                    inmuebles = _repo.ObtenerDisponibles(idPropietario);
                    break;
                case "inmueblesNoDisponibles":
                    inmuebles = _repo.ObtenerNoDisponibles(idPropietario);
                    break;
                default:
                    inmuebles = _repo.BuscarPorPropietario(idPropietario);
                    break;
            }

            return PartialView("_InmueblesPartial", inmuebles);
        }

        [Authorize(Policy = "AdminEmpleado")]

        public IActionResult verificarDisponibilidad(int inmuebleId, DateTime fechaInicio, DateTime fechaFin)
        {
            bool isAvailable = _repoContrato.disponiblePorFechas(inmuebleId, fechaInicio, fechaFin);
            return Json(isAvailable); // Retornar JSON para uso en JavaScript si es necesario
        }
        [Authorize(Policy = "AdminEmpleado")]

        [HttpGet]
        public IActionResult BuscarInmuebles()
        {   // Obtengo los tipos de inmueble
            var tipos = _repositorioTipo.ObtenerTodos();

            // Creo una lista con la opción por defecto para Tipo
            var listaTipos = new List<SelectListItem>
    {
        new SelectListItem { Value = "", Text = "Seleccione tipo de inmueble" }
    };
            if (tipos != null && tipos.Any())
            {
                listaTipos.AddRange(tipos.Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Descripcion
                }));
            }

            ViewBag.Tipo = new SelectList(listaTipos, "Value", "Text");

            // Pasar un modelo vacío o nulo al cargar la vista de búsqueda
            return View(new List<Inmueble>());
        }
        [Authorize(Policy = "AdminEmpleado")]
        [HttpPost]
        public IActionResult BuscarInmuebles(string uso, string tipo, int? ambientes, decimal? precioMinimo, decimal? precioMaximo, DateTime? fechaInicio, DateTime? fechaFin)
        {

            // Guardo las fechas en ViewBag
            ViewBag.FechaInicio = fechaInicio;
            ViewBag.FechaFin = fechaFin;

            // Verificar que las fechas estén seleccionadas
            if (!fechaInicio.HasValue || !fechaFin.HasValue || string.IsNullOrEmpty(uso))
            {
                ModelState.AddModelError(string.Empty, "Debe seleccionar las fechas y el uso para realizar la búsqueda.");
                return PartialView("_PartialBuscar", new List<Inmueble>()); // Retornar una lista vacía
            }
            // Obtener todos los inmuebles
            var inmuebles = _repo.ObtenerTodos().AsQueryable();
            // Filtrar según los parámetros
            if (!string.IsNullOrEmpty(uso))
            {
                inmuebles = inmuebles.Where(i => i.Uso.ToString() == uso);
            }
            Console.WriteLine($"Inmuebles después de filtrar por uso: {inmuebles.Count()}");

            if (!string.IsNullOrEmpty(tipo))
            {
                inmuebles = inmuebles.Where(i => i.TipoId.ToString() == tipo);
            }
            Console.WriteLine($"Inmuebles después de filtrar por tipo: {inmuebles.Count()}");

            if (ambientes.HasValue)
            {
                inmuebles = inmuebles.Where(i => i.Ambientes == ambientes.Value);
            }
            Console.WriteLine($"Inmuebles después de filtrar por v: {inmuebles.Count()}");

            if (precioMinimo.HasValue)
            {
                inmuebles = inmuebles.Where(i => i.Precio >= precioMinimo.Value);
            }
            Console.WriteLine($"Inmuebles después de filtrar por n: {inmuebles.Count()}");

            if (precioMaximo.HasValue)
            {
                inmuebles = inmuebles.Where(i => i.Precio <= precioMaximo.Value);
            }
            Console.WriteLine($"Inmuebles después de filtrar por d: {inmuebles.Count()}");

            if (fechaInicio.HasValue && fechaFin.HasValue)
            {
                inmuebles = inmuebles.Where(i => _repoContrato.disponiblePorFechas(i.Id, fechaInicio.Value, fechaFin.Value));
            }
            Console.WriteLine($"Inmuebles después de filtrar por t: {inmuebles.Count()}");

            // Verificar que los parámetros no sean nulos
            Console.WriteLine($"Uso: {uso}, Tipo: {tipo}, Ambientes: {ambientes}, Precio Mínimo: {precioMinimo}, Precio Máximo: {precioMaximo}, Fecha Inicio: {fechaInicio}, Fecha Fin: {fechaFin}");
            // Retornar la lista filtrada a la vista
            return PartialView("_PartialBuscar", inmuebles.ToList());

        }
    }
}