using inmobiliaria_AT.Models;
using Microsoft.AspNetCore.Authorization;
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
    private readonly RepositorioUsuario _repoUsuario;
    public ContratoController(ILogger<ContratoController> logger, RepositorioContrato repo, RepositorioUsuario repoUsuario, RepositorioPropietario repoProp, RepositorioInmueble repoInmueble, RepositorioInquilino repoInquilino)
    {
        _logger = logger;
        _repo = repo;
        _repoInmueble = repoInmueble;
        _repoProp = repoProp;
        _repoInquilino = repoInquilino;
        _repoUsuario = repoUsuario;
        _repo.vigenciaContrato();

    }
    [Authorize(Policy = "AdminEmpleado")]
    public IActionResult ObtenerPorId(int id)
    {
        var contrato = _repo.ObtenerPorId(id); // Obtener el contrato desde el repositorio
        if (contrato == null)
        {
            return NotFound(); // Devuelve 404 si no se encuentra el contrato
        }
        return Json(contrato); // Devuelve el contrato como JSON
    }

    [Authorize(Policy = "AdminEmpleado")]

    [HttpGet("ObtenerTodos")]
    public IActionResult ObtenerTodos()
    {
        var contratos = _repo.ObtenerTodos();

        return Json(contratos);
    }
    [Authorize(Policy = "AdminEmpleado")]

    public IActionResult Index(int? inquilinoId, int? inmuebleId, DateTime? fechaDesde, DateTime? fechaHasta, int? duracionContrato)
    {
        var propietarios = _repoProp.ObtenerTodos();
        var inmuebles = _repoInmueble.ObtenerTodos();
        var inquilinos = _repoInquilino.ObtenerTodos();
        var contratos = _repo.ObtenerTodos();

        // aplica los filtros solo si se selecciona alguna opción
        if (inquilinoId.HasValue)
        {
            contratos = contratos.Where(c => c.Inqui.Id == inquilinoId.Value).ToList();
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

       if (duracionContrato.HasValue)
    {
        // Definir los rangos para los filtros de duración
        int rangoInferior = 0;
        int rangoSuperior = 0;

        switch (duracionContrato.Value)
        {
            case 30:
                rangoInferior = 28; // dias aproximados para un mes
                rangoSuperior = 32;
                break;
            case 60:
                rangoInferior = 58; // Dias aproximados para dos meses
                rangoSuperior = 62;
                break;
            case 90:
                rangoInferior = 88; // dias aproximados para tres meses
                rangoSuperior = 92;
                break;
        }

        // Filtra los contratos donde la duracion en dias este dentro del rango seleccionado
        contratos = contratos.Where(c => (c.FechaFin - c.FechaInicio).TotalDays >= rangoInferior 
                                         && (c.FechaFin - c.FechaInicio).TotalDays <= rangoSuperior).ToList();
    }
        // Envia la lista filtrada a la vista
        ViewBag.Propietarios = new SelectList(propietarios, "Id", "NombreCompleto");
        ViewBag.Inmuebles = new SelectList(inmuebles, "Id", "Direccion");
        ViewBag.Inquilinos = new SelectList(inquilinos, "Id", "NombreCompleto");


        return View(contratos);
    }
    [Authorize(Policy = "AdminEmpleado")]

    public IActionResult ListaVencidos(int? inquilinoId, int? inmuebleId, DateTime? fechaDesde, DateTime? fechaHasta)
    {
        // Obtiene todos los propietarios e inmuebles para cargar en los selects
        var propietarios = _repoProp.ObtenerTodos();
        var inmuebles = _repoInmueble.ObtenerTodos();
        var inquilinos = _repoInquilino.ObtenerTodos();

        // pbtiene todos los contratos
        var contratos = _repo.ObtenerVencidos();

        // aplica los filtros solo si se selecciona alguna opción
        if (inquilinoId.HasValue)
        {
            contratos = contratos.Where(c => c.Inqui.Id == inquilinoId.Value).ToList();
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
        ViewBag.Inquilinos = new SelectList(inquilinos, "Id", "NombreCompleto");

        return View(contratos);
    }
    [Authorize(Policy = "AdminEmpleado")]

    public IActionResult Editar(int id)
    {

        //obtengo la lista de propietarios
        var propietarios = _repoProp.ObtenerTodos();
        var inmuebles = _repoInmueble.ObtenerDisponiblesTotales();
        var inquilinos = _repoInquilino.ObtenerTodos();
        if (id > 0)
        {

            Contrato cont = _repo.ObtenerPorId(id);

            Inmueble inmuContra = _repoInmueble.ObtenerPorId(cont.Inmu.Id);

            //Agrego el inmueble que ya tiene el contrato, porque si no solo traigo los no alguilados
            inmuebles.Add(inmuContra);
            ViewBag.Contrato = cont;

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
            contrato.CalcularCantidadPagos();


            // Guardar los cambios en la base de datos
            _repo.Modificar(contrato);
            return View(contrato);
        }
    }

    [Authorize(Policy = "AdminEmpleado")]

    [HttpGet]
    public IActionResult Renovar(int id)
    {
        // Obtén el contrato usando el ID
        Contrato contrato = _repo.ObtenerPorId(id);
        contrato.CalcularCantidadPagos();

        if (contrato == null)
        {
            return NotFound();
        }

        // Obtén el inquilino y el inmueble asociados al contrato
        var inquilino = _repoInquilino.ObtenerPorId(contrato.Inqui.Id);
        Inmueble inmueble = _repoInmueble.ObtenerPorId(contrato.Inmu.Id);

        // Pasa los datos a la vista usando ViewBag
        ViewBag.Inmueble = inmueble;
        ViewBag.Inquilino = inquilino;
        ViewBag.Contrato = contrato;

        return View(contrato);
    }

    [Authorize(Policy = "AdminEmpleado")]

    [HttpPost]
    public IActionResult Renovar(Contrato contratoRenovado)
    {
        var usuarioActual = _repoUsuario.ObtenerPorId(Int32.Parse(User.FindFirst("UserId")?.Value));
        contratoRenovado.CalcularCantidadPagos();


        try
        {
            Contrato contratoOriginal = _repo.ObtenerPorId(contratoRenovado.Id);
            if (contratoOriginal == null)
            {
                return NotFound();
            }

            contratoRenovado.UsuCreacion = usuarioActual;
            contratoRenovado.Inqui = contratoOriginal.Inqui;
            contratoRenovado.Inmu = contratoOriginal.Inmu;
            contratoRenovado.Prop = contratoOriginal.Prop;
            _logger.LogInformation("Renovando contrato: Dirección Inmueble: {Direccion}, Propietario: {Propietario}, Inquilino: {Inquilino}, Fecha Fin: {FechaFin}, Fecha Inicio: {FechaInicio}, Descripción: {Descripcion}, Observaciones: {Observaciones}, Usuario de Creación: {UsuarioCreacion}",
            contratoRenovado.Inmu?.Direccion,
            contratoRenovado.Prop?.NombreCompleto,
            contratoRenovado.Inqui?.NombreCompleto,
            contratoRenovado.FechaFin,
            contratoRenovado.FechaInicio,
            contratoRenovado.Descripcion,
            contratoRenovado.Observaciones,
            contratoRenovado.UsuCreacion?.NombreCompleto);

            _repo.Renovar(contratoRenovado);

            TempData["SuccessMessage"] = "Contrato renovado correctamente.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Ocurrió un error al intentar renovar el contrato.");
            _logger.LogError(ex, "Error al renovar el contrato.");
        }

        return View(contratoRenovado);
    }



    [Authorize(Policy = "AdminEmpleado")]

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

    [Authorize(Policy = "AdminEmpleado")]

    [HttpPost]
    public IActionResult Guardar(Contrato contrato)
    {
        var usuarioActual = _repoUsuario.ObtenerPorId(Int32.Parse(User.FindFirst("UserId")?.Value));
        contrato.UsuCreacion = usuarioActual;
        contrato.CalcularCantidadPagos();


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
    [Authorize(Policy = "Administrador")]

    public IActionResult Eliminar(int id)
    {
        _repo.Baja(id);
        TempData["SuccessMessage"] = "Datos de contrato eliminados correctamente.";
        return RedirectToAction(nameof(Index));
    }
    [Authorize(Policy = "AdminEmpleado")]

    public IActionResult Anular(int id, String observ)
    {
        observ = observ + "Fecha de Anulación: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        var usuarioActual = _repoUsuario.ObtenerPorId(Int32.Parse(User.FindFirst("UserId")?.Value));

        _repo.Anular(id, observ, usuarioActual.Id);
        TempData["SuccessMessage"] = "Contrato anulado correctamente.";
        return RedirectToAction(nameof(Index));
    }
    [Authorize(Policy = "AdminEmpleado")]

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
    [Authorize(Policy = "AdminEmpleado")]

    [HttpGet("ObtenerPorIdJSON")]
    public IActionResult ObtenerPorIdJSON(int id)
    {
        var contrato = _repo.ObtenerPorId(id);
        if (contrato == null)
        {
            return NotFound();
        }

        // Devuelve solo lo necesario
        var resultado = new
        {
            Id = contrato.Id,
            Precio = contrato.Inmu?.Precio // Asumiendo que Inmu no es null
        };

        return Json(resultado);
    }
    [Authorize(Policy = "AdminEmpleado")]

    [HttpGet("ObtenerInmueblesDisponibles")]
    public IActionResult ObtenerInmueblesDisponibles(DateTime? fechaInicio, DateTime? fechaFin)
    {
        // Obtiene todos los inmuebles y contratos
        var inmuebles = _repoInmueble.ObtenerTodos();
        var contratos = _repo.ObtenerTodos();

        // Filtra inmuebles basados en el rango de fechas
        var inmueblesNoDisponibles = contratos
            .Where(c => (fechaInicio.HasValue && c.FechaInicio <= fechaFin) || (fechaFin.HasValue && c.FechaFin >= fechaInicio))
            .Select(c => c.Inmu)
            .Distinct()
            .ToList();

        var inmueblesDisponibles = inmuebles
            .Except(inmueblesNoDisponibles, new InmuebleEqualityComparer())
            .ToList();

        return Json(inmueblesDisponibles);
    }
    [Authorize(Policy = "AdminEmpleado")]

    // Comparador de igualdad para Inmueble
    public class InmuebleEqualityComparer : IEqualityComparer<Inmueble>
    {
        public bool Equals(Inmueble x, Inmueble y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(Inmueble obj)
        {
            return obj.Id.GetHashCode();
        }
    }

    [Authorize(Policy = "AdminEmpleado")]

    public JsonResult ObtenerInmueblesPorFechas(DateTime fechaInicio, DateTime fechaFin)
    {
        Console.WriteLine($"Fecha Inicio: {fechaInicio}, Fecha Fin: {fechaFin}");

        // Filtrar inmuebles entre fechaInicio y fechaFin
        var inmueblesDisponibles = _repoInmueble.ObtenerTodos()
            .Where(i =>
                !_repo.ObtenerTodos().Any(c =>
                    c.Inmu.Id == i.Id &&
                    ((c.FechaInicio <= fechaFin && c.FechaFin >= fechaInicio) ||
                     (c.FechaInicio >= fechaInicio && c.FechaFin <= fechaFin))
                )
            )
            .Select(i => new { value = i.Id, text = i.Direccion, precio = i.Precio }) // Asegúrate de que i.Precio esté definido
            .ToList();

        Console.WriteLine($"Inmuebles encontrados: {inmueblesDisponibles.Count}");

        return Json(inmueblesDisponibles);
    }


}