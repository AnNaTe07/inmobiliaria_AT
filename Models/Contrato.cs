namespace inmobiliaria_AT.Models;
using System.ComponentModel.DataAnnotations;

public class Contrato
{
    public int Id { get; set; }

    public Inquilino Inqui { get; set; } = null!;
    public Inmueble Inmu { get; set; } = null!;
    public Propietario Prop { get; set; } = null!;

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }

    public int Estado { get; set; }

    public string Descripcion { get; set; } = "";

    public string Observaciones { get; set; } = "";

    public Usuario UsuCreacion { get; set; }

    public Usuario? UsuAnulacion { get; set; }

    public int? Pagos { get; set; }

    // Metodo para calcular la cantidad de pagos
    public void CalcularCantidadPagos()
    {
        var duration = FechaFin - FechaInicio;
        if (duration.Days < 30)
        {
            Pagos = 1; // Pago único
        }
        else
        {
            Pagos = (int)Math.Ceiling(duration.TotalDays / 30); // Pagos mensuales
        }
    }
    public decimal? PrecioInmueble => Inmu?.Precio ?? 0;

    public string DireccionInmueble => Inmu != null ? Inmu.Direccion : "Dirección no disponible";

}