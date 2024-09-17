using Microsoft.AspNetCore.Mvc;
using inmobiliaria_AT.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic;
namespace inmobiliaria_AT.Controllers;


public class PagoController : Controller
{
    private readonly ILogger<PagoController> _logger;
    private readonly RepositorioPago _repo;
    private readonly RepositorioConcepto concepto;
    private readonly RepositorioInmueble _repoInmueble;

    private readonly RepositorioContrato _repoContrato;

    public PagoController(ILogger<PagoController> logger, RepositorioContrato repoContrato, RepositorioInmueble repoInmueble, RepositorioPago repo, RepositorioConcepto repoConcepto)
    {
        _logger = logger;
        _repo = repo;
        concepto = repoConcepto;
        _repoInmueble = repoInmueble;
        _repoContrato = repoContrato;

    }


    public IActionResult Index()
    {
        return View(_repo.ObtenerTodos());
    }


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


    /*

    public IActionResult Editar(int id)
    {
        var cont = contrato.ObtenerTodos();
        var concept = concepto.ObtenerTodos();
        ViewBag.Contrato = new SelectList(cont, "Id", "Direccion");
        ViewBag.Concepto = new SelectList(concept, "Id", "Nombre");

        if (id == 0)
        {
            return View(new Pago()); // Si no hay ID, devuelve un objeto vacío de Pago.
        }
        else
        {
            return View(contrato.ObtenerPorId(id));
        }
    }
    */

    public IActionResult ListadoPagos(int id)
    {
        ViewBag.ContratoId = id;

        return View(_repo.ObtenerPorContrato(id));
    }



    [HttpPost]
    public IActionResult Nuevo(Pago pago)
    {
        var contratos = _repoContrato.ObtenerTodos();
        var conceptos = concepto.ObtenerTodos();

        ViewBag.Contrato = new SelectList(contratos, "Id", "DireccionInmueble");
        ViewBag.Concepto = new SelectList(conceptos, "Id", "Nombre");

        _repo.Alta(pago);
        TempData["SuccessMessage"] = "Pago generado correctamente.";


        return RedirectToAction("Index"); ;
    }





    public IActionResult Anular(int id)
    {
        _repo.Anular(id);
        TempData["SuccessMessage"] = "Pago anulado correctamente.";

        return RedirectToAction(nameof(Index));
    }



    public IActionResult Eliminar(int id)
    {
        _repo.Baja(id);
        TempData["SuccessMessage"] = "Pago eliminado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    /*
        public IActionResult Multa(int idContrato)
        {
            int tiempoRestante = 0;
            var contrato = _repoContrato.ObtenerPorId(idContrato);
            if (contrato != null)
            {
                DateTime? fechaFin = contrato.FechaFin; // Si es DateTime?, permite valores nulos
                tiempoRestante = fechaFin.HasValue ? (fechaFin.Value - DateTime.Now).Days : 0; // Verificar si tiene valor


            }
            ViewBag.Contrato = contrato;
            ViewBag.TiempoRestante = tiempoRestante;
            return View(contrato);

        }
    */

    public IActionResult Multa(int idContrato)
    {
        int tiempoRestante = 0;
        if (idContrato == 0)
        {
            // Si el ID no llega correctamente, devolver un mensaje de error
            return NotFound("ID del contrato no válido.");
        }

        // Obtener el contrato por su ID
        var contrato = _repoContrato.ObtenerPorId(idContrato);

        if (contrato != null)
        {
            // Verificar si la fecha de finalización tiene valor
            DateTime? fechaFin = contrato.FechaFin;

            // Si tiene valor, calcular el tiempo restante en días
            tiempoRestante = fechaFin.HasValue ? (fechaFin.Value - DateTime.Now).Days : 0;
            ViewBag.TiempoRestante = tiempoRestante;
            ViewBag.Contrato = contrato;
        }
        else
        {
            // Manejar el caso en que el contrato no se encuentre (opcional)
            return NotFound("Contrato no encontrado");
        }
        // Retornar la vista con el contrato como modelo
        return View(contrato);
    }



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


        _repo.AltaMulta(idContrato, Detalle, Fecha);

        // 2. Anular el contrato
        _repoContrato.Anular(idContrato, "Contrato anulado por rescisión en la fecha " + Fecha.ToString("dd/MM/yyyy") + " a " + tiempoRestante + " dias del vencimiento."); 

        TempData["SuccessMessage"] = "Pago registrado y contrato anulado correctamente.";

        // Redirigir a la vista de contratos
        return RedirectToAction("Index", "Contrato");
    }


}
