namespace inmobiliaria_AT.Models;

public class Pago
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public Decimal Monto { get; set; }
    public Contrato Contrato { get; set; }
    public bool Estado { get; set; }
    public DateTime FechaAnulacion { get; set; }
   public Usuario UsuPago { get; set; }
   public Usuario UsuAnulacion { get; set; }
    public String Detalle { get; set; } = "";
    public Concepto Concepto { get; set; }
}