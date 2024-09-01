using inmobiliaria_AT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace inmobiliaria_AT.Controllers;

public class ContratoController : Controller
{
    private readonly ILogger<ContratoController> _logger;
    private readonly RepositorioContrato _repo;
    private readonly RepositorioPropietario  _repoProp;
    private readonly RepositorioInmueble _repoInmueble;
    private readonly RepositorioInquilino _repoInquilino;

    public ContratoController(ILogger<ContratoController> logger, RepositorioContrato repo, RepositorioPropietario repoProp, RepositorioInmueble repoInmueble, RepositorioInquilino repoInquilino)
    {
        _logger = logger;
        _repo = repo;
        _repoInmueble = repoInmueble;
        _repoProp = repoProp;
        _repoInquilino = repoInquilino;
        if (!_repo.TestConnection())
    {
        // Manejo del error si la conexión falla
        _logger.LogError("No se pudo conectar a la base de datos.");
    }

    }
public IActionResult Index()
{
    Console.WriteLine("Ejecutando Index...");
    var contratos = _repo.ObtenerTodos();
    Console.WriteLine($"Número de contratos recuperados: {contratos.Count} : {contratos[0].Prop.Id}");
    return View(contratos);
}




    public IActionResult Editar(int id)
    {

        //obtengo la lista de propietarios
        var propietarios = _repoProp.ObtenerTodos();
        var inmuebles = _repoInmueble.ObtenerTodos();
        var inquilinos = _repoInquilino.ObtenerTodos();

        ViewBag.Propietarios = new SelectList(propietarios, "Id", "NombreCompleto");
        ViewBag.Inmuebles = new SelectList(inmuebles,"Id","Direccion" );
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
        TempData["SuccessMessage"] = "Datos de contrato eliminados correctamente.";
        return RedirectToAction(nameof(Index));
    }





}








