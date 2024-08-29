using inmobiliaria_AT.Models;
using Microsoft.AspNetCore.Mvc;
namespace inmobiliaria_AT.Controllers;

public class ContratoController : Controller
{
    private readonly ILogger<ContratoController> _logger;
    private readonly RepositorioContrato _repo;
    public ContratoController(ILogger<ContratoController> logger, RepositorioContrato repo)
    {
        _logger = logger;
        _repo = repo;
    }
    public IActionResult Index()
    {
        var contrato = _repo.ObtenerTodos(); //obtiene todos los contratos desde el repositorio
        return View(contrato);  //devuelve a la vista con la lista de contratos 
    }

    public IActionResult Editar(int id)
    {
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

    public IActionResult Detalle(int id)
    {
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
        TempData["SuccessMessage"] = "Datos de propietario eliminados correctamente.";
        return RedirectToAction(nameof(Index));
    }





}








