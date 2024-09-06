using Microsoft.AspNetCore.Mvc;
using inmobiliaria_AT.Models;
namespace inmobiliaria_AT.Controllers;


public class PagosController : Controller
{
    private readonly ILogger<PagosController> _logger;
    private readonly RepositorioPago _repo;

    public PagosController(ILogger<PagosController> logger, RepositorioPago repo)
    {
        _logger = logger;
        _repo = repo;
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

    public IActionResult Editar(int id)
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

    [HttpPost]
    public IActionResult Guardar(Pago pago)
    {
        if (pago != null)
        {
            _repo.Alta(pago);
            TempData["SuccessMessage"] = "Pago realizado correctamente.";

        }
        return RedirectToAction("Index");

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
