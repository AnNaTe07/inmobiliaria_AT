namespace inmobiliaria_AT.Models;

public class Contrato
{
    public int Id { get; set; }
    public int IdInquilino { get; set; }

    public int IdInmueble { get; set; }
    public int IdPropietario { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public decimal Monto { get; set; }
    public string Estado { get; set; } = "";
    public string Tipo { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public int Plazo { get; set; }
    public decimal PorcentajeActualizacion { get; set; }
    public int PeriodoActualizacion { get; set; } 
    public string Observaciones { get; set; } = "";

}