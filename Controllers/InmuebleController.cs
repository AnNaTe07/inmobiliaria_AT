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


        public IActionResult Index()
        {
            var inmuebles = _repo.ObtenerTodos();
            return View(inmuebles);
        }



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

        public IActionResult Disponibles()
        {
            var inmuebles = _repo.ObtenerDisponibles();
            return View(inmuebles);
        }


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
            ViewBag.SelectedPropietarioNombre = null; // Asegúrate de pasar el nombre como null en el GET

            // Pasar una lista vacía de inmuebles
            var inmuebles = new List<Inmueble>();

            return View(inmuebles);
        }

        [HttpPost]
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

    }
}