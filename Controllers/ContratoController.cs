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
}