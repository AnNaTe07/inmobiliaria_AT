namespace inmobiliaria_AT.Models;

public enum UsoInmueble{
    Comercial,
    Residencial
}
public class Inmueble
{
    public int Id { get; set; }
    public UsoInmueble Uso { get; set; }
    public string Direccion { get; set; } = "";
    public string Tipo { get; set; } = "";
    public int Ambientes { get; set; }
    public decimal Latitud { get; set; }
    public decimal Longitud { get; set; }
    public decimal Precio { get; set; }
    public int IdPropietario { get; set; }
}