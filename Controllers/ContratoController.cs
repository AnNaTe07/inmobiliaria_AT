using inmobiliaria_AT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace inmobiliaria_AT.Controllers;

public class ContratoController : Controller
{
    private readonly ILogger<ContratoController> _logger;
    private readonly RepositorioContrato _repo;
    private readonly RepositorioPropietario _repoProp;
    private readonly RepositorioInmueble _repoInmueble;
    private readonly RepositorioInquilino _repoInquilino;
    public ContratoController(ILogger<ContratoController> logger, RepositorioContrato repo, RepositorioPropietario repoProp, RepositorioInmueble repoInmueble, RepositorioInquilino repoInquilino)
    {
        _logger = logger;
        _repo = repo;
        _repoInmueble = repoInmueble;
        _repoProp = repoProp;
        _repoInquilino = repoInquilino;

        _repo.vigenciaContrato();

    }

    /* public IActionResult Index()
      {
          var propietarios = _repoProp.ObtenerTodos();
          var inmuebles = _repoInmueble.ObtenerTodos();
          var contratos = _repo.ObtenerTodos();
          ViewBag.Propietarios = new SelectList(propietarios, "Id", "NombreCompleto");
          ViewBag.Inmuebles = new SelectList(inmuebles, "Id", "Direccion");
          return View(contratos);
      }

  

  
    public IActionResult Index(int? propietarioId, int? inmuebleId)
    {
        //obtiene todos los propietarios e inmuebles para cargar en los selects
        var propietarios = _repoProp.ObtenerTodos();
        var inmuebles = _repoInmueble.ObtenerTodos();

        // obtiene todos los contratos y luego filtrarlos segun los parametros recibidos
        var contratos = _repo.ObtenerTodos();

        //aplica los filtros si hay valores seleccionados
        if (propietarioId.HasValue)
        {
            contratos = contratos.Where(c => c.Prop.Id == propietarioId.Value).ToList();
        }

        if (inmuebleId.HasValue)
        {
            contratos = contratos.Where(c => c.Inmu.Id == inmuebleId.Value).ToList();
        }

        // Envia la lista filtrada a la vista
        ViewBag.Propietarios = new SelectList(propietarios, "Id", "NombreCompleto");
        ViewBag.Inmuebles = new SelectList(inmuebles, "Id", "Direccion");

        return View(contratos);
    }

*/
    public IActionResult Index(int? propietarioId, int? inmuebleId, DateTime? fechaDesde, DateTime? fechaHasta)
    {
        // Obtiene todos los propietarios e inmuebles para cargar en los selects
        var propietarios = _repoProp.ObtenerTodos();
        var inmuebles = _repoInmueble.ObtenerTodos();

        // pbtiene todos los contratos
        var contratos = _repo.ObtenerTodos();

        // aplica los filtros solo si se selecciona alguna opción
        if (propietarioId.HasValue)
        {
            contratos = contratos.Where(c => c.Prop.Id == propietarioId.Value).ToList();
        }

        if (inmuebleId.HasValue)
        {
            contratos = contratos.Where(c => c.Inmu.Id == inmuebleId.Value).ToList();
        }

        // aplica el filtro por fechas
        if (fechaDesde.HasValue && fechaHasta.HasValue)
        {
            // Filtro en conjunto: Contratos donde el rango de fechas está dentro del rango especificado
            contratos = contratos.Where(c => c.FechaInicio.Date >= fechaDesde.Value.Date && c.FechaFin.Date <= fechaHasta.Value.Date).ToList();
        }
        else if (fechaDesde.HasValue)
        {
            // Filtro por fecha desde: Contratos donde elinicio es despues o igual a la fecha desde
            contratos = contratos.Where(c => c.FechaInicio.Date >= fechaDesde.Value.Date).ToList();
        }
        else if (fechaHasta.HasValue)
        {
            // Filtro por fecha hasta: Contratos dond fin es antes o igual a la fecha hasta
            contratos = contratos.Where(c => c.FechaFin.Date <= fechaHasta.Value.Date).ToList();
        }

        // Envia la lista filtrada a la vista
        ViewBag.Propietarios = new SelectList(propietarios, "Id", "NombreCompleto");
        ViewBag.Inmuebles = new SelectList(inmuebles, "Id", "Direccion");

        return View(contratos);
    }


    public IActionResult ListaVencidos(int? propietarioId, int? inmuebleId, DateTime? fechaDesde, DateTime? fechaHasta)
    {
        // Obtiene todos los propietarios e inmuebles para cargar en los selects
        var propietarios = _repoProp.ObtenerTodos();
        var inmuebles = _repoInmueble.ObtenerTodos();

        // pbtiene todos los contratos
        var contratos = _repo.ObtenerVencidos();

        // aplica los filtros solo si se selecciona alguna opción
        if (propietarioId.HasValue)
        {
            contratos = contratos.Where(c => c.Prop.Id == propietarioId.Value).ToList();
        }

        if (inmuebleId.HasValue)
        {
            contratos = contratos.Where(c => c.Inmu.Id == inmuebleId.Value).ToList();
        }

        // aplica el filtro por fechas
        if (fechaDesde.HasValue && fechaHasta.HasValue)
        {
            // Filtro en conjunto: Contratos donde el rango de fechas está dentro del rango especificado
            contratos = contratos.Where(c => c.FechaInicio.Date >= fechaDesde.Value.Date && c.FechaFin.Date <= fechaHasta.Value.Date).ToList();
        }
        else if (fechaDesde.HasValue)
        {
            // Filtro por fecha desde: Contratos donde elinicio es despues o igual a la fecha desde
            contratos = contratos.Where(c => c.FechaInicio.Date >= fechaDesde.Value.Date).ToList();
        }
        else if (fechaHasta.HasValue)
        {
            // Filtro por fecha hasta: Contratos dond fin es antes o igual a la fecha hasta
            contratos = contratos.Where(c => c.FechaFin.Date <= fechaHasta.Value.Date).ToList();
        }

        // Envia la lista filtrada a la vista
        ViewBag.Propietarios = new SelectList(propietarios, "Id", "NombreCompleto");
        ViewBag.Inmuebles = new SelectList(inmuebles, "Id", "Direccion");

        return View(contratos);
    }



    public IActionResult Editar(int id)
    {

        //obtengo la lista de propietarios
        var propietarios = _repoProp.ObtenerTodos();
        var inmuebles = _repoInmueble.ObtenerDisponiblesTotales();
        var inquilinos = _repoInquilino.ObtenerTodos();

        if (id > 0)
        {
            //Obtengo el inmueble que esta en el contrato 

            Contrato cont = _repo.ObtenerPorId(id);
            Inmueble inmuContra = _repoInmueble.ObtenerPorId(cont.Inmu.Id);

            //Agrego el inmueble que ya tiene el contrato, porque si no solo traigo los no alguilados
            inmuebles.Add(inmuContra);

        }

        ViewBag.Propietarios = new SelectList(propietarios, "Id", "NombreCompleto");
        ViewBag.Inmuebles = new SelectList(inmuebles, "Id", "Direccion");
        ViewBag.Inquilinos = new SelectList(inquilinos, "Id", "NombreCompleto");

        if (id == 0)
        {
            return View();
        }
        else
        {

            var contrato = _repo.ObtenerPorId(id);
            return View(contrato);
        }
    }


    public IActionResult Renovar(int id, Contrato contratoActualizado)
{
    // Si el contratoActualizado tiene valores (es decir, viene de un formulario POST)
    if (contratoActualizado != null && contratoActualizado.Id != 0)
    {
        // Aquí debes validar si el contrato está correctamente actualizado
        if (ModelState.IsValid)
        {
            _repo.Renovar(contratoActualizado);
            TempData["SuccessMessage"] = "Contrato renovado correctamente.";
            return RedirectToAction("Index"); // Redirige a la lista de contratos
        }
        // Si la validación falla, volvemos a cargar el formulario
        return View(contratoActualizado);
    }
    // Si es la primera vez que se carga el formulario (GET)
    Contrato contrato = _repo.ObtenerPorId(id);
    if (contrato == null)
    {
        return NotFound();
    }

    // Obtengo el inquilino asociado al contrato
    var inquilino = _repoInquilino.ObtenerPorId(contrato.Inqui.Id);
  
    // Obtengo el inmueble asociado al contrato
    Inmueble inmueble = _repoInmueble.ObtenerPorId(contrato.Inmu.Id);
   
    // Paso los datos a la vista usando ViewBag
    ViewBag.Inmueble = inmueble;
    ViewBag.Inquilino = inquilino;
    ViewBag.Contrato = contrato;

    return View(contrato); // Muestra la vista con el contrato sin guardar aún.
}


    public IActionResult Detalle(int id)
    {
        if (id == 0)
        {
            return View();
        }
        else
        {
            var contrato = _repo.ObtenerPorId(id);
            if (contrato.FechaFin != default(DateTime))
            {
                var fechaFin = contrato.FechaFin;
                var fechaActual = DateTime.Now;

                // Calcula la diferencia en meses y días
                var resultado = CalcularMesesYDiasRestantes(fechaActual, fechaFin);

                // Pasar el resultado a la vista a través de ViewBag
                ViewBag.MesesYDiasRestantes = resultado;
            }
            else
            {
                // Manejo en caso de que FechaFin sea nula
                ViewBag.MesesYDiasRestantes = "Fecha no disponible";
            }

            return View(contrato);
        }
    }




    [HttpPost]
    public IActionResult Guardar(Contrato contrato)
    {
        if (contrato.Id == 0)
        {
            _repo.Alta(contrato);
            TempData["SuccessMessage"] = "Contrato generado correctamente.";
        }
        else
        {
            _repo.Modificar(contrato);
            TempData["SuccessMessage"] = "Contrato modificado correctamente.";
        }
        return RedirectToAction(nameof(Index));


    }


    public IActionResult Eliminar(int id)
    {
        _repo.Baja(id);
        TempData["SuccessMessage"] = "Datos de contrato eliminados correctamente.";
        return RedirectToAction(nameof(Index));
    }


    public IActionResult Anular(int id, String observ)
    {

        _repo.Anular(id, observ);
        TempData["SuccessMessage"] = "Contrato anulado correctamente.";
        return RedirectToAction(nameof(Index));
    }


    private string CalcularMesesYDiasRestantes(DateTime fechaActual, DateTime fechaFin)
    {
        int diferenciaEnAnios = fechaFin.Year - fechaActual.Year;
        int diferenciaEnMeses = fechaFin.Month - fechaActual.Month;

        int mesesRestantes = diferenciaEnAnios * 12 + diferenciaEnMeses;
        int diasRestantes = fechaFin.Day - fechaActual.Day;

        // Ajustar si el día de la fecha de vencimiento es menor que el día actual
        if (diasRestantes < 0)
        {
            mesesRestantes--;
            // Calcula los días restantes en el mes anterior
            var diasEnMesAnterior = DateTime.DaysInMonth(fechaActual.Year, fechaActual.Month);
            diasRestantes += diasEnMesAnterior;
        }

        if (mesesRestantes > 0 && diasRestantes > 0)
        {
            return $"{mesesRestantes} meses y {diasRestantes} días";
        }
        else if (mesesRestantes == 0 && diasRestantes > 0)
        {
            return $"{diasRestantes} día";
        }
        else
        {
            return $"{mesesRestantes} meses";
        }



    }



}
