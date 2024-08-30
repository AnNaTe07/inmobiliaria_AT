namespace inmobiliaria_AT.Models;


/*public enum TipoContrato
{
    Compraventa,
    Permuta
    ,
    Arrendamiento

}*/

public class Contrato
{
    public int Id { get; set; }
    public Inquilino Inqui { get; set; } = null!;
    public Inmueble Inmu { get; set; } = null!;
    public Propietario Prop { get; set; } = null!;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public decimal Monto { get; set; }
    public bool Estado { get; set; } = true;
    //public TipoContrato Tipo { get; set; }
    public string Descripcion { get; set; } = "";
    public int Plazo { get; set; }
    public decimal PorcentajeActualizacion { get; set; }
    public int PeriodoActualizacion { get; set; }
    public string Observaciones { get; set; } = "";

}