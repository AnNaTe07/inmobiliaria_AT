namespace inmobiliaria_AT.Models;

public enum UsoInmueble
{
    Comercial,
    Residencial
}
public class Inmueble
{
    public int Id { get; set; }
    public UsoInmueble Uso { get; set; }
    public string Direccion { get; set; } = "";
    public int TipoId { get; set; }
    public string TipoDescripcion { get; set; } = "";
    public int Ambientes { get; set; }
    public decimal Latitud { get; set; }
    public decimal Longitud { get; set; }
    public decimal Superficie { get; set; }
    public decimal Precio { get; set; }
    public int IdPropietario { get; set; }
    public Propietario PropietarioInmueble { get; set; }
}