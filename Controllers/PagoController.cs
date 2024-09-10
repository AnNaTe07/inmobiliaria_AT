using Microsoft.AspNetCore.Mvc;
using inmobiliaria_AT.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace inmobiliaria_AT.Controllers;


public class PagoController : Controller
{
    private readonly ILogger<PagoController> _logger;
    private readonly RepositorioPago _repo;
    private RepositorioConcepto concepto;
    private RepositorioContrato contrato;

    public PagoController(ILogger<PagoController> logger, RepositorioPago repo, RepositorioContrato repoContrato, RepositorioConcepto repoConcepto)
    {
        _logger = logger;
        _repo = repo;
        contrato = repoContrato;
        concepto = repoConcepto;
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


    [HttpPost]
    public IActionResult Nuevo(Pago pago)
    {
        // Obtener contratos y conceptos para el select nuevamente
        var contratos = contrato.ObtenerTodos();
        var conceptos = concepto.ObtenerTodos();

        ViewBag.Contrato = new SelectList(contratos, "Id", "DireccionInmueble");
        ViewBag.Concepto = new SelectList(conceptos, "Id", "Nombre");

        // Si el modelo es válido, guardar el pago
        if (ModelState.IsValid)
        {
            _repo.Alta(pago);
            TempData["SuccessMessage"] = "Pago realizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // Si no es válido, mostrar la vista nuevamente
        return View(pago);
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



}
