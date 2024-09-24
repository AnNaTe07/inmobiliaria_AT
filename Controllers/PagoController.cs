using Microsoft.AspNetCore.Mvc;
using inmobiliaria_AT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic;
using System.Security.Claims;
using Org.BouncyCastle.Utilities;
namespace inmobiliaria_AT.Controllers;


public class PagoController : Controller
{
    private readonly ILogger<PagoController> _logger;
    private readonly RepositorioPago _repo;
    private readonly RepositorioConcepto _repoConcepto;
    private readonly RepositorioInmueble _repoInmueble;

    private readonly RepositorioContrato _repoContrato;

    private readonly RepositorioPropietario _repoProp;

    private readonly RepositorioUsuario _repoUsuario;



    public PagoController(ILogger<PagoController> logger, RepositorioUsuario repositorioUsuario, RepositorioPropietario repoProp, RepositorioContrato repoContrato, RepositorioInmueble repoInmueble, RepositorioPago repo, RepositorioConcepto repoConcepto)
    {
        _logger = logger;
        _repo = repo;
        _repoConcepto = repoConcepto;
        _repoInmueble = repoInmueble;
        _repoContrato = repoContrato;
        _repoProp = repoProp;
        _repoUsuario = repositorioUsuario;

    }

    [Authorize(Policy = "AdminEmpleado")]

    public IActionResult Index(int? propietarioId, int? inmuebleId, int? conceptoId, bool? estado, DateTime? fechaDesde, DateTime? fechaHasta)
    {
        var pagos = _repo.ObtenerTodos();
        var propietarios = _repoProp.ObtenerTodos();
        var inmuebles = _repoInmueble.ObtenerTodos();
        ViewBag.Propietarios = new SelectList(propietarios, "Id", "NombreCompleto");
        ViewBag.Inmuebles = new SelectList(inmuebles, "Id", "Direccion");

        // Filtro por Propietario
        if (propietarioId.HasValue)
        {
            pagos = pagos.Where(p => p.Contrato.Prop.Id == propietarioId.Value).ToList();
        }

        // Filtro por Inmueble
        if (inmuebleId.HasValue)
        {
            pagos = pagos.Where(p => p.Contrato.Inmu.Id == inmuebleId.Value).ToList();
        }

        // Filtro por Estado
        if (estado.HasValue)
        {
            pagos = pagos.Where(p => p.Estado == estado.Value).ToList();
        }

        // Filtro por Fecha
        if (fechaDesde.HasValue)
        {
            // Ignora el horario, solo compara las fechas
            var fechaDesdeDateOnly = fechaDesde.Value.Date;
            pagos = pagos.Where(p => p.Fecha.Date >= fechaDesdeDateOnly).ToList();
        }

        if (fechaHasta.HasValue)
        {
            // ignora el horario, solo compara las fechas
            var fechaHastaDateOnly = fechaHasta.Value.Date;
            pagos = pagos.Where(p => p.Fecha.Date <= fechaHastaDateOnly).ToList();
        }

        return View(pagos);
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
            return View(_repo.ObtenerPorId(id));
        }
    }

    [Authorize(Policy = "AdminEmpleado")]

    public IActionResult ListadoPagos(int id)
    {
        ViewBag.contrato = _repoContrato.ObtenerPorId(id);

        return View(_repo.ObtenerPorContrato(id));
    }

    [Authorize(Policy = "AdminEmpleado")]

    [HttpGet]
    public IActionResult Nuevo(int id)
    {
        // Obtiene todos los contratos y conceptos y los pasa como valor y "detalle"
        var contratos = _repoContrato.ObtenerTodos()
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = "Pago " + (_repo.ObtenerCantidadPagosPorContrato(c.Id) + 1) + ": " + c.Inmu.Direccion + " - " + c.Inqui.NombreCompleto

            }).ToList();

        ViewBag.Contratos = new SelectList(contratos, "Value", "Text");

        var nuevoPago = new Pago
        {
            Fecha = DateTime.Now
        };

        if (id != 0)
        {
            var contratoSeleccionado = _repoContrato.ObtenerPorId(id);
            ViewBag.contratoSeleccionado = contratoSeleccionado;
            if (contratoSeleccionado != null)
            {
                nuevoPago.Contrato = contratoSeleccionado; // Asigna el contrato al pago            
            }
            else
            {
                ModelState.AddModelError("", "Contrato no encontrado.");
            }
        }

        return View(nuevoPago);
    }

    [Authorize(Policy = "AdminEmpleado")]

    [HttpPost]
    public IActionResult Nuevo(Pago pago)
    {

        var userId = User.FindFirst("UserId")?.Value;
        var usuarioActual = _repoUsuario.ObtenerPorId(Int32.Parse(userId));
        pago.UsuPago = usuarioActual;
        try
        {
            if (pago != null)
            {
                _repo.Alta(pago);
                TempData["SuccessMessage"] = $"Pago generado correctamente.";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "No se pudo generar el pago.");
            }


        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al guardar el pago: {ex.Message}");
            ModelState.AddModelError("", "Error al guardar el pago.");
        }

        return View(pago);
    }

    [Authorize(Policy = "AdminEmpleado")]

    [HttpGet]
    public IActionResult Editar(int id)
    {

        // Obtener el pago a editar por su ID
        var pago = _repo.ObtenerPorId(id);
        var contratoSeleccionado = _repoContrato.ObtenerPorId(pago.Contrato.Id);
        if (pago == null)
        {
            return NotFound("No se encontró el pago.");
        }

        var contratos = _repoContrato.ObtenerTodos()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Inmu.Direccion + " - " + c.Inqui.NombreCompleto
                }).ToList();

        ViewBag.Contratos = new SelectList(contratos, "Value", "Text", contratoSeleccionado.Id);
        //ViewBag.Contratos = new SelectList(contratos, "Id", "DireccionInmueble", contratoSeleccionado.Id);
        ViewBag.contratoSeleccionado = contratoSeleccionado;
        // Pasa el pago como modelo a la vista
        return View(pago);
    }

    [Authorize(Policy = "AdminEmpleado")]

    [HttpPost]
    public IActionResult Editar(Pago pago)
    {
        var userId = User.FindFirst("UserId")?.Value;
        var usuarioActual = _repoUsuario.ObtenerPorId(Int32.Parse(userId));
        pago.Detalle = pago.Detalle + " - Pago editado por: " + usuarioActual.NombreCompleto + " - " + DateTime.Now; ;
        try
        {

            if (pago != null)
            {
                _repo.Editar(pago);
                TempData["SuccessMessage"] = "Pago modificado correctamente.";
                return RedirectToAction("Index");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al modificar el pago: {ex.Message}");
            ModelState.AddModelError("", "Error al modificar el pago.");
        }


        return View(pago);
    }

    [Authorize(Policy = "AdminEmpleado")]

    [HttpGet]
    public IActionResult ObtenerPorIdJSON(int id)
    {
        var contrato = _repoContrato.ObtenerPorId(id);
        if (contrato != null)
        {
            return Json(new { Precio = contrato.PrecioInmueble });
        }
        return Json(null);
    }

    [Authorize(Policy = "AdminEmpleado")]

    public IActionResult Anular(int id)
    {
        var userId = Int32.Parse(User.FindFirst("UserId")?.Value);

        _repo.Anular(id, userId);
        TempData["SuccessMessage"] = "Pago anulado correctamente.";

        return RedirectToAction(nameof(Index));
    }


    [Authorize(Policy = "AdminEmpleado")]

    public IActionResult Eliminar(int id)
    {
        _repo.Baja(id);
        TempData["SuccessMessage"] = "Pago eliminado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Policy = "AdminEmpleado")]

    public IActionResult Multa(int idContrato)
    {
        int tiempoRestante = 0;
        if (idContrato == 0)
        {
            return NotFound("ID del contrato no válido.");
        }

        var contrato = _repoContrato.ObtenerPorId(idContrato);

        if (contrato != null)
        {
            DateTime? fechaFin = contrato.FechaFin;
            var pagosAdeudados = _repo.CalcularPagosAdeudados(contrato);
            var importeAdeudado = pagosAdeudados * contrato.Inmu.Precio;
            tiempoRestante = fechaFin.HasValue ? (fechaFin.Value - DateTime.Now).Days : 0;

            ViewBag.PagosAdeudados = pagosAdeudados;
            ViewBag.ImporteAdeudado = importeAdeudado;
            ViewBag.TiempoRestante = tiempoRestante;
            ViewBag.Multa = _repo.CalcularMulta(contrato);
            ViewBag.Contrato = contrato;
        }
        else
        {
            return NotFound("Contrato no encontrado");
        }

        return View(contrato);
    }


    [Authorize(Policy = "AdminEmpleado")]

    [HttpPost]
    public IActionResult ConfirmarAnulacion(int idContrato)
    {


        // Verifica si idContrato es null o tiene un valor inválido
        if (idContrato <= 0)
        {
            TempData["ErrorMessage"] = "ID de contrato no válido.";
            return RedirectToAction("Index", "Contrato");
        }
        var contrato = _repoContrato.ObtenerPorId(idContrato);
        int tiempoRestante = 0;
        if (contrato != null)
        {
            // Verificar si la fecha de finalización tiene valor
            DateTime? fechaFin = contrato.FechaFin;

            // Si tiene valor, calcular el tiempo restante en días
            tiempoRestante = fechaFin.HasValue ? (fechaFin.Value - DateTime.Now).Days : 0;
        }


        var Fecha = DateTime.Now;
        var Detalle = "Multa por rescisión de contrato a " + tiempoRestante + " dias del vencimiento.";

        var userId = Int32.Parse(User.FindFirst("UserId")?.Value);

        _repo.AltaMulta(idContrato, Detalle, Fecha, userId);

        // 2. Anular el contrato
        _repoContrato.Anular(idContrato, "Contrato anulado por rescisión en la fecha " + Fecha.ToString("dd/MM/yyyy") + " a " + tiempoRestante + " dias del vencimiento.", userId);

        TempData["SuccessMessage"] = "Pago registrado y contrato anulado correctamente.";

        // Redirigir a la vista de contratos
        return RedirectToAction("Index", "Contrato");
    }


}
