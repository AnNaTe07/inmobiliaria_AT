namespace inmobiliaria_AT.Models;

public class Propietario
{
    public int Id { get; set; }
    public string Nombre { get; set; }="";
    public string Apellido { get; set; }="";
    public string Documento { get; set; }="";    
    public string Telefono { get; set; }="";
    public string Email { get; set; }="";
    public string Direccion { get; set; }="";
}